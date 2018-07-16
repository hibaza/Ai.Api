using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ai.Domain
{

    public class AppSettings
    {
        public string Version { get; set; }
        public BaseUrls BaseUrls { get; set; }
        public MongoDB MongoDB { get; set; }
        public decimal Confidence { get; set; }
    }

    public class BaseUrls
    {
        public string AiMessagerLink { get; set; }
        public string ShopOrderUrl { get; set; }
        public string StockBalance { get; set; }
        
    }

    public class MongoDB
    {
        public string ConnAi { get; set; }
        public string AiDb { get; set; }
        public string ConnChat { get; set; }
        public string ChatDb { get; set; }
    }
    

    public class Configuration
    {
        private static string GetKey(string key)
        {
            return ""; //ConfigurationManager.AppSettings[key];
        }

        public static string BaseUrl
        {
            get
            {
                string baseUrl = GetKey("BaseUrl");
                if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Remove(baseUrl.Length - 1);
                return baseUrl;
            }
        }


        public static string CloudBaseUrl
        {
            get
            {
                string baseUrl = GetKey("CloudBaseUrl");
                if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Remove(baseUrl.Length - 1);
                return baseUrl;
            }
        }

        public static string DataFolderPath
        {
            get
            {
                string path = GetKey("DataFolderPath");
                if (path.EndsWith("\\")) path = path.Remove(path.Length - 1);
                return path;
            }
        }


        public static int DefaultPageSize
        {
            get
            {
                return int.Parse(GetKey("DefaultPageSize"));
            }
        }

        public static string EncryptionKey
        {
            get
            {
                return "bazaVietnam8888";
            }
        }

        public static string ApiBaseUrl
        {
            get
            {
                return GetKey("ApiBaseUrl");
            }
        }

    }
}
