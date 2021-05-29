using System.Collections.Generic;
using System.Text;
using StarWarsApp.Core.DataModels;

namespace StarWarsApp.Services
{
	public class PersonCSVFormatterService : ICSVFormatterService<Person>
	{
		/// <summary>
		/// Formats an IEnumerable<Person> as a CSV
		/// </summary>
		/// <param name="entities">The Persons to format for output</param>
		/// <returns></returns>
		public string Format(IEnumerable<Person> entities)
		{
			var output = new StringBuilder();

			foreach (var entity in entities)
			{
				output.AppendLine($"\"{entity.FirstAppearedInName}\",\"{entity.Homeworld}\",\"{entity.BirthYear}\",\"{entity.Name}\"");
			}

			return output.ToString();
		}
	}
}
