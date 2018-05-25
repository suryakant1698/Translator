using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ApiProperties
    {
        public static string key { get; } = @"trnsl.1.1.20180221T115806Z.fd9b3cd8eb379bd8.78ee99f0964f76567858212e8828e456b1303485";
        public static string Translate { get; } = @"https://translate.yandex.net/api/v1.5/tr.json/translate";
        public static string DetectSourcceLanguage { get; } = @"https://translate.yandex.net/api/v1.5/tr.json/detect";
        public static string GetAvailableLanguages { get; } = @"https://translate.yandex.net/api/v1.5/tr.json/getLangs";
    }
}