using Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tags = TagApi.Select();

            foreach(var t in tags)
            {
                var content = ContentApi.Search(1000000, 1, "question", t.Name, null, null);

                foreach(var c in content)
                {
                    TagApi.Tag(c.Id, t.Id);
                    Console.Write('.');
                }
            }

            
            //RelationAttacher ra = new RelationAttacher();
            //DateAdder da = new DateAdder();
            //try
            //{
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            //Console.ReadLine();
        }
    }

    public class RowAndIdx
    {
        public Data.StackOverflowModel.Posts.row Row { get; set; }
        public int Index { get; set; }
    }

    public class DateAdder
    {
        public DateAdder()
        {
            var d = new StackOverflowData();
            var posts = d.GetPosts().rows;
            var postsWithIndexes = new List<RowAndIdx>();

            for (int i = 0; i < posts.Count; i++)
            {
                var post = posts[i];
                postsWithIndexes.Add(new RowAndIdx { Row = post, Index = i + 1 });
            }

            foreach (var answer in postsWithIndexes)
            {
                AddCreatedDate(answer.Index, DateTime.ParseExact(answer.Row.CreationDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture));
            }
        }

        private void AddCreatedDate(int contentid, DateTime date)
        {
            var dateStr = date.ToString("yyyy-MM-dd h:m:s");
            ContentApi.AddCreatedDate(contentid, dateStr);
        }
    }

    public class RelationAttacher
    {
        public RelationAttacher()
        {
            var d = new StackOverflowData();
            var posts = d.GetPosts().rows;
            var questions = new Dictionary<int, RowAndIdx>();
            var answers = new List<RowAndIdx>();

            for (int i = 0; i < posts.Count; i++)
            {
                var post = posts[i];
                if (post.PostTypeId == "1")
                {
                    questions.Add(int.Parse(post.Id), new RowAndIdx { Row = post, Index = i + 1 });
                }
                else
                {
                    answers.Add(new RowAndIdx { Row = post, Index = i + 1 });
                }
            }

            //for (int i = 0; i < answers.Count; i++)
            foreach (var answer in answers)
            {
                if (answer.Row.ParentId != null)
                {
                    var question = questions[int.Parse(answer.Row.ParentId)];
                    AddRelation(question.Index, answer.Index);
                }
            }
        }

        private void AddRelation(int parentId, int childId)
        {
            ContentApi.Relate(parentId, childId);
        }
    }
}
