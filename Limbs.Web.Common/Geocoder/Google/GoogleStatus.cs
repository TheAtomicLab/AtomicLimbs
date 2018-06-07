namespace Limbs.Web.Common.Geocoder.Google
{
	public enum GoogleStatus
	{
		Error = 0,
		Ok = 1,
		ZeroResults = 2,
		OverQueryLimit = 3,
		RequestDenied = 4,
		InvalidRequest = 5
	}
}