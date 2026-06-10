//
// Colorimetry  Copyright (C) 2026  Aptivi
//
// This file is part of Colorimetry
//
// Colorimetry is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Colorimetry is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Colorimetry.Models.Conversion;
using Colorimetry.Transformation;
using Colorimetry.Languages;
using Textify.General;

namespace Colorimetry.Models
{
    /// <summary>
    /// The LMS class instance
    /// </summary>
    [DebuggerDisplay("LMS = {L};{M};{S}")]
    public class Lms : BaseColorModel, IEquatable<Lms>
    {
        /// <summary>
        /// The L value [0.0 -> 1.0]
        /// </summary>
        public double L { get; private set; }
        /// <summary>
        /// The M value [0.0 -> 1.0]
        /// </summary>
        public double M { get; private set; }
        /// <summary>
        /// The S value [0.0 -> 1.0]
        /// </summary>
        public double S { get; private set; }

        /// <summary>
        /// lms:&lt;L&gt;;&lt;M&gt;;&lt;S&gt;
        /// </summary>
        public override string ToString() =>
            $"lms:{L:0.##};{M:0.##};{S:0.##}";

        /// <summary>
        /// Does the string specifier represent a valid LMS specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid LMS specifier</param>
        /// <param name="checkParts">Whether to check the parts count or not</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierValid(string specifier, bool checkParts = false) =>
            specifier.Contains(";") &&
            specifier.StartsWith("lms:") &&
            (!checkParts || (checkParts && specifier.Substring(4).Split(';').Length == 3));

        /// <summary>
        /// Does the string specifier represent a valid LMS specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid LMS specifier</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierAndValueValid(string specifier)
        {
            if (!IsSpecifierValid(specifier, true))
                return false;

            var specifierArray = specifier.Substring(4).Split(';');
            int y = Convert.ToInt32(specifierArray[0]);
            if (y < 0 || y > 1)
                return false;
            int db = Convert.ToInt32(specifierArray[1]);
            if (db < 0 || db > 1)
                return false;
            int dr = Convert.ToInt32(specifierArray[2]);
            if (dr < 0 || dr > 1)
                return false;
            return true;
        }

        /// <summary>
        /// Parses the specifier and returns an instance of <see cref="Lms"/> converted to <see cref="RedGreenBlue"/>
        /// </summary>
        /// <param name="specifier">Specifier of RGB</param>
        /// <param name="settings">Settings to use. Use null for global settings</param>
        /// <returns>An instance of <see cref="RedGreenBlue"/></returns>
        /// <exception cref="ColorException"></exception>
        public static new RedGreenBlue ParseSpecifierToRgb(string specifier, ColorSettings? settings = null)
        {
            var lms = ParseSpecifier(specifier);
            var rgb = ConversionTools.ToRgb(lms);
            int r = rgb.R;
            int g = rgb.G;
            int b = rgb.B;

            // Now, transform
            settings ??= new(ColorTools.GlobalSettings);
            var finalRgb = TransformationTools.GetTransformedColor(r, g, b, settings);

            // Make a new RGB class
            return new(finalRgb.r, finalRgb.g, finalRgb.b);
        }

        /// <summary>
        /// Parses the specifier and returns an instance of <see cref="Lms"/>
        /// </summary>
        /// <param name="specifier">Specifier of LMS</param>
        /// <returns>An instance of <see cref="Lms"/></returns>
        /// <exception cref="ColorException"></exception>
        public static Lms ParseSpecifier(string specifier)
        {
            if (!IsSpecifierValid(specifier))
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDLMSSPECIFIER").FormatString(specifier) + ": lms:<l>;<m>;<s>");

            // Split the VT sequence into three parts
            var specifierArray = specifier.Substring(4).Split(';');
            if (specifierArray.Length == 3)
            {
                // We got the LMS whole values! First, check to see if we need to filter the color for the color-blind
                double l = Convert.ToDouble(specifierArray[0]);
                if (l < 0 || l > 1)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSELMSLLEVEL") + $" {l}");
                double m = Convert.ToDouble(specifierArray[1]);
                if (m < 0 || m > 1)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSELMSMLEVEL") + $" {m}");
                double s = Convert.ToDouble(specifierArray[2]);
                if (s < 0 || s > 1)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSELMSSLEVEL") + $" {s}");

                // First, we need to convert from LMS to RGB
                var LMS = new Lms(l, m, s);
                return LMS;
            }
            else
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDLMSSPECIFIEREXCEED").FormatString(specifier) + ": lms:<l>;<m>;<s>");
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals((Lms)obj);

        /// <inheritdoc/>
        public bool Equals(Lms other) =>
            other is not null &&
            L == other.L &&
            M == other.M &&
            S == other.S;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -882565174;
            hashCode = hashCode * -1521134295 + L.GetHashCode();
            hashCode = hashCode * -1521134295 + M.GetHashCode();
            hashCode = hashCode * -1521134295 + S.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(Lms left, Lms right) =>
            EqualityComparer<Lms>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(Lms left, Lms right) =>
            !(left == right);

        internal Lms(double l, double m, double s)
        {
            L = l;
            M = m;
            S = s;
        }
    }
}
