﻿using OSDBnet;
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

        // Create result list
        private ResultListModel resultList = new ResultListModel();

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
                var filePath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                upload.File.SaveAs(filePath);

                // Parse file
                error = ParseSubtitles(fileName, upload.Time, upload.Style);

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

        private string ParseSubtitles(string fileName, TimeSpan totalTime, string style)
        {
            string result = null;
            var parser = new SubtitlesParser.Classes.Parsers.SubParser();

            var filePath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                try
                {
                    var mostLikelyFormat = parser.GetMostLikelyFormat(fileName);
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
                                subtitle = subtitle + " ";
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

        // GET: Search
        public ActionResult Search()
        {
            SearchModel model = new SearchModel();

            model.LanguagesList = from language in model.Languages
                                  select new SelectListItem
                                  {
                                      Text = language.Item1,
                                      Value = language.Item2,
                                  };

            return View(model);
        }

        // POST: Search
        [HttpPost]
        public ActionResult Search(SearchModel searchModel)
        {
            if (searchModel.Title == null || searchModel.Title == "")
            {
                TempData["error"] = "Sorry, that is not a valid subtitle query";
                return RedirectToAction("Error");
            }

            //
            // Open Subtitles query

            var osdb = Osdb.Login("Subtitle Companion v0.1");
            var results = osdb.SearchSubtitlesFromQuery(searchModel.Language, searchModel.Title, searchModel.Season, searchModel.Episode);

            resultList.Subtitles = results;

            resultList.Results = from result in results
                                  select new SelectListItem
                                  {
                                      Text = result.SubtitleFileName,
                                      Value = result.SubtitleId
                                  };

            //
            // Time Style

            IEnumerable<TimeStyle> stlyeTypes = Enum.GetValues(typeof(TimeStyle))
                                                         .Cast<TimeStyle>();
            resultList.StylesList = from stlye in stlyeTypes
                                  select new SelectListItem
                                  {
                                      Text = "Time " + stlye.ToString(),
                                      Value = stlye.ToString()
                                  };

            //
            // View

            TempData["results"] = resultList;
            return RedirectToAction("Results");
        }

        // GET: Results
        public ActionResult Results()
        {
            resultList = (ResultListModel)TempData["results"];

            if (resultList == null || resultList.Subtitles.Count() == 0)
            {
                TempData["error"] = "Sorry, no results could be found.";
                return RedirectToAction("Error");
            }

            return View(resultList);
        }

        // POST: Results
        [HttpPost]
        public ActionResult Results(ResultListModel model)
        {
            Subtitle selectedResult = new Subtitle();
            string error = "Sorry, no file could be found.";

            if (model.File != null)
            {
                for (int i = 0; i < model.Subtitles.Count(); i++)
                {
                    if (model.File == model.Subtitles[i].SubtitleId)
                    {
                        selectedResult = model.Subtitles[i];
                        break;
                    }
                }

                // Download file
                var osdb = Osdb.Login("Subtitle Companion v0.1");
                osdb.DownloadSubtitleToPath(Server.MapPath("~/App_Data"), selectedResult);

                // Parse file
                error = ParseSubtitles(selectedResult.SubtitleFileName, model.Time, model.Style);

                // Delete file off server
                var filePath = Path.Combine(Server.MapPath("~/App_Data"), selectedResult.SubtitleFileName);
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

        // GET: Viewer
        public ActionResult Viewer()
        {
            subtitleList = (SubtitleListModel) TempData["subtitles"];
            return View(subtitleList);
        }

        // POST: Viewer
        [HttpPost]
        public ActionResult Viewer(SubtitleListModel model, string command)
        {
            if (command.Equals("Search"))
            {
                return RedirectToAction("Search");
            }
            else if (command.Equals("Upload"))
            {
                return RedirectToAction("Upload");
            }
            else if (command.Equals("Sync"))
            {
                TempData["subtitles"] = model;
                return RedirectToAction("Sync");
            }
            else if (command.Equals("Delete"))
            {
                TempData["subtitles"] = null;
                return RedirectToAction("Viewer");
            }
            else
            {
                for (int i = 0; i < model.Subtitles.Count(); i++)
                {
                    string timeStr = model.Subtitles[i].Time;
                    string editTimeStr = command;
                    if (timeStr.Equals(editTimeStr))
                    {
                        model.ID = i;
                        break;
                    }
                }

                TempData["subtitles"] = model;
                return RedirectToAction("Sync");
            }
        }

        // GET: Sync
        public ActionResult Sync()
        {
            subtitleList = (SubtitleListModel)TempData["subtitles"];

            if (subtitleList == null || subtitleList.Subtitles.Count == 0)
            {
                TempData["error"] = "Sorry, we could not find any subtitles to synchronise.";
                return RedirectToAction("Error");
            }

            return View(subtitleList);
        }

        // POST: Sync
        [HttpPost]
        public ActionResult Sync(SubtitleListModel model, string cmd)
        {
            if (cmd == "Sync")
            {
                TimeSpan timeDiff = (model.EditTime - TimeSpan.Parse(model.Subtitles[model.ID].Time));

                for (int i = 0; i < model.Subtitles.Count(); ++i)
                {
                    TimeSpan timeSpan = TimeSpan.Parse(model.Subtitles[i].Time);
                    timeSpan += timeDiff;

                    model.Subtitles[i].Time = timeSpan.ToString();
                }
            }

            TempData["subtitles"] = model;
            return new RedirectResult(Url.Action("Viewer") + "#" + model.EditTime.ToString());
        }

        // GET: Error
        public ActionResult Error()
        {
            string errorMsg = (string)TempData["error"];
            return View((object)errorMsg);
        }
    }
}