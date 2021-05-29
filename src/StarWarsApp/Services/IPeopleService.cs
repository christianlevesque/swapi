using System.Threading.Tasks;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public interface IPeopleService
	{
		Task<ServiceResult<Person>> GetPersonData(string resource, int filmId);
	}
}
