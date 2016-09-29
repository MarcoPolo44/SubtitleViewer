using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace SubtitleViewerWeb.Models
{
    public class UploadModel
    {
        public int ID { get; set; }

        [Required]
        public HttpPostedFileBase File { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH:mm:ss}")]
        public DateTime Time { get; set; }
    }
}