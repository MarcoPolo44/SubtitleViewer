using OSDBnet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SubtitleViewerWeb.Models
{
    public class ResultListModel
    {
        [Required]
        public IEnumerable<SelectListItem> Results { get; set; } = new List<SelectListItem>();
        public string File { get; set; }

        public IList<Subtitle> Subtitles { get; set; } = new List<Subtitle>();

        [Required]
        public IEnumerable<SelectListItem> StylesList { get; set; } = new List<SelectListItem>();
        public string Style { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH:mm:ss}")]
        [RegularExpression(@"-?([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Time must be in the format - HH:MM:SS")]
        public TimeSpan Time { get; set; }
    }
}