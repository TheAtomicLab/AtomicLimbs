using System;

namespace Limbs.Web.Common.Geocoder
{
	public struct Distance
	{
	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        return obj is Distance distance && Equals(distance);
	    }

	    public const double EarthRadiusInMiles = 3956.545;
		public const double EarthRadiusInKilometers = 6378.135;
		private const double ConversionConstant = 0.621371192;

	    public double Value { get; }

	    public DistanceUnits Units { get; }

	    public Distance(double value, DistanceUnits units)
		{
			Value = Math.Round(value, 8);
			Units = units;
		}

		#region Helper Factory Methods

		public static Distance FromMiles(double miles)
		{
			return new Distance(miles, DistanceUnits.Miles);
		}

		public static Distance FromKilometers(double kilometers)
		{
			return new Distance(kilometers, DistanceUnits.Kilometers);
		}

		#endregion

		#region Unit Conversions

		private Distance ConvertUnits(DistanceUnits units)
		{
			if (Units == units) return this;

			double newValue;
			switch (units)
			{
				case DistanceUnits.Miles:
					newValue = Value * ConversionConstant;
					break;
				case DistanceUnits.Kilometers:
					newValue = Value / ConversionConstant;
					break;
				default:
					newValue = 0;
					break;
			}

			return new Distance(newValue, units);
		}

		public Distance ToMiles()
		{
			return ConvertUnits(DistanceUnits.Miles);
		}

		public Distance ToKilometers()
		{
			return ConvertUnits(DistanceUnits.Kilometers);
		}

		#endregion

	    public bool Equals(Distance obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(Distance obj, bool normalizeUnits)
		{
			if (normalizeUnits)
				obj = obj.ConvertUnits(Units);
			return Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Value} {Units}";
		}

		#region Operators

		public static Distance operator *(Distance d1, double d)
		{
			double newValue = d1.Value * d;
			return new Distance(newValue, d1.Units);
		}

		public static Distance operator +(Distance left, Distance right)
		{
			double newValue = left.Value + right.ConvertUnits(left.Units).Value;
			return new Distance(newValue, left.Units);
		}

		public static Distance operator -(Distance left, Distance right)
		{
			double newValue = left.Value - right.ConvertUnits(left.Units).Value;
			return new Distance(newValue, left.Units);
		}

		public static bool operator ==(Distance left, Distance right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Distance left, Distance right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(Distance left, Distance right)
		{
			return (left.Value < right.ConvertUnits(left.Units).Value);
		}

		public static bool operator <=(Distance left, Distance right)
		{
			return (left.Value <= right.ConvertUnits(left.Units).Value);
		}

		public static bool operator >(Distance left, Distance right)
		{
			return (left.Value > right.ConvertUnits(left.Units).Value);
		}

		public static bool operator >=(Distance left, Distance right)
		{
			return (left.Value >= right.ConvertUnits(left.Units).Value);
		}

		public static implicit operator double(Distance distance)
		{
			return distance.Value;
		}

		#endregion
	}
}
