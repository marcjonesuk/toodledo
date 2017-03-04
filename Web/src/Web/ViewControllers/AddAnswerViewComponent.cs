using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewControllers
{
    public class AddAnswerViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int questionId)
        {
            return View(new Answer());
        }
    }
}
