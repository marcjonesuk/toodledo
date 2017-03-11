using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

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
        public List<Content> Content { get; set; }
        public string SearchText { get; set; }
        public int ResultsCount { get; set; }
        public int Page { get; set; }
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
    }

    public class QuestionsController : Controller
    {
        public IActionResult Show(int id, string response = null)
        {
            var content = ContentApi.Select(id);
            ViewData["Title"] = content.Title;
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

        //[HttpPost]
        //public int Vote([FromBody]VoteRequest req)
        //{
        //    var api = new Api();
        //    return api.Vote(req.AnswerId, req.Direction);
        //}

        [HttpPost]
        public string Answer([FromBody]AnswerRequest req)
        {
            if (req.AnswerId == 0)
            {
                var md = Markdown.Encode(req.Answer);
                ContentApi.InsertAsChild(req.QuestionId, new Content() { Type="answer", Body = req.Answer, UserId = 1 });
                return md;
            }
            else
            {
                var md = Markdown.Encode(req.Answer);
                ContentApi.Update(req.AnswerId, null, req.Answer, md);
                return md;
            }
        }

        //public IActionResult Search(string text)
        //{
        //    var api = new Api();
        //    var result = new QuestionList() { Page = 1 };
        //    result.SearchText = text;
        //    if (!string.IsNullOrWhiteSpace(text))
        //        result.Questions = api.GetAll().Where(q => q.Body.ToLower().Contains(text) || q.Title.ToLower().Contains(text)).Take(25).ToList();
        //    return View("Results", result);
        //}

        //public IActionResult Search(int p, string order, string q, int pagesize)
        //{
        //    if (order == null)
        //        order = "mostrecent";

        //    if (p == 0)
        //        p = 1;

        //    if (pagesize == 0)
        //        pagesize = 10;

        //    ViewData["Title"] = "Latest questions";

        //    var api = new Api();
        //    var result = new ContentList() { Page = p };

        //    if (string.IsNullOrEmpty(q))
        //    {
        //        var data = api.GetAll();
        //        result.ResultsCount = data.Count;
        //        result.Content = data.Skip((p - 1) * pagesize).Take(pagesize).ToList();
        //    }
        //    else
        //    {
        //        var data = api.GetAll().Where(question => question.Body.ToLower().Contains(q) || question.Title.ToLower().Contains(q));
        //        result.ResultsCount = data.Count();
        //        result.Questions = data.Skip((p - 1) * pagesize).Take(pagesize).ToList();
        //    }

        //    result.SearchText = q;
        //    result.MaxPages = (int)Math.Floor((double)result.ResultsCount / pagesize);

        //    if (result.ResultsCount % pagesize != 0)
        //        result.MaxPages++;

        //    result.MaxPages = Math.Min(pagesize, result.MaxPages);
        //    return View("Results", result);
        //}

        public IActionResult Search(int p, string order, string q, int pagesize)
        {
            var db = new DbApi();
            var content = ContentApi.SelectByType("question");

            var result = new ContentListModel();
            result.Content = content;
            result.ResultsCount = 10;
            result.MaxPages = 1;
            result.SearchText = q;
            return View("Results", result);
        }

        public IActionResult Ask(int? id)
        {
            if (id == null)
                return View(new Content());

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
