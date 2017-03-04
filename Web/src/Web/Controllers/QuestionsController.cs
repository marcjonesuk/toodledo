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
        public List<Question> Questions { get; set; }
        public string SearchText { get; set; }
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
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }

    public class QuestionsController : Controller
    {
        public IActionResult Show(int id)
        {
            var question = (new Api()).Get(id);
            return View(question);
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
        public void Vote([FromBody]VoteRequest req)
        {
            var api = new Api();
            api.Vote(req.AnswerId, req.Direction);
        }

        [HttpPost]
        public void Answer([FromBody]AnswerRequest req)
        {
            var api = new Api();
            api.AddAnswer(req.QuestionId, req.Answer);
        }

        public IActionResult Search(string text)
        {
            var api = new Api();
            var result = new QuestionList();
            result.SearchText = text;
            result.Questions = api.GetAll().Where(q => q.Body.ToLower().Contains(text) || q.Title.ToLower().Contains(text)).ToList();
            return View("Results", result);
        }

        public IActionResult Latest()
        {
            var api = new Api();
            var result = new QuestionList();
            result.SearchText = "";
            result.Questions = api.GetAll();
            return View("Results", result);
        }

        public IActionResult Ask()
        {
            return View(new Question());
        }

        [HttpPost]
        public IActionResult Ask(Question question)
        {
            var api = new Api();
            var id = api.Add(new Question() {
                Title = question.Title,
                Body = question.Body
            });
            return RedirectToAction("Show", new { Id = id });
        }

        [HttpPost]
        public IActionResult New(Question question)
        {
            var id = (new Api()).Add(question);
            return RedirectToAction("Get", new{ id });
        }
    }
}
