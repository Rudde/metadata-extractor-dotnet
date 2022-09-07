﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor
{
    /// <summary>Represents a latitude and longitude pair, giving a position on earth in spherical coordinates.</summary>
    /// <remarks>
    /// Values of latitude and longitude are given in degrees.
    /// <para />
    /// This type is immutable.
    /// </remarks>
    public sealed class GeoLocation
    {
        /// <summary>
        /// Initialises an instance of <see cref="GeoLocation"/>.
        /// </summary>
        /// <param name="latitude">the latitude, in degrees</param>
        /// <param name="longitude">the longitude, in degrees</param>
        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = null;
        }

        /// <summary>
        /// Initialises an instance of <see cref="GeoLocation"/>.
        /// </summary>
        /// <param name="latitude">the latitude, in degrees</param>
        /// <param name="longitude">the longitude, in degrees</param>
        /// <param name="altitude">the altitude, in meters</param>
        public GeoLocation(double latitude, double longitude, double? altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        /// <value>the latitudinal angle of this location, in degrees.</value>
        public double Latitude { get; }

        /// <value>the longitudinal angle of this location, in degrees.</value>
        public double Longitude { get; }

        /// <value>the altitude of this location, in meters, null indicates there is no altitude present in exif</value>
        public double? Altitude { get; }

        /// <value>true, if both latitude and longitude are equal to zero</value>
        public bool IsZero => Latitude == 0 && Longitude == 0;

        #region Static helpers/factories

        /// <summary>
        /// Converts a decimal degree angle into its corresponding DMS (degrees-minutes-seconds) representation as a string,
        /// of format:
        /// <c>-1° 23' 4.56"</c>
        /// </summary>
        [Pure]
        public static string DecimalToDegreesMinutesSecondsString(double value)
        {
            var dms = DecimalToDegreesMinutesSeconds(value);
            return $"{dms[0]:0.##}\u00b0 {dms[1]:0.##}' {dms[2]:0.##}\"";
        }

        /// <summary>
        /// Converts a decimal degree angle into its corresponding DMS (degrees-minutes-seconds) component values, as
        /// a double array.
        /// </summary>
        [Pure]
        public static double[] DecimalToDegreesMinutesSeconds(double value)
        {
            var d = (int)value;
            var m = Math.Abs((value % 1) * 60);
            var s = (m % 1) * 60;
            return new[] { d, (int)m, s };
        }

        /// <summary>
        /// Converts DMS (degrees-minutes-seconds) rational values, as given in
        /// <see cref="GpsDirectory"/>, into a single value in degrees,
        /// as a double.
        /// </summary>
        [Pure]
        public static double? DegreesMinutesSecondsToDecimal(Rational degs, Rational mins, Rational secs, bool isNegative)
        {
            var value = Math.Abs(degs.ToDouble()) + mins.ToDouble() / 60.0d + secs.ToDouble() / 3600.0d;
            if (double.IsNaN(value))
                return null;
            if (isNegative)
                value *= -1;
            return value;
        }

        #endregion

        #region Equality and Hashing

        private bool Equals(GeoLocation other) => Latitude.Equals(other.Latitude) &&
                                                  Longitude.Equals(other.Longitude) &&
                                                  Altitude.Equals(other.Altitude);

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is GeoLocation location && Equals(location);
        }

        public override int GetHashCode() => unchecked(((Latitude.GetHashCode() * 397) + (Altitude.GetHashCode() * 14)) ^ Longitude.GetHashCode());

        #endregion

        #region ToString

        /// <returns>
        /// Returns a string representation of this object, of format:
        /// <c>1.23, 4.56, 8M</c>
        /// </returns>
        public override string ToString() => Latitude + ", " + Longitude + (Altitude == null ? string.Empty : $", {Altitude}M");

        /// <returns>
        /// a string representation of this location, of format:
        /// <c>-1° 23' 4.56", 54° 32' 1.92"</c>
        /// </returns>
        [Pure]
        public string ToDmsString() => DecimalToDegreesMinutesSecondsString(Latitude) + ", " + DecimalToDegreesMinutesSecondsString(Longitude);

        #endregion
    }
}
