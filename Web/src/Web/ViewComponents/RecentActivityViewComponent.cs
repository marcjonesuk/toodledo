using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewControllers
{
    public class RecentActivityViewComponent : ViewComponent
    {
        private static List<Content> _cache;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_cache == null)
            {
                _cache = ContentApi.Search(25, 1, null, null, null, null);
            }
            return View(_cache);
        }
    }
}
