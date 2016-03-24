using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Task1
{
    public class Book
    {
        public string Name { get; private set; }
        public string Author { get; private set; }
        private List<string> tags;
        public Book(string name, string author,params string[] tag)
        {
            Name = name;
            Author = author;;
            tags = new List<string>();
            tags.AddRange(tag);
        }

        public List<string> GetTags()
        {
            List<string> tagsList = new List<string>();
            tagsList.AddRange(tags);
            return tagsList;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Book book = obj as Book;
            if (book == null) return false;
            if (this.Name == book.Name && this.Author == book.Author && this.tags.Count == book.GetTags().Count)
            {
                List<string> tagsList = book.GetTags();
                for(int i=0; i < tags.Count ; i++)
                    if (tags[i] != tagsList[i])
                        return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Book lhs, Book rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (((object)lhs == null) || ((object)rhs == null))
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Book lhs, Book rhs)
        {
            return !(lhs == rhs);
        }
    }
}
