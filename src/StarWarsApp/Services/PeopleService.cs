using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public class PeopleService : IPeopleService
	{
		private readonly IDictionary<string, ServiceResult<Person>> _cache;
		private readonly IFilmService _filmService;
		private readonly ILogger<IPeopleService> _logger;
		private readonly IPlanetService _planetService;
		private readonly ISWAPIService _swapi;

		/// <summary>
		/// Creates a new PeopleService, which is used to communicate with the People SWAPI endpoint
		/// </summary>
		/// <param name="swapi"></param>
		/// <param name="planetService"></param>
		/// <param name="filmService"></param>
		/// <param name="logger"></param>
		public PeopleService(ISWAPIService swapi, IPlanetService planetService, IFilmService filmService, ILogger<IPeopleService> logger)
		{
			_swapi = swapi;
			_planetService = planetService;
			_filmService = filmService;
			_logger = logger;
			_cache = new Dictionary<string, ServiceResult<Person>>();

			_logger.LogInformation("New PeopleService created");
		}

		/// <summary>
		/// Gets the relevant data for the character indicated
		/// </summary>
		/// <param name="resource">The SWAPI endpoint for the character to retrieve</param>
		/// <param name="filmId">The episodic ID of the film this character first appears in</param>
		/// <returns></returns>
		public async Task<ServiceResult<Person>> GetPersonData(string resource, int filmId)
		{
			_logger.LogInformation("Getting person data for person {resource}", resource);
			if (_cache.ContainsKey(resource))
			{
				_logger.LogInformation("Returning cached Person for {resource}", resource);
				return _cache[resource];
			}

			_logger.LogInformation("No cached Person for person {resource}. Populating cache.", resource);
			var result = await _swapi.Fetch<Person>(resource);
			var person = result.Result;
			_logger.LogInformation($"Loaded person {person.Name} of {person.Homeworld}, born {person.BirthYear}");

			// Populate planet
			var planet = await _planetService.GetPlanetData(person.Homeworld);
			if (planet.Status != Status.Success)
			{
				_logger.LogError("SWAPI failed to fetch planet from {Resource}", person.Homeworld);
				result.Status = Status.Error;
				return result;
			}

			person.Homeworld = planet.Result.Name;

			// Parse name into surname-given
			var names = person.Name?.Split(' ') ?? Array.Empty<string>();
			if (names.Length == 0)
			{
				_logger.LogError("SWAPI returned a character without a name from {Resource}", resource);
				result.Status = Status.Error;
				return result;
			}

			person.FirstName = GetFirstName(names);
			person.LastName = GetLastName(names);

			// Determine film name of earliest appearance
			person.FirstAppearedInId = filmId;
			var film = await _filmService.GetFilmData(filmId);
			person.FirstAppearedInName = film.Result.Title;

			// Parse age
			try
			{
				person.Age = GetAge(person.BirthYear);
			}
			catch (FormatException)
			{
				_logger.LogError("SWAPI returned a character with an unexpected date format from {Resource}", resource);
				result.Status = Status.Error;
				return result;
			}

			_cache.Add(resource, result);
			return result;
		}

		private string GetFirstName(string[] names)
		{
			// If only one name is present, it is a surname
			return names.Length > 1 ? names[0] : string.Empty;
		}

		private string GetLastName(string[] names)
		{
			return names[^1];
		}

		private float GetAge(string birthYear)
		{
			if (birthYear == "unknown")
			{
				return 0;
			}

			// All ages will be compared to an arbitrary date in the past
			// It's so high to guarantee that no character will be older
			// This creates an arbitrarily high age for each character
			// so BBY/ABY can be compared uniformly
			const float baseAge = 4000;

			var yearType = birthYear.Substring(birthYear.Length - 3);
			var yearNumber = birthYear.Replace(yearType, "");
			var year = float.Parse(yearNumber);

			return yearType == "BBY"
				? baseAge + year
				: baseAge - year;
		}
	}
}
