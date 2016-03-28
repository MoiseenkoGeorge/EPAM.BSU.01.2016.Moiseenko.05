using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Task1
{
    class Program
    {
        static void Main()
        {
            Book book1 = new Book("iam", "me", "book", "myBook");
            Book book2 = new Book("iam", "me", "book", "myBook","lol");
            FileStream fileStream = new FileStream("1.txt",FileMode.Create);
            BookFileStorage bookStorage = new BookFileStorage(fileStream);
            bookStorage.AddBook(book1);
            bookStorage.AddBook(book2);
            bookStorage.FindByTag("tg");
            bookStorage.RemoveBook(book1);
        }
    }
}
