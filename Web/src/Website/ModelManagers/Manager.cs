using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models.ContentViewModels;
using Website.RequestObjects;

namespace Website
{
    public class ContentManager
    {
        readonly User user;

        public ContentManager()
        {
        }

        public ContentManager(User user)
        {
            this.user = user;
        }

        private void Authorized()
        {
            if (user == null)
                throw new UnauthorizedAccessException("No User set");
        }

        public ContentViewModel Get(int contentId)
        {
            return ContentApi.Select(contentId)
                .AsViewModel()
                .WithAll();
        }

        public ContentViewModel Update(ContentRequest request)
        {
            Authorized();

            if (request.ContentId == null)
                throw new ArgumentNullException(nameof(request.ContentId));

            var htmlBody = Markdown.Encode(request.Body);
            ContentApi.Update(request.ContentId.Value, request.Title, request.Body, htmlBody);

            return ContentApi.Select(request.ContentId.Value)
                .AsViewModel()
                .WithAll();
        }

        public ContentViewModel Create(ContentRequest request)
        {
            Authorized();

            var content = new Content();
            content.Title = request.Title;
            content.Body = request.Body;
            content.HtmlBody = Markdown.Encode(content.Body);
            content.UserId = user.Id;
            content.Type = request.Type;
            var id = ContentApi.Insert(content);

            if (request.ParentId != null)
            {
                ContentApi.Relate(request.ParentId.Value, id);
            }

            return ContentApi.Select(id).AsViewModel()
                .WithChildren()
                .WithChildrenCount()
                .WithTags();
        }

        public List<ContentViewModel> Search(SearchRequest request)
        {
            request.PageSize = request.PageSize ?? 10;
            request.Page = request.Page ?? 1;

            var contents = ContentApi.Search(10, request.Page, request.Type, request.Text, request.OrderBy, null);
            var searchResult = contents.Select(c => c.AsViewModel()
                    .WithTags()
                    .WithChildrenCount()
                    .WithUser()).ToList();

            return searchResult;
        }
    }
}
