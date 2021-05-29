using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public class FilmService : IFilmService
	{
		private readonly ISWAPIService _swapi;
		private readonly ILogger<IFilmService> _logger;
		private readonly IDictionary<int, ServiceResult<Film>> _cache;
		private const string FilmEndpoint = "https://swapi.dev/api/films";

		/// <summary>
		/// Creates a new FilmService, which is used to communicate with the Films SWAPI endpoint
		/// </summary>
		/// <param name="swapi"></param>
		/// <param name="logger"></param>
		public FilmService(ISWAPIService swapi, ILogger<IFilmService> logger)
		{
			_swapi = swapi;
			_logger = logger;
			_cache = new Dictionary<int, ServiceResult<Film>>();

			_logger.LogInformation("New FilmService created");
		}

		/// <summary>
		/// Retrieves the relevant data for the indicated film.
		///
		/// The ID represents the episodic ID of the film, not its ID in the API.
		/// </summary>
		/// <param name="id">The episodic ID of the film to retrieve</param>
		/// <returns></returns>
		public async Task<ServiceResult<Film>> GetFilmData(int id)
		{
			_logger.LogInformation("Getting film data for film {id}", id);
			if (_cache.ContainsKey(id))
			{
				_logger.LogInformation("Returning cached Film for film {id}", id);
				return _cache[id];
			}

			_logger.LogInformation("No cached Film for film {id}. Populating cache.", id);
			var newFilm = await _swapi.Fetch<Film>($"{FilmEndpoint}/{TranslateFilmNumber(id)}/");
			if (newFilm.Status == Status.Success)
			{
				_cache.Add(id, newFilm);
			}
			else
			{
				_logger.LogError("Failed processing film {id}", id);
			}

			return newFilm;
		}

		/// <summary>
		/// Translates between API film numbers and episodic film numbers. The function is purely reversible, so this function can be used to translate either to OR from episodic film numbers.
		/// </summary>
		/// <param name="n">The film number to translate</param>
		/// <returns>The translated film number</returns>
		public static int TranslateFilmNumber(int n) =>
			n switch
			{
				1 => 4,
				2 => 5,
				3 => 6,
				4 => 1,
				5 => 2,
				6 => 3,
				_ => throw new ArgumentOutOfRangeException(nameof(n), $"The SWAPI only supports films numbered 1-6, {n} given")
			};
	}
}
