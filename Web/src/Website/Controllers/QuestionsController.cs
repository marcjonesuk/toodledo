using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Website.Models;
using System.Security.Claims;
using Website;
using Website.Models.ContentViewModels;
using Website.RequestObjects;
using Website.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Data.Db;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace Web.Controllers
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
            Results = new List<ContentViewModel>();
        }

        public List<Tag> Tags { get; set; }
        public List<ContentViewModel> Results { get; set; }
        public SearchRequest Request { get; set; }

        public int ResultsCount { get; set; }
        public int MaxPages { get; set; }
    }

    public class ContentPageModel
    {
        public ContentViewModel Content { get; set; }
        public string Response { get; set; }
        public bool AllowEdit { get; set; }

        public List<ContentViewModel> SimilarQuestions { get; set; } 
    }

    public class QuestionsController : BaseController
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly SignInManager<ApplicationUser> signInManager;

        public QuestionsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMemoryCache cache)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            DbApi.Cache = cache;
        }

        public IActionResult Show(int id)
        {
            var contentController = new ContentManager();
            var content = contentController.Get(id);
            ViewData["Title"] = content.Title;
            var answer = content.Children.FirstOrDefault();
            if (answer != null)
            {
                content.Children = new List<ContentViewModel> { answer };
            }
            content.History = ContentHistoryApi.SelectByContentId(id);
            return View(new ContentPageModel() { Content = content });
        }

        public IActionResult Similar(int id)
        {
            var content = ContentApi.Select(id);
            WebRequest request = WebRequest.Create($"http://localhost:63683/api/Values?search={content.Title}&max=50&minScore=0");
            request.Method = "GET";
            var response = request.GetResponseAsync().Result;
            IEnumerable<int> ids;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var s = reader.ReadToEnd().Replace("[", "").Replace("]", "").Split(',');
                ids = s.Where(str => !string.IsNullOrEmpty(str)).Select(i => int.Parse(i)).ToList();
            }
            var results = ids.Where(i => i != id).Select(i => ContentApi.Select(i)).Where(c => c.Type == "question").Select(c => c.AsViewModel().WithTags().WithUser()).Take(3).ToList();
            return View(results);
        }

        public IActionResult Edit(int id)
        {
            var content = ContentApi.Select(id);
            return View(content);
        }

        //[HttpPost]
        //public void DeleteAnswer([FromBody]DeleteRequest req)
        //{
        //    var api = new Api();
        //    api.DeleteAnswer(req.AnswerId);
        //}

        [HttpPost]
        public int Vote([FromBody]VoteRequest req)
        {
            return VoteApi.Vote(req.ContentId, 1, req.Direction);
        }

        [HttpPost]
        public string Answer([FromBody]ContentRequest req)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return null;
            }
            var contentManager = new ContentManager(user);

            req.Type = "answer";
            if (req.ContentId == null)
            {
                var c = contentManager.Create(req);
                return c.HtmlBody;
            }
            else
            {
                var c = contentManager.Update(req);
                return c.HtmlBody;
            }
        }

        public IActionResult SearchOld(int p, string o, string q, int t)
        {
            if (p == 0)
                p = 1;

            int? tagId = t;
            if (t == 0)
                tagId = null;

            if (o == null)
                o = "created-desc";

            var searchRequest = new SearchRequest();
            searchRequest.Text = q;
            searchRequest.Page = p;
            searchRequest.OrderBy = o;
            searchRequest.Type = "question";
            searchRequest.TagId = tagId;

            var manager = new ContentManager();
            var results = manager.Search(searchRequest);
            var resultPage = new SearchResultViewModel() { Results = results, Request = searchRequest };
            resultPage.ResultsCount = ContentApi.GetSearchResultCount("question", q, t);
            resultPage.MaxPages = Math.Min(5, (int)Math.Floor((double)resultPage.ResultsCount / 10));

            if (resultPage.ResultsCount % 10 != 0)
                resultPage.MaxPages++;

            //move to Tag controller
            resultPage.Tags = TagApi.Select().OrderByDescending(tag => tag.Count).Take(8).ToList();

            return View("Results", resultPage);
        }

        public IActionResult Search(int p, string o, string q, int t)
        {
            if (q == null)
                return SearchOld(p, o, q, t);

            WebRequest request = WebRequest.Create($"http://localhost:63683/api/Values?search={q}&max=100");
            request.Method = "GET";
            var response = request.GetResponseAsync().Result;
            IEnumerable<int> ids;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                ids = reader.ReadToEnd().Replace("[", "").Replace("]", "").Split(',').Select(id => int.Parse(id));
            }
            var results = ids.Select(i => ContentApi.Select(i).AsViewModel().WithTags().WithUser()).ToList();

            var searchRequest = new SearchRequest();
            searchRequest.Text = q;
            searchRequest.Page = p;
            searchRequest.OrderBy = o;
            searchRequest.Type = "question";

            var resultPage = new SearchResultViewModel() { Results = results, Request = searchRequest };
            resultPage.ResultsCount = results.Count;
            resultPage.MaxPages = Math.Min(5, (int)Math.Floor((double)resultPage.ResultsCount / 10));
            resultPage.Results = results.Skip((p - 1) * 10).Take(10).ToList();
            return View("Search", resultPage);
        }

        [HttpGet]
        public IActionResult SearchForTag(int tagId)
        {
            return Search(1, null, null, tagId);
        }

        [Authorize]
        public IActionResult Ask(int? id)
        {
            if (id == null)
            {
                return View(new ContentRequest { Type = "question", AvailableTags = TagApi.SelectSuggestions() });
            }

            var c = ContentApi.Select(id.Value);
            ViewData["Title"] = c.Title;
            return View(c.AsRequest());
        }

        [HttpPost]
        public IActionResult Ask(ContentRequest request)
        {
            var user = GetCurrentUser();
            var manager = new ContentManager(user);

            if (request.ContentId == null)
            {
                var id = manager.Create(request).Id;
                return RedirectToAction("Show", new { Id = id });
            }
            else
            {
                manager.Update(request);
                return RedirectToAction("Show", new { Id = request.ContentId });
            }
        }
    }
}
