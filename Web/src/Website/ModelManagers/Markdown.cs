using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website
{
    public class Markdown
    {
        public static string Encode(string text)
        {
            var md = new MarkdownSharp.Markdown();
            text = System.Net.WebUtility.HtmlEncode(text);
            return md.Transform(text);
            //question.HtmlBody = question.HtmlBody.Replace("<code>", "<pre class='prettyprint'>");
            //question.HtmlBody = question.HtmlBody.Replace("</code>", "</pre>");
        }
    }
}
