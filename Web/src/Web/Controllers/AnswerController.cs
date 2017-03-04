using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Web.Controllers
{
    public class AnswerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add(int questionId)
        {
            return View(new Answer());
        }

        //[HttpPost]
        //public IActionResult Add(Answer answer)
        //{
        //    return View();
        //}
    }
}
