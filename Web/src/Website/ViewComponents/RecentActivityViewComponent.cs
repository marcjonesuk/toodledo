using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Controllers;
using Website.Models.ContentViewModels;

namespace Web.ViewControllers
{
    public class RecentActivityViewComponent : ViewComponent
    {
        private static List<ContentViewModel> _cache;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var controller = new ContentController();
                var search = controller.Search(new SearchRequest() { PageSize = 25 });

                foreach (var content in search)
                {
                    if (content.Type == "answer")
                    {
                        var parent = ContentApi.GetParent(content.Id).First();
                        content.Title = parent.Title;
                        content.ParentId = parent.Id;
                    }
                }
                return View(_cache);
            }
            catch(Exception e)
            {
                return View(new List<Content>());
            }
        }
    }
}
