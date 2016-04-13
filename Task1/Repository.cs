using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public interface IRepository<T> where T : class
    {
        void Load();
        void Save();
        void Add(T entity);
        void Delete(T entity);
        List<T> FindByTag(string tag);
        void SortByTag(Comparison<T> comparison);
    }
}
