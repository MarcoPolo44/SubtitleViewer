using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SubtitleViewerWeb.Models
{
    public class UploadModel
    {
        public int ID { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        [Required]
        public IEnumerable<SelectListItem> StylesList { get; set; } = new List<SelectListItem>();
        public string Style { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH:mm:ss}")]
        [RegularExpression(@"-?([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Time must be in the format - HH:MM:SS")]
        public TimeSpan Time { get; set; }
    }

    public enum TimeStyle
    {
        Elapsed,
        Remaining
    }
}