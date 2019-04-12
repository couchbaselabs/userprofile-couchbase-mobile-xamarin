using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Lite.Query;
using UserProfileDemo.Core.Respositories;
using UserProfileDemo.Models;

namespace UserProfileDemo.Repositories
{
    public sealed class UniversityRepository : BaseRepository, IUniversityRepository
    {
        public UniversityRepository() : base("universities")
        { }

        public async Task<List<University>> SearchByName(string name, string country = null)
        {
            List<University> universities = null;

            var database = await GetDatabaseAsync();

            if (database != null)
            {
                var whereQueryExpression = Function.Lower(Expression.Property("name")).Like(Expression.String($"%{name.ToLower()}%"));

                if (!string.IsNullOrEmpty(country))
                {
                    var countryQueryExpression = Function.Lower(Expression.Property("country")).Like(Expression.String($"%{country.ToLower()}%"));

                    whereQueryExpression = whereQueryExpression.And(countryQueryExpression);
                }

                var results = QueryBuilder.Select(SelectResult.All())
                                                  .From(DataSource.Database(database))
                                                  .Where(whereQueryExpression)
                                                  .Execute()
                                                  .AllResults();

                if (results?.Count > 0)
                {
                    universities = new List<University>();

                    foreach (var result in results)
                    {
                        var dictionary = result.GetDictionary("universities");

                        if (dictionary != null)
                        {
                            var university = new University
                            {
                                Name = dictionary.GetString("name"),
                                Country = dictionary.GetString("country")
                            };

                            universities.Add(university);
                        }
                    }
                }
            }

            return universities;
        }
    }
}
