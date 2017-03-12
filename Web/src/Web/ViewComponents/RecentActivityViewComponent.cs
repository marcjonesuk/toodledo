using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Controllers;

namespace Web.ViewControllers
{
    public class RecentActivityViewComponent : ViewComponent
    {
        private static List<Content> _cache;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                //if (_cache == null)
                //{
                var controller = new ContentController();
                _cache = ContentApi.Search(25, 1, null, null, null, null);
                //}

                foreach (var content in _cache)
                {
                    if (content.Type == "answer")
                    {
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
