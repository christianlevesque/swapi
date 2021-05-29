using System.Threading.Tasks;
using Moq;
using StarWarsApp;
using Xunit;

namespace UnitTests.StarWarsApp.ProgramTests
{
	public class Main
	{
		[Fact]
		public async Task CallsGenerateCharacterReport()
		{
			var app = new Mock<IApp>();
			Program.InitApp(app.Object);
			await Program.Main();

			app.Verify(
				a => a.GenerateCharacterReport(
					new[]
					{
						2,
						4,
						6
					}
				),
				Times.Once
			);
		}
	}
}
