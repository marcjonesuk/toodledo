using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models.ContentViewModels;
using Website;
using Website.RequestObjects;
using Microsoft.Extensions.Caching.Memory;

namespace Web.ViewControllers
{
    public class RecentActivityViewComponent : ViewComponent
    {
        private IMemoryCache _cache;

        public RecentActivityViewComponent(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var key = "recent-activity";
                List<ContentViewModel> data;
                if (!_cache.TryGetValue(key, out data))
                {
                    var controller = new ContentManager();
                    data = controller.Search(new SearchRequest() { PageSize = 25 });

                    foreach (var content in data)
                    {
                        if (content.Type == "answer")
                        {
                            var parent = ContentApi.GetParent(content.Id).First();
                            content.Title = parent.Title;
                            content.ParentId = parent.Id;
                        }
                    }
                    _cache.Set(key, data, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
                }
                return View(data);
            }
            catch (Exception e)
            {
                return View(new List<ContentViewModel>());
            }
        }
    }
}
