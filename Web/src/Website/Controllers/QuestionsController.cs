using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Website.Models;
using System.Security.Claims;
using Website.Controllers;
using Website.Models.ContentViewModels;

namespace Web.Controllers
{
    public class Markdown
    {
        public static string Encode(string text)
        {
            var md = new MarkdownSharp.Markdown();
            return md.Transform(text);
            //question.HtmlBody = question.HtmlBody.Replace("<code>", "<pre class='prettyprint'>");
            //question.HtmlBody = question.HtmlBody.Replace("</code>", "</pre>");
        }
    }

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

    public class DeleteRequest
    {
        public int AnswerId { get; set; }
    }

    public class VoteRequest
    {
        public int AnswerId { get; set; }
        public int Direction { get; set; }
    }

    public class AnswerRequest
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }

    public class ContentPageModel
    {
        public ContentViewModel Content { get; set; }
        public string Response { get; set; }
        public bool AllowEdit { get; set; }
    }

    [AllowAnonymous]
    public class QuestionsController : Controller
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
            var contentController = new ContentController(userManager);
            var content = contentController.Index(id);
            ViewData["Title"] = content.Title;
            return View(new ContentPageModel() { Content = content } );
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
            return VoteApi.Vote(req.AnswerId, 1, req.Direction);
        }

        [HttpPost]
        public string Answer([FromBody]AnswerRequest req)
        {
            var controller = new ContentController(userManager);
            if (req.AnswerId == 0)
            {
                var c = controller.CreateChild(new CreateContentRequest() { ParentId = req.QuestionId, Body = req.Answer, Type = "answer" });
                return c.HtmlBody;
            }
            else
            {
                var c = controller.Update(new CreateContentRequest() { ParentId = req.QuestionId, Body = req.Answer, Type = "answer" });
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

            var controller = new ContentController(userManager);
            var results = controller.Search(searchRequest);
            var resultPage = new SearchResultPageModel() { Results = results, Request = searchRequest };
            resultPage.ResultsCount = ContentApi.GetSearchResultCount("question", q, t);
            resultPage.MaxPages = Math.Min(5, (int)Math.Floor((double)resultPage.ResultsCount / 10));

            if (resultPage.ResultsCount % 10 != 0)
                resultPage.MaxPages++;

            //move to Tag controller
            resultPage.Tags = TagApi.Select().OrderByDescending(tag => tag.Count).Take(8).ToList();

            return View("Results", resultPage);
        }

        public User GetCurrentUser()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var user = claim.Value;
            var currentUser = UserApi.GetByAspNetId(user);
            return currentUser;
        }

        [Authorize]
        public IActionResult Ask(int? id)
        {
            if (id == null)
                return View(new Content() { UserId = GetCurrentUser().Id, Type = "question" });

            var c = ContentApi.Select(id.Value);
            ViewData["Title"] = c.Title;
            return View(c);
        }

        [HttpPost]
        public IActionResult Ask(Content request)
        {
            var controller = new ContentManager(userManager);
            if (request.Id == 0)
            {
                var id = controller.Create(new CreateContentRequest() { Type = "question", Title = request.Title, Body = request.Body });
                return RedirectToAction("Show", new { Id = id, Response = "Your question was added" });
            }
            else
            {
                throw new NotImplementedException();
                //ContentApi.Update(content.Id, content.Title, content.Body, Markdown.Encode(content.Body));
            }
        }

        //[HttpPost]
        //public IActionResult New(Question question)
        //{
        //    return RedirectToAction("Get", new { id });
        //}
    }
}
