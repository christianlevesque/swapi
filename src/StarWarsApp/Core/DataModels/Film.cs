using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StarWarsApp.Core.DataModels
{
	public class Film
	{
		/// <summary>
		/// Represents the SWAPI resource URLs for each of the film's characters
		/// </summary>
		[JsonPropertyName("characters")]
		public IList<string> CharacterResources { get; set; }
		
		/// <summary>
		/// The name of the Film
		///
		/// The Title only includes the Film's name without "Episode __"
		/// </summary>
		public string Title { get; set; }
	}
}
