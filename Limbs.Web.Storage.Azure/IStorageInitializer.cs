namespace Limbs.Web.Storage.Azure
{
	public interface IStorageInitializer
	{
		void Initialize();
		void Drop();
	}
}