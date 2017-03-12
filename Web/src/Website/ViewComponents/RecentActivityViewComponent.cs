﻿using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models.ContentViewModels;
using Website;
using Website.RequestObjects;

namespace Web.ViewControllers
{
    public class RecentActivityViewComponent : ViewComponent
    {
        private static List<ContentViewModel> _cache;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                if (_cache == null) {
                    var controller = new ContentManager();
                    _cache = controller.Search(new SearchRequest() { PageSize = 25 });

                    foreach (var content in _cache)
                    {
                        if (content.Type == "answer")
                        {
                            var parent = ContentApi.GetParent(content.Id).First();
                            content.Title = parent.Title;
                            content.ParentId = parent.Id;
                        }
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