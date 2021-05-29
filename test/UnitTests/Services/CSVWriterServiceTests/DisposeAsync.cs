using System;
using System.IO;
using System.Threading.Tasks;
using StarWarsApp.Services;
using Xunit;

namespace UnitTests.Services.CSVWriterServiceTests
{
	public class DisposeAsync
	{
		[Fact]
		public async Task CallsStreamWriterDisposeAsync()
		{
			var stream = new MemoryStream();
			var sr = new StreamWriter(stream);
			var writer = new CSVWriterService(sr);

			await writer.DisposeAsync();

			await Assert.ThrowsAsync<ObjectDisposedException>(() => sr.WriteAsync(""));
		}
	}
}
