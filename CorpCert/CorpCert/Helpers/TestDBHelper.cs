using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using L2Test.Models;

namespace L2Test.Helpers
{
    public class TestDBHelper
    {
        public void NewQuestion(string Question, string Pic, string Category, string[] Answers, int[] IsCorrect)
        {
            Password Pass = new Password();
            int QuestionID = 0;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO TestQuestions (Question, Pic, Category) VALUES (@question, @pic, @category) SET @ID = SCOPE_IDENTITY();";
                    command.Parameters.AddWithValue("@question", Question);
                    command.Parameters.AddWithValue("@pic", Pic);
                    command.Parameters.AddWithValue("@category", Category);
                    command.Parameters.Add("@ID", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        QuestionID = (int)command.Parameters["@ID"].Value;
                    }
                    catch { throw; }
                }
            }

            for(int i = 0; i < Answers.Length; i++)
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "INSERT INTO TestAnswers (QuestionID, Answer, AnswerID, IsCorrect) VALUES (@question, @answer, @answerID, @correct)";
                        command.Parameters.AddWithValue("@question", QuestionID);
                        command.Parameters.AddWithValue("@answer", Answers[i]);
                        command.Parameters.AddWithValue("@answerID", Pass.Generate());
                        command.Parameters.AddWithValue("@correct", IsCorrect[i]);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                        catch { throw; }
                    }
                }
            }
        }

        public List<TestQuestions> GetQuestions()
        {
            var Questions = new List<TestQuestions>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                connection.Open();
                string query = "SELECT * FROM TestQuestions";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Test = new TestQuestions();
                            Test.Key = reader.GetInt32(reader.GetOrdinal("P_Id"));
                            Test.Question = reader.GetString(reader.GetOrdinal("Question"));
                            Test.Pic = reader.GetString(reader.GetOrdinal("Pic"));
                            Test.Category = reader.GetString(reader.GetOrdinal("Category"));
                            Test.Answers = null;
                            Questions.Add(Test);
                        }
                    }
                }
            }

            foreach(var Q in Questions)
            {
                var Answers = new List<TestAnswers>();
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
                {
                    connection.Open();
                    string query = "SELECT * FROM TestAnswers WHERE QuestionID = " + Q.Key;
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var List = new TestAnswers();
                                List.Key = reader.GetInt32(reader.GetOrdinal("P_Id"));
                                List.QuestionID = reader.GetInt32(reader.GetOrdinal("QuestionID"));
                                List.Answer = reader.GetString(reader.GetOrdinal("Answer"));
                                List.AnswerID = reader.GetString(reader.GetOrdinal("AnswerID"));
                                List.IsCorrect = (bool)reader["IsCorrect"];
                                Answers.Add(List);
                            }
                        }
                    }
                }
                Q.Answers = Answers;
            }

            return Questions;
        }

        public List<TestQuestions> GetQuestion(int Key)
        {
            var Question = new List<TestQuestions>();
            var Answers = new List<TestAnswers>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                connection.Open();
                string query = "SELECT * FROM TestAnswers WHERE QuestionID = " + Key;
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var List = new TestAnswers();
                            List.Key = reader.GetInt32(reader.GetOrdinal("P_Id"));
                            List.QuestionID = reader.GetInt32(reader.GetOrdinal("QuestionID"));
                            List.Answer = reader.GetString(reader.GetOrdinal("Answer"));
                            List.AnswerID = reader.GetString(reader.GetOrdinal("AnswerID"));
                            List.IsCorrect = (bool)reader["IsCorrect"];
                            Answers.Add(List);
                        }
                    }
                }
            }
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                connection.Open();
                string query = String.Format("SELECT * FROM TestQuestions WHERE P_Id = '{0}'", Key);
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Test = new TestQuestions();
                            Test.Key = reader.GetInt32(reader.GetOrdinal("P_Id"));
                            Test.Question = reader.GetString(reader.GetOrdinal("Question"));
                            Test.Pic = reader.GetString(reader.GetOrdinal("Pic"));
                            Test.Category = reader.GetString(reader.GetOrdinal("Category"));
                            Test.Answers = Answers;
                            Question.Add(Test);
                        }
                    }
                }
            }
            return Question;
        }

        public void RemoveQuestion(int Key)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM TestQuestions WHERE P_Id = @Key";
                    command.Parameters.AddWithValue("@Key", Key);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch { throw; }
                }
            }
            RemoveAnswers(Key);
        }

        private static void RemoveAnswers(int Key)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM TestAnswers WHERE QuestionID = @Key";
                    command.Parameters.AddWithValue("@Key", Key);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch { throw; }
                }
            }
        }

        public static string PurgeTest()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM TestQuestions";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        return "Test DB Question PurgeTest Error: " + e.Message;
                    }
                }
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM TestAnswers";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        return "Test DB Answer PurgeTest Error: " + e.Message;
                    }
                }
                return "";
            }
        }

    }
}