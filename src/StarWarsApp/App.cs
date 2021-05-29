using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;

namespace StarWarsApp
{
	public class App : IApp
	{
		private readonly IFilmService _filmService;
		private readonly IPeopleService _peopleService;
		private readonly ICSVWriterService _csvWriterService;
		private readonly ICSVFormatterService<Person> _csvFormatterService;
		private readonly ILogger<App> _logger;
		private readonly HashSet<string> _personResources;
		private readonly SortedSet<Person> _persons;

		/// <summary>
		/// Creates a new instance of the application logic
		/// </summary>
		/// <param name="filmService"></param>
		/// <param name="peopleService"></param>
		/// <param name="csvWriterService"></param>
		/// <param name="csvFormatterService"></param>
		/// <param name="logger"></param>
		public App(
			IFilmService filmService,
			IPeopleService peopleService,
			ICSVWriterService csvWriterService,
			ICSVFormatterService<Person> csvFormatterService,
			ILogger<App> logger
		)
		{
			_filmService = filmService;
			_peopleService = peopleService;
			_csvWriterService = csvWriterService;
			_csvFormatterService = csvFormatterService;
			_logger = logger;
			_personResources = new HashSet<string>();
			_persons = new SortedSet<Person>(new PersonComparer());
		}

		/// <summary>
		/// Creates the character list in )current_dir_/output.csv
		/// </summary>
		/// <param name="films">The films for which to fetch characters (episodic)</param>
		/// <returns></returns>
		public async Task GenerateCharacterReport(IEnumerable<int> films)
		{
			_logger.LogInformation("Generating Star Wars character report");

			// Populate the data
			if (!await PopulateFilmData(films))
			{
				return;
			}

			// Format the CSV
			_logger.LogInformation("Character data loaded. Formatting data for output.");
			var output = _csvFormatterService.Format(_persons);

			// Write to the CSV
			_logger.LogInformation("Character data formatted. Writing to output file.");
			await _csvWriterService.Write(output);
		}

		/// <summary>
		/// Populates all the data on the requested films and characters
		/// </summary>
		/// <param name="films">The films for which to fetch data (episodic)</param>
		/// <returns></returns>
		private async Task<bool> PopulateFilmData(IEnumerable<int> films)
		{
			// First, pull in film data
			// Then, for each film, pull in character data if the character hasn't already been loaded
			foreach (var filmId in films)
			{
				var filmResult = await _filmService.GetFilmData(filmId);
				if (filmResult.Status != Status.Success)
				{
					_logger.LogError("Film data could not be loaded for film {filmId}", filmId);
					return false;
				}

				// I wanted to extract this nested loop, but the only way to do so
				// would be to add more complexity to the data that is stored in memory.
				// I viewed this as a valid tradeoff. 
				foreach (var characterResource in filmResult.Result.CharacterResources)
				{
					// If the person has already been added, we don't want their data again
					if (!_personResources.Add(characterResource))
					{
						continue;
					}

					// If data population fails, the entire process should be considered a failure
					if (!await PopulatePersonData(characterResource, filmId))
					{
						return false;
					}
				}
			}

			return true;
		}

		private async Task<bool> PopulatePersonData(string personResource, int filmId)
		{
			var personResult = await _peopleService.GetPersonData(personResource, filmId);
			if (personResult.Status != Status.Success)
			{
				_logger.LogError("Person data could not be loaded for person at {resource}", personResource);
				return false;
			}

			_persons.Add(personResult.Result);
			return true;
		}
	}
}
