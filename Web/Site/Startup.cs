using LuceneSearch;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(Site.Startup))]
namespace Site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Searcher.Instance.Open(@"c:\lucene2");
            Indexer.Instance.Start(TimeSpan.FromMinutes(1));
        }
    }
}
