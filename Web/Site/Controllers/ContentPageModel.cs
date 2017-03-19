using System.Collections.Generic;
using Website.Models.ContentViewModels;

namespace Web.Controllers
{
    public class ContentPageModel
    {
        public ContentViewModel Content { get; set; }
        public string Response { get; set; }
        public bool AllowEdit { get; set; }

        public List<ContentViewModel> SimilarQuestions { get; set; } 
    }
}
