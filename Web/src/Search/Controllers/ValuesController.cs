using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Search.Controllers
{
    public class ValuesController : ApiController
    {
        private static bool built;

        public IEnumerable<int> Get(string search, int max = 100, int minScore = 1)
        {
            if (!built)
            {
                BuildIndexes();
                built = true;
            }
            try
            {
                var dir = FSDirectory.Open(new System.IO.DirectoryInfo(@"C:\lucene"));
                Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                Lucene.Net.QueryParsers.QueryParser parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_30, "body", analyzer);
                Lucene.Net.Search.Query query = null;
                query = parser.Parse(search);
                var searcher = new Lucene.Net.Search.IndexSearcher(dir);
                var hits = searcher.Search(query, max);
                var doc = searcher.Doc(hits.ScoreDocs[0].Doc);
                var result = hits.ScoreDocs.Where(s => s.Score > minScore).Select(h => int.Parse(searcher.Doc(h.Doc).GetField("id").StringValue));
                return result;
            }
            catch(Exception e )
            {
                throw;
            }
        }

        public static void BuildIndexes()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=toodledo;Integrated Security=SSPI;");
            SqlCommand cmd = new SqlCommand();
            //SqlDataReader reader;
            cmd.CommandText = "SELECT id, title, body FROM dbo.Content";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            connection.Open();
            var reader = cmd.ExecuteReader();

            var dir = FSDirectory.Open(new System.IO.DirectoryInfo(@"C:\lucene"));
            Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Lucene.Net.Index.IndexWriter writer = new Lucene.Net.Index.IndexWriter(dir, analyzer, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);
            writer.DeleteAll();

            while (reader.Read())
            {
                var id = reader.GetFieldValue<int>(0);
                var title = reader.GetFieldValue<string>(1);
                var body = reader.GetFieldValue<string>(2);
                writer.AddDocument(create_doc(id, title, body));
            }
            connection.Close();

            writer.Optimize();
            writer.Commit();
        }

        static Lucene.Net.Documents.Document create_doc(int id, string text, string body)
        {
            Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();

            doc.Add(new Lucene.Net.Documents.Field(
                "title",
                text,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED
                ));

            doc.Add(new Lucene.Net.Documents.Field(
                "body",
                body,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "id",
                Convert.ToString(id),
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            return doc;
        }
    }
}
