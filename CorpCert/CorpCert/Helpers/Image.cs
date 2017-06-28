using System.IO;
using System.Web;

namespace L2Test.Helpers
{
    public class Image
    {
        public string Upload(HttpPostedFileBase file)
        {
            string result = "No Image";
            if ((file != null) && (file.ContentLength > 0) && (file.ContentLength <= 15000000))//max 15MB
                try
                {
                    string ext = Path.GetExtension(file.FileName);
                    Password Pass = new Password();
                    string NewFilename = Path.GetFileNameWithoutExtension(file.FileName) + Pass.Generate() + ext;
                    string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images"), NewFilename);

                    if (FileTypeIsImage(ext))
                    {
                        file.SaveAs(path);
                        result = NewFilename;
                    }                 
                }
                catch
                {
                    result = "Failed to upload";
                }
            return result;
        }

        private bool FileTypeIsImage(string ext)
        {
            string type = ext.ToLower();
            if ((type == ".png") || (type == ".jpeg") || (type == ".jpg") || (type == ".bmp") || (type == ".gif") || (type == ".tif") || (type == ".tiff"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}