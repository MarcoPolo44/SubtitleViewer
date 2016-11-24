using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SubtitleViewerWeb.Models
{
    public class SubtitleListModel
    {
        public List<SubtitleModel> Subtitles { get; set; } = new List<SubtitleModel>();

        [Required]
        [Display(Name = "Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH:mm:ss}")]
        [RegularExpression(@"([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Time must be in the format - HH:MM:SS")]
        public TimeSpan EditTime { get; set; }
    }

    public class SubtitleModel
    {
        public int ID { get; set; }
        public string Time { get; set; }

        [AllowHtml]
        public string Subtitle { get; set; }
    }
}