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
    public class BookStorage
    {
        private readonly Stream fileStream;

        public BookStorage(Stream fileStream)
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
                throw ex;
            }
        }

        public void AddBook(Book book)
        {
            BinaryWriter binaryWriter = null;
            try
            {
                binaryWriter = new BinaryWriter(fileStream, new UTF8Encoding(), true);
                binaryWriter.Write(book.Name);
                binaryWriter.Write(book.Author);
                string tags = book.GetTags().Aggregate("", (current, item) => current + (item + ',')).TrimEnd(new char[] { ',' });
                binaryWriter.Write(tags);
            }
            catch (IOException ex)
            {
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
                return books;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                binaryReader?.Dispose();
                fileStream.Position = 0;
            }
        }

        public void RemoveBook(Book book)
        {
            Book foundBook;
            BinaryReader binaryReader = null;
            string name, author, tags;
            string[] tagArray;
            long delPosition,newLength;
            try
            {
                binaryReader = new BinaryReader(fileStream);
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
            catch (Exception)
            {

                throw;
            }
            finally
            {
                binaryReader?.Dispose();
            }
        }

        private void RemoveBookFromFile(long startPosition, long endPosition, long newLengthOfFile)
        {
            BinaryReader binaryReader = null;
            BinaryWriter binaryWriter = null;
            try
            {
                binaryWriter = new BinaryWriter(fileStream);
                binaryWriter.Seek((int)startPosition,SeekOrigin.Begin);
                binaryReader = new BinaryReader(fileStream);
                binaryReader.
            }
            catch (Exception)
            {
                
                throw;
            }
            fileStream.SetLength(newLengthOfFile);
        }
    }
}
