using System;
using StarWarsApp.Core.DataModels;
using StarWarsApp.Services;
using Xunit;

namespace UnitTests.Services.PersonCSVFormatterServiceTests
{
	public class Format
	{
		private readonly PersonCSVFormatterService _formatter;

		public Format()
		{
			_formatter = new PersonCSVFormatterService();
		}

		[Fact]
		public void FormatsPersonInCsv()
		{
			var people = new Person[]
			{
				new Person
				{
					FirstAppearedInName = "A New Hope",
					Homeworld = "Tatooine",
					BirthYear = "19BBY",
					Name = "Luke Skywalker"
				}
			};

			var output = _formatter.Format(people);
			var expectedOutput = $"\"A New Hope\",\"Tatooine\",\"19BBY\",\"Luke Skywalker\"{Environment.NewLine}";
			
			Assert.Equal(expectedOutput, output);
		}
	}
}
