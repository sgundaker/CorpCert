using System;
using L2Test.Helpers;
using System.Text;
using System.Collections.Generic;

namespace L2Test.Models
{
    public class TechModels
    {
        public int TechKey { get; set; }
        public string TechName { get; set; }
        public string TechID { get; set; }
        public DateTime Time { get; set; }

        public bool isValid(string ID)
        {
            TechDBHelper dbhelp = new TechDBHelper();
            var tech = dbhelp.CheckID(ID);

            if (tech != null)
            {
                foreach (var entry in tech)
                {
                    TimeSpan span = DateTime.Now - entry.Time;
                    if (span.TotalMinutes < 90)
                        return true;
                }
            }
            return false;
        }

        public void CreateNew(string tech)
        {
            TechDBHelper dbhelp = new TechDBHelper();
            Password pass = new Password();
            string techID = pass.Generate();
            dbhelp.NewID(tech, techID);
        }

        public string GetValid()
        {
            TechDBHelper dbhelp = new TechDBHelper();
            var Temp = dbhelp.ListAll();
            List<TechModels> List = new List<TechModels>();
            DateTime current = DateTime.Now;

            foreach(var ID in Temp)
            {
                TimeSpan span = current.Subtract(ID.Time);
                if (span.TotalMinutes < 90) 
                    List.Add(ID);
            }

            string ValidIDString = "";
            foreach (var ID in List)
            {
                StringBuilder sb = new StringBuilder(ValidIDString);
                sb.Append("<p class='passwordList'>");
                sb.Append(ID.TechName);
                sb.Append(": ");
                sb.Append(ID.TechID);
                sb.Append("</p>");
                ValidIDString = sb.ToString();
            }
            return ValidIDString;
        }
    }
}