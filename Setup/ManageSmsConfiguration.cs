using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FOS.DataLayer;
using FOS.Shared;

namespace FOS.Setup
{
    // Backs the single-row dbo.Tbl_SmsConfiguration table - raw ADO.NET against
    // dbContext.Database.Connection.ConnectionString rather than an EF function
    // import, same convention as the other recently-added tables in this app,
    // to avoid touching the fragile .edmx for a table this new.
    public class ManageSmsConfiguration
    {
        // Only the 5 templates save one-by-one (each own Save button); URL/Login/
        // Password save together via SaveCredentials below. The column name here
        // comes from the caller (whichever template Save button was clicked) - so
        // it's whitelisted to just these 5 before being interpolated into the
        // UPDATE statement, since it can't be parameterized like a normal value.
        private static readonly Dictionary<string, int> TemplateFieldLengths = new Dictionary<string, int>
        {
            { "Template1", 1000 },
            { "Template2", 1000 },
            { "Template3", 1000 },
            { "Template4", 1000 },
            { "Template5", 1000 }
        };

        public static SmsConfigurationData Get()
        {
            using (FOSDataModel dbContext = new FOSDataModel())
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(
                @"SELECT Url, LoginId, Password, Mask, Template1, Template2, Template3, Template4, Template5, UpdatedOn
                  FROM dbo.Tbl_SmsConfiguration WHERE ID = 1", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SmsConfigurationData
                        {
                            Url = reader["Url"] as string,
                            LoginId = reader["LoginId"] as string,
                            Password = reader["Password"] as string,
                            Mask = reader["Mask"] as string,
                            Template1 = reader["Template1"] as string,
                            Template2 = reader["Template2"] as string,
                            Template3 = reader["Template3"] as string,
                            Template4 = reader["Template4"] as string,
                            Template5 = reader["Template5"] as string,
                            UpdatedOn = reader["UpdatedOn"] as DateTime?
                        };
                    }
                }
            }

            // No row yet (table just created, migration script hasn't been applied/seeded) -
            // callers (CBSSmsSender, the welcome SMS template) fall back to their existing
            // Web.config/hardcoded defaults when a field here comes back blank.
            return new SmsConfigurationData();
        }

        // Saves exactly one template (whichever Save button the user clicked), not
        // the whole row - per the explicit "only templates save one by one" requirement.
        public static void SaveTemplateField(string field, string value, int updatedBy)
        {
            int length;
            if (!TemplateFieldLengths.TryGetValue(field, out length))
            {
                throw new ArgumentException("Unknown SMS template field: " + field);
            }

            using (FOSDataModel dbContext = new FOSDataModel())
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(
                "UPDATE dbo.Tbl_SmsConfiguration SET [" + field + "] = @Value, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE ID = 1",
                conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Value", SqlDbType.NVarChar, length) { Value = (object)value ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@UpdatedOn", SqlDbType.DateTime) { Value = DateTime.Now });
                cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedBy });

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                {
                    // The seed row (ID = 1) from SQL/CreateTbl_SmsConfiguration.sql is
                    // missing - nothing to update. Surface this instead of silently
                    // doing nothing, so it isn't mistaken for a successful save.
                    throw new InvalidOperationException("No SMS configuration row exists yet - run SQL/CreateTbl_SmsConfiguration.sql first.");
                }
            }
        }

        // URL/Login/Password/Mask save together as one group, unlike the templates.
        public static void SaveCredentials(string url, string loginId, string password, string mask, int updatedBy)
        {
            using (FOSDataModel dbContext = new FOSDataModel())
            using (var conn = new SqlConnection(dbContext.Database.Connection.ConnectionString))
            using (var cmd = new SqlCommand(
                @"UPDATE dbo.Tbl_SmsConfiguration
                  SET Url = @Url, LoginId = @LoginId, Password = @Password, Mask = @Mask,
                      UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy
                  WHERE ID = 1",
                conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Url", SqlDbType.NVarChar, 300) { Value = (object)url ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@LoginId", SqlDbType.NVarChar, 200) { Value = (object)loginId ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 200) { Value = (object)password ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Mask", SqlDbType.NVarChar, 100) { Value = (object)mask ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@UpdatedOn", SqlDbType.DateTime) { Value = DateTime.Now });
                cmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = updatedBy });

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                {
                    throw new InvalidOperationException("No SMS configuration row exists yet - run SQL/CreateTbl_SmsConfiguration.sql first.");
                }
            }
        }
    }
}
