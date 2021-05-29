using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace UnitTests.Mocks
{
	internal static class NetworkMockFactory
	{
		private static StringContent _defaultMessage = new StringContent("{}");

		internal static Mock<HttpMessageHandler> GeneralHttpMessageHandler(HttpStatusCode status = HttpStatusCode.OK, HttpContent message = null)
		{
			message ??= _defaultMessage;

			var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			handler.Protected()
			       .Setup<Task<HttpResponseMessage>>(
				        "SendAsync",
				        ItExpr.IsAny<HttpRequestMessage>(),
				        ItExpr.IsAny<CancellationToken>()
			        )
			       .ReturnsAsync(
				        new HttpResponseMessage
				        {
					        StatusCode = status,
					        Content = message
				        }
			        )
			       .Verifiable();

			return handler;
		}

		internal static Mock<HttpMessageHandler> ThrowsHttpMessageHandler<T>(T e)
			where T : Exception
		{
			var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
			handler
			   .Protected()
			   .Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>()
				)
			   .Throws(e)
			   .Verifiable();

			return handler;
		}
	}
}
