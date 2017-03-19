using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace LuceneSearch
{
    public class Searcher
    {
        private static Lazy<Searcher> _lazy = new Lazy<Searcher>();
        public static Searcher Instance {
            get { return _lazy.Value; }
        }

        private QueryParser _parser;
        private FSDirectory _dir;
        private IndexWriter _writer;

        public void Open(string indexPath)
        {
            _dir = FSDirectory.Open(new System.IO.DirectoryInfo(indexPath));
            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            _parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Body", analyzer);
            _writer = new IndexWriter(_dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private Document CreateDocument(Searchable item)
        {
            Document doc = new Document();
            doc.Add(new Field("Title", item.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Body", item.Body, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Username", item.Username, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Type", item.Type, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Id", Convert.ToString(item.Id), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

        public void Index(Searchable item)
        {
            _writer.AddDocument(CreateDocument(item));
            _writer.Optimize();
            _writer.Commit();
        }

        public void Index(IEnumerable<Searchable> items)
        {
            foreach (var i in items)
            {
                _writer.AddDocument(CreateDocument(i));
            }
            _writer.Optimize();
            _writer.Commit();
        }

        public void Rebuild(List<Searchable> items)
        {
            _writer.DeleteAll();
            Index(items);
        }

        public void Delete(Searchable item)
        {
        }

        public void Update(int id, string title, string body)
        {
        }

        public IEnumerable<SearchResult> Search(string text, int maxResults, int minScore)
        {
            var query = _parser.Parse(text);
            var searcher = new Lucene.Net.Search.IndexSearcher(_dir);
            var hits = searcher.Search(query, maxResults);

            if (hits.ScoreDocs.Length == 0)
                return new List<SearchResult>();

            var doc = searcher.Doc(hits.ScoreDocs[0].Doc);
            var result = hits.ScoreDocs
                .Where(s => s.Score > minScore)
                .Select(h => new SearchResult() { Id = int.Parse(searcher.Doc(h.Doc).GetField("Id").StringValue), Score = h.Score });
            return result;
        }
    }
}
