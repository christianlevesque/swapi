using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StarWarsApp.Core;

namespace StarWarsApp.Services
{
	/// <summary>
	/// The service responsible for communicating with the Star Wars API
	/// </summary>
	public class SWAPIService : ISWAPIService
	{
		private readonly HttpClient _client;
		private readonly ILogger<ISWAPIService> _logger;

		/// <summary>
		/// A wrapper around the HttpClient used to communicate with the Star Wars API
		/// </summary>
		/// <param name="client"></param>
		/// <param name="logger"></param>
		public SWAPIService(HttpClient client, ILogger<ISWAPIService> logger)
		{
			_client = client;
			_logger = logger;
			
			_logger.LogInformation("New SWAPIService created");
		}

		/// <summary>
		/// Retrieves data from the Star Wars API and parses it as the specified data model
		/// </summary>
		/// <param name="resource">The URL of the resource to fetch</param>
		/// <typeparam name="TEntity">The data model to encapsulate the JSON response</typeparam>
		/// <returns></returns>
		public async Task<ServiceResult<TEntity>> Fetch<TEntity>(string resource)
			where TEntity : class
		{
			_logger.LogInformation("Fetching entity from {resource}", resource);
			var serviceResult = new ServiceResult<TEntity>();

			// Fetch the entity from the API
			var apiResult = await GetRawEntity(resource);
			if (apiResult.Status != Status.Success)
			{
				// _logger.LogError("Failed to fetch {entity}. Status code: {status}", typeof(TEntity).Name, (int) apiResult.Status);
				serviceResult.Status = apiResult.Status;
				return serviceResult;
			}

			// Deserialize the API result into a POCO
			var entityResult = DeserializeEntity<TEntity>(apiResult.Result, resource);
			if (entityResult.Status != Status.Success)
			{
				serviceResult.Status = entityResult.Status;
				return serviceResult;
			}

			_logger.LogInformation("Entity from {resource} fetched successfully", resource);
			serviceResult.Result = entityResult.Result;
			return serviceResult;
		}

		/// <summary>
		/// Fetches an entity from the SWAPI as a JSON string
		/// </summary>
		/// <param name="resource">The URL of the resource to fetch</param>
		/// <returns>The result of the API call</returns>
		private async Task<ServiceResult<string>> GetRawEntity(string resource)
		{
			var apiResult = new ServiceResult<string>();
			try
			{
				_logger.LogInformation("Sending SWAPI request to {resource}", resource);
				var apiResponse = await _client.GetAsync(resource);
				if (!apiResponse.IsSuccessStatusCode)
				{
					_logger.LogError("SWAPI request to {resource} failed with HTTP status {status}", resource, apiResponse.StatusCode);
					apiResult.Status = apiResponse.StatusCode == HttpStatusCode.NotFound ? Status.NotFound : Status.Error;
					return apiResult;
				}

				apiResult.Result = await apiResponse.Content.ReadAsStringAsync();
			}
			catch (HttpRequestException e)
			{
				_logger.LogError("Failed to fetch entity at {resource}: {exception}", resource, e);
				apiResult.Status = Status.Error;
			}

			return apiResult;
		}

		/// <summary>
		/// Converts the raw JSON representation of a SWAPI entity into its data model representation
		/// </summary>
		/// <param name="json">The raw JSON to convert</param>
		/// <param name="resource">The URL of the resource to convert</param>
		/// <typeparam name="TEntity">The type of the entity to target for deserialization</typeparam>
		/// <returns>The deserialized entity</returns>
		private ServiceResult<TEntity> DeserializeEntity<TEntity>(string json, string resource)
			where TEntity : class
		{
			var result = new ServiceResult<TEntity>();
			var deserializeOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			try
			{
				_logger.LogInformation("Deserializing entity from {resource}", resource);
				result.Result = JsonSerializer.Deserialize<TEntity>(json, deserializeOptions);
			}
			catch (JsonException e)
			{
				_logger.LogError("Failed to deserialize entity from {resource}: {exception}", resource, e);
				result.Status = Status.Error;
			}
			// This final catch block can be reached,
			// but not effectively tested without making DeserializeEntity public
			catch (Exception e)
			{
				_logger.LogError("An unknown error occurred during deserialization of {resource}: {exception}", resource, e);
				result.Status = Status.Error;
			}

			return result;
		}
	}
}
