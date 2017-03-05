using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Question
    {
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }

        public List<Answer> Answers { get; set; }

        public List<string> Tags { get; set; }

        public Question()
        {
            Created = DateTime.UtcNow;
            Answers = new List<Answer>();
            Tags = new List<string>();
        }
    }

    public class Answer
    {
        private static int _idCounter;

        public Answer()
        {
            Id = _idCounter;
            _idCounter++;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public string HtmlBody { get; set; }
    }

    public class Api
    {
        private static List<Question> _questions = new List<Question>();

        public int Add(Question question)
        {
            question.Id = _questions.Count;
            var md = new MarkdownSharp.Markdown();
            question.HtmlBody = md.Transform(question.Body);
            //todo this will break any comments with code tags!!
            question.HtmlBody = question.HtmlBody.Replace("<code>", "<pre class='prettyprint'>");
            question.HtmlBody = question.HtmlBody.Replace("</code>", "</pre>");
            _questions.Add(question);
            return question.Id;
        }

        public Question Get(int id)
        {
            var q = _questions[id];
            q.Answers = q.Answers.OrderBy(a => a.Score).Reverse().ToList();
            return q;
        }

        public void DeleteAnswer(int answerId)
        {
            Question question = null;
            Answer answer = null;
            foreach (var q in _questions)
            {
                foreach (var a in q.Answers)
                {
                    if (a.Id == answerId)
                    {
                        question = q;
                        answer = a;
                        break;
                    }
                }
            }

            if (question != null)
                question.Answers.Remove(answer);
        }

        public void Vote(int answerId, int direction)
        {
            foreach(var q in _questions)
            {
                foreach(var a in q.Answers)
                {
                    if (a.Id == answerId)
                        a.Score += direction;
                }
            }
        }

        public List<Question> GetAll()
        {
            return _questions.OrderBy(q => q.Created).Reverse().ToList();
        }

        public void AddAnswer(int questionId, string answer)
        {
            var a = new Answer() { Body = answer };
            var md = new MarkdownSharp.Markdown();
            a.HtmlBody = md.Transform(a.Body);
            a.HtmlBody = a.HtmlBody.Replace("<code>", "<pre class='prettyprint'>");
            a.HtmlBody = a.HtmlBody.Replace("</code>", "</pre>");
            _questions[questionId].Answers.Add(a);
        }

        static Api()
        {
            var questions = new List<Question>();
            questions.Add(new Question() { Id = 0, Body = @"<pre>
var i = 0;
</pre>

<p>How the hell do i do this????</p>", Title = "How do i get this working?!?", Username = "fred" });
            questions.Add(new Question() { Id = 1, Body = @"I made following code for Gender field in a form and i can't find my error. Any help is appreciated

    <pre>Gender =
 if (male.isSelected()) Gender = 'Male';
            else if (female.isSelected()) Gender = 'Female';</pre>
            I am new to NetBeans and this Site.So Please Help Me

I get the error in if statement', Title = 'argh2', Username = 'fred' });", Title = "Error in code", Username = "sillyidiot" });

            questions.Add(new Question() { Id = 2, Body = "help", Title = "argh3", Username = "fred" });
            _questions = questions;

            foreach(var q in _questions)
            {
                q.Tags.Add("Stuff");

                var md = new MarkdownSharp.Markdown();
                q.HtmlBody = md.Transform(q.Body);
                foreach (var a in q.Answers)
                    a.HtmlBody = md.Transform(a.Body);
            }
            _questions[0].Tags.Add("Things");
        }
    }
}
