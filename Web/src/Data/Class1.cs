using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Question
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Answer> Answers { get; set; }

        public List<string> Tags { get; set; }

        public Question()
        {
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
    }

    public class Api
    {
        private static List<Question> _questions = new List<Question>();

        public int Add(Question question)
        {
            question.Id = _questions.Count;
            _questions.Add(question);
            return question.Id;
        }

        public Question Get(int id)
        {
            return _questions[id];
        }

        public Question DeleteAnswer(int questionId, int answerId)
        { 
            _questions[questionId].Answers.RemoveAt(answerId);
            return _questions[questionId];
        }

        public List<Question> GetAll()
        {
            return _questions;
        }

        public void AddAnswer(int questionId, string answer)
        {
            _questions[questionId].Answers.Add(new Answer() { Body = answer });
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
            }
            _questions[0].Tags.Add("Things");
        }
    }
}
