using System.Collections.Generic;
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

        // tag::searchByName[]
        public async Task<List<University>> SearchByName(string name, string country = null)
        // end::searchByName[]
        {
            List<University> universities = null;

            var database = await GetDatabaseAsync();

            if (database != null)
            {
                // tag::buildquery[]
                var whereQueryExpression = Function.Lower(Expression.Property("name")).Like(Expression.String($"%{name.ToLower()}%")); // <1>

                if (!string.IsNullOrEmpty(country))
                {
                    var countryQueryExpression = Function.Lower(Expression.Property("country")).Like(Expression.String($"%{country.ToLower()}%")); // <2>

                    whereQueryExpression = whereQueryExpression.And(countryQueryExpression); // <3>
                }

                var query = QueryBuilder.Select(SelectResult.All()) // <4>
                                        .From(DataSource.Database(database)) // <5>
                                        .Where(whereQueryExpression); // <6>

                // end::buildquery[]

                // tag::runquery[]
                var results = query.Execute().AllResults();

                if (results?.Count > 0)
                {
                    universities = new List<University>(); // <1>

                    foreach (var result in results)
                    {
                        var dictionary = result.GetDictionary("universities");

                        if (dictionary != null)
                        {
                            var university = new University
                            {
                                Name = dictionary.GetString("name"), // <2>
                                Country = dictionary.GetString("country") // <2>
                            };

                            universities.Add(university);
                        }
                    }
                }
                // end::runquery[]
            }

            return universities;
        }
    }
}
