using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {

                //var l = new LocalRepository();
                //l.SetAsync(new Content() { Id = "" });

                //var item = new Content() { Id = "test1", Body = "body" };

                //DocumentDBRepository<Content>.Initialize();

                //var posts = DocumentDBRepository<Content>.GetItemsAsync(c => c.UserId == null).Result;

                //DocumentDBRepository<Content>.CreateItemAsync(item).Wait();

                //var dv = new StackOverflowData();
                //dv.Test();

                //var x = DocumentDBRepository<Content>.GetItemsAsync(a => a.Body == "body").Result;

                //foreach(var i in x)
                //{
                //    DocumentDBRepository<Content>.DeleteItemAsync(i.Id);
                //}
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }
}
