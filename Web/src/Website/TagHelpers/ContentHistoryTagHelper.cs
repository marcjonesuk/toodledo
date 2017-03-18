using Data;
using Data.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.TagHelpers
{
    public class ContentHistoryTagHelper : TagHelper
    {
        public List<ContentHistory> History { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = "";
            foreach (var item in History)
            {
                var changedByUser = UserApi.GetById(item.ChangedByUserId);
                content += "<div>" + item.ChangedField + ":" + item.OldValue + "</div>";
                content += "<div>Changed by " + changedByUser.DisplayName + "</div>";
            }

            //var address = MailTo + "@" + EmailDomain;
            // output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetHtmlContent(content);
        }
    }
}
