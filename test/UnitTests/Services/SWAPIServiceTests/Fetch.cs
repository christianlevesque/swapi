using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using UnitTests.Mocks;
using Xunit;

namespace UnitTests.Services.SWAPIServiceTests
{
	public class Fetch
	{
		private const string Url = "https://swapi.dev/api/films/1";
		private readonly Mock<ILogger<SWAPIService>> _logger;

		public Fetch()
		{
			_logger = LoggerMockFactory.GenericLogger<SWAPIService>();
		}

		[Fact]
		public async Task ReturnsFailingServiceResultIfApiReturns404()
		{
			var handler = NetworkMockFactory.GeneralHttpMessageHandler(HttpStatusCode.NotFound);
			var client = new HttpClient(handler.Object);

			var service = new SWAPIService(client, _logger.Object);

			var result = await service.Fetch<Film>("https://notarealuri.com");
			Assert.Equal(Status.NotFound, result.Status);
		}

		[Fact]
		public async Task ReturnsFailingServiceResultIfApiReturnsError()
		{
			var handler = NetworkMockFactory.GeneralHttpMessageHandler(HttpStatusCode.ServiceUnavailable);
			var client = new HttpClient(handler.Object);

			var service = new SWAPIService(client, _logger.Object);

			var result = await service.Fetch<Film>(Url);
			Assert.Equal(Status.Error, result.Status);
		}

		[Fact]
		public async Task ReturnsFailingServiceResultIfApiRequestThrows()
		{
			var handler = NetworkMockFactory.ThrowsHttpMessageHandler(new HttpRequestException());
			var client = new HttpClient(handler.Object);

			var service = new SWAPIService(client, _logger.Object);

			var result = await service.Fetch<Film>(Url);
			Assert.Equal(Status.Error, result.Status);
		}

		[Fact]
		public async Task ReturnsFailingServiceResultIfJsonInvalid()
		{
			var responseContent = new StringContent("invalid json");
			var handler = NetworkMockFactory.GeneralHttpMessageHandler(message: responseContent);
			var client = new HttpClient(handler.Object);

			var service = new SWAPIService(client, _logger.Object);

			var result = await service.Fetch<Film>(Url);
			Assert.Equal(Status.Error, result.Status);
		}

		[Fact]
		public async Task ReturnsSuccessfulServiceResultIfEverythingSucceeds()
		{
			var characterUrl = "https://swapi.dev/api/people/1";
			var responseContent = new StringContent($"{{\"characters\": [\"{characterUrl}\"]}}");
			var handler = NetworkMockFactory.GeneralHttpMessageHandler(message: responseContent);
			var client = new HttpClient(handler.Object);

			var service = new SWAPIService(client, _logger.Object);

			var result = await service.Fetch<Film>(Url);

			Assert.Equal(Status.Success, result.Status);
			Assert.IsType<ServiceResult<Film>>(result);
			Assert.NotNull(result.Result);
			Assert.Single(result.Result.CharacterResources);
			Assert.Equal(characterUrl, result.Result.CharacterResources[0]);
		}
	}
}
