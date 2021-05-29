using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Mocks
{
	internal static class LoggerMockFactory
	{
		internal static Mock<ILogger<T>> GenericLogger<T>()
		{
			return new Mock<ILogger<T>>();
		}
	}
}
