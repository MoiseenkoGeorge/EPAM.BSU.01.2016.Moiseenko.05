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
            Book book = new Book("iam", "me", "tag", "chlen");
            FileStream fileStream = new FileStream("1.txt",FileMode.Truncate);
            BookStorage bookStorage = new BookStorage(fileStream);
            bookStorage.AddBook(book);
            bookStorage.FindByTag("tg");
        }
    }
}
