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

    public class ContentListModel
    {
        public ContentListModel()
        {
            Content = new List<Content>();
        }
        public List<Tag> Tags { get; set; }
        public List<Content> Content { get; set; }
        public string SearchText { get; set; }
        public int ResultsCount { get; set; }
        public int Page { get; set; }
        public string OrderBy { get; set; }
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

    public class ShowContentModel
    {
        public Content Content { get; set; }
        public string Response { get; set; }
        public bool AllowEdit { get; set; }
    }

    public class QuestionsController : Controller
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly SignInManager<ApplicationUser> signInManager;

        public QuestionsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Show(int id, string response = null)
        {
            var content = ContentApi.Select(id);
            ViewData["Title"] = content.Title;

            var user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(new ShowContentModel() { Content = content, Response = response });
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
            if (req.AnswerId == 0)
            {
                var md = Markdown.Encode(req.Answer);
                var htmlBody = Markdown.Encode(req.Answer);
                ContentApi.InsertAsChild(req.QuestionId, new Content() { Type="answer", Body = req.Answer, UserId = 1, HtmlBody = htmlBody });
                return md;
            }
            else
            {
                var md = Markdown.Encode(req.Answer);
                ContentApi.Update(req.AnswerId, null, req.Answer, md);
                return md;
            }
        }

        public IActionResult Search(int p, string o, string q, int t)
        {
            var db = new DbApi();

            if (p == 0)
                p = 1;

            int? tagId = t;
            if (t == 0)
                tagId = null;

            if (o == null)
                o = "created-desc";

            var content = ContentApi.Search(10, p, "question", q, o, tagId);
            
            var result = new ContentListModel();
            result.Content = content;
            result.ResultsCount = ContentApi.GetSearchResultCount("question", q, t);

            result.MaxPages = Math.Min(5, (int)Math.Floor((double)result.ResultsCount / 10));

            if (result.ResultsCount % 10 != 0)
                result.MaxPages++;

            result.Page = p;
            result.SearchText = q;
            result.OrderBy = o;
            result.Tags = TagApi.Select().OrderByDescending(tag => tag.Count).Take(8).ToList();

            return View("Results", result);
        }

        [Authorize]
        public IActionResult Ask(int? id)
        {
            if (id == null)
                return View(new Content() { UserId = 60, Type = "question" });

            var c = ContentApi.Select(id.Value);
            ViewData["Title"] = c.Title;
            return View(c);
        }

        [HttpPost]
        public IActionResult Ask(Content content)
        {
            int id = content.Id;
            if (id == 0)
            {
                content.HtmlBody = Markdown.Encode(content.Body);
                id = ContentApi.Insert(content);
            }
            else
            {
                ContentApi.Update(content.Id, content.Title, content.Body, Markdown.Encode(content.Body));
            }

            return RedirectToAction("Show", new { Id = id, Response = "Your question was added" });
        }

        //[HttpPost]
        //public IActionResult New(Question question)
        //{
        //    return RedirectToAction("Get", new { id });
        //}
    }
}
