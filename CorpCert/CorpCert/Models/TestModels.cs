using L2Test.Helpers;
using System.Collections.Generic;

namespace L2Test.Models
{
    public static class Test
    {
        public static List<TestQuestions> QuestionList()
        {
            TestDBHelper dbhelp = new TestDBHelper();
            List<TestQuestions> FullList = dbhelp.GetQuestions();
            return FullList;
        }

        public static List<TestQuestions> SingleQuestion(string uid)
        {
            int id = System.Int32.Parse(uid);
            TestDBHelper dbhelp = new TestDBHelper();
            List<TestQuestions> Question = dbhelp.GetQuestion(id);
            return Question;
        }
    }

    public class TestQuestions
    {
        public int Key { get; set; }
        public string Question { get; set; }
        public string Pic { get; set; }
        public string Category { get; set; }
        public List<TestAnswers> Answers { get; set; }
}

    public class TestAnswers
    {
        public int Key { get; set; }
        public int QuestionID { get; set; }
        public string Answer { get; set; }
        public string AnswerID { get; set; }
        public bool? IsCorrect { get; set; } //0 = correct 1 = incorrect
    }

    public class GradeCatagories
    {
        public string category { get; set; }
        public float PossibleAnsweres { get; set; }
        public float CorrectAnswers { get; set; }
    }

    public class TestResultModel
    {
        public string tech { get; set; }
        public int question { get; set; }
        public string[] answers { get; set; }
        public string comment { get; set; }
        public static int PassingScore = Config.GetInt("PassingScore"); //this is out of 100.

    }

    public class TestEditModel
    {
        public string Question { get; set; }
        public string QuestionID { get; set; }
        public string Pic { get; set; }
        public string Catagory { get; set; }
        public string CategoryDropDown { get; set; }
        public string[] Answers { get; set; }
        public int[] Corrects { get; set; }

        public void NewQuestion(TestEditModel jsonData)
        {
            string Question = jsonData.Question;
            string Catagory = jsonData.Catagory;
            string Pic = jsonData.Pic.TrimStart('\"').TrimEnd('\"');
            TestDBHelper Help = new TestDBHelper();

            if (jsonData.CategoryDropDown != "1")
                Catagory = jsonData.CategoryDropDown;

            if (Question == null)
                Question = " ";

            if (Catagory == null)
                Catagory = "No Catagory Selected";

            Help.NewQuestion(Question, Pic, Catagory, jsonData.Answers, jsonData.Corrects);
        }

        public void EditQuestion(TestEditModel jsonData)
        {
            string Question = jsonData.Question;
            string Catagory = jsonData.Catagory;
            string Pic = jsonData.Pic.TrimStart('\"').TrimEnd('\"');
            TestDBHelper Help = new TestDBHelper();

            if (jsonData.CategoryDropDown != "1")
                Catagory = jsonData.CategoryDropDown;

            if (Question == null)
                Question = " ";

            if (Catagory == null)
                Catagory = "No Catagory Selected";

            EditTest.Delete(jsonData.QuestionID);
            Help.NewQuestion(Question, Pic, Catagory, jsonData.Answers, jsonData.Corrects);
        }
    }
}