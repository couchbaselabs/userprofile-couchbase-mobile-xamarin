using System;

namespace UserProfileDemo.Core.Respositories
{
    public interface IRepository<T, K> : IDisposable
    {
        T Get(K id);
        bool Save(T obj);
    }
}
