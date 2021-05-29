using System;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using UnitTests.Mocks;
using Xunit;

namespace UnitTests.Services.FilmServiceTests
{
	public class TranslateFilmNumber
	{
		[Fact]
		public void TranslatesValidValues()
		{
			Assert.Equal(4, FilmService.TranslateFilmNumber(1));
			Assert.Equal(5, FilmService.TranslateFilmNumber(2));
			Assert.Equal(6, FilmService.TranslateFilmNumber(3));
			Assert.Equal(1, FilmService.TranslateFilmNumber(4));
			Assert.Equal(2, FilmService.TranslateFilmNumber(5));
			Assert.Equal(3, FilmService.TranslateFilmNumber(6));
		}

		[Fact]
		public void ThrowsIfOutOfRange()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => FilmService.TranslateFilmNumber(7));
		}
	}
}
