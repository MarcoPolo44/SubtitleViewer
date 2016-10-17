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
        public string Language { get; set; }

        public int Season { get; set; }
        public int Episode { get; set; }
    }

    public enum Languages
    {
        All,
        English,
        German
    }
}