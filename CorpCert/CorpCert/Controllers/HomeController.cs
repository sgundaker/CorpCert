using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using L2Test.Helpers;
using L2Test.Models;
using System.IO;
using Ionic.Zip;

namespace L2Test.Controllers
{
    public class HomeController : Controller
    {
        //AllowAnonymous means that users do not need to authenticate to use/see this page
        //Authorize means that users have to be logged in to view the page.

        [HttpGet]
        [AllowAnonymous] 
        public ActionResult Index() 
        {
            return RedirectToAction("Home");
        }

        [HttpGet]
        [AllowAnonymous]//Techs need to access this page to start taking the test
        public ActionResult Home()
        {
            ViewBag.HomePageText = Config.GetString("HomePage");

            if (TempData["error"] == null)
                ViewBag.Error = "";
            else
                ViewBag.Error = TempData["error"].ToString();

            return View();
        }

        [HttpPost]
        [AllowAnonymous]//Used to authenticate the techs temp ID
        public ActionResult Home(string formTechID)
        {
            return Redirect("~/Home/Test/" + formTechID);
        }

        [HttpGet]
        [AllowAnonymous]//Techs need access to this page
        public ActionResult Test()
        {
            TechModels Check = new TechModels();
            TestQuestions List = new TestQuestions();

            ViewBag.TimeToTakeTest = Config.GetInt("TimeToTakeTest");
            string URL = Request.Url.ToString();
            string TechID = Path.GetFileName(URL);

            if (Check.isValid(TechID))
                return View();

            TempData["error"] = "ERROR: Invalid ID. Please verify you entered the ID correctly. ID is only valid for 90 minutes, ask lead to create a new ID for you if your ID is not working.";
            return RedirectToAction("Home");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(string Error = "")
        {
            ViewBag.Err = Error;
            TestQuestions List = new TestQuestions();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [Authorize]
        public ActionResult Edit(TestEditModel jsonData)
        {
            TestEditModel help = new TestEditModel();
            help.NewQuestion(jsonData);
            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditQuestion()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [Authorize]
        public ActionResult EditQuestion(TestEditModel jsonData)
        {
            TestEditModel Help = new TestEditModel();
            Help.EditQuestion(jsonData);
            return Json(new { success = true});
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            Image ImageHelp = new Image();
            return Json(ImageHelp.Upload(file));
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(string uid)
        {
            EditTest.Delete(uid);
            return Redirect("~/Home/Edit");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Manage(string formTechName)
        {
            TechModels tech = new TechModels();
            tech.CreateNew(formTechName);
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ManageDelete(string uid)
        {
            UserMgmt.Delete(uid);
            return Redirect("~/Home/Manage");
        }

        [HttpPost]
        [Authorize]
        public ActionResult ManageEdit(string uid, string newPassword)
        {
            UserMgmt.PaswordUpdate(uid, newPassword);
            return Redirect("~/Home/Manage");
        }

        [HttpPost]
        [AllowAnonymous] //Used to submit test
        public ActionResult TestResults(IEnumerable<TestResultModel> jsonData)
        {
            GradeTest help = new GradeTest();
            return Content(help.Submit(jsonData));
        }

        [HttpPost]
        [AllowAnonymous] //Used to save snapshot of HTML at time the submit button was pressed.
        public void TestArchive(string html, string tech)
        {
            GradeTest help = new GradeTest();
            help.Archive(html, tech);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReportCards()
        {
            ReportCardHelper helper = new ReportCardHelper();
            ViewBag.GradedResults = helper.GetGradedReport(Server.MapPath(@"~\Views\Tests\Graded"));
            ViewBag.ArchiveResults = helper.GetGradedReport(Server.MapPath(@"~\Views\Tests\Ungraded"));
            return View();
        }

        [HttpGet]
        [Authorize]
        public FilePathResult Backup()
        {
            DatabaseBackup DB = new DatabaseBackup();
            string fileName = "L2Test_Backup_" + DateTime.Now.ToString("yyyy-MM-dd") + ".zip";
            DB.Backup(Server.MapPath(@"~\Content\Backup\tmp\"));

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(Server.MapPath(@"~\Content\Backup\tmp\"));
                zip.AddDirectoryByName("Images");
                zip.AddDirectory(Server.MapPath(@"~\Content\Images"), "Images");
                zip.AddDirectory(Server.MapPath(@"~\Views\Tests"));
                zip.Save(Server.MapPath(@"~\Content\Backup\" + fileName));
            }
            return new FilePathResult(Server.MapPath(@"~\Content\Backup\" + fileName), "application/zip");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Restore(HttpPostedFileBase file)
        {
            DatabaseBackup DB = new DatabaseBackup();
            DB.Restore(file);
            return Redirect("~/Home/Edit");
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.AboutPageText = Config.GetString("AboutPage");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Configuration()
        {
            ViewBag.HomePage = Config.GetString("HomePage");
            ViewBag.AboutPage = Config.GetString("AboutPage");
            ViewBag.NumberOfQuestions = Config.GetInt("NumberOfQuestions");
            ViewBag.PassingScore = Config.GetInt("PassingScore");
            ViewBag.TimeToTakeTest = Config.GetInt("TimeToTakeTest");
            ViewBag.TimeToStartTest = Config.GetInt("TimeToStartTest");
            ViewBag.Name = Config.GetString("Name");

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [Authorize] 
        public ActionResult Configuration(int formQuestions = -1, int formScore = -1, int formTimeToTake = -1, int formStart = -1, string formHomePage = "", string formAboutPage = "", string formName ="")
        {
            //Get all the current values
            string HomePage = Config.GetString("HomePage");
            string AboutPage = Config.GetString("AboutPage");
            string Name = Config.GetString("Name");
            int NumberOfQuestions = Config.GetInt("NumberOfQuestions");
            int PassingScore = Config.GetInt("PassingScore");
            int TimeToTakeTest = Config.GetInt("TimeToTakeTest");
            int TimeToStartTest = Config.GetInt("TimeToStartTest");

            //Only update values if entry is valid and was changed/submitted
            if (formQuestions > 0) { NumberOfQuestions = formQuestions; }
            if (formScore > 0) { PassingScore = formScore; }
            if (formTimeToTake > 0) { TimeToTakeTest = formTimeToTake; }
            if (formStart > 0) { TimeToStartTest = formStart; }
            if (formHomePage != "") { HomePage = formHomePage; }
            if (formAboutPage != "") { AboutPage = formAboutPage; }
            if (formName != "") { Name = formName; }

            Config.Update(HomePage, AboutPage, NumberOfQuestions, PassingScore, TimeToTakeTest, TimeToStartTest, Name);

            return Redirect("~/Home/Configuration");
        }

        [ChildActionOnly]
        public ActionResult SiteName()
        {
            return new ContentResult {Content = Config.GetString("Name")};
        }
    }
}