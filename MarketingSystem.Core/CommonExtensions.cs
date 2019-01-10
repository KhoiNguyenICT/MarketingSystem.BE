using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MarketingSystem.Core
{
    public static class CommonExtensions
    {
        public static string NormalizeFileName(this string fileName)
        {
            return WebUtility.UrlDecode(fileName).Trim('"').Replace("&", "and").FixMultipleSpaces();
        }

        public static Guid ToGuid(this string str)
        {
            return Guid.Parse(str);
        }

        public static string GenerateUniqueFileName(this string fileName)
        {
            var fileExt = Path.GetExtension(fileName);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            return $"{fileNameWithoutExt.ToLower()}-{Guid.NewGuid()}{fileExt.ToLower()}";
        }

        public static string ToCamelCasing(this string str)
        {
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string ToSlug(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            try
            {
                str = str.CleanString();
                str = Regex.Replace(str, @"[^0-9a-zA-Z]", "-");
                str = Regex.Replace(str, @"\-+", "-");
                str = Regex.Replace(str, @"^\-", "");
                str = Regex.Replace(str, @"\-$", "");
                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string CleanString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            try
            {
                str = str.ToLower().Trim();
                str = Regex.Replace(str, @"[\r|\n]", " ");
                str = Regex.Replace(str, @"\s+", " ");
                str = Regex.Replace(str, "[áàảãạâấầẩẫậăắằẳẵặ]", "a");
                str = Regex.Replace(str, "[éèẻẽẹêếềểễệ]", "e");
                str = Regex.Replace(str, "[iíìỉĩị]", "i");
                str = Regex.Replace(str, "[óòỏõọơớờởỡợôốồổỗộ]", "o");
                str = Regex.Replace(str, "[úùủũụưứừửữự]", "u");
                str = Regex.Replace(str, "[yýỳỷỹỵ]", "y");
                str = Regex.Replace(str, "[đ]", "d");

                //Remove special character
                str = Regex.Replace(str, "[\"`~!@#$%^&*()-+=?/>.<,{}[]|]\\]", "");
                str = str.Replace("̀", "").Replace("̣", "").Replace("̉", "").Replace("̃", "").Replace("́", "");
                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string MapContentType(this string fileName)
        {
            var fileExt = Path.GetExtension(fileName).ToLower();
            switch (fileExt)
            {
                case ".png":
                    return "image/png";

                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";

                case ".gif":
                    return "image/gif";

                case ".bmp":
                    return "image/bmp";

                case ".tif":
                case ".tiff":
                    return "image/tiff";

                case ".ico":
                    return "image/x-icon";

                case ".js":
                    return "application/javascript";

                case ".css":
                    return "text/css";

                case ".json":
                    return "application/json";

                case ".xml":
                    return "application/xml";

                case ".htm":
                case ".html":
                    return "text/html";

                case ".zip":
                    return "application/zip";

                case ".rar":
                    return "application/x-rar-compressed";

                case ".swf":
                    return "application/x-shockwave-flash";

                case ".xhtml":
                    return "application/xhtml+xml";

                case ".pdf":
                    return "application/pdf";
            }
            return "binary/octet-stream";
        }

        public static string FixMultipleSpaces(this string strInput)
        {
            return Regex.Replace(strInput, @"\s+", " ");
        }
    }
}