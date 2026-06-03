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
    /// The YPbPr class instance
    /// </summary>
    [DebuggerDisplay("YPbPrHDTV = {Y};{Pb};{Pr}")]
    public class YPbPrHDTV : BaseColorModel, IEquatable<YPbPrHDTV>
    {
        /// <summary>
        /// The Y value [0 -> 700 mV]
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// The Pb value [0 -> 700 mV]
        /// </summary>
        public double Pb { get; private set; }
        /// <summary>
        /// The Pr value [0 -> 700 mV]
        /// </summary>
        public double Pr { get; private set; }

        /// <summary>
        /// YPbPrHDTV:&lt;Y&gt;;&lt;Pb&gt;;&lt;Pr&gt;
        /// </summary>
        public override string ToString() =>
            $"ypbprhdtv:{Y:0.##};{Pb:0.##};{Pr:0.##}";

        /// <summary>
        /// Does the string specifier represent a valid YPbPr specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid YPbPr specifier</param>
        /// <param name="checkParts">Whether to check the parts count or not</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierValid(string specifier, bool checkParts = false) =>
            specifier.Contains(";") &&
            specifier.StartsWith("ypbprhdtv:") &&
            (!checkParts || (checkParts && specifier.Substring(10).Split(';').Length == 3));

        /// <summary>
        /// Does the string specifier represent a valid YPbPr specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid YPbPr specifier</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierAndValueValid(string specifier)
        {
            if (!IsSpecifierValid(specifier, true))
                return false;

            var specifierArray = specifier.Substring(10).Split(';');
            int y = Convert.ToInt32(specifierArray[0]);
            if (y < 0 || y > 700)
                return false;
            int pb = Convert.ToInt32(specifierArray[1]);
            if (pb < 0 || pb > 700)
                return false;
            int pr = Convert.ToInt32(specifierArray[2]);
            if (pr < 0 || pr > 700)
                return false;
            return true;
        }

        /// <summary>
        /// Parses the specifier and returns an instance of <see cref="YPbPrHDTV"/> converted to <see cref="RedGreenBlue"/>
        /// </summary>
        /// <param name="specifier">Specifier of RGB</param>
        /// <param name="settings">Settings to use. Use null for global settings</param>
        /// <returns>An instance of <see cref="RedGreenBlue"/></returns>
        /// <exception cref="ColorException"></exception>
        public static new RedGreenBlue ParseSpecifierToRgb(string specifier, ColorSettings? settings = null)
        {
            var YPbPr = ParseSpecifier(specifier);
            var rgb = ConversionTools.ToRgb(YPbPr);
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
        /// Parses the specifier and returns an instance of <see cref="YPbPrHDTV"/>
        /// </summary>
        /// <param name="specifier">Specifier of YPbPr</param>
        /// <returns>An instance of <see cref="YPbPrHDTV"/></returns>
        /// <exception cref="ColorException"></exception>
        public static YPbPrHDTV ParseSpecifier(string specifier)
        {
            // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYPBPRSPECIFIER -> "Invalid YPbPr color specifier \"{0}\". Ensure that it's on the correct format"
            if (!IsSpecifierValid(specifier))
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYPBPRSPECIFIER").FormatString(specifier) + ": YPbPrHDTV:<y>;<pb>;<pr>");

            // Split the VT sequence into three parts
            var specifierArray = specifier.Substring(10).Split(';');
            if (specifierArray.Length == 3)
            {
                // We got the YPbPr whole values! First, check to see if we need to filter the color for the color-blind
                double y = Convert.ToDouble(specifierArray[0]);
                if (y < 0 || y > 700)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYPBPRYLEVEL") + $" {y}");
                double pb = Convert.ToDouble(specifierArray[1]);
                if (pb < 0 || pb > 700)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYPBPRX1LEVEL") + $" {pb}");
                double pr = Convert.ToDouble(specifierArray[2]);
                if (pr < 0 || pr > 700)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYPBPRY2LEVEL") + $" {pr}");

                // First, we need to convert from YPbPr to RGB
                var YPbPr = new YPbPrHDTV(y, pb, pr);
                return YPbPr;
            }
            else
                // TODO: COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYPBPRSPECIFIEREXCEED -> "Invalid YPbPr color specifier \"{0}\". The specifier may not be more than three elements. Ensure that it's on the correct format"
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYPBPRSPECIFIEREXCEED").FormatString(specifier) + ": YPbPrHDTV:<y>;<pb>;<pr>");
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals((YPbPrHDTV)obj);

        /// <inheritdoc/>
        public bool Equals(YPbPrHDTV other) =>
            other is not null &&
            Y == other.Y &&
            Pb == other.Pb &&
            Pr == other.Pr;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -882565174;
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Pb.GetHashCode();
            hashCode = hashCode * -1521134295 + Pr.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(YPbPrHDTV left, YPbPrHDTV right) =>
            EqualityComparer<YPbPrHDTV>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(YPbPrHDTV left, YPbPrHDTV right) =>
            !(left == right);

        internal YPbPrHDTV(double y, double pb, double pr)
        {
            Y = y;
            Pb = pb;
            Pr = pr;
        }
    }
}
