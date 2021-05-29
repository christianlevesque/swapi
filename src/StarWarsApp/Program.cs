using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;

namespace StarWarsApp
{
	public class Program
	{
		private static LogLevel _logLevel = LogLevel.Debug;
		private static ICSVFormatterService<Person> _csvFormatterService;
		private static ICSVWriterService _csvWriterService;
		private static IFilmService _filmService;
		private static IPeopleService _peopleService;
		private static IPlanetService _planetService;
		private static ISWAPIService _swapiService;
		private static HttpClient _client;
		private static ILoggerFactory _loggerFactory;
		private static IApp _app;

		public static async Task Main()
		{
			Init();
			InitApp();

			await _app.GenerateCharacterReport(
				new[]
				{
					2,
					4,
					6
				}
			);
			await _csvWriterService.DisposeAsync();
		}

		private static void Init()
		{
			_loggerFactory = LoggerFactory.Create(
				builder =>
				{
					builder.AddFilter("Microsoft", _logLevel)
					       .AddFilter("System", _logLevel)
					       .AddConsole()
					       .AddEventLog();
				}
			);

			_client = new HttpClient();
			_swapiService = new SWAPIService(_client, _loggerFactory.CreateLogger<ISWAPIService>());
			_filmService = new FilmService(_swapiService, _loggerFactory.CreateLogger<IFilmService>());
			_planetService = new PlanetService(_swapiService, _loggerFactory.CreateLogger<IPlanetService>());
			_peopleService = new PeopleService(
				_swapiService,
				_planetService,
				_filmService,
				_loggerFactory.CreateLogger<IPeopleService>()
			);
			_csvWriterService = new CSVWriterService();
			_csvFormatterService = new PersonCSVFormatterService();
		}

		public static void InitApp(IApp app = null)
		{
			// Only sets _app if it's null
			// if app is null, create new App
			// This allows manually calling InitApp OR not
			// and the desired IApp will always be used
			_app ??= app ??
			       new App(
				       _filmService,
				       _peopleService,
				       _csvWriterService,
				       _csvFormatterService,
				       _loggerFactory.CreateLogger<App>()
			       );
		}
	}
}
