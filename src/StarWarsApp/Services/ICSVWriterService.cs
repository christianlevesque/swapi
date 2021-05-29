using System;
using System.Threading.Tasks;

namespace StarWarsApp.Services
{
	public interface ICSVWriterService : IAsyncDisposable
	{
		Task Write(string output);
	}
}
