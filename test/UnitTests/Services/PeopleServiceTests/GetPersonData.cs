using System.Threading.Tasks;
using Moq;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using UnitTests.Mocks;
using Xunit;

namespace UnitTests.Services.PeopleServiceTests
{
	public class GetPersonData
	{
		private readonly Person _defaultPerson;
		private readonly Mock<IPlanetService> _planetService;

		public GetPersonData()
		{
			_defaultPerson = new Person
			{
				Name = "Luke Skywalker",
				BirthYear = "19 BBY",
				Homeworld = "https://swapi.dev/api/planets/1/"
			};
			_planetService = new Mock<IPlanetService>();
			_planetService.Setup(s => s.GetPlanetData(It.IsAny<string>()))
			              .ReturnsAsync(new ServiceResult<Planet> {Result = new Planet {Name = "Tatooine"}});
		}

		[Fact]
		public async Task CallsApiServiceFetchMethod()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.NotNull(result);
			Assert.NotNull(result.Result);
			Assert.IsType<Person>(result.Result);
			Assert.Equal(Status.Success, result.Status);
		}

		[Fact]
		public async Task SetsFirstNameIfPresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal("Luke", result.Result.FirstName);
		}

		[Fact]
		public async Task SetsLastNameIfPresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal("Skywalker", result.Result.LastName);
		}

		[Fact]
		public async Task SetsLastNameIfOnlyOneNamePresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(
				new Person
				{
					Name = "Yoda",
					BirthYear = "900 BBY"
				}
			);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal(string.Empty, result.Result.FirstName);
			Assert.Equal("Yoda", result.Result.LastName);
		}

		[Fact]
		public async Task ReturnsErrorResultIfNoNamePresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(new Person());
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal(Status.Error, result.Status);
		}

		[Fact]
		public async Task SetsAgeIfBirthYearValid()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal(3981, result.Result.Age);
		}

		[Fact]
		public async Task ReturnsErrorResultIfBirthYearInvalid()
		{
			var swapi = ServiceMockFactory.SWAPISevice(
				new Person
				{
					Name = "Luke Skywalker",
					BirthYear = "BBY"
				}
			);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal(Status.Error, result.Status);
		}

		[Fact]
		public async Task ReturnsResultFromCacheIfPresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result1 = await service.GetPersonData(string.Empty, 1);
			var result2 = await service.GetPersonData(string.Empty, 1);

			Assert.Equal(result1, result2);
		}

		[Fact]
		public async Task ReturnsErrorResultIfGetPlanetDataFails()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();

			_planetService.Setup(s => s.GetPlanetData(It.IsAny<string>()))
			              .ReturnsAsync(
				               new ServiceResult<Planet>
				               {
					               Status = Status.Error
				               }
			               );

			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal(Status.Error, result.Status);
		}

		[Fact]
		public async Task SetsMovieTitle()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);

			Assert.Equal("A New Hope", result.Result.FirstAppearedInName);
		}

		[Fact]
		public async Task ReturnsZeroAgeIfBirthYearUnknown()
		{
			_defaultPerson.BirthYear = "unknown";
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);
			Assert.Equal(0f, result.Result.Age);
		}

		[Fact]
		public async Task ReturnsBaseAgeMinusYearIfBBY()
		{
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);
			Assert.Equal(3981f, result.Result.Age);
		}

		[Fact]
		public async Task ReturnsBaseAgePlusYearIfABY()
		{
			_defaultPerson.BirthYear = "19ABY";
			var swapi = ServiceMockFactory.SWAPISevice(_defaultPerson);
			var films = ServiceMockFactory.FilmService();
			var logger = LoggerMockFactory.GenericLogger<IPeopleService>();
			var service = new PeopleService(
				swapi.Object,
				_planetService.Object,
				films.Object,
				logger.Object
			);

			var result = await service.GetPersonData(string.Empty, 1);
			Assert.Equal(4019f, result.Result.Age);
		}
	}
}
