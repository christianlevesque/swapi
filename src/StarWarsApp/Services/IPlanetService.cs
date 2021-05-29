using System.Threading.Tasks;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public interface IPlanetService
	{
		Task<ServiceResult<Planet>> GetPlanetData(string resource);
	}
}
