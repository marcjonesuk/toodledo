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
        private QueryParser _parser;
        private FSDirectory _dir;
        private IndexWriter _writer;

        public Searcher(string indexPath)
        {
            _dir = FSDirectory.Open(new System.IO.DirectoryInfo(indexPath));
            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            _parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "body", analyzer);
            _writer = new IndexWriter(_dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private Document CreateDocument(Searchable item)
        {
            Document doc = new Document();
            doc.Add(new Field("title", item.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("body", item.Body, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("id", Convert.ToString(item.Id), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

        public void Add(Searchable item)
        {
            _writer.AddDocument(CreateDocument(item));
            _writer.Optimize();
            _writer.Commit();
        }

        public void Add(List<Searchable> items)
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
            Add(items);
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
            var doc = searcher.Doc(hits.ScoreDocs[0].Doc);
            var result = hits.ScoreDocs
                .Where(s => s.Score > minScore)
                .Select(h => new SearchResult() { Id = int.Parse(searcher.Doc(h.Doc).GetField("id").StringValue), Score = h.Score });
            return result;
        }
    }
}
