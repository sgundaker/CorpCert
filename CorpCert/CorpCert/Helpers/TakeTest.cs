using System;
using System.Collections.Generic;
using System.Linq;
using L2Test.Models;
using System.Text;

namespace L2Test.Helpers
{
    public class TakeTest
    {
        public string StartTest()
        {
            int NumberOfQuestions = Config.GetInt("NumberOfQuestions");
            int QuestionNumber = 1;
            var List = Test.QuestionList();
            Random rnd = new Random();
            //take list of all questions, put them in random order and truncate any beyond the number of question that should be on each test.
            var TestList = List.OrderBy(x => rnd.Next()).ToList().Take(NumberOfQuestions); 
            string QuestionString = "";
            foreach (var Question in TestList)
            {
                char Index = 'A';
                List<SortModel> SortedAnswers = new List<SortModel>();
                //Randomize the order of answeres
                foreach (var A in Question.Answers)
                {
                    SortedAnswers.Add(new SortModel(A.Answer, A.AnswerID));
                }

                SortedAnswers = SortedAnswers.OrderBy(x => rnd.Next()).ToList();
                //Build String.
                StringBuilder sb = new StringBuilder(QuestionString);
                sb.Append("<li class='well' id='");
                sb.Append(Question.Key);
                sb.Append("'><p class='TestQuestion'>");

                if (Question.Pic != "No Image" && Question.Pic != null)
                {
                    sb.Append("<img src='/Content/Images/");
                    sb.Append(Question.Pic);
                    sb.Append("' height='200' id='picSample'></br>");
                }

                
                sb.Append(QuestionNumber);
                QuestionNumber++;
                sb.Append(": ");
                sb.Append(Question.Question);
                sb.Append("</p>");

                sb.Append("<div class='comment-box'><span class='ShowComment' tabindex='0'>Add Comment</button></span><div class='HideComment'><textarea class='form-control' name='Comment' maxlength='400'></textarea></div></div>");

                foreach (var A in SortedAnswers)
                {
                    sb.Append("<p class='TestAnswer' id='");
                    sb.Append(A.AnswerKey);
                    sb.Append("'>");
                    sb.Append(Index);
                    sb.Append(". ");
                    sb.Append(A.Answer);
                    sb.Append("</p>");
                    if (Index != 'Z')
                    {
                        Index++;
                    }
                }
                
                sb.Append("</li>");
                QuestionString = sb.ToString();
            }
            return QuestionString;
        }
    }

    public class SortModel
    {
        public string Answer { get; set; }
        public string AnswerKey { get; set; }

        public SortModel(string answer, string aid)
        {
            this.Answer = answer;
            this.AnswerKey = aid;
        }
    }
}