using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ApiProperties
    {
        public static  string Translate { get; } = @"https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20180221T115806Z.fd9b3cd8eb379bd8.78ee99f0964f76567858212e8828e456b1303485&text={0}&lang={1}";
        public static string DetectSourcceLanguage { get; } = @"https://translate.yandex.net/api/v1.5/tr.json/detect?key=trnsl.1.1.20180221T115806Z.fd9b3cd8eb379bd8.78ee99f0964f76567858212e8828e456b1303485&text={0}";
        public static string GetAvailableLanguages { get; } = @"https://translate.yandex.net/api/v1.5/tr.json/getLangs?key=trnsl.1.1.20180221T115806Z.fd9b3cd8eb379bd8.78ee99f0964f76567858212e8828e456b1303485&ui={1}";
    }
}