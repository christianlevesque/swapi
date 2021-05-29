using System;
using System.Threading.Tasks;
using Moq;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using UnitTests.Mocks;
using Xunit;

namespace UnitTests.Services.FilmServiceTests
{
	public class GetFilmData
	{
		private const string FilmEndpoint = "https://swapi.dev/api/films";
		
		[Fact]
		public async Task CallsApiServiceFetchMethod()
		{
			var swapi = ServiceMockFactory.SWAPISevice(new Film());
			var logger = LoggerMockFactory.GenericLogger<IFilmService>();
			var service = new FilmService(swapi.Object, logger.Object);

			var result = await service.GetFilmData(1);

			Assert.NotNull(result);
			Assert.NotNull(result.Result);
			Assert.IsType<Film>(result.Result);
			Assert.Equal(Status.Success, result.Status);
		}

		[Fact]
		public async Task ReturnsResultFromCacheIfPresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(new Film());
			var logger = LoggerMockFactory.GenericLogger<IFilmService>();
			var service = new FilmService(swapi.Object, logger.Object);

			var result1 = await service.GetFilmData(1);
			var result2 = await service.GetFilmData(1);

			Assert.Equal(result1, result2);
		}

		[Fact]
		public async Task ReturnsErrorStatusIfApiCallFails()
		{
			var swapi = ServiceMockFactory.SWAPISevice(new Film());
			swapi.Setup(s => s.Fetch<Film>(It.IsAny<string>()))
			     .ReturnsAsync(
				      new ServiceResult<Film>
				      {
					      Status = Status.Error
				      }
			      );
			var logger = LoggerMockFactory.GenericLogger<IFilmService>();
			var service = new FilmService(swapi.Object, logger.Object);

			var result = await service.GetFilmData(1);

			Assert.Equal(Status.Error, result.Status);
		}
	}
}
