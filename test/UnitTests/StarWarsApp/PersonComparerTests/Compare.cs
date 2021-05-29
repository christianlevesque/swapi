using StarWarsApp;
using StarWarsApp.Core.DataModels;
using Xunit;

namespace UnitTests.StarWarsApp.PersonComparerTests
{
	public class Compare
	{
		private readonly PersonComparer _comparer = new PersonComparer();

		[Fact]
		public void SortsByFirstAppearance()
		{
			var p1 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Homeworld = string.Empty,
				FirstAppearedInId = 2
			};
			var p2 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Homeworld = string.Empty,
				FirstAppearedInId = 1
			};
			var p3 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Homeworld = string.Empty,
				FirstAppearedInId = 3
			};
			
			Assert.True(_comparer.Compare(p1, p2) > 0);
			Assert.True(_comparer.Compare(p1, p3) < 0);
			Assert.Equal(0, _comparer.Compare(p1, p1));
		}

		[Fact]
		public void SortsByHomeworld()
		{
			var p1 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Age = 1,
				Homeworld = "Tatooine"
			};
			var p2 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Age = 1,
				Homeworld = "Coruscant"
			};
			var p3 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Age = 1,
				Homeworld = "Tython"
			};
			
			Assert.True(_comparer.Compare(p1, p2) > 0);
			Assert.True(_comparer.Compare(p1, p3) < 0);
			Assert.Equal(0, _comparer.Compare(p1, p1));
		}

		[Fact]
		public void SortsByAge()
		{
			var p1 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Age = 57,
				Homeworld = string.Empty
			};
			var p2 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Age = 60,
				Homeworld = string.Empty
			};
			var p3 = new Person
			{
				FirstName = string.Empty,
				LastName = string.Empty,
				Age = 28,
				Homeworld = string.Empty
			};
			
			Assert.True(_comparer.Compare(p1, p2) < 0);
			Assert.True(_comparer.Compare(p1, p3) > 0);
			Assert.Equal(0, _comparer.Compare(p1, p1));
		}
		
		[Fact]
		public void SortsByLastName()
		{
			var p1 = new Person
			{
				FirstName = string.Empty,
				LastName = "Skywalker",
				Age = 1,
				Homeworld = string.Empty
			};
			var p2 = new Person
			{
				FirstName = string.Empty,
				LastName = "Kenobi",
				Age = 1,
				Homeworld = string.Empty
			};
			var p3 = new Person
			{
				FirstName = string.Empty,
				LastName = "Yoda",
				Age = 1,
				Homeworld = string.Empty
			};
			
			Assert.True(_comparer.Compare(p1, p2) > 0);
			Assert.True(_comparer.Compare(p1, p3) < 0);
			Assert.Equal(0, _comparer.Compare(p1, p1));
		}

		[Fact]
		public void SortsByFirstName()
		{
			var p1 = new Person
			{
				FirstName = "Luke",
				LastName = string.Empty,
				Age = 1,
				Homeworld = string.Empty
			};
			var p2 = new Person
			{
				FirstName = "Obi-Wan",
				LastName = string.Empty,
				Age = 1,
				Homeworld = string.Empty
			};
			var p3 = new Person
			{
				FirstName = string.Empty,
				LastName = "Yoda",
				Age = 1,
				Homeworld = string.Empty
			};
			
			Assert.True(_comparer.Compare(p1, p2) < 0);
			Assert.True(_comparer.Compare(p1, p3) < 0);
			Assert.Equal(0, _comparer.Compare(p1, p1));
		}
	}
}
