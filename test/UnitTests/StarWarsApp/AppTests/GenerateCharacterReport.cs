using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using StarWarsApp;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using UnitTests.Mocks;
using Xunit;

namespace UnitTests.StarWarsApp.AppTests
{
	public class GenerateCharacterReport
	{
		private readonly Mock<IFilmService> _filmService;
		private readonly Mock<IPeopleService> _peopleService;
		private readonly Mock<ICSVWriterService> _csvWriterService;
		private readonly Mock<ICSVFormatterService<Person>> _csvFormatterService;
		private readonly Mock<ILogger<App>> _logger;
		private readonly App _app;

		private readonly Person _defaultPerson;
		private readonly ServiceResult<Person> _defaultResult;
		private readonly IEnumerable<int> _films;

		public GenerateCharacterReport()
		{
			_defaultPerson = new Person
			{
				BirthYear = "19BBY",
				Name = "Luke Skywalker",
				Homeworld = "https://swapi.dev/api/planets/1"
			};
			_defaultResult = new ServiceResult<Person>
			{
				Result = _defaultPerson
			};
			_films = new[]
			{
				2
			};

			_filmService = ServiceMockFactory.FilmService();

			_peopleService = new Mock<IPeopleService>();
			_peopleService.Setup(s => s.GetPersonData(It.IsAny<string>(), It.IsAny<int>()))
			              .ReturnsAsync(_defaultResult);

			_csvWriterService = new Mock<ICSVWriterService>();
			_csvFormatterService = new Mock<ICSVFormatterService<Person>>();
			_logger = LoggerMockFactory.GenericLogger<App>();

			_app = new App(
				_filmService.Object,
				_peopleService.Object,
				_csvWriterService.Object,
				_csvFormatterService.Object,
				_logger.Object
			);
		}

		[Fact]
		public async Task CallsFilmAndPeopleServices()
		{
			await _app.GenerateCharacterReport(_films);

			_filmService.Verify(f => f.GetFilmData(2), Times.Once);
			_peopleService.Verify(p => p.GetPersonData("https://swapi.dev/api/people/1", 2));
		}

		[Fact]
		public async Task ReturnsEarlyIfFilmServiceFails()
		{
			_filmService.Setup(f => f.GetFilmData(2))
			            .ReturnsAsync(
				             new ServiceResult<Film>
				             {
					             Status = Status.Error
				             }
			             );

			await _app.GenerateCharacterReport(_films);

			_peopleService.Verify(p => p.GetPersonData(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
		}

		[Fact]
		public async Task ReturnsEarlyIfPeopleServiceFails()
		{
			_peopleService.Setup(p => p.GetPersonData(It.IsAny<string>(), It.IsAny<int>()))
			              .ReturnsAsync(
				               new ServiceResult<Person>
				               {
					               Status = Status.Error
				               }
			               );

			await _app.GenerateCharacterReport(_films);

			_csvFormatterService.Verify(c => c.Format(It.IsAny<IEnumerable<Person>>()), Times.Never);
		}

		[Fact]
		public async Task OnlyPopulatesPersonOnce()
		{
			_filmService.Setup(f => f.GetFilmData(2))
			            .ReturnsAsync(
				             new ServiceResult<Film>
				             {
					             Result = new Film
					             {
						             CharacterResources = new[]
						             {
							             "https://swapi.dev/api/people/1",
							             "https://swapi.dev/api/people/1"
						             }
					             }
				             }
			             );

			await _app.GenerateCharacterReport(_films);

			_peopleService.Verify(p => p.GetPersonData("https://swapi.dev/api/people/1", 2), Times.Once);
		}

		[Fact]
		public async Task CallsFormatterFormatMethod()
		{
			await _app.GenerateCharacterReport(_films);
			_csvFormatterService.Verify(c => c.Format(It.IsAny<IEnumerable<Person>>()), Times.Once);
		}

		[Fact]
		public async Task CallsWriterWriteMethod()
		{
			await _app.GenerateCharacterReport(_films);
			_csvWriterService.Verify(w => w.Write(It.IsAny<string>()), Times.Once);
		}
	}
}
