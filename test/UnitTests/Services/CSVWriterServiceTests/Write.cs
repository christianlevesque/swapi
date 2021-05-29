using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StarWarsApp.Services;
using Xunit;

namespace UnitTests.Services.CSVWriterServiceTests
{
	public class Write
	{
		[Fact]
		public async Task CallsStreamWriterWriteMethod()
		{
			var output = "my output here";
			var stream = new MemoryStream();
			var sr = new StreamWriter(stream);
			var writer = new CSVWriterService(sr);

			await writer.Write(output);
			await sr.FlushAsync();
			
			var bytes = stream.ToArray();
			var contents = Encoding.UTF8.GetString(bytes);
			
			Assert.Equal(14, bytes.Length);
			Assert.Equal(output, contents);
		}
	}
}
