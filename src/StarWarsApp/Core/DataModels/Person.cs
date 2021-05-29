using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StarWarsApp.Core.DataModels
{
	public class Person
	{
		/// <summary>
		/// The Person's first name
		///
		/// For characters with only one name (e.g., Yoda), this will be an empty string
		///
		/// The FirstName is generated based on the Name property from the API JSON
		/// </summary>
		[JsonIgnore]
		public string FirstName { get; set; }

		/// <summary>
		/// The Person's last name
		///
		/// For characters with only one name (e.g., Yoda), this will be the name
		///
		/// The LastName is generated based on the Name property from the API JSON
		/// </summary>
		[JsonIgnore]
		public string LastName { get; set; }
		
		/// <summary>
		/// The Person's age relative to 5000 BBY
		///
		/// The Age is generated based on the BirthYear property from the API JSON
		/// </summary>
		[JsonIgnore]
		public float Age { get; set; }
		
		/// <summary>
		/// The name of the first film the Person appeared in
		///
		/// The FirstAppearedInName is supplied by the PersonService
		/// </summary>
		[JsonIgnore]
		public string FirstAppearedInName { get; set; }
		
		/// <summary>
		/// The ID of the first film the Person appeared in
		///
		/// The FirstAppearedInId is supplied by the PersonService
		/// </summary>
		[JsonIgnore]
		public int FirstAppearedInId { get; set; }

		/// <summary>
		/// The full name of the Person
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// The homeworld of the Person
		///
		/// When the API is parsed, this is a URL resource string. The PersonService will replace this with the name of the planet before returning
		/// </summary>
		public string Homeworld { get; set; }

		/// <summary>
		/// The birth year of the Person
		///
		/// This property will be in the form XXABY/BBY
		/// </summary>
		[JsonPropertyName("birth_year")]
		public string BirthYear { get; set; }
	}
}
