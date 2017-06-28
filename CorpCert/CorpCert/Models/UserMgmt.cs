using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using L2Test.Helpers;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace L2Test.Models
{
    public class UserMgmt
    {
        public string UserList()
        {
            var context = new IdentityDbContext(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString());
            var List = context.Users.ToList();
            string UserString = "";
            foreach (var User in List)
            {
                string jsfixD = '"' + "deleteUser('" + User.UserName + "' , '" + User.Id + "')" + '"'; //Added this because otherwise stringbuilder cannot format the variable in a way that Javascript can accept.
                string jsfixR = '"' + "editUser('" + User.UserName + "' , '" + User.Id + "')" + '"';
                StringBuilder sb = new StringBuilder(UserString);
                sb.Append("<li>");
                sb.Append(User.Email);
                sb.AppendFormat("<button type='button' class='btn btn-danger' onclick={0}>Delete User</button>", jsfixD);
                sb.AppendFormat("<button type='button' class='btn btn-info' onclick={0}>Reset Password</button>", jsfixR);
                sb.Append("</li>");
                UserString = sb.ToString();
            }
            return UserString;
        }

        public static void Delete(string key)
        {
            using (var db = new IdentityDbContext(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString()))
            {
                var user = db.Users.Find(key);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public static void PaswordUpdate(string key, string newPassword)
        {
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new IdentityDbContext(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString())));
            userManager.RemovePassword(key);
            userManager.AddPassword(key, newPassword);
        }

        //Checks to see if there are any managment users. If there are not then it runs the install view.
        public bool isInstalled() 
        {
            var context = new IdentityDbContext(ConfigurationManager.ConnectionStrings["L2TestConnection"].ToString());
            try
            {
                var List = context.Users.ToList();
                if (List.Count > 0) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }
    }
}