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
            FileStream fileStream = new FileStream("1.xml",FileMode.Open);
            BookXmlStorage bxStorage = new BookXmlStorage(fileStream);
            //bxStorage.Add(book1);
            //bxStorage.Add(book2);
            //bxStorage.Save();
            bxStorage.Load();
            bxStorage.SerializeSave("serialize.dat");
            bxStorage.SerializeLoad("serialize.dat");
            //BookFileStorage bookStorage = new BookFileStorage(fileStream);
            //bookStorage.Add(book1);
            //bookStorage.Add(book2);
            //bookStorage.FindByTag("tg");
            //bookStorage.Delete(book1);
        }
    }
}
