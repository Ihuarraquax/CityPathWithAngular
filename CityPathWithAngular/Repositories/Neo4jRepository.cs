using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityPathWithAngular.Extensions;
using CityPathWithAngular.Models;
using CityPathWithAngular.Models.RequestResponse;
using CityPathWithAngular.Services;
using Neo4j.Driver;

namespace CityPathWithAngular.Repositories
{
    public interface INeo4jRepository
    {
        public Task<List<Place>> Search(string search);
        // Task WipeDatabase();
        // Task AddIntersection(Intersection intersection);
        // Task AddPathBetweenIntersection(Intersection a, Intersection b);
        Task<List<Place>> GetAllPlaces();
        Task NewPlace(NewPlaceModel model);
    }

    public class Neo4jRepository : INeo4jRepository
    {
        private readonly IDriver _driver;

        public Neo4jRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task WipeDatabase()
        {
            var session = _driver.AsyncSession(WithDatabase);
            try
            {
                await session.WriteTransactionAsync(async transaction =>
                {
                    await transaction.RunAsync(@"
                        MATCH (n)
                        DETACH DELETE n"
                    );
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        // public async Task AddIntersection(Intersection intersection)
        // {
        //     var session = _driver.AsyncSession(WithDatabase);
        //     try
        //     {
        //         await session.WriteTransactionAsync(async transaction =>
        //         {
        //             await transaction.RunAsync(@"
        //                 CREATE (n:Intersection {lat: $lat, lon: $lon, street1: $street1, street2: $street2})",
        //                 new {lat = intersection.Lat, lon = intersection.Lon, street1 = intersection.Street1, street2 = intersection.Street2}
        //             );
        //         });
        //     }
        //     finally
        //     {
        //         await session.CloseAsync();
        //     }
        // }
        //
        // public async Task AddPathBetweenIntersection(Intersection a, Intersection b)
        // {
        //     var s1Coord = new GeoCoordinate(a.Lat, a.Lon);
        //     var s2Coord = new GeoCoordinate(b.Lat, b.Lon);
        //
        //     var distance = s1Coord.DistanceTo(s2Coord);
        //
        //
        //     var session = _driver.AsyncSession(WithDatabase);
        //     try
        //     {
        //         await session.WriteTransactionAsync(async transaction =>
        //         {
        //             await transaction.RunAsync(@"
        //                 MATCH
        //                   (a:Intersection),
        //                   (b:Intersection)
        //                 WHERE a.street1 = $as1 AND a.street2 = $as2 AND b.street1 = $bs1 AND b.street2 = $bs2
        //                 CREATE (a)-[r:Path {distance: $dist}]->(b)",
        //                 new {as1 = a.Street1, as2 = a.Street2, bs1 = b.Street1, bs2 = b.Street2, dist = distance}
        //             );
        //         });
        //     }
        //     finally
        //     {
        //         await session.CloseAsync();
        //     }
        // }

        public async Task<List<Place>> GetAllPlaces()
        {
            var session = _driver.AsyncSession(WithDatabase);
            try
            {
                return await session.ReadTransactionAsync(async transaction =>
                {
                    var cursor = await transaction.RunAsync(@"
                        MATCH (place:Place)
                        RETURN place.name AS name,
                               id(place) AS id"
                    );

                    return await cursor.ToListAsync(record => new Place
                    {
                        Id = record["id"].As<long>(),
                        Name = record["name"].As<string>()
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task NewPlace(NewPlaceModel model)
        {
            var session = _driver.AsyncSession(WithDatabase);
            try
            {
                await session.WriteTransactionAsync(async transaction =>
                {
                    await transaction.RunAsync(@"
                        CREATE (place:Place {name: $name})",
                        new {name = model.Name}
                    );
                    foreach (var modelSasiad in model.Sasiads)
                    {
                        await transaction.RunAsync(@"
                        MATCH
                          (a:Place),
                          (b:Place)
                        WHERE a.name = $name AND b.name = $name2
                        CREATE (a)-[r:Path {distance: $dist}]->(b)",
                            new {name = model.Name,name2 = modelSasiad.Name, dist = modelSasiad.Distance}
                        );
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Place>> Search(string search)
        {
            var session = _driver.AsyncSession(WithDatabase);
            try
            {
                return await session.ReadTransactionAsync(async transaction =>
                {
                    var cursor = await transaction.RunAsync(@"
                        MATCH (place:Place)
                        WHERE TOLOWER(place.name) CONTAINS TOLOWER($name)
                        RETURN place.name AS name,
                               id(place) AS id,
                               place.latitude AS latitude,
                               place.longitude AS longitude",
                        new {name = search}
                    );

                    return await cursor.ToListAsync(record => new Place
                    {
                        Id = record["id"].As<long>(),
                        Name = record["name"].As<string>(),
                        Coordinate = new GeoCoordinate(record["latitude"].As<float>(), record["longitude"].As<float>())
                    });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private static void WithDatabase(SessionConfigBuilder sessionConfigBuilder)
        {
            var neo4jVersion = System.Environment.GetEnvironmentVariable("NEO4J_VERSION") ?? "";
            if (!neo4jVersion.StartsWith("4"))
            {
                return;
            }

            sessionConfigBuilder.WithDatabase(Database());
        }

        private static string Database()
        {
            return System.Environment.GetEnvironmentVariable("NEO4J_DATABASE") ?? "movies";
        }
    }
}