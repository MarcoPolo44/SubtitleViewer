using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleViewerWeb.Models
{
    public class SubtitleListModel
    {
        public List<SubtitleModel> Subtitles { get; set; } = new List<SubtitleModel>();
    }

    public class SubtitleModel
    {
        public int ID { get; set; }
        public string Time { get; set; }
        public string Subtitle { get; set; }
    }
}