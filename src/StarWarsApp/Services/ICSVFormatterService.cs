using System.Collections.Generic;

namespace StarWarsApp.Services
{
	public interface ICSVFormatterService<in TEntity>
	{
		string Format(IEnumerable<TEntity> entities);
	}
}
