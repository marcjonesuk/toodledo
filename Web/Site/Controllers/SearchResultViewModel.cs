using System.Collections.Generic;
using Data;
using Website.Models.ContentViewModels;
using Website.RequestObjects;

namespace Web.Controllers
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
            Results = new List<ContentViewModel>();
        }

        public List<Tag> Tags { get; set; }
        public List<ContentViewModel> Results { get; set; }
        public SearchRequest Request { get; set; }

        public int ResultsCount { get; set; }
        public int MaxPages { get; set; }
    }
}
