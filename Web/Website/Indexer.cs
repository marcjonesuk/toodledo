using Data;
using LuceneSearch;
using System;
using System.Linq;
using System.Timers;
using Website;

namespace Website
{
    public class Indexer
    {
        private static Lazy<Indexer> _instance = new Lazy<Indexer>();
        public static Indexer Instance {  get { return _instance.Value; } }
        private Timer _t;

        public void Start(TimeSpan interval)
        {
            _t = new Timer();
            _t.Elapsed += T_Elapsed;
            _t.Interval = interval.TotalMilliseconds;
            _t.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _t.Stop();
                Update();
            }
            finally
            {
                _t.Start();
            }
        }

        private void Update()
        {
            var content = ContentApi.NeedIndexing();
            var vms = content.Select(c => c.AsViewModel().WithUser());
            Searcher.Instance.Index(vms.Select(c => new Searchable() { Id = c.Id, Title = c.Title, Body = c.Body, Type = c.Type, Username = c.User.DisplayName }));
            
            foreach(var c in content)
            {
                ContentApi.MarkAsIndexed(c.Id);
            }
        }
    }
}