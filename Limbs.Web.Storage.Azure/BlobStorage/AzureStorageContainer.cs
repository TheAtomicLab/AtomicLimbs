namespace Limbs.Web.Storage.Azure.BlobStorage
{
    public sealed class AzureStorageContainer
    {
        public static readonly AzureStorageContainer UserFiles = new AzureStorageContainer(1, "userfiles");
        //public static readonly AzureStorageContainer SINGLESIGNON = new AzureStorageContainer(2, "sample");


        private readonly int _value;
        private readonly string _name;
                
        private AzureStorageContainer(int value, string name)
        {
            this._value = value;
            this._name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}