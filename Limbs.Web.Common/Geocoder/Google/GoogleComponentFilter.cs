namespace Limbs.Web.Common.Geocoder.Google
{
	public class GoogleComponentFilter
	{
		public string ComponentFilter { get; set; }

		public GoogleComponentFilter(string component, string value)
		{
			ComponentFilter = $"{component}:{value}";
		}
	}
}
