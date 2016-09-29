using SubtitleViewerWeb.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SubtitleViewerWeb.Controllers
{
    public class SubtitleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subtitle
        public ActionResult Index()
        {
            return View();
        }

        // GET: Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: Upload
        [HttpPost]
        public ActionResult Upload([Bind(Include = "ID,File,Time")] UploadModel upload)
        {
            // Clear old entries
            db.SubtitleModel.RemoveRange(db.SubtitleModel);
            db.SaveChanges();

            // Parse subtitle file
            if (upload.File != null && upload.File.ContentLength > 0)
            {
                var fileName = Path.GetFileName(upload.File.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                upload.File.SaveAs(path);

                ParseSubtitles(upload.File, upload.Time);
            }
            
            return RedirectToAction("Viewer");
        }

        private void ParseSubtitles(HttpPostedFileBase file, DateTime totalTime)
        {
            var parser = new SubtitlesParser.Classes.Parsers.SubParser();

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                try
                {
                    var mostLikelyFormat = parser.GetMostLikelyFormat(file.FileName);
                    var items = parser.ParseStream(fileStream, Encoding.UTF8, mostLikelyFormat);
                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            SubtitleModel model = new SubtitleModel();

                            var timeSpan = TimeSpan.FromMilliseconds(item.StartTime);
                            var time = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                            if (totalTime != null)
                            {
                                var totalTimeSpan = new TimeSpan(totalTime.Hour, totalTime.Minute, totalTime.Second);
                                var timeDiff = totalTimeSpan - time;
                                model.Time = timeDiff.ToString();
                            }
                            else
                            {
                                model.Time = time.ToString();
                            }

                            string subtitle = "";
                            foreach (var line in item.Lines)
                            {
                                subtitle = subtitle + line;
                                subtitle = subtitle + "\n";
                            }
                            model.Subtitle = subtitle;

                            db.SubtitleModel.Add(model);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Not items found!");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Parsing of file {0}: FAILURE\n{1}", file.FileName, ex);
                }
            }
        }

        // GET: View
        public ActionResult Viewer()
        {
            return View(db.SubtitleModel.ToList());
        }
    }
}