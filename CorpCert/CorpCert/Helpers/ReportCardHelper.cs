using System.Text;
using System.IO;
using System;

namespace L2Test.Helpers
{
    public class ReportCardHelper
    {
        public string GetGradedReport(string PathToArchive)
        {
            DirectoryInfo d = new DirectoryInfo(PathToArchive);
            FileInfo[] ReportCardList = d.GetFiles("*.html");
            Array.Reverse(ReportCardList);
            string ReportCards = "";

            StringBuilder sb = new StringBuilder(ReportCards);

            foreach (var ReportCard in ReportCardList)
            {
                string Name = ReportCard.Name.Replace(".html", "");
                {
                    sb.Append("<li class='Record'><a Class='getTest' id='");
                    sb.Append(ReportCard.FullName);
                    sb.Append("'>");
                    sb.Append(Name);
                    sb.Append("</a>");
                }
                ReportCards = sb.ToString();
            }
            return ReportCards;
        }
    }
}

