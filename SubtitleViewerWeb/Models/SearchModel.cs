using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SubtitleViewerWeb.Models
{
    public class SearchModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public IEnumerable<SelectListItem> LanguagesList { get; set; } = new List<SelectListItem>();

        [Required]
        public string Language { get; set; }

        public int? Season { get; set; }
        public int? Episode { get; set; }

        public List<Tuple<String, String>> Languages = new List<Tuple<String, String>>()
        {
            Tuple.Create("Afrikaans", "afr"),
            Tuple.Create("Albanian", "alb"),
            Tuple.Create("Arabic", "ara"),
            Tuple.Create("Armenian", "arm"),
            Tuple.Create("Asturian", "ast"),
            Tuple.Create("Azerbaijani", "aze"),
            Tuple.Create("Basque", "baq"),
            Tuple.Create("Belarusian", "bel"),
            Tuple.Create("Bengali", "ben"),
            Tuple.Create("Bosnian", "bos"),
            Tuple.Create("Breton", "bre"),
            Tuple.Create("Bulgarian", "bul"),
            Tuple.Create("Burmese", "bur"),
            Tuple.Create("Catalan", "cat"),
            Tuple.Create("Chinese (simplified)", "chi"),
            Tuple.Create("Chinese (traditional)", "zht"),
            Tuple.Create("Chinese (bilingual)", "zhe"),
            Tuple.Create("Croatian", "hrv"),
            Tuple.Create("Czech", "cze"),
            Tuple.Create("Danish", "dan"),
            Tuple.Create("Dutch", "dut"),
            Tuple.Create("English", "eng"),
            Tuple.Create("Esperanto", "epo"),
            Tuple.Create("Estonian", "est"),
            Tuple.Create("Extremaduran", "ext"),
            Tuple.Create("Finnish", "fin"),
            Tuple.Create("French", "fre"),
            Tuple.Create("Galician", "glg"),
            Tuple.Create("Georgian", "geo"),
            Tuple.Create("German", "ger"),
            Tuple.Create("Greek", "ell"),
            Tuple.Create("Hebrew", "heb"),
            Tuple.Create("Hindi", "hin"),
            Tuple.Create("Hungarian", "hun"),
            Tuple.Create("Icelandic", "ice"),
            Tuple.Create("Indonesian", "ind"),
            Tuple.Create("Italian", "ita"),
            Tuple.Create("Japanese", "jpn"),
            Tuple.Create("Kazakh", "kaz"),
            Tuple.Create("Khmer", "khm"),
            Tuple.Create("Korean", "kor"),
            Tuple.Create("Latvian", "lav"),
            Tuple.Create("Lithuanian", "lit"),
            Tuple.Create("Luxembourgish", "ltz"),
            Tuple.Create("Macedonian", "mac"),
            Tuple.Create("Malay", "may"),
            Tuple.Create("Malayalam", "mal"),
            Tuple.Create("Manipuri", "mni"),
            Tuple.Create("Mongolian", "mon"),
            Tuple.Create("Montenegrin", "mne"),
            Tuple.Create("Norwegian", "nor"),
            Tuple.Create("Occitan", "oci"),
            Tuple.Create("Persian", "per"),
            Tuple.Create("Polish", "pol"),
            Tuple.Create("Portuguese", "por"),
            Tuple.Create("Portuguese (BR)", "pob"),
            Tuple.Create("Portuguese (MZ)", "pom"),
            Tuple.Create("Romanian", "rum"),
            Tuple.Create("Russian", "rus"),
            Tuple.Create("Serbian", "scc"),
            Tuple.Create("Sinhalese", "sin"),
            Tuple.Create("Slovak", "slo"),
            Tuple.Create("Slovenian", "slv"),
            Tuple.Create("Spanish", "spa"),
            Tuple.Create("Swahili", "swa"),
            Tuple.Create("Swedish", "swe"),
            Tuple.Create("Syriac", "syr"),
            Tuple.Create("Tagalog", "tgl"),
            Tuple.Create("Tamil", "tam"),
            Tuple.Create("Telugu", "tel"),
            Tuple.Create("Thai", "tha"),
            Tuple.Create("Turkish", "tur"),
            Tuple.Create("Ukrainian", "ukr"),
            Tuple.Create("Urdu", "urd"),
            Tuple.Create("Vietnamese", "vie")
        };
    }
}