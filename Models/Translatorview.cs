using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Translatorview
    {
        public string sourceLanguage { get; set; }
        public string SourceText { get; set; }
        public string DestinationLanguage { get; set; }
        public string DestinationText { get; set; }

    }
}