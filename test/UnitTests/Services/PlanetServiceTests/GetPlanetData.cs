using System.Threading.Tasks;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using UnitTests.Mocks;
using Xunit;

namespace UnitTests.Services.PlanetServiceTests
{
	public class GetPlanetData
	{
		[Fact]
		public async Task CallsApiServiceFetchMethod()
		{
			var swapi = ServiceMockFactory.SWAPISevice(new Planet());
			var logger = LoggerMockFactory.GenericLogger<IPlanetService>();
			var service = new PlanetService(swapi.Object, logger.Object);

			var result = await service.GetPlanetData(string.Empty);

			Assert.NotNull(result);
			Assert.NotNull(result.Result);
			Assert.IsType<Planet>(result.Result);
			Assert.Equal(Status.Success, result.Status);
		}

		[Fact]
		public async Task ReturnsResultFromCacheIfPresent()
		{
			var swapi = ServiceMockFactory.SWAPISevice(new Planet());
			var logger = LoggerMockFactory.GenericLogger<IPlanetService>();
			var service = new PlanetService(swapi.Object, logger.Object);

			var result1 = await service.GetPlanetData(string.Empty);
			var result2 = await service.GetPlanetData(string.Empty);

			Assert.Equal(result1, result2);
		}
	}
}
