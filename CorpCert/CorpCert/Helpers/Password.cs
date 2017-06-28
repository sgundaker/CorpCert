using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace L2Test.Helpers
{
    public class Password
    {
        Random rnd = new Random();

        public string Generate(){ 
            int length = 10;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }        
    }
}