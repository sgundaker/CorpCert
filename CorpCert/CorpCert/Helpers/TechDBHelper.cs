using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using L2Test.Models;

namespace L2Test.Helpers
{
    public class TechDBHelper
    {
        public void NewID(string tech, string techID)
        {
            PurgeTechs();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO Login (Tech, TechID, Time) VALUES (@tech, @techID, @time)";
                    command.Parameters.AddWithValue("@tech", tech);
                    command.Parameters.AddWithValue("@techID", techID);
                    command.Parameters.AddWithValue("@time", DateTime.Now);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch { throw; }
                }
            }
        }

        public List<TechModels> CheckID(string ID)
        {
            var TechID = new List<TechModels>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                connection.Open();
                string query = String.Format("SELECT * FROM Login WHERE TechID = '{0}'", ID);
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Tech = new TechModels();
                            Tech.TechKey = reader.GetInt32(reader.GetOrdinal("P_Id"));
                            Tech.TechName = reader.GetString(reader.GetOrdinal("Tech"));
                            Tech.TechID = ID;
                            Tech.Time = reader.GetDateTime(reader.GetOrdinal("Time"));

                            TechID.Add(Tech);
                        }
                    }
                }
            }
            return TechID;
        }

        public List<TechModels> ListAll()
        {
            var TechID = new List<TechModels>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                connection.Open();
                string query = String.Format("SELECT * FROM Login");
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Tech = new TechModels();
                            Tech.TechKey = reader.GetInt32(reader.GetOrdinal("P_Id"));
                            Tech.TechName = reader.GetString(reader.GetOrdinal("Tech"));
                            Tech.TechID = reader.GetString(reader.GetOrdinal("TechID"));
                            Tech.Time = reader.GetDateTime(reader.GetOrdinal("Time"));

                            TechID.Add(Tech);
                        }
                    }
                }
            }
            return TechID;
        }

        public string GetName(string id)
        {
            string Result = "";
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                connection.Open();
                string query = "SELECT Tech FROM Login WHERE TechID = '" + id + "'";
                SqlCommand Command = new SqlCommand(query, connection);
                var DBResult = Command.ExecuteScalar();
                Result = Convert.ToString(DBResult);
                connection.Close();
            }
            return Result;
        }

        public void PurgeTechs()
        {
            int Timeout = Config.GetInt("TimeToStartTest");
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText ="DELETE FROM Login WHERE Time < DATEADD(day, -" + Timeout + ", GETDATE())";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine("TechDBHelper.PurgeTechs Error: " + e.Message);
                    }
                }
            }
        }
    }
}