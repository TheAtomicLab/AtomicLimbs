namespace Limbs.Web.Storage.Azure.TableStorage
{
	public interface IPersister<TDataRow>
	{
		/// <summary>
		/// Get a single entity by its key (partitionKey+rowKey)
		/// </summary>
		/// <param name="partitionKey">The PartitionKey.</param>
		/// <param name="rowKey">The RowKey</param>
		/// <returns>The entity if exists; null otherwise.</returns>
		TDataRow Get(string partitionKey, string rowKey);

		/// <summary>
		/// Save a new instance or, Update and existing one, to the table
		/// </summary>
		/// <param name="dataRow">The instance to be saved.</param>
		void Add(TDataRow dataRow);

		/// <summary>
		/// Delete an entity instance by its key (partitionKey+rowKey)
		/// </summary>
		/// <param name="partitionKey">The PartitionKey.</param>
		/// <param name="rowKey">The RowKey</param>
		void Delete(string partitionKey, string rowKey);

		/// <summary>
		///  Delete an entity instance
		/// </summary>
		/// <param name="dataRow">The instance to be deleted.</param>
		void Delete(TDataRow dataRow);

		/// <summary>
		/// Update an existing instance (previously uploaded)
		/// </summary>
		/// <param name="dataRow">The instance to be saved.</param>
		void Update(TDataRow dataRow);
	}
}