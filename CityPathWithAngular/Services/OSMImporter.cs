using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CityPathWithAngular.Models;
using CityPathWithAngular.Repositories;
using Microsoft.AspNetCore.Components.Web;
using MoreLinq;
using OsmSharp;
using OsmSharp.Streams;

namespace CityPathWithAngular.Services
{
    public class OSMImporter
    {
        private readonly INeo4jRepository _neo4JRepository;

        public OSMImporter(INeo4jRepository neo4JRepository)
        {
            _neo4JRepository = neo4JRepository;
        }

        // public async void AddToDatabase()
        // {
        //     var jsonString = await File.ReadAllTextAsync(@"Data\intersections.json");
        //     var intersections = JsonSerializer.Deserialize<List<Intersection>>(jsonString);
        //
        //     await _neo4JRepository.WipeDatabase();
        //     for (int i = 0; i < intersections.Count; i++)
        //     {
        //         await _neo4JRepository.AddIntersection(intersections[i]);
        //     }
        //
        //     // var intersectionPair = intersections.Where(_ => _ != null)
        //     //     .SelectMany(i1 => intersections.Where(_ => _ != null)
        //     //         .Select(i2 => new IntersectionPair{i1 = i1, i2 = i2})
        //     //     ).Distinct().ToList();
        //
        //     var intersectionPair = intersections
        //         .SelectMany(i1 => intersections.Where(i2 =>
        //                 (i1.Street1 == i2.Street1 || i1.Street1 == i2.Street2) || (i1.Street2 == i2.Street1 || i1.Street2 == i2.Street2))
        //             .Select(i2 => new IntersectionPair {i1 = i1, i2 = i2})
        //         ).Distinct().Where(_ => _.i1.Street1 != _.i2.Street1 && _.i1.Street2 != _.i2.Street2).ToList();
        //
        //     for (int i = 0; i < intersectionPair.Count; i++)
        //     {
        //         await _neo4JRepository.AddPathBetweenIntersection(intersectionPair[i].i1, intersectionPair[i].i2);
        //     }
        //
        //     var jsonText = await File.ReadAllTextAsync(@"Data\places.json");
        //     var places = JsonSerializer.Deserialize<List<Place>>(jsonText);
        //
        //     for (int i = 0; i < places.Count; i++)
        //     {
        //         var closest = intersections
        //             .GroupBy(x => Math.Pow((places[i].Coordinate.Latitude - x.Lat), 2) + Math.Pow((places[i].Coordinate.Longitude - x.Lon), 2))
        //             .OrderBy(x => x.Key)
        //             .First();
        //         await _neo4JRepository.AddPlace(places[i], closest.FirstOrDefault(), closest.Key);
        //     }
        // }

        public void GenerateIntersections()
        {
            using (var fileStream = File.OpenRead(@"Data\siedlce.osm.pbf"))
            {
                var fileStreamSource = new PBFOsmStreamSource(fileStream);
                var streets = fileStreamSource.Where(_ => _.Tags != null)
                    .Where(_ => _.Type == OsmGeoType.Way)
                    .Where(_ => _.Tags.ContainsKey("highway"))
                    // .Where(_ => _.Tags.ContainsKey("name"))
                    // .Where(_ => !_.Tags["name"].Contains('(') && !_.Tags["name"].Contains(')'))
                    .Select(_ => (Way) _)
                    .ToList();
                var nodes = fileStreamSource.Where(_ => _.Type == OsmGeoType.Node).ToList();

                var intersectionsFound = new HashSet<FoundIntersectionModel>();

                for (int i = 0; i < streets.Count; i++)
                {
                    for (int j = 0; j < streets.Count; j++)
                    {
                        if (i == j) continue;
                        var common = streets[i].Nodes.Intersect(streets[j].Nodes);
                        if (common.Any())
                        {
                            common.ForEach(_ =>
                            {
                                intersectionsFound.Add(new FoundIntersectionModel
                                {
                                    NodeId = _,
                                    Street1Id = streets[i].Id,
                                    Street2Id = streets[j].Id,
                                });
                            });
                        }
                    }
                }

                var intersectionList = new HashSet<Intersection>();
                intersectionsFound.ForEach(intersection =>
                {
                    var node = (Node) nodes.Where(_ => _.Id == intersection.NodeId).FirstOrDefault();

                    intersectionList.Add(new Intersection
                    {
                        Lat = node.Latitude.Value,
                        Lon = node.Longitude.Value,
                        Street1 = streets.Where(_ => _.Id == intersection.Street1Id).Select(_ => $"{_.Id.Value}").FirstOrDefault(),
                        Street2 = streets.Where(_ => _.Id == intersection.Street2Id).Select(_ => $"{_.Id.Value}").FirstOrDefault(),
                    });
                });

                string json = JsonSerializer.Serialize(intersectionList);
                File.WriteAllText(@"Data\intersections.json", json);
            }
        }

        public void GeneratePlaces()
        {
            using (var fileStream = File.OpenRead(@"Data\siedlce.osm.pbf"))
            {
                var fileStreamSource = new PBFOsmStreamSource(fileStream);

                var nodes = fileStreamSource.Where(_ => _.Type == OsmGeoType.Node && _.Tags != null && _.Tags.ContainsKey("name"))
                    .Select(_ => (Node) _).ToList();
                var places = nodes.Select(_ => new Place(_.Id.Value, _.Tags["name"], _.Latitude.Value, _.Longitude.Value)).ToList();

                string json = JsonSerializer.Serialize(places);
                File.WriteAllText(@"Data\places.json", json);
            }
        }
    }

    public class IntersectionPair
    {
        public Intersection i1 { get; set; }
        public Intersection i2 { get; set; }

        protected bool Equals(IntersectionPair other)
        {
            return (Equals(i1, other.i1) && Equals(i2, other.i2)) || (Equals(i1, other.i2) && Equals(i2, other.i1));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntersectionPair) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(i1, i2);
        }
    }

    public class FoundIntersectionModel
    {
        public long? NodeId { get; set; }
        public long? Street1Id { get; set; }
        public long? Street2Id { get; set; }

        protected bool Equals(FoundIntersectionModel other)
        {
            return (Street1Id == other.Street1Id && Street2Id == other.Street2Id) || (Street1Id == other.Street2Id && Street2Id == other.Street1Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FoundIntersectionModel) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street1Id, Street2Id);
        }
    }

    public class Intersection
    {
        public float Lat { get; set; }
        public float Lon { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }

        protected bool Equals(Intersection other)
        {
            return (Street1 == other.Street1 && Street2 == other.Street2) || (Street1 == other.Street2 && Street2 == other.Street1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Intersection) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street1, Street2);
        }
    }
}