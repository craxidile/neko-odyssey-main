using System.Collections.Generic;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
	public interface IRepositoryBase<T, TId> : IReadOnlyRepositoryBase<T, TId>, IRootRepository<T, TId> 
		where T : IAggregateRoot
		where TId : struct
	{
		T Add(T entity);
		IEnumerable<T> AddRange(IEnumerable<T> entities);
		T Update(T entity);
		void Remove(TId id);
		T Remove(T entity);
		IEnumerable<T> RemoveRange(IEnumerable<T> entities);
	}
}
