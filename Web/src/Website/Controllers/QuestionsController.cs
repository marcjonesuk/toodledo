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

namespace Web.Controllers
{
    public class SearchResultPageModel
    {
        public SearchResultPageModel()
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
    }

    public class QuestionsController : BaseController
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly SignInManager<ApplicationUser> signInManager;

        public QuestionsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
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
            return View(new ContentPageModel() { Content = content });
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
            var contentManager = new ContentManager(GetCurrentUser());
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

        public IActionResult Search(int p, string o, string q, int t)
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

            var manager = new ContentManager();
            var results = manager.Search(searchRequest);
            var resultPage = new SearchResultPageModel() { Results = results, Request = searchRequest };
            resultPage.ResultsCount = ContentApi.GetSearchResultCount("question", q, t);
            resultPage.MaxPages = Math.Min(5, (int)Math.Floor((double)resultPage.ResultsCount / 10));

            if (resultPage.ResultsCount % 10 != 0)
                resultPage.MaxPages++;

            //move to Tag controller
            resultPage.Tags = TagApi.Select().OrderByDescending(tag => tag.Count).Take(8).ToList();

            return View("Results", resultPage);
        }

        [Authorize]
        public IActionResult Ask(int? id)
        {
            if (id == null)
            {
                return View(new ContentRequest { Type = "question" });
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
