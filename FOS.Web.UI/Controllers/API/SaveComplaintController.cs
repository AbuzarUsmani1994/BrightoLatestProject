using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SaveComplaintController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ComplaintsDto rm)
        {
            Tbl_SaveComplaints jobDet = new Tbl_SaveComplaints();

            try
            {

                if (!rm.CustomerID.HasValue)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "CustomerID is required",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                Retailer ret = db.Retailers.FirstOrDefault(r => r.ID == rm.CustomerID);
                if (ret == null)
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Retailer not found",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };
                }

                // Set basic complaint details
                jobDet.CustomerID = rm.CustomerID.Value;
                jobDet.SOID = rm.SOID;
                jobDet.DealerID = rm.DealerID;
                jobDet.ProductID = rm.ProductID;
                jobDet.ProductNatureID = rm.ProductNatureID;
                jobDet.ProductBatchNo = rm.ProductBatchNo ?? string.Empty;
                jobDet.ShakingTime = rm.ShakingTime ?? string.Empty;
                jobDet.ColorCode = rm.ColorCode ?? string.Empty;
                jobDet.Quantity = rm.Quantity;
                jobDet.WetSampleAttached = rm.WetSampleAttached;
                jobDet.SubstrateColor = rm.SubstrateColor ?? string.Empty;
                jobDet.TinningRatio = rm.TinningRatio ?? string.Empty;
                jobDet.NoOfCoatsApplied = rm.NoOfCoatsApplied ?? string.Empty;
                jobDet.TimeDurationWithinCoats = rm.TimeDurationWithinCoats ?? string.Empty;
                jobDet.ThinningSolventID = rm.ThinningSolventID;
                jobDet.AppliedToolID = rm.AppliedToolID;
                jobDet.Status = "Launched";
                jobDet.IsActive = true;
                jobDet.Remarks = rm.Remarks ?? string.Empty;

                // Safely get region ID
                int? regionid = null;
                if (rm.SOID.HasValue)
                {
                    var saleOfficer = db.SaleOfficers.FirstOrDefault(x => x.ID == rm.SOID.Value);
                    regionid = saleOfficer?.RegionID;
                }

                // Set creation date based on region
                jobDet.CreatedOn = (regionid == 24)
                    ? DateTime.UtcNow.AddHours(3)
                    : DateTime.UtcNow.AddHours(4);

                // Handle Picture1
                try
                {
                    if (!string.IsNullOrEmpty(rm.Picture1))
                    {
                        jobDet.Picture1 = ConvertIntoByte(rm.Picture1, "ComplaintPictures",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintPictures");
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to process Picture1");
                    jobDet.Picture1 = null;
                }

                // Handle Picture2
                try
                {
                    if (!string.IsNullOrEmpty(rm.Picture2))
                    {
                        jobDet.Picture2 = ConvertIntoByte(rm.Picture2, "ComplaintPictures",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintPictures");
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to process Picture2");
                    jobDet.Picture2 = null;
                }

                try
                {
                    if (!string.IsNullOrEmpty(rm.StickerPicture))
                    {
                        jobDet.StrickerPicture = ConvertIntoByte1(rm.StickerPicture, "StickerPictures",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintPictures");
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to process Sticker Picture");
                    jobDet.Picture2 = null;
                }

                // Handle Voice Note - Save as WAV format which is more reliable
                try
                {
                    if (!string.IsNullOrEmpty(rm.VoiceNote))
                    {
                        Log.Instance.Info($"Processing AAC voice note, length: {rm.VoiceNote.Length} characters");

                        // Process as AAC format
                        jobDet.VoiceNote = VoiceNoteProcessor.ProcessAndSaveVoiceNote(rm.VoiceNote, "ComplaintAudio",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintAudio");

                        if (jobDet.VoiceNote != null)
                        {
                            Log.Instance.Info($"AAC voice note processed successfully: {jobDet.VoiceNote}");
                        }
                        else
                        {
                            Log.Instance.Warn("AAC voice note could not be processed, stored as null");
                        }
                    }
                    else
                    {
                        Log.Instance.Info("No voice note provided");
                        jobDet.VoiceNote = null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to process AAC VoiceNote");
                    jobDet.VoiceNote = null;
                }

                // Handle Video
                try
                {
                    if (!string.IsNullOrEmpty(rm.Video))
                    {
                        jobDet.Video = FileHandler.ConvertVideo(rm.Video, "ComplaintVideos",
                            DateTime.Now.ToString("dd-MM-yyyy-HHmmss"), "ComplaintVideos");
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to process Video");
                    jobDet.Video = null;
                }

                // Save main complaint record
                db.Tbl_SaveComplaints.Add(jobDet);
                db.SaveChanges();

                // Save complaint update record
                var complaintUpdate = new Tbl_ComplaintUpdate
                {
                    ComplaintID = jobDet.ID,
                    ComplaintNumber = $"{jobDet.ID}-{DateTime.Now.Year}",
                    LauncedTime = jobDet.CreatedOn,
                    LaunchedBy = jobDet.SOID,
                    ComplaintStatusID = 1,
                    LastUpdatedAt = DateTime.UtcNow.AddHours(3),
                    Picture = jobDet.Picture1,
                    Audio = jobDet.VoiceNote,
                    Video = jobDet.Video
                };

                db.Tbl_ComplaintUpdate.Add(complaintUpdate);
                db.SaveChanges();

                // Save Nature of Complaint records
                if (rm.NatureOfComplaintModel != null && rm.NatureOfComplaintModel.Count > 0)
                {
                    foreach (var item in rm.NatureOfComplaintModel.Where(item => item != null))
                    {
                        db.Tbl_SaveNatureOfComplaint.Add(
                            new Tbl_SaveNatureOfComplaint
                            {
                                ComplaintMasterID = jobDet.ID,
                                NatureOfComplaintID = item.NatureOfComplaitID,
                                Name = item.Name ?? string.Empty,
                                IsChecked = item.IsChecked,
                                CreatedOn = jobDet.CreatedOn,
                            });
                    }
                    db.SaveChanges();
                }

                // Save Substrate records
                if (rm.SubstrateModel != null && rm.SubstrateModel.Count > 0)
                {
                    foreach (var item in rm.SubstrateModel.Where(item => item != null))
                    {
                        db.Tbl_SaveConditionOfSubstrate.Add(
                            new Tbl_SaveConditionOfSubstrate
                            {
                                ComplaintMasterID = jobDet.ID,
                                SubstrateID = item.SubstrateID,
                                Name = item.Name ?? string.Empty,
                                IsChecked = item.IsChecked,
                                CreatedOn = jobDet.CreatedOn,
                            });
                    }
                    db.SaveChanges();
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint Punched Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Complaint API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }

        public static class VoiceNoteProcessor
        {
            /// <summary>
            /// Process voice note and save in AAC/MP3 format
            /// </summary>
            public static string ProcessAndSaveVoiceNote(string voiceData, string dealerName, string sendDateTime, string folderName)
            {
                try
                {
                    if (string.IsNullOrEmpty(voiceData))
                    {
                        Log.Instance.Warn("Voice data is null or empty");
                        return null;
                    }

                    Log.Instance.Info($"Processing voice note, length: {voiceData.Length} characters");

                    // Quick validation for very short data
                    if (voiceData.Length < 100)
                    {
                        Log.Instance.Warn("Voice data too short to be valid audio");
                        return null;
                    }

                    byte[] audioBytes = ExtractAudioBytesOptimized(voiceData);

                    if (audioBytes == null || audioBytes.Length == 0)
                    {
                        Log.Instance.Warn("No audio data extracted");
                        return null;
                    }

                    Log.Instance.Info($"Successfully extracted {audioBytes.Length} bytes of audio data");

                    // Determine file extension based on content
                    string fileExtension = GetAudioFileExtension(audioBytes);
                    string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
                    string outputPath = HttpContext.Current.Server.MapPath($@"~/Images/{folderName}/{fileStorageName}.{fileExtension}");

                    // Ensure directory exists
                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        Log.Instance.Info($"Created directory: {directory}");
                    }

                    // Save the audio file
                    File.WriteAllBytes(outputPath, audioBytes);

                    // Verify file was saved
                    if (File.Exists(outputPath))
                    {
                        FileInfo fileInfo = new FileInfo(outputPath);
                        Log.Instance.Info($"Audio file saved successfully: {outputPath}, Size: {fileInfo.Length} bytes, Format: {fileExtension}");

                        return $"/Images/{folderName}/{fileStorageName}.{fileExtension}";
                    }
                    else
                    {
                        Log.Instance.Error("Audio file was not created");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Voice note processing failed");
                    return null;
                }
            }

            private static byte[] ExtractAudioBytesOptimized(string voiceData)
            {
                try
                {
                    if (string.IsNullOrEmpty(voiceData))
                        return null;

                    // Log only first 100 chars for performance
                    Log.Instance.Info($"Voice data starts with: {voiceData.Substring(0, Math.Min(100, voiceData.Length))}");

                    // Handle the specific case with "//" prefix that contains complex data
                    if (voiceData.StartsWith("//"))
                    {
                        Log.Instance.Info("Processing mobile audio data with // prefix");

                        // Remove the // prefix
                        string cleanData = voiceData.Substring(2);

                        // Try multiple decoding strategies
                        return TryMultipleDecodingStrategies(cleanData);
                    }

                    // Handle standard data URLs
                    if (voiceData.StartsWith("data:"))
                    {
                        Log.Instance.Info("Processing as data URL");
                        return ExtractFromDataUrl(voiceData);
                    }

                    // Handle direct Base64
                    if (IsValidBase64Optimized(voiceData))
                    {
                        Log.Instance.Info("Processing as direct Base64");
                        return Convert.FromBase64String(voiceData);
                    }

                    // Final fallback
                    Log.Instance.Info("Processing as raw string data");
                    return Encoding.UTF8.GetBytes(voiceData);
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to extract audio bytes");
                    return null;
                }
            }

            private static byte[] TryMultipleDecodingStrategies(string data)
            {
                // Strategy 1: Try as direct Base64 first
                if (IsValidBase64Optimized(data))
                {
                    try
                    {
                        Log.Instance.Info("Strategy 1: Direct Base64 decoding");
                        return Convert.FromBase64String(data);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Warn($"Strategy 1 failed: {ex.Message}");
                    }
                }

                // Strategy 2: Try URL decoding first, then Base64
                try
                {
                    Log.Instance.Info("Strategy 2: URL decode + Base64");
                    string urlDecoded = HttpUtility.UrlDecode(data);
                    if (IsValidBase64Optimized(urlDecoded))
                    {
                        return Convert.FromBase64String(urlDecoded);
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Warn($"Strategy 2 failed: {ex.Message}");
                }

                // Strategy 3: Try with padding adjustments
                try
                {
                    Log.Instance.Info("Strategy 3: Base64 with padding adjustment");
                    string paddedData = data.PadRight(data.Length + (4 - data.Length % 4) % 4, '=');
                    if (IsValidBase64Optimized(paddedData))
                    {
                        return Convert.FromBase64String(paddedData);
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Warn($"Strategy 3 failed: {ex.Message}");
                }

                // Strategy 4: Treat as raw string (fallback)
                Log.Instance.Info("Strategy 4: Treating as raw string data");
                return Encoding.UTF8.GetBytes(data);
            }

            private static byte[] ExtractFromDataUrl(string dataUrl)
            {
                try
                {
                    var parts = dataUrl.Split(',');
                    if (parts.Length > 1)
                    {
                        string base64Data = parts[1];
                        if (IsValidBase64Optimized(base64Data))
                        {
                            return Convert.FromBase64String(base64Data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Failed to extract from data URL");
                }
                return null;
            }

            private static bool IsValidBase64Optimized(string base64String)
            {
                if (string.IsNullOrEmpty(base64String) || base64String.Length < 4)
                    return false;

                // Quick length check
                if (base64String.Length % 4 != 0)
                {
                    // Try with padding
                    string padded = base64String.PadRight(base64String.Length + (4 - base64String.Length % 4) % 4, '=');
                    base64String = padded;
                }

                // Check for valid Base64 characters
                string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
                if (base64String.Any(c => !base64Chars.Contains(c)))
                    return false;

                try
                {
                    // Test decode only first 1000 characters for performance
                    int testLength = Math.Min(base64String.Length, 1000);
                    testLength = testLength - (testLength % 4); // Ensure multiple of 4

                    if (testLength > 0)
                    {
                        Convert.FromBase64String(base64String.Substring(0, testLength));
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private static string GetAudioFileExtension(byte[] audioData)
            {
                if (audioData == null || audioData.Length < 4)
                    return "aac"; // Default fallback

                // Check for MP3 (starts with ID3 tag or 0xFF 0xFB)
                if ((audioData[0] == 'I' && audioData[1] == 'D' && audioData[2] == '3') ||
                    (audioData[0] == 0xFF && (audioData[1] & 0xE0) == 0xE0))
                {
                    return "mp3";
                }

                // Check for AAC/ADTS
                if (audioData[0] == 0xFF && (audioData[1] & 0xF0) == 0xF0)
                {
                    return "aac";
                }

                // Check for WAV
                if (audioData[0] == 'R' && audioData[1] == 'I' && audioData[2] == 'F' && audioData[3] == 'F')
                {
                    return "wav";
                }

                // Default to AAC
                return "aac";
            }
        }

        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }

        public string ConvertIntoByte1(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }

        public static class FileHandler
        {
            private static readonly long MaxFileSize = 50 * 1024 * 1024;

            public static string ConvertImage(string base64, string dealerName, string sendDateTime, string folderName)
            {
                try
                {
                    string cleanBase64 = CleanBase64String(base64);
                    ValidateBase64(cleanBase64);

                    byte[] bytes = Convert.FromBase64String(cleanBase64);

                    string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
                    string outputPath = HttpContext.Current.Server.MapPath($@"~/Images/{folderName}/{fileStorageName}.jpg");

                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllBytes(outputPath, bytes);
                    Log.Instance.Info($"Image saved: {outputPath}");
                    return $"/Images/{folderName}/{fileStorageName}.jpg";
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Image conversion failed");
                    throw new Exception($"Image conversion failed: {ex.Message}");
                }
            }

            public static string ConvertVideo(string base64, string dealerName, string sendDateTime, string folderName)
            {
                try
                {
                    string cleanBase64 = CleanBase64String(base64);
                    ValidateBase64(cleanBase64);

                    byte[] bytes = Convert.FromBase64String(cleanBase64);

                    string fileExtension = ".mp4"; // Default to mp4
                    string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
                    string outputPath = HttpContext.Current.Server.MapPath($@"~/Images/{folderName}/{fileStorageName}{fileExtension}");

                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllBytes(outputPath, bytes);
                    Log.Instance.Info($"Video saved: {outputPath}");
                    return $"/Images/{folderName}/{fileStorageName}{fileExtension}";
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Video conversion failed");
                    throw new Exception($"Video conversion failed: {ex.Message}");
                }
            }

            private static string CleanBase64String(string base64String)
            {
                if (string.IsNullOrEmpty(base64String))
                    return base64String;

                if (base64String.StartsWith("data:"))
                {
                    var parts = base64String.Split(',');
                    if (parts.Length > 1)
                    {
                        return parts[1];
                    }
                }

                return base64String;
            }

            private static void ValidateBase64(string base64)
            {
                if (string.IsNullOrEmpty(base64))
                    throw new ArgumentException("Base64 string cannot be null or empty");

                string cleanBase64 = CleanBase64String(base64);

                if (cleanBase64.Length % 4 != 0 && cleanBase64.Length > 100)
                    throw new ArgumentException($"Invalid base64 string length: {cleanBase64.Length}");

                var fileSize = (cleanBase64.Length * 3) / 4;
                if (fileSize > MaxFileSize)
                    throw new ArgumentException($"File size too large: {fileSize} bytes");
            }
        }

        //public static class FileHandlerForSticker
        //{
        //    private static readonly long MaxFileSize = 50 * 1024 * 1024;

        //    public static string ConvertImage(string base64, string dealerName, string sendDateTime, string folderName)
        //    {
        //        try
        //        {
        //            string cleanBase64 = CleanBase64String(base64);
        //            ValidateBase64(cleanBase64);

        //            byte[] bytes = Convert.FromBase64String(cleanBase64);

        //            using (MemoryStream ms = new MemoryStream(bytes))
        //            {
        //                using (Image image = Image.FromStream(ms))
        //                {
        //                    string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
        //                    string outputPath = HttpContext.Current.Server.MapPath($@"~/Images/{folderName}/{fileStorageName}.jpg");

        //                    string directory = Path.GetDirectoryName(outputPath);
        //                    if (!Directory.Exists(directory))
        //                    {
        //                        Directory.CreateDirectory(directory);
        //                    }

        //                    image.Save(outputPath, ImageFormat.Jpeg);
        //                    Log.Instance.Info($"Image saved: {outputPath}");
        //                    return $"/Images/{folderName}/{fileStorageName}.jpg";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Instance.Error(ex, "Image conversion failed");
        //            throw new Exception($"Image conversion failed: {ex.Message}");
        //        }
        //    }

        //    public static string ConvertVideo(string base64, string dealerName, string sendDateTime, string folderName)
        //    {
        //        try
        //        {
        //            string cleanBase64 = CleanBase64String(base64);
        //            ValidateBase64(cleanBase64);

        //            byte[] bytes = Convert.FromBase64String(cleanBase64);

        //            string fileExtension = ".mp4"; // Default to mp4
        //            string fileStorageName = $"{dealerName}_{sendDateTime}_{Guid.NewGuid():N}";
        //            string outputPath = HttpContext.Current.Server.MapPath($@"~/Videos/{folderName}/{fileStorageName}{fileExtension}");

        //            string directory = Path.GetDirectoryName(outputPath);
        //            if (!Directory.Exists(directory))
        //            {
        //                Directory.CreateDirectory(directory);
        //            }

        //            File.WriteAllBytes(outputPath, bytes);
        //            Log.Instance.Info($"Video saved: {outputPath}");
        //            return $"/Videos/{folderName}/{fileStorageName}{fileExtension}";
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Instance.Error(ex, "Video conversion failed");
        //            throw new Exception($"Video conversion failed: {ex.Message}");
        //        }
        //    }

        //    private static string CleanBase64String(string base64String)
        //    {
        //        if (string.IsNullOrEmpty(base64String))
        //            return base64String;

        //        if (base64String.StartsWith("data:"))
        //        {
        //            var parts = base64String.Split(',');
        //            if (parts.Length > 1)
        //            {
        //                return parts[1];
        //            }
        //        }

        //        return base64String;
        //    }

        //    private static void ValidateBase64(string base64)
        //    {
        //        if (string.IsNullOrEmpty(base64))
        //            throw new ArgumentException("Base64 string cannot be null or empty");

        //        string cleanBase64 = CleanBase64String(base64);

        //        if (cleanBase64.Length % 4 != 0 && cleanBase64.Length > 100)
        //            throw new ArgumentException($"Invalid base64 string length: {cleanBase64.Length}");

        //        var fileSize = (cleanBase64.Length * 3) / 4;
        //        if (fileSize > MaxFileSize)
        //            throw new ArgumentException($"File size too large: {fileSize} bytes");
        //    }
        //}

        // DTO Classes
        public class SuccessResponse { }

        public class ComplaintsDto
        {
            public int? SOID { get; set; }
            public int? CustomerID { get; set; }
            public int? DealerID { get; set; }
            public int? ProductID { get; set; }
            public int? ProductNatureID { get; set; }
            public int? ThinningSolventID { get; set; }
            public int? AppliedToolID { get; set; }
            public string ProductBatchNo { get; set; }
            public string ShakingTime { get; set; }
            public string ColorCode { get; set; }
            public decimal Quantity { get; set; }
            public string TinningRatio { get; set; }
            public string NoOfCoatsApplied { get; set; }
            public string TimeDurationWithinCoats { get; set; }
            public bool WetSampleAttached { get; set; }
            public string SubstrateColor { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string Picture1 { get; set; }
            public string Picture2 { get; set; }
            public string StickerPicture { get; set; }
            public string VoiceNote { get; set; }
            public string Video { get; set; }
            public List<SubstrateDto> SubstrateModel { get; set; }
            public List<NatureOfComplaitDto> NatureOfComplaintModel { get; set; }
        }

        public class SubstrateDto
        {
            public string Name { get; set; }
            public bool IsChecked { get; set; }
            public int SubstrateID { get; set; }
        }

        public class NatureOfComplaitDto
        {
            public string Name { get; set; }
            public bool IsChecked { get; set; }
            public int NatureOfComplaitID { get; set; }
        }
    }
}
