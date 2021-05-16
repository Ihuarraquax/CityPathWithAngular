using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CityPathWithAngular.Models;
using CityPathWithAngular.Models.RequestResponse;
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
        Task<TrackFinderResponse> FindTrack(TrackFinderRequest model);
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
                            new {name = model.Name, name2 = modelSasiad.Name, dist = modelSasiad.Distance}
                        );
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<TrackFinderResponse> FindTrack(TrackFinderRequest model)
        {
            var session = _driver.AsyncSession(WithDatabase);
            try
            {
                return await session.ReadTransactionAsync(async transaction =>
                {
                    var cursor = await transaction.RunAsync(@"
                        MATCH (source:Place {name: '$name1'}), (target:Place {name: '$name2'})
                        CALL gds.beta.shortestPath.dijkstra.stream({
                            nodeProjection: 'Place',
                            relationshipProjection: 'Path',
                            relationshipProperties: 'distance',
                            sourceNode: id(source),
                            targetNode: id(target),
                            relationshipWeightProperty: 'distance'
                        })
                        YIELD index, sourceNode, targetNode, totalCost, nodeIds, costs
                        RETURN
                            index,
                            gds.util.asNode(sourceNode).name AS sourceNodeName,
                            gds.util.asNode(targetNode).name AS targetNodeName,
                            totalCost,
                            [nodeId IN nodeIds | gds.util.asNode(nodeId).name] AS nodeNames,
                            costs
                        ORDER BY index", new {name1 = model.From, name2 = model.To}
                    );
                    var list1 = await cursor.ToListAsync();
                    var list = await cursor.ToListAsync(record => new TrackFinderResponse
                        {
                            TotalCost = record["totalCost"].As<double>(),
                            NodeNames = record["nodeNames"].As<string[]>(),
                            Costs = record["costs"].As<double[]>(),
                        }
                    );

                    return list[0];
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
            var neo4jVersion = Environment.GetEnvironmentVariable("NEO4J_VERSION") ?? "";
            if (!neo4jVersion.StartsWith("4"))
            {
                return;
            }

            sessionConfigBuilder.WithDatabase(Database());
        }

        private static string Database()
        {
            return Environment.GetEnvironmentVariable("NEO4J_DATABASE") ?? "movies";
        }
    }
}