using Lucene.Net.Store;
using LuceneSearch;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SearchTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var search = new Searcher(@"c:\lucene");
            var result = search.Search("beer", 100, 0);
            Console.WriteLine(result.Count());
            Console.ReadLine();
        }

        public void Update(int id, string title)
        {

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

            var dir = FSDirectory.Open(new System.IO.DirectoryInfo(@"index"));
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