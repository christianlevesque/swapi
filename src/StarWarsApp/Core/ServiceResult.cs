namespace StarWarsApp.Core
{
	/// <summary>
	/// Represents the return value of a Service
	/// </summary>
	/// <typeparam name="TResult">The type of the entity returned from the Service</typeparam>
	public class ServiceResult<TResult>
	{
		/// <summary>
		/// The entity returned from the Service, if any
		/// </summary>
		public TResult Result { get; set; }

		/// <summary>
		/// The return Status of the Service
		/// </summary>
		public Status Status { get; set; } = Status.Success;
	}
}
