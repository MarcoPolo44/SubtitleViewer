using SubtitleViewerWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SubtitleViewerWeb.Controllers
{
    public class SubtitlesController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // Create subtitle list
        private SubtitleListModel subtitleList = new SubtitleListModel();

        // GET: Subtitle
        public ActionResult Index()
        {
            return View();
        }

        // GET: Upload
        public ActionResult Upload()
        {
            UploadModel model = new UploadModel();
            IEnumerable<TimeStyle> stlyeTypes = Enum.GetValues(typeof(TimeStyle))
                                                         .Cast<TimeStyle>();
            model.StylesList = from stlye in stlyeTypes
                               select new SelectListItem
                             {
                                 Text = "Time " + stlye.ToString(),
                                 Value = stlye.ToString()
                             };

            return View(model);
        }

        // POST: Upload
        [HttpPost]
        public ActionResult Upload([Bind(Include = "ID,File,Style,Time")] UploadModel upload)
        {
            string error = "Sorry, no file could be found.";

            if (upload.File != null && upload.File.ContentLength > 0)
            {
                // Upload file
                var fileName = Path.GetFileName(upload.File.FileName);
                var filePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                upload.File.SaveAs(filePath);

                // Parse file
                error = ParseSubtitles(upload.File, upload.Time, upload.Style);

                // Delete file off server
                System.IO.File.Delete(filePath);
            }

            if (error == null)
            {
                TempData["subtitles"] = subtitleList;
                return RedirectToAction("Viewer");
            }
            else
            {
                TempData["error"] = error;
                return RedirectToAction("Error");
            }
        }

        private string ParseSubtitles(HttpPostedFileBase file, TimeSpan totalTime, string style)
        {
            string result = null;
            var parser = new SubtitlesParser.Classes.Parsers.SubParser();

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                try
                {
                    var mostLikelyFormat = parser.GetMostLikelyFormat(file.FileName);
                    if (mostLikelyFormat == null)
                        throw new ArgumentException("Sorry, we do not support this file type.");

                    var items = parser.ParseStream(fileStream, Encoding.Default, mostLikelyFormat);
                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            SubtitleModel model = new SubtitleModel();

                            var timeSpan = TimeSpan.FromMilliseconds(item.StartTime);
                            var time = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                            if (style == "Remaining")
                            {
                                var timeDiff = totalTime - time;
                                model.Time = timeDiff.ToString();
                            }
                            else if (style == "Elapsed")
                            {
                                model.Time = time.ToString();
                            }
                            else
                            {
                                throw new ArgumentException("Sorry, the time style was not defined.");
                            }

                            string subtitle = "";
                            foreach (var line in item.Lines)
                            {
                                subtitle = subtitle + line;
                                subtitle = subtitle + "\n";
                            }
                            model.Subtitle = subtitle;

                            // Add subtitle to list
                            subtitleList.Subtitles.Add(model);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Sorry, not subtitles counld be found!");
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return result;
        }

        // GET: View
        public ActionResult Viewer()
        {
            subtitleList = (SubtitleListModel) TempData["subtitles"];
            return View(subtitleList);
        }

        // POST: Viewer
        [HttpPost]
        public ActionResult Viewer(SubtitleListModel model, string command)
        {
            if (command.Equals("Upload New"))
            {
                return RedirectToAction("Upload");
            }
            else if (command.Equals("Edit Time"))
            {
                TempData["subtitles"] = model;
                return RedirectToAction("Edit");
            }
            else if (command.Equals("Delete All"))
            {
                TempData["subtitles"] = null;
                return RedirectToAction("Viewer");
            }
            else
            {
                TempData["error"] = "Sorry, this action is not available.";
                return RedirectToAction("Error");
            }
        }

        // GET: Edit
        public ActionResult Edit()
        {
            subtitleList = (SubtitleListModel)TempData["subtitles"];

            if (subtitleList == null || subtitleList.Subtitles.Count == 0)
            {
                TempData["error"] = "Sorry, we could not find any subtitles to edit.";
                return RedirectToAction("Error");
            }

            return View(subtitleList);
        }

        // POST: Edit
        [HttpPost]
        public ActionResult Edit(SubtitleListModel model, string cmd)
        {
            if (cmd == "Edit")
            {
                TimeSpan timeDiff = (model.EditTime - TimeSpan.Parse(model.Subtitles[0].Time));

                for (int i = 0; i < model.Subtitles.Count(); ++i)
                {
                    TimeSpan timeSpan = TimeSpan.Parse(model.Subtitles[i].Time);
                    timeSpan += timeDiff;

                    model.Subtitles[i].Time = timeSpan.ToString();
                }
            }

            TempData["subtitles"] = model;
            return RedirectToAction("Viewer");
        }

        // GET: Error
        public ActionResult Error()
        {
            string errorMsg = (string)TempData["error"];
            return View((object)errorMsg);
        }
    }
}