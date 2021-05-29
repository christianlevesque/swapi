using System.Collections.Generic;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp
{
	public class PersonComparer : Comparer<Person>
	{
		/// <summary>
		/// Sorts two Person objects
		///
		/// The Persons are first sorted by the Person.FirstAppearedInId property, followed by Person.Homeworld, Person.Age, Person.LastName, and finally Person.FirstName.
		/// </summary>
		/// <param name="x">The first Person to compare</param>
		/// <param name="y">The second Person to compare</param>
		/// <returns></returns>
		public override int Compare(Person x, Person y)
		{
			var comparison = x.FirstAppearedInId.CompareTo(y.FirstAppearedInId);
			if (comparison != 0)
			{
				return comparison;
			}

			comparison = x.Homeworld.CompareTo(y.Homeworld);
			if (comparison != 0)
			{
				return comparison;
			}

			comparison = x.Age.CompareTo(y.Age);
			if (comparison != 0)
			{
				return comparison;
			}

			comparison = x.LastName.CompareTo(y.LastName);
			if (comparison != 0)
			{
				return comparison;
			}

			comparison = x.FirstName.CompareTo(y.FirstName);
			if (comparison != 0)
			{
				return comparison;
			}

			return comparison;
		}
	}
}
