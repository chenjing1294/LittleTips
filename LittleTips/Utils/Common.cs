using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;

namespace LittleTips.Utils
{
    public class Common
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static readonly JsonSerializerOptions UglyJsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static List<string> ReadAllLine(string fileName)
        {
            List<string> lines = new List<string>();
            Assembly assembly = Assembly.GetAssembly(typeof(Common));
            string resourceName = assembly.GetName().Name + ".g";
            ResourceManager rm = new ResourceManager(resourceName, assembly);
            using (ResourceSet set = rm.GetResourceSet(CultureInfo.InvariantCulture, true, true))
            {
                UnmanagedMemoryStream s = (UnmanagedMemoryStream) set.GetObject(fileName, true);
                StreamReader reader = new StreamReader(s);
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line);
                    }
                }

                reader.Close();
            }

            return lines;
        }

        public static string ReadString(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            Assembly assembly = Assembly.GetAssembly(typeof(Common));
            string resourceName = assembly.GetName().Name + ".g";
            ResourceManager rm = new ResourceManager(resourceName, assembly);
            using (ResourceSet set = rm.GetResourceSet(CultureInfo.InvariantCulture, true, true))
            {
                UnmanagedMemoryStream s = (UnmanagedMemoryStream) set.GetObject(fileName, true);
                StreamReader reader = new StreamReader(s);
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        sb.Append(line);
                    }
                }

                reader.Close();
            }

            return sb.ToString();
        }


        public static string Get(string uri)
        {
            //先根据用户请求的uri构造请求地址
            string serviceUrl = string.Format(uri);
            //创建Web访问对象
            HttpWebRequest myRequest = (HttpWebRequest) WebRequest.Create(serviceUrl);

            //通过Web访问对象获取响应内容
            HttpWebResponse myResponse = (HttpWebResponse) myRequest.GetResponse();
            if (myResponse.StatusCode == HttpStatusCode.OK)
            {
                if (myResponse.GetResponseStream() != null)
                {
                    //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    string res = reader.ReadToEnd(); //利用StreamReader就可以从响应内容从头读到尾
                    reader.Close();
                    return res;
                }
            }

            myResponse.Close();
            return null;
        }

        public static System.Windows.Input.Key WinformsToWPFKey(System.Windows.Forms.Keys inputKey)
        {
            // Put special case logic here if there's a key you need but doesn't map...  
            try
            {
                return (System.Windows.Input.Key) Enum.Parse(typeof(System.Windows.Input.Key), inputKey.ToString());
            }
            catch
            {
                // There wasn't a direct mapping...    
                return System.Windows.Input.Key.None;
            }
        }

        public static System.Windows.Forms.Keys WPFToWinformsKey(System.Windows.Input.Key inputKey)
        {
            // Put special case logic here if there's a key you need but doesn't map...  
            try
            {
                return (System.Windows.Forms.Keys) Enum.Parse(typeof(System.Windows.Forms.Keys), inputKey.ToString());
            }
            catch
            {
                // There wasn't a direct mapping...    
                return System.Windows.Forms.Keys.None;
            }
        }

        public static System.Windows.Forms.Keys StringToWinformsKey(string inputKey)
        {
            // Put special case logic here if there's a key you need but doesn't map...  
            try
            {
                return (System.Windows.Forms.Keys) Enum.Parse(typeof(System.Windows.Forms.Keys), inputKey);
            }
            catch
            {
                // There wasn't a direct mapping...    
                return System.Windows.Forms.Keys.None;
            }
        }
    }
}