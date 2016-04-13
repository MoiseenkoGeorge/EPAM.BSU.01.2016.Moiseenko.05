using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NLog;
namespace Task1
{
    
    public class BookFileStorage 
    {
        private readonly Stream fileStream;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public BookFileStorage(Stream fileStream)
        {
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
                Logger.Fatal("Fatal exception from argument {0} Error: {1}",nameof(fileStream), ex.Message);
                throw ex;
            }
        }

        public void Add(Book book)
        {
            BinaryWriter binaryWriter = null;
            try
            {
                binaryWriter = new BinaryWriter(fileStream, new UTF8Encoding(), true);
                binaryWriter.BaseStream.Position = binaryWriter.BaseStream.Length;
                WriteOneBook(binaryWriter,book);
            }
            catch (IOException ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
            finally
            {
                binaryWriter?.Dispose();
                fileStream.Position = 0;
            }
        }

        public List<Book> FindByTag(string tag)
        {
            BinaryReader binaryReader = null;
            List<Book> books = new List<Book>();
            string name, author, tags;
            string[] tagArray;
            try
            {
                binaryReader = new BinaryReader(fileStream, new UTF8Encoding(), true);
                binaryReader.BaseStream.Position = 0;
                while (true)
                {
                    name = binaryReader.ReadString();
                    author = binaryReader.ReadString();
                    tags = binaryReader.ReadString();
                    tagArray = tags.Split(',');
                    if (tagArray.Any(item => item == tag))
                        books.Add(new Book(name, author, tagArray));
                }
            }
            catch (IOException ex)
            {
                Logger.Info(books.Count == 0 ? "has found book by tag" + tag : "not found book by tag" + tag);
                return books;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
            finally
            {
                binaryReader?.Dispose();
            }
        }

        public void Delete(Book book)
        {
            Book foundBook;
            BinaryReader binaryReader = null;
            string name, author, tags;
            string[] tagArray;
            long delPosition;
            try
            {
                binaryReader = new BinaryReader(fileStream,new UTF8Encoding(),true);
                binaryReader.BaseStream.Position = 0;
                while (true)
                {
                    delPosition = fileStream.Position;
                    name = binaryReader.ReadString();
                    author = binaryReader.ReadString();
                    tags = binaryReader.ReadString();
                    tagArray = tags.Split(',');
                    foundBook = new Book(name,author,tagArray);
                    if (book == foundBook)
                    {
                        RemoveBookFromFile(delPosition, fileStream.Position,
                            fileStream.Length - (fileStream.Position - delPosition));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
            finally
            {
                binaryReader?.Dispose();
            }
        }

        public void SortByTag(Comparison<Book> comparison)
        {
            Book leftBook, rightBook;
            BinaryReader binaryReader = null;
            int booksCount = FindNumberOfBooks();
            long startPosition;
            try
            {
                binaryReader = new BinaryReader(fileStream, new UTF8Encoding(), true);
                for (int i = booksCount - 1; i > 0; i--)
                {
                    binaryReader.BaseStream.Position = 0;
                    for (int j = 0; j < i; j++)
                    {
                        startPosition = binaryReader.BaseStream.Position;
                        leftBook = ReadOneBook(binaryReader);
                        var temp = binaryReader.BaseStream.Position;
                        rightBook = ReadOneBook(binaryReader);
                        if(comparison(leftBook,rightBook) > 0)
                            Swap(leftBook,rightBook,startPosition);
                        binaryReader.BaseStream.Position = temp;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
            finally
            {
                binaryReader?.Dispose();
            }
        }

        private void RemoveBookFromFile(long writerPosition, long readerPosition, long newLengthOfFile)
        {
            BinaryReader binaryReader = null;
            BinaryWriter binaryWriter = null;
            try
            {
                binaryWriter = new BinaryWriter(fileStream, new UTF8Encoding(), true);
                binaryReader = new BinaryReader(fileStream, new UTF8Encoding(), true);
                while (true)
                {
                    if (readerPosition == fileStream.Length)
                        break;
                    binaryReader.BaseStream.Seek(readerPosition++, SeekOrigin.Begin);
                    byte oneByte = binaryReader.ReadByte();
                    binaryWriter.BaseStream.Seek(writerPosition++, SeekOrigin.Begin);
                    binaryWriter.Write(oneByte);
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                throw ex;
            }
            finally
            {
                binaryWriter?.Dispose();
                binaryReader?.Dispose();
            }
            fileStream.SetLength(newLengthOfFile);
        }

        private int FindNumberOfBooks()
        {
            BinaryReader binaryReader = null;
            string name, author, tags;
            int counter = 0;
            try
            {
                binaryReader = new BinaryReader(fileStream, new UTF8Encoding(), true);
                while (true)
                {
                    name = binaryReader.ReadString();
                    author = binaryReader.ReadString();
                    tags = binaryReader.ReadString();
                    counter++;
                }
            }
            catch (IOException)
            {
                Logger.Info("find count of books in storage when sort, is {0}",counter);
                return counter;
            }
            finally
            {
                binaryReader?.Dispose();
            }
        }

        private Book ReadOneBook(BinaryReader binaryReader)
        {
            var name = binaryReader.ReadString();
            var author = binaryReader.ReadString();
            var tags = binaryReader.ReadString();
            var tagArray = tags.Split(',');
            return new Book(name,author,tagArray);
        }

        private void Swap(Book lhs, Book rhs,long position)
        {
            BinaryWriter binaryWriter = null;
            try
            {
                binaryWriter = new BinaryWriter(fileStream);
                binaryWriter.BaseStream.Position = position;
                WriteOneBook(binaryWriter,rhs);
                WriteOneBook(binaryWriter,lhs);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                binaryWriter?.Dispose();
            }
        }

        private void WriteOneBook(BinaryWriter binaryWriter,Book book)
        {
            binaryWriter.Write(book.Name);
            binaryWriter.Write(book.Author);
            string tags = book.GetTags().Aggregate("", (current, item) => current + (item + ',')).TrimEnd(new char[] { ',' });
            binaryWriter.Write(tags);
        }
    }
}
