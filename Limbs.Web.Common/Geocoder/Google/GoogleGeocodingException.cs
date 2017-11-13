using System;

namespace Limbs.Web.Common.Geocoder.Google
{
	public class GoogleGeocodingException : Exception
	{
	    private const string DefaultMessage = "There was an error processing the geocoding request. See Status or InnerException for more information.";

		public GoogleStatus Status { get; }

		public GoogleGeocodingException(GoogleStatus status)
			: base(DefaultMessage)
		{
			this.Status = status;
		}

		public GoogleGeocodingException(Exception innerException)
			: base(DefaultMessage, innerException)
		{
			this.Status = GoogleStatus.Error;
		}
	}
}