using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;
using NLog;

namespace Task1
{
    public class BookXmlStorage : IRepository<Book>
    {
        private List<Book> listBook = new List<Book>();
        private readonly Stream fileStream;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BookXmlStorage(Stream fileStream)
        {
            if(fileStream == null)
                throw new ArgumentNullException();
            try
            {
                if (fileStream.CanSeek && fileStream.CanRead && fileStream.CanWrite)
                    this.fileStream = fileStream;
                else
                    throw new ArgumentException("Require canSeek, canRead and canWrite stream", nameof(fileStream));
                fileStream.Position = 0;
            }
            catch (ArgumentException ex)
            {
                Logger.Fatal("Fatal exception from argument {0} Error: {1}", nameof(fileStream), ex.Message);
                throw ex;
            }
        }  
        public void Add(Book entity)
        {
            if(!listBook.Contains(entity))
                listBook.Add(entity);
        }

        public void Delete(Book entity)
        {
            listBook.Remove(entity);
        }

        public List<Book> FindByTag(string tag)
        {
            if(tag == null)
                throw new ArgumentNullException();
            List<Book> books = listBook.Where(book => book.GetTags().Any(x => x == tag)).ToList();
            Logger.Info(books.Count == 0 ? "has found book by tag" + tag : "not found book by tag" + tag);
            return books;
        }

        public void SortByTag(Comparison<Book> comparison)
        {
            for(int i=listBook.Count-1; i>=0; i--)
                for (int j = 0; j < i; j++)
                {
                    if(comparison(listBook[j],listBook[j+1]) > 0)
                        Swap(listBook[j], listBook[j+1]);
                }
        }

        public void Load()
        {
            fileStream.Position = 0;
            using (var reader = XmlReader.Create(fileStream))
            {
                XElement el;
                string author = null, name = null;
                listBook = new List<Book>();
                List<string> tags = new List<string>();
                bool canread = reader.Read();
                while (true)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Tags":
                                    tags = new List<string>();
                                    canread = reader.Read();
                                    if (!canread)
                                        return;
                                    break;
                                case "Name":
                                    el = (XElement)XNode.ReadFrom(reader);
                                    name = el.Value;
                                    break;
                                case "Author":
                                    el = (XElement)XNode.ReadFrom(reader);
                                    author = el.Value;
                                    break;
                                case "Tag":
                                    el = (XElement)XNode.ReadFrom(reader);
                                    tags.Add(reader.Value);
                                    break;
                                default:
                                    canread = reader.Read();
                                    if (!canread)
                                        return;
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Book")
                                listBook.Add(new Book(name, author, tags.ToArray()));
                            canread = reader.Read();
                            if (!canread)
                                return;
                            break;
                        default:
                            canread = reader.Read();
                            if (!canread)
                                return;
                            break;
                    }
                }
            }
        }

        public void SerializeLoad(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var filestream = new FileStream(fileName, FileMode.Open))
            {
                listBook = (List<Book>) formatter.Deserialize(filestream);
            }
        }
        public void Save()
        {
            fileStream.SetLength(0);
            using (var xmlWriter = XmlWriter.Create(fileStream))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Books");
                foreach (var book in listBook)
                {
                    xmlWriter.WriteStartElement("Book");
                    xmlWriter.WriteElementString("Name", book.Name);
                    xmlWriter.WriteElementString("Author",book.Author);
                    xmlWriter.WriteStartElement("Tags");
                    foreach (var tag in book.GetTags())
                    {
                        xmlWriter.WriteElementString("Tag",tag);
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndDocument();
            }
        }

        public void SerializeSave(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var filestream = new FileStream(fileName,FileMode.Create))
            {
                formatter.Serialize(filestream,listBook);
            }
        }
        private void Swap(Book lhs,Book rhs)
        {
            Book temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}