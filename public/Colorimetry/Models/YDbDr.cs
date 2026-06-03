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
    /// The YDbDr class instance
    /// </summary>
    [DebuggerDisplay("YDbDr = {Y};{Db};{Dr}")]
    public class YDbDr : BaseColorModel, IEquatable<YDbDr>
    {
        /// <summary>
        /// The Y value [0.0 -> 1.0]
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// The Db value [−1.333 -> 1.333]
        /// </summary>
        public double Db { get; private set; }
        /// <summary>
        /// The Dr value [−1.333 -> 1.333]
        /// </summary>
        public double Dr { get; private set; }

        /// <summary>
        /// ydbdr:&lt;Y&gt;;&lt;Db&gt;;&lt;Dr&gt;
        /// </summary>
        public override string ToString() =>
            $"ydbdr:{Y:0.##};{Db:0.##};{Dr:0.##}";

        /// <summary>
        /// Does the string specifier represent a valid YDbDr specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid YDbDr specifier</param>
        /// <param name="checkParts">Whether to check the parts count or not</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierValid(string specifier, bool checkParts = false) =>
            specifier.Contains(";") &&
            specifier.StartsWith("ydbdr:") &&
            (!checkParts || (checkParts && specifier.Substring(6).Split(';').Length == 3));

        /// <summary>
        /// Does the string specifier represent a valid YDbDr specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid YDbDr specifier</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierAndValueValid(string specifier)
        {
            if (!IsSpecifierValid(specifier, true))
                return false;

            var specifierArray = specifier.Substring(6).Split(';');
            int y = Convert.ToInt32(specifierArray[0]);
            if (y < 0 || y > 1)
                return false;
            int db = Convert.ToInt32(specifierArray[1]);
            if (db < -1.333 || db > 1.333)
                return false;
            int dr = Convert.ToInt32(specifierArray[2]);
            if (dr < -1.333 || dr > 1.333)
                return false;
            return true;
        }

        /// <summary>
        /// Parses the specifier and returns an instance of <see cref="YDbDr"/> converted to <see cref="RedGreenBlue"/>
        /// </summary>
        /// <param name="specifier">Specifier of RGB</param>
        /// <param name="settings">Settings to use. Use null for global settings</param>
        /// <returns>An instance of <see cref="RedGreenBlue"/></returns>
        /// <exception cref="ColorException"></exception>
        public static new RedGreenBlue ParseSpecifierToRgb(string specifier, ColorSettings? settings = null)
        {
            var YDbDr = ParseSpecifier(specifier);
            var rgb = ConversionTools.ToRgb(YDbDr);
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
        /// Parses the specifier and returns an instance of <see cref="YDbDr"/>
        /// </summary>
        /// <param name="specifier">Specifier of YDbDr</param>
        /// <returns>An instance of <see cref="YDbDr"/></returns>
        /// <exception cref="ColorException"></exception>
        public static YDbDr ParseSpecifier(string specifier)
        {
            // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYDBDRSPECIFIER -> "Invalid YDbDr color specifier \"{0}\". Ensure that it's on the correct format"
            if (!IsSpecifierValid(specifier))
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYDBDRSPECIFIER").FormatString(specifier) + ": ydbdr:<y>;<db>;<dr>");

            // Split the VT sequence into three parts
            var specifierArray = specifier.Substring(6).Split(';');
            if (specifierArray.Length == 3)
            {
                // We got the YDbDr whole values! First, check to see if we need to filter the color for the color-blind
                double y = Convert.ToDouble(specifierArray[0]);
                // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEYDBDRYLEVEL -> "The Y value is out of range (0.0 -> 1.0)."
                if (y < 0 || y > 1)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYDBDRYLEVEL") + $" {y}");
                double db = Convert.ToDouble(specifierArray[1]);
                // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEYDBDRDBLEVEL -> "The Db value is out of range (−1.333 -> 1.333)."
                if (db < -1.333 || db > 1.333)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYDBDRDBLEVEL") + $" {db}");
                double dr = Convert.ToDouble(specifierArray[2]);
                // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEYDBDRDRLEVEL -> "The Dr value is out of range (−1.333 -> 1.333)."
                if (dr < -1.333 || dr > 1.333)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYDBDRDRLEVEL") + $" {dr}");

                // First, we need to convert from YDbDr to RGB
                var YDbDr = new YDbDr(y, db, dr);
                return YDbDr;
            }
            else
                // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYDBDRSPECIFIEREXCEED -> "Invalid YDbDr color specifier \"{0}\". The specifier may not be more than three elements. Ensure that it's on the correct format"
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYDBDRSPECIFIEREXCEED").FormatString(specifier) + ": ydbdr:<y>;<db>;<dr>");
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals((YDbDr)obj);

        /// <inheritdoc/>
        public bool Equals(YDbDr other) =>
            other is not null &&
            Y == other.Y &&
            Db == other.Db &&
            Dr == other.Dr;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -882565174;
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Db.GetHashCode();
            hashCode = hashCode * -1521134295 + Dr.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(YDbDr left, YDbDr right) =>
            EqualityComparer<YDbDr>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(YDbDr left, YDbDr right) =>
            !(left == right);

        internal YDbDr(double y, double db, double dr)
        {
            Y = y;
            Db = db;
            Dr = dr;
        }
    }
}
