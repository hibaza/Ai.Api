using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ai.Core.Helpers
{
    public partial class CommonHelper
    {

        public static string GenerateNineDigitUniqueNumber()
        {
            return (DateTimeToUnixTimestamp(DateTime.UtcNow) / 10).ToString();
        }

        public static string GenerateDigitUniqueNumber()
        {
            return DateTimeToTimestamp13Digits(DateTime.UtcNow).ToString();
        }

        public static string ToStringFixedLength(long value, int length)
        {
            return value.ToString().PadLeft(length, '0');
        }
        public static string ToStringFixedLength(string value, int length)
        {
            return value.PadLeft(length, '0');
        }

        public static bool ValidEmail(string email)
        {
            return email.Contains("@");
        }

        public static string GetFileNameFromUrl(string url)
        {
            string fileName = null;
            try
            {
                fileName = new Uri(url).Segments.Last();
            }
            catch { }

            return fileName;
        }

        public static string EnsureMaximumLength(string str, int maxLength)
        {
            if (String.IsNullOrWhiteSpace(str))
                return str;

            if (str.Length > maxLength)
                return str.Substring(0, maxLength);
            else
                return str;
        }

        public static string EnsureNumericOnly(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }


        public static string FormatCurrency(double? value)
        {
            return string.Format("{0:0,0.}đ", value).Replace(",", ".");
        }

        public static long DateTimeToUnixTimestamp(DateTime? dateTime)
        {

            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = dateTime == null ? 0 : ((DateTime)dateTime - unixStart).Ticks;
            return (long)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public static long DateTimeToTimestamp13Digits(DateTime? dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = dateTime == null ? 0 : ((DateTime)dateTime - unixStart).Ticks;
            return (long)unixTimeStampInTicks / 10000;
        }

        public static DateTime UnixTimestampToDateTime(long unixTime)
        {
            unixTime = Convert.ToInt64(unixTime.ToString().Substring(0,10));
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }


        public static string FormatKey(string parent, string key)
        {
            return string.IsNullOrWhiteSpace(parent) ? Core.Helpers.CommonHelper.FormatKey(key) : Core.Helpers.CommonHelper.FormatKey(parent) + "_" + Core.Helpers.CommonHelper.FormatKey(key);
        }

        public static string FormatKey(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? GenerateDigitUniqueNumber() : value.Replace(".", "___").Replace(":", "__").Replace("@", "_at_").Replace("$", "_s_").Replace("=", "_eq_").Replace("&", "_and_").Replace("?", "_q_").Replace("%", "_p_").Replace("/", "_sl_");
        }

        public static string FormatKeyChannelHotline(string business_id, string channel_id)
        {
            return business_id.Trim() + "_" + (!string.IsNullOrWhiteSpace(channel_id) ? channel_id.Trim() : "unknow");
        }
        public static string FormatKeyExtIdFromCallEvent(string ext_id)
        {
            return ext_id.Replace(".","_");
        }
        public static string DigitsOnly(string text)
        {
            return Regex.Replace(text, "[^0-9]", "");
        }
        public static bool ExistsDigits(string text)
        {
            return text.Any(c => char.IsDigit(c));
        }

        public static string replaceSpecial(string text)
        {
            try
            {
                return text.Replace("'", " ").Replace(System.Environment.NewLine, " ").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            }
            catch { return text; }
        }

        public static List<string> FindPhoneNumbers(string text)
        {
            List<string> list = new List<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(text)) return list;
                //text = text.Replace("-", "").Replace(".", "");
                //string pattern = @"\(?\d{4,5}\)?[-\.]? *\d{3,3}[-\.]? *[-\.]?\d{3,3}";
                //Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                //Match match = regex.Match(text);
                //while (match.Success)
                //{
                //    string phoneNumber = DigitsOnly(match.Groups[0].Value);
                //    list.Add(phoneNumber);
                //    match = match.NextMatch();
                //}
                var phone = DigitsOnly(text);
                if (phone != null && phone.Length > 8 && phone.Length < 12)
                    list.Add(phone);
                return list;
            }
            catch { return list; }
        }


        public static string Hmacsha256(string content, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(content);
            byte[] hash;
            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes))
            {
                hash = hmacsha256.ComputeHash(messageBytes);
            }

            StringBuilder sbHash = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sbHash.Append(hash[i].ToString("x2"));
            }
            return sbHash.ToString();
        }

        public static string removeSpecialString(string str)
        {
            return  Regex.Replace(str, "[^0-9a-zA-Z]+", "");
        }
        public static string removeSpecialFile(string nameFile)
        {
            var specials = nameFile.Split('.');
            var str = "";
            for (var i = 0; i < specials.Length - 1; i++)
            {
                str += specials[i];
            }
            return removeSpecialString(str) +"."+ specials[specials.Length - 1];
        }

        public static bool dynamicExistsKey(ExpandoObject expandoObj,
                               string name)
        {
            return ((IDictionary<string, object>)expandoObj).ContainsKey(name);
        }

        public static string GetSipFromChannel(string name)
        {
            try
            {
                var sp = name.Split(new string[] { "/", "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length > 1)
                    return sp[1];
                return name;
            }
            catch (Exception ex) { return name; }
        }

        public static string BsonToJson(BsonDocument bson)
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }; // key part
            return bson.ToJson(jsonWriterSettings);
        }

        public static BsonDocument JsonToBsonDocument(string json)
        {
           return MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);
        }

        public static BsonArray JsonToBsonArray(string json)
        {
            return MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(json);
        }

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".zip", "application/zip"},
                {".rar", "application/rar"},
                {".exe", "application/exe"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},

                {".mp3", "audio/mp3"},
                {".wav", "audio/wav"},
                {".sms", "audio/sms"}
            };
        }

        public static List<string> CutStringMax(string str, int max)
        {
            var list = new List<string>();
            if (str != "")
            {
                var lengths = str.Length / max;
                for (var i = 0; i <= lengths; i++)
                {
                    try
                    {
                        var next = str.Length - (i * max) < ((i + 1) * max) ? str.Length - (i * max) : (i + 1) * max;
                        var strNew = str.Substring(i * max, next);
                        if (strNew != "")
                            list.Add(strNew);
                    }
                    catch { }
                }
            }
            return list;
        }
    }
}
