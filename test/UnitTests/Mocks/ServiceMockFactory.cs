using System.Collections.Generic;
using Moq;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;

namespace UnitTests.Mocks
{
	internal static class ServiceMockFactory
	{
		internal static Mock<ISWAPIService> SWAPISevice<TEntity>(TEntity expected = null, Status status = Status.Success)
			where TEntity : class
		{
			var service = new Mock<ISWAPIService>();

			service.Setup(s => s.Fetch<TEntity>(It.IsAny<string>()))
			       .ReturnsAsync(
				        new ServiceResult<TEntity>
				        {
					        Result = expected,
					        Status = status
				        }
			        );

			return service;
		}

		internal static Mock<IPlanetService> PlanetService()
		{
			return new();
		}

		internal static Mock<IFilmService> FilmService()
		{
			var service = new Mock<IFilmService>();

			service.Setup(s => s.GetFilmData(It.IsAny<int>()))
			       .ReturnsAsync(
				        new ServiceResult<Film>
				        {
					        Result = new()
					        {
						        CharacterResources = new List<string>
						        {
							        "https://swapi.dev/api/people/1"
						        },
						        Title = "A New Hope"
					        }
				        }
			        );

			return service;
		}
	}
}
