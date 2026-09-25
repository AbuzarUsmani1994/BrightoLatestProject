using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Http;
using System.Web;

namespace FOS.Web.UI.Controllers.API
{
    // Mirrors BusinessAffiliatesDSRReportController: same request/response shape,
    // same PDF-generation approach, for Competitor Activities instead. SuccessResponse
    // is intentionally not redeclared here - it resolves to the shared top-level
    // class in OrderForExcelController.cs (same namespace), exactly like
    // BusinessAffiliatesDSRReportController already relies on.
    public class CompetitorActivitiesDSRReportController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [HttpPost]
        public Result<SuccessResponse> Post(CompetitorActivitySummary rm)
        {
            try
            {
                DateTime fromDate = DateTime.Parse(rm.DateFrom);
                DateTime toDate = DateTime.Parse(rm.DateTo).AddDays(1);

                List<CompetitorActivityDSRRow> result = FetchData(fromDate, toDate, rm.SOID);

                if (result == null || result.Count == 0)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "No data found for the selected date range",
                        ResultType = ResultType.Success,
                        Exception = null,
                    };
                }

                string soName = "All";
                var so = db.SaleOfficers.Where(u => u.ID == rm.SOID).FirstOrDefault();
                if (so != null) soName = so.Name;

                string fileName = "CompetitorActivitiesDSR_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                string outDir = HttpContext.Current.Server.MapPath("~/PDF");
                if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
                string outPath = Path.Combine(outDir, fileName);

                BuildPdf(outPath, result, soName, fromDate, toDate.AddDays(-1));

                SuccessResponse d = new SuccessResponse
                {
                    data = "http://116.58.33.11:81/PDF/" + fileName
                };

                return new Result<SuccessResponse>
                {
                    Data = d,
                    Message = "Downloaded",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "CompetitorActivitiesDSRReport failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = ex.Message,
                    ResultType = ResultType.Failure,
                    Exception = null,
                };
            }
        }

        private List<CompetitorActivityDSRRow> FetchData(DateTime fromDate, DateTime toDate, int soId)
        {
            var list = new List<CompetitorActivityDSRRow>();

            using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand("usp_GetCompetitorActivitiesDSR", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = fromDate });
                cmd.Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = toDate });
                cmd.Parameters.Add(new SqlParameter("@SOID", SqlDbType.Int) { Value = soId });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CompetitorActivityDSRRow
                        {
                            ActivityID = reader["ActivityID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ActivityID"]),
                            ActivityDate = reader["ActivityDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ActivityDate"]),
                            SOID = reader["SOID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SOID"]),
                            SaleOfficerName = reader["SaleOfficerName"] as string,
                            RegionID = reader["RegionID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["RegionID"]),
                            RegionName = reader["RegionName"] as string,
                            CityID = reader["CityID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CityID"]),
                            CityName = reader["CityName"] as string,
                            ActivityType = reader["ActivityType"] as string,
                            CompetitorName = reader["CompetitorName"] as string,
                            Remarks = reader["Remarks"] as string,
                            VideoUrl = reader["VideoUrl"] as string,
                            CreatedDate = reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedDate"]),
                        });
                    }
                }
            }

            return list;
        }

        private void BuildPdf(string outPath, List<CompetitorActivityDSRRow> rows, string soName, DateTime from, DateTime to)
        {
            using (var fs = new FileStream(outPath, FileMode.Create))
            {
                Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 25f, 25f);
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                var soFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                var brandFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                var dateFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var headFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);

                var header = new PdfPTable(3);
                header.WidthPercentage = 100;
                header.SetWidths(new float[] { 35f, 15f, 50f });
                header.SpacingAfter = 12f;

                var leftCell = new PdfPCell();
                leftCell.Border = Rectangle.NO_BORDER;
                leftCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                leftCell.AddElement(new Paragraph("Print Date: " + DateTime.Now.ToString("d/M/yyyy"), labelFont));
                leftCell.AddElement(new Paragraph(" ", labelFont));
                leftCell.AddElement(new Paragraph(soName, soFont));
                header.AddCell(leftCell);

                var logoCell = new PdfPCell();
                logoCell.Border = Rectangle.NO_BORDER;
                logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
                logoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                try
                {
                    string logoPath = HttpContext.Current.Server.MapPath("~/Images/BrightologoNew.png");
                    if (System.IO.File.Exists(logoPath))
                    {
                        var logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(110f, 55f);
                        logoCell.AddElement(logo);
                    }
                }
                catch { }
                header.AddCell(logoCell);

                var rightCell = new PdfPCell();
                rightCell.Border = Rectangle.NO_BORDER;
                rightCell.HorizontalAlignment = Element.ALIGN_LEFT;
                rightCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                rightCell.PaddingLeft = 8f;
                var brandP = new Paragraph("Brighto Paints", brandFont);
                brandP.Alignment = Element.ALIGN_LEFT;
                rightCell.AddElement(brandP);
                rightCell.AddElement(new Paragraph(" ", labelFont));
                var rangeP = new Paragraph(
                    string.Format("DateFrom: {0:dd-MMM-yyyy}    DateTo: {1:dd-MMM-yyyy}", from, to),
                    dateFont);
                rangeP.Alignment = Element.ALIGN_LEFT;
                rightCell.AddElement(rangeP);
                header.AddCell(rightCell);

                doc.Add(header);

                string[] headers = { "Date", "Sale Officer", "Region", "City", "Activity Type", "Competitor", "Remarks", "Video URL" };
                float[] widths = { 8f, 12f, 10f, 10f, 12f, 14f, 22f, 12f };

                var table = new PdfPTable(headers.Length);
                table.WidthPercentage = 100;
                table.SetWidths(widths);
                table.HeaderRows = 1;

                foreach (var h in headers)
                {
                    var c = new PdfPCell(new Phrase(h, headFont));
                    c.BackgroundColor = new BaseColor(63, 127, 95);
                    c.HorizontalAlignment = Element.ALIGN_CENTER;
                    c.Padding = 5f;
                    table.AddCell(c);
                }

                foreach (var r in rows)
                {
                    table.AddCell(Cell(r.ActivityDate.HasValue ? r.ActivityDate.Value.ToString("dd-MM-yyyy") : "", cellFont));
                    table.AddCell(Cell(r.SaleOfficerName, cellFont));
                    table.AddCell(Cell(r.RegionName, cellFont));
                    table.AddCell(Cell(r.CityName, cellFont));
                    table.AddCell(Cell(r.ActivityType, cellFont));
                    table.AddCell(Cell(r.CompetitorName, cellFont));
                    table.AddCell(Cell(r.Remarks, cellFont));
                    table.AddCell(Cell(r.VideoUrl, cellFont));
                }

                doc.Add(table);
                doc.Close();
            }
        }

        private static PdfPCell Cell(string text, iTextSharp.text.Font font)
        {
            var c = new PdfPCell(new Phrase(text ?? "", font));
            c.Padding = 4f;
            return c;
        }
    }

    public class CompetitorActivitySummary
    {
        public int SOID { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class CompetitorActivityDSRRow
    {
        public int ActivityID { get; set; }
        public DateTime? ActivityDate { get; set; }
        public int? SOID { get; set; }
        public string SaleOfficerName { get; set; }
        public int? RegionID { get; set; }
        public string RegionName { get; set; }
        public int? CityID { get; set; }
        public string CityName { get; set; }
        public string ActivityType { get; set; }
        public string CompetitorName { get; set; }
        public string Remarks { get; set; }
        public string VideoUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
