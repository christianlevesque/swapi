using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public class PlanetService : IPlanetService
	{
		private readonly ISWAPIService _swapi;
		private readonly ILogger<IPlanetService> _logger;
		private readonly IDictionary<string, ServiceResult<Planet>> _cache;

		/// <summary>
		/// Creates a new PlanetService, which is used to communicate with the Planets SWAPI endpoint
		/// </summary>
		/// <param name="swapi"></param>
		/// <param name="logger"></param>
		public PlanetService(ISWAPIService swapi, ILogger<IPlanetService> logger)
		{
			_swapi = swapi;
			_logger = logger;
			_cache = new Dictionary<string, ServiceResult<Planet>>();

			_logger.LogInformation("New PlanetService created");
		}

		/// <summary>
		/// Gets the relevant data for the planet indicated
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public async Task<ServiceResult<Planet>> GetPlanetData(string resource)
		{
			_logger.LogInformation("Getting planet data for planet {resource}", resource);
			if (_cache.ContainsKey(resource))
			{
				_logger.LogInformation("Returning cached Planet for {resource}", resource);
				return _cache[resource];
			}

			_logger.LogInformation("No cached Planet for {resource}. Populating cache.", resource);
			var newPlanet = await _swapi.Fetch<Planet>(resource);
			_cache.Add(resource, newPlanet);

			return newPlanet;
		}
	}
}
