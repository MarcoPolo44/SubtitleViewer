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
        public ActionResult Upload(HttpPostedFileBase file)
        {
            // Clear old entries
            db.SubtitleModel.RemoveRange(db.SubtitleModel);
            db.SaveChanges();

            // Parse subtitle file
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                file.SaveAs(path);

                ParseSubtitles(file);
            }
            
            return RedirectToAction("Viewer");
        }

        private void ParseSubtitles(HttpPostedFileBase file)
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
                            model.Time = time.ToString();

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