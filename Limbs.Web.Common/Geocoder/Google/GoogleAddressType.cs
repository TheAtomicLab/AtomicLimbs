namespace Limbs.Web.Common.Geocoder.Google
{
	/// <remarks>
	/// http://code.google.com/apis/maps/documentation/geocoding/#Types
	/// </remarks>
	public enum GoogleAddressType
	{
		Unknown = 0,
		StreetAddress = 1,
		Route = 2,
		Intersection = 3,
		Political = 4,
		Country = 5,
		AdministrativeAreaLevel1 = 6,
		AdministrativeAreaLevel2 = 7,
		AdministrativeAreaLevel3 = 8,
		ColloquialArea = 9,
		Locality = 10,
		SubLocality = 11,
		Neighborhood = 12,
		Premise = 13,
		Subpremise = 14,
		PostalCode = 15,
		NaturalFeature = 16,
		Airport = 17,
		Park = 18,
		PointOfInterest = 19,
		PostBox = 20,
		StreetNumber = 21,
		Floor = 22,
		Room = 23,
		PostalTown = 24,
		Establishment = 25,
		SubLocalityLevel1 = 26,
		SubLocalityLevel2 = 27,
		SubLocalityLevel3 = 28,
		SubLocalityLevel4 = 29,
		SubLocalityLevel5 = 30,
		PostalCodeSuffix = 31
	}
}