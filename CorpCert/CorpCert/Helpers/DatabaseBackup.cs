using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace L2Test.Helpers
{
    public class DatabaseBackup
    {
        public string Backup(string path)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["L2TestConnection"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            string backupFileName = String.Format("{0}{1}-{2}.BAK", path, sqlConStrBuilder.InitialCatalog, DateTime.Now.ToString("yyyy-MM-dd"));

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                string query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'", sqlConStrBuilder.InitialCatalog, backupFileName);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return backupFileName;
        }

        public void Restore(HttpPostedFileBase file)
        {
            string BackupPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Backup"), "Database.BAK");
            string connectionString = ConfigurationManager.ConnectionStrings["Restore"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            string connectionStringDest = ConfigurationManager.ConnectionStrings["L2TestConnection"].ConnectionString;
            var sqlConStrBuilderDest = new SqlConnectionStringBuilder(connectionStringDest);
            Upload(file);

            //Set DB to single user so that no other user is connected (required for restore)
            string Query1 ="ALTER DATABASE [" + sqlConStrBuilderDest.InitialCatalog + "] SET Single_User WITH Rollback Immediate";
            //Restore the DB
            string Query2 = "RESTORE DATABASE [" + sqlConStrBuilderDest.InitialCatalog + "] FROM DISK='" + BackupPath + "'";
            //Set DB to multi-user so that it can be connected to again now that the restore is complete
            string Query3 = "ALTER DATABASE [" + sqlConStrBuilderDest.InitialCatalog + "] SET Multi_User";

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                connection.Open();
                var command1 = new SqlCommand(Query1, connection);
                command1.ExecuteNonQuery();
                var command2 = new SqlCommand(Query2, connection);
                command2.ExecuteNonQuery();
                var command3 = new SqlCommand(Query3, connection);
                command3.ExecuteNonQuery();
            }
        }

        public void Upload(HttpPostedFileBase file)
        {
            if ((file != null) && file.ContentLength > 0)
            {
                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Backup"), "Database.BAK");
                file.SaveAs(path);
            }
        }
    }
}