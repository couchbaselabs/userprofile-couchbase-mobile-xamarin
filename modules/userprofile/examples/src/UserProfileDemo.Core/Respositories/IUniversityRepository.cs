using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserProfileDemo.Models;

namespace UserProfileDemo.Core.Respositories
{
    public interface IUniversityRepository : IDisposable
    {
        Task<List<University>> SearchByName(string name, string country = null);
    }
}
