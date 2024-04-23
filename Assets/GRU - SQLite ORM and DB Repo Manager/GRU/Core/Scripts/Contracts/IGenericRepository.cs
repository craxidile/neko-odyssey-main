using System.Collections.Generic;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
