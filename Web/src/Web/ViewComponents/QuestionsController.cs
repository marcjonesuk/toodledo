using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Web.Controllers
{
    public class QuestionList
    {
        public QuestionList()
        {
            Questions = new List<Question>();
        }
        public List<Question> Questions { get; set; }
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

    public class ShowQuestionPage
    {
        public Question Question { get; set; }
        public string Response { get; set; }
    }

    public class QuestionsController : Controller
    {
        public IActionResult Show(int id, string response = null)
        {
            var question = (new Api()).Get(id);
            ViewData["Title"] = question.Title;
            return View(new ShowQuestionPage() { Question = question, Response = response });
        }

        public IActionResult Edit(int id)
        {
            var question = (new Api()).Get(id);
            return View(question);
        }

        [HttpPost]
        public void DeleteAnswer([FromBody]DeleteRequest req)
        {
            var api = new Api();
            api.DeleteAnswer(req.AnswerId);
        }

        [HttpPost]
        public int Vote([FromBody]VoteRequest req)
        {
            var api = new Api();
            return api.Vote(req.AnswerId, req.Direction);
        }

        [HttpPost]
        public string Answer([FromBody]AnswerRequest req)
        {
            var api = new Api();

            if (req.AnswerId == 0)
            {
                api.AddAnswer(req.QuestionId, req.Answer);
                return null;
            }
            else
            {
                return api.EditAnswer(req.AnswerId, req.Answer).HtmlBody;
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

        public IActionResult Search(int p, string order, string q, int pagesize)
        {
            if (order == null)
                order = "mostrecent";

            if (p == 0)
                p = 1;

            if (pagesize == 0)
                pagesize = 10;

            ViewData["Title"] = "Latest questions";

            var api = new Api();
            var result = new QuestionList() { Page = p };

            if (string.IsNullOrEmpty(q))
            {
                var data = api.GetAll();
                result.ResultsCount = data.Count;
                result.Questions = data.Skip((p - 1) * pagesize).Take(pagesize).ToList();
            }
            else {
                var data = api.GetAll().Where(question => question.Body.ToLower().Contains(q) || question.Title.ToLower().Contains(q));
                result.ResultsCount = data.Count();
                result.Questions = data.Skip((p - 1) * pagesize).Take(pagesize).ToList();
            }

            result.SearchText = q;
            result.MaxPages = (int)Math.Floor((double)result.ResultsCount / pagesize);

            if (result.ResultsCount % pagesize != 0)
                result.MaxPages++;

            result.MaxPages = Math.Min(pagesize, result.MaxPages);
            return View("Results", result);
        }

        public IActionResult Ask(int? id)
        {
            if (id == null)
                return View(new Question());

            var api = new Api();
            var q = api.Get(id.Value);

            ViewData["Title"] = q.Title;
            return View(q);
        }

        [HttpPost]
        public IActionResult Ask(Question question)
        {
            var api = new Api();
            int id;

            if (question.Id == -1)
            {
                id = api.Add(new Question()
                {
                    Title = question.Title,
                    Body = question.Body
                });
            }
            else
            {
                id = api.Update(question.Id, question.Title, question.Body);
            }

            return RedirectToAction("Show", new { Id = id, Response = "Your question was added" });
        }

        [HttpPost]
        public IActionResult New(Question question)
        {
            var id = (new Api()).Add(question);
            return RedirectToAction("Get", new{ id });
        }
    }
}
