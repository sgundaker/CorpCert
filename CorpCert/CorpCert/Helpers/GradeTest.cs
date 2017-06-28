using L2Test.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace L2Test.Helpers
{
    public class GradeTest
    {
        public float CorrectAnswers = 0f;
        public float PossibleAnswers = 0f;
        public int PassingScore = Config.GetInt("PassingScore"); //this is out of 100.
        public List<GradeCatagories> categoryGrades = new List<GradeCatagories>();

        //Control the grading proccess
        public Dictionary<string, float> Grading(List<TestResultModel> TechAnswers)
        {
            TechDBHelper TechDB = new TechDBHelper();
            string TName = TechDB.GetName(TechAnswers[0].tech);
            string Results = PrintResults(TechAnswers);
            SaveResults(Results, TName);
            return FinalScore();
        }

        //Get a list of the questions that were on the techs test.
        public List<TestQuestions> GetTestQuestions(List<TestResultModel> TechAnswers)
        {
            List<TestQuestions> FullTest = Test.QuestionList();
            List<TestQuestions> TestQuestions = FullTest.ToList();
            List<int> TempList = new List<int>();

            foreach (var QID in TechAnswers)
            {
                TempList.Add(QID.question);
            }

            foreach (var Question in FullTest)
            {
                if (!TempList.Contains(Question.Key))
                {
                    TestQuestions.Remove(Question);
                }
            }
            return TestQuestions;
        }

        //grades test and writes results of each answer to HTML string
        public string PrintResults(List<TestResultModel> TechAnswers)
        {
            List<TestQuestions> TestQuestions = GetTestQuestions(TechAnswers);
            Dictionary<int, string[]> AnswerDict = TechAnswers.ToDictionary(x => x.question, x => x.answers);
            string ResultString = "";

            //Account for unanswered questions so that there are no null values and unanswered questions are marked incorrect
            foreach (var Question in TestQuestions)
            {
                if (AnswerDict[Question.Key] == null)
                {
                    AnswerDict[Question.Key] = new string[] { "Unanswered" }; //Unanswered is 8 characters, all valid keys are 10 so this can never be a correct answer
                }
            }

            StringBuilder sb = new StringBuilder(ResultString);

            foreach (var Question in TestQuestions)
            {
                PossibleAnswers++;
                int TempPossible = 0;
                int TempCorrect = 0;

                //Question block
                sb.Append("<li class='well' id='");
                sb.Append(Question.Key);
                sb.Append("'><p class='TestResultQuestion'>");

                if (Question.Pic != "No Image" && Question.Pic != null)
                    {
                        sb.Append("<img src='/Content/Images/");
                        sb.Append(Question.Pic);
                        sb.Append("' height='200' id='picSample'></br>");
                    }
                sb.Append("category:<b>");
                sb.Append(Question.Category);
                sb.Append("</b> - ");
                sb.Append(Question.Question);
                sb.Append("</p>");

                char Index = 'A';
                foreach (var A in Question.Answers)
                {
                    TempPossible++;
                    string[] Selected = AnswerDict[Question.Key];

                    if (A.IsCorrect == true)
                    {
                        if (Selected.Contains(A.AnswerID)) //Correct answer selected
                        {
                            TempCorrect++;
                            sb.Append("<p class='CorrectAnswer' id='");
                            sb.Append(A.AnswerID);
                            sb.Append("'><img src='/selected.png' height='20' width='20'>");
                            sb.Append(Index);
                            sb.Append(". ");
                            sb.Append(A.Answer);
                            sb.Append("</p>");

                        }
                        else { //Correct answer not selected
                            sb.Append("<p class='WrongAnswer' id='");
                            sb.Append(A.AnswerID);
                            sb.Append("'>");
                            sb.Append(Index);
                            sb.Append(". ");
                            sb.Append(A.Answer);
                            sb.Append("</p>");
                        }
                    }
                    else
                    {
                        if (Selected.Contains(A.AnswerID)) //Incorrect answer selected
                        {
                            sb.Append("<p class='WrongAnswer' id='");
                            sb.Append(A.AnswerID);
                            sb.Append("'><img src='/selected.png' height='20' width='20'>");
                            sb.Append(Index);
                            sb.Append(". ");
                            sb.Append(A.Answer);
                            sb.Append("</p>");
                        }
                        else { //Incorrect answer not selected
                            TempCorrect++;
                            sb.Append("<p class='TestAnswer' id='");
                            sb.Append(A.AnswerID);
                            sb.Append("'>");
                            sb.Append(Index);
                            sb.Append(". ");
                            sb.Append(A.Answer);
                            sb.Append("</p>");
                        }
                    }
                    if (Index != 'Z') Index++;
                }
                    if (TempPossible == TempCorrect)
                    {
                        CorrectAnswers++;
                        if (categoryGrades.Exists(x => x.category == Question.Category))
                        {
                            foreach (var c in categoryGrades)
                            {
                                if (c.category == Question.Category)
                                {
                                    c.CorrectAnswers++;
                                    c.PossibleAnsweres++;
                                }
                            }
                        }
                        else
                        {
                            var CatList = new GradeCatagories();
                            CatList.category = Question.Category;
                            CatList.CorrectAnswers = 1f;
                            CatList.PossibleAnsweres = 1f;
                            categoryGrades.Add(CatList);
                        }
                    }
                    else
                    {
                        if (categoryGrades.Exists(x => x.category == Question.Category))
                        {
                            foreach (var c in categoryGrades)
                            {
                                if (c.category == Question.Category)
                                {
                                    c.PossibleAnsweres++;
                                }
                            }
                        }
                        else
                        {
                            var CatList = new GradeCatagories();
                            CatList.category = Question.Category;
                            CatList.PossibleAnsweres = 1f;
                            categoryGrades.Add(CatList);
                        }
                    }

                foreach(var Q in TechAnswers)
                {
                    if((Q.question == Question.Key) && (Q.comment != "") && (Q.comment != null))
                    {
                        sb.Append("<p class='TechComment'>");
                        sb.Append(Q.comment);
                        sb.Append("</p>");
                    }
                }
            }

        //End question block
        sb.Append("</li>");
        ResultString = sb.ToString();
        return ResultString;
    }

        //Save a copy of the graded test
        public string SaveResults(string Results, string Tech)
        {
            string WebPageString = "";
            string Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            string FileDate = DateTime.Now.ToString("yyyy-MM-dd_hh-mm");
            String TestPath = HttpContext.Current.Server.MapPath("/Views/Tests/Graded/" + FileDate + "_" + Tech + ".html");
            String CssPath = "/Content/Site.css";
            StringBuilder sb = new StringBuilder(WebPageString);

            //Head
            sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'/><meta name='viewport' content='width=device-width'/><link rel='stylesheet' href='http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css'><link rel='stylesheet' type='text/css' href='");
            sb.Append(CssPath);
            sb.Append("'><title>");
            sb.Append(Tech);
            sb.Append(" - ");
            sb.Append(Date);
            sb.Append("</title></head><body>");

            //Body 
            sb.Append("<h2>Final results for ");
            sb.Append(Tech);
            sb.Append(". Test completed ");
            sb.Append(Date);
            sb.Append("</h2></br>");

            //Final Score
            sb.Append("<div class='LeadViewResults'>");
            if ((CorrectAnswers / PossibleAnswers * 100f) >= PassingScore) { sb.Append("<p class='greenResult'>"); }
            else { sb.Append("<p class='redResult'>"); }
            sb.Append("<b>Final Score: ");
            sb.Append((CorrectAnswers / PossibleAnswers * 100f));
            sb.Append("%</b></br>Questions asked:");
            sb.Append(PossibleAnswers);
            sb.Append("  Correctly Answered:");
            sb.Append(CorrectAnswers);
            sb.Append("</p>");

            //List the categorys
            foreach (var c in categoryGrades)
            {
                if ((c.CorrectAnswers / c.PossibleAnsweres * 100f) >= PassingScore) { sb.Append("<p class='greenResult'"); }
                else if ((c.CorrectAnswers / c.PossibleAnsweres * 100f) > (PassingScore - 10)) { sb.Append("<p class='yellowResult'"); }
                else { sb.Append("<p class='redResult'"); }

                sb.Append(">");
                sb.Append(c.category);
                sb.Append(": ");
                sb.Append((c.CorrectAnswers / c.PossibleAnsweres * 100f));
                sb.Append("%</br>Questions asked:");
                sb.Append(c.PossibleAnsweres);
                sb.Append("  Correctly Answered:");
                sb.Append(c.CorrectAnswers);
                sb.Append("</p>");
            }
            sb.Append("</div>");

            //Techs answered below

            sb.Append("</br></br><h2>Answers selected by the technician are preceded by a white checkmark:</h2>");
            sb.Append("<ul class='Test' style='list-style-type:none'>");
            sb.Append(Results);
            sb.Append("</ul><script src='http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js'></script></body></html>");

            WebPageString = sb.ToString();

            File.WriteAllText(TestPath, WebPageString);

            return TestPath;
        }

        //Gets the final score of the test
        private Dictionary<string, float> FinalScore()
        {
            Dictionary<string, float> results = new Dictionary<string, float>();
            results.Add("FinalScore", (CorrectAnswers / PossibleAnswers) * 100f);
            foreach (var category in categoryGrades)
            {
                results.Add(category.category, (category.CorrectAnswers / category.PossibleAnsweres) * 100f);
            }
            return results;
        }

        //Create HTML to show tech after test is graded
        public string Submit(IEnumerable<TestResultModel> json)
        {
            List<TestResultModel> TechsAnswers = new List<TestResultModel>();
            GradeTest TestHelp = new GradeTest();
            string grades = "";
            foreach (var answer in json)
            {
                TechsAnswers.Add(answer);
            }
            Dictionary<string, float> results = TestHelp.Grading(TechsAnswers);
            StringBuilder sb = new StringBuilder(grades);

            sb.Append("<div class='TechViewResults'>");
            foreach (var grade in results)
            {   //Color the results
                if (grade.Value >= PassingScore) { sb.Append("<p class='greenResult'"); }
                else if (grade.Value > PassingScore - 10) { sb.Append("<p class='yellowResult'"); }
                else { sb.Append("<p class='redResult'"); }

                //Change font size for overall score
                if (grade.Key == "FinalScore")
                    sb.Append("style='font-size:3em'");

                //Finish listing the category scores
                sb.Append(">");
                sb.Append(grade.Key);
                sb.Append(": ");
                sb.Append(grade.Value);
                sb.Append("%</p>");

            }
            sb.Append("</br><center><H3>A copy of the test has been saved for management.</H3></center></div>");
            grades = sb.ToString();

            return grades;
        }

        //Save a copy of the test as it was when the submit button was pressed, prior to grading.
        public void Archive(string html, string tech)
        {
            string TName = "Unknown Tech";
            TechDBHelper Tdbhelp = new TechDBHelper();
            var TechDB = Tdbhelp.ListAll();

            foreach (var ID in TechDB)
            {
                if (ID.TechID == tech)
                    TName = ID.TechName;
            }

            string WebPageString = "";
            string FileDate = DateTime.Now.ToString("yyyy-MM-dd_hh-mm");
            String TestPath = HttpContext.Current.Server.MapPath("/Views/Tests/Ungraded/" + FileDate + "_" + TName + ".html");
            String CssPath = HttpContext.Current.Server.MapPath("/Content/Site.css");
            StringBuilder sb = new StringBuilder(WebPageString);

            sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'/><meta name='viewport' content='width=device-width'/><link rel='stylesheet' href='http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css'><link rel='stylesheet' type='text/css' href='");
            sb.Append(CssPath);
            sb.Append("'><title>L2 Test</title></head><body>");
            sb.Append(html);
            sb.Append("</body></html>");

            WebPageString = sb.ToString();

            File.WriteAllText(TestPath, WebPageString);
        }
    }
}