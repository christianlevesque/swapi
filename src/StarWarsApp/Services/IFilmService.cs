using System.Threading.Tasks;
using StarWarsApp.Core;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public interface IFilmService
	{
		Task<ServiceResult<Film>> GetFilmData(int id);
	}
}
