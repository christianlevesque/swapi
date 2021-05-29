using System.Threading.Tasks;
using StarWarsApp.Core;

namespace StarWarsApp.Services
{
	/// <summary>
	/// Describes the contract for classes that read data from a REST API
	///
	/// Instead of taking an int ID, it takes a resource string. This is because the SWAPI returns a fully-formed URL instead of just the ID of the resource. I have mixed feelings about that, but I don't get to make the API, just call it. 
	/// </summary>
	public interface ISWAPIService
	{
		/// <summary>
		/// Fetches the data model representation of a SWAPI entity
		/// </summary>
		/// <param name="resource">The URL of the entity to fetch</param>
		/// <typeparam name="TEntity">The type of the entity</typeparam>
		/// <returns>The entity as a data model</returns>
		Task<ServiceResult<TEntity>> Fetch<TEntity>(string resource)
			where TEntity : class;
	}
}
