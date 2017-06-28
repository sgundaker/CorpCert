using System;
using System.Collections.Generic;
using L2Test.Models;
using System.Text;

//temp
namespace L2Test.Helpers
{
    public class EditTest
    {
        public string EditListString()
        {
            var List = Test.QuestionList();
            List.Reverse();
            string QuestionString = "";
            foreach (var Question in List)
            {
                char Index = 'A';
                StringBuilder sb = new StringBuilder(QuestionString);
                sb.Append("<li class='well'><a href='/Home/EditQuestion/");
                sb.Append(Question.Key);
                sb.Append("' class='TestEdit'>Edit</a><p class='TestQuestion'>");
                sb.Append(Question.Question);

                if (Question.Pic != "No Image" && Question.Pic != null)
                {
                    sb.Append("</br><img src='/Content/Images/");
                    sb.Append(Question.Pic);
                    sb.Append("' height='200' style='margin:5px; border:1px solid #f9f9f9; border-radius:3px;'></br>");
                }

                sb.Append("</p>");

                foreach(var A in Question.Answers)
                {
                    sb.Append("<p class='TestAnswer");
                    sb.Append(A.IsCorrect);
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

                sb.Append("<p class='Testcategory'>category: ");
                sb.Append(Question.Category);
                sb.Append("</p>");
                QuestionString = sb.ToString();
            }
            return QuestionString;
        }

        public string SingleQuestionString(string uid)
        {
            var List = Test.SingleQuestion(uid);
            string QuestionString = "";
            foreach (var Question in List)
            {
                StringBuilder sb = new StringBuilder(QuestionString);
                sb.Append("<fieldset>");

                if (Question.Pic != "No Image" && Question.Pic != null)
                {
                    sb.Append("<center><img src='/Content/Images/");
                    sb.Append(Question.Pic);
                    sb.Append("' height='200' id='picSample' style='margin:5px; border:1px solid white; border-radius:3px;'></center></br>");
                }

                sb.Append("<div class='form-group col-md-8'><label class='col-md-4 control-label' for='formQuestion'>Question Text: </label><div class='col-md-8'><textarea class='col-md-12' id='formQuestion' name='formQuestion' maxlength='899'>");
                sb.Append(Question.Question);
                sb.Append("</textarea></div></div><div class='form-group col-md-8'><label class='col-md-4 control-label' for='formCategory'>Select Category</label><div class='col-md-8'><select id='formCategory' name='formCategory' class='form-control'><option value='1'>New Catagoy</option>");
                sb.Append(CategoryList(Question.Category));
                sb.Append("</select></div></div><div class='form-group col-md-8'><label class='col-md-4 control-label' for='newCategory'>New Category</label><div class='col-md-8'><input id='newCategory' name='newCategory' type='text' class='form-control input-md'></div></div><input type='hidden' name='picURL' id='picURL' value='");
                sb.Append(Question.Pic);
                sb.Append("'>");

                foreach (var A in Question.Answers)
                {
                    sb.Append("<div class='form-group col-md-8'><label class='col-md-4 control-label' for='formAnswer'>Answer: </label><div class='col-md-8'><div class='input-group'><input name='formAnswer' class='form-control' type='text' maxlength='255' value='");
                    sb.Append(A.Answer);
                    sb.Append("'><span class='input-group-addon'><input type='checkbox' name='formC' value='1'");
                    if (A.IsCorrect == true)
                    {
                        sb.Append(" checked");
                    }
                    sb.Append("></span></div></div></div>");
                }
                sb.Append("<div id='AddHere'></div><div class='form-group col-md-8'><label class='col-md-4 control-label' for='filebutton'>Add/Change Image:</label><div class='col-md-8'><input id ='filebutton' name='formFile' class='input-file' type='file'><button onclick='NoImage()'>Remove Image</button></div></div><br /><br /><div class='form-group col-md-8'><div class='col-md-8'><button class='btn btn-info' onclick='AddAnswer()'><span class='glyphicon glyphicon-plus'></span>Add More Answers</button><button type='button' class='btn btn-danger' data-toggle='modal' data-target='#deleteModal'>Remove Question From Test</button><button id='SubmitButton' name='SubmitButton' class='btn btn-success' onclick='SendQuestion()'>Submit</button></div></div></fieldset>");
                QuestionString = sb.ToString();
            }
            return QuestionString;
        }

        //Gets list of categories for the dropdown list on the edit page.
        public List<string> CategoryArray()
        {
            var List = Test.QuestionList();
            List<string> categorys = new List<string>();
            foreach (var Question in List)
            {
                if (categorys.Contains(Question.Category) == false)
                {
                    categorys.Add(Question.Category);
                }
            }
            return categorys;
        }

        //Makes string of categories for the dropdown list on the edit page.
        public string CategoryList(string Selected = "Not Selected")
        {
            var List = CategoryArray();
            string categoryString = "";
            foreach (var category in List)
            {
                StringBuilder sb = new StringBuilder(categoryString);
                sb.Append("<option value='");
                sb.Append(category);
                sb.Append("'");
                if (Selected == category)
                {
                    sb.Append(" selected");
                }
                sb.Append(">");
                sb.Append(category);
                sb.Append("</option>");
                categoryString = sb.ToString();
            }
            return categoryString;
        }

        public static void Delete(string uid)
        {
            int id = Int32.Parse(uid);
            TestDBHelper dbhelp = new TestDBHelper();
            dbhelp.RemoveQuestion(id);
        }
    }
}