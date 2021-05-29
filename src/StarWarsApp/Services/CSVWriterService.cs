using System;
using System.IO;
using System.Threading.Tasks;

namespace StarWarsApp.Services
{
	public class CSVWriterService : ICSVWriterService,
	                                IAsyncDisposable
	{
		private readonly StreamWriter _writer;

		/// <summary>
		/// Creates a new CSV writer with the given StreamWriter or filename
		/// </summary>
		/// <param name="writer">The StreamWriter that receives the CSV output</param>
		/// <param name="filename">The filename to write the CSV to if a StreamWriter isn't supplied</param>
		public CSVWriterService(StreamWriter writer = null, string filename = null)
		{
			filename ??= $"{Directory.GetCurrentDirectory()}/output.csv";
			_writer = writer ?? new StreamWriter(filename);
		}

		/// <summary>
		/// Writes the character report to the instance's StreamWriter
		/// </summary>
		/// <param name="output">The character report to write out</param>
		/// <returns></returns>
		public async Task Write(string output)
		{
			await _writer.WriteAsync(output);
		}

		public async ValueTask DisposeAsync()
		{
			await _writer.DisposeAsync();
		}
	}
}
