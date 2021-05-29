using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsApp
{
	public interface IApp
	{
		public Task GenerateCharacterReport(IEnumerable<int> films);
	}
}
