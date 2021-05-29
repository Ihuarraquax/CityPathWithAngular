using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityPathWithAngular.Models;
using CityPathWithAngular.Models.RequestResponse;
using CityPathWithAngular.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CityPathWithAngular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly INeo4jRepository _neo4JRepository;

        public PlacesController(INeo4jRepository neo4JRepository)
        {
            _neo4JRepository = neo4JRepository;
        }

        [Route("search")]
        [HttpGet]
        public async Task<List<Place>> SearchPlaces([FromQuery(Name = "q")] string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _neo4JRepository.GetAllPlaces();
            }

            return await _neo4JRepository.Search(search);
        }

        [HttpGet]
        public async Task<List<Place>> GetAllPlaces()
        {
            return await _neo4JRepository.GetAllPlaces();
        }

        [HttpGet("{id}")]
        public async Task<PlaceDetails> GetPlace(long id)
        {
            return await _neo4JRepository.GetPlace(id);
        }

        [HttpPost]
        public async Task NewPlace(NewPlaceModel model)
        {
            await _neo4JRepository.NewPlace(model);
        }

        [HttpPost("FindTrack")]
        public async Task<TrackFinderResponse> FindTrack(TrackFinderRequest model)
        {
            return await _neo4JRepository.FindTrack(model);
        }

        [HttpDelete("{id}")]
        public async Task DeleteTrack(int id)
        {
            await _neo4JRepository.DeletePlace(id);
        }
        
        [HttpDelete("paths/{id}")]
        public async Task DeletePath(int id)
        {
            await _neo4JRepository.DeletePath(id);
        }
    }
}