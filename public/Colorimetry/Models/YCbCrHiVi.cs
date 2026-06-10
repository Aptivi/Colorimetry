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
    /// The YCbCr class instance
    /// </summary>
    [DebuggerDisplay("YCbCrHiVi = {Y};{Cb};{Cr}")]
    public class YCbCrHiVi : BaseColorModel, IEquatable<YCbCrHiVi>
    {
        /// <summary>
        /// The Y value [16 -> 235]
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// The Cb value [16 -> 240]
        /// </summary>
        public double Cb { get; private set; }
        /// <summary>
        /// The Cr value [16 -> 240]
        /// </summary>
        public double Cr { get; private set; }

        /// <summary>
        /// ycbcrhivi:&lt;Y&gt;;&lt;Cb&gt;;&lt;Cr&gt;
        /// </summary>
        public override string ToString() =>
            $"ycbcrhivi:{Y:0.##};{Cb:0.##};{Cr:0.##}";

        /// <summary>
        /// Does the string specifier represent a valid YCbCr specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid YCbCr specifier</param>
        /// <param name="checkParts">Whether to check the parts count or not</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierValid(string specifier, bool checkParts = false) =>
            specifier.Contains(";") &&
            specifier.StartsWith("ycbcrhivi:") &&
            (!checkParts || (checkParts && specifier.Substring(10).Split(';').Length == 3));

        /// <summary>
        /// Does the string specifier represent a valid YCbCr specifier?
        /// </summary>
        /// <param name="specifier">Specifier that represents a valid YCbCr specifier</param>
        /// <returns>True if the specifier is valid; false otherwise.</returns>
        public static new bool IsSpecifierAndValueValid(string specifier)
        {
            if (!IsSpecifierValid(specifier, true))
                return false;

            var specifierArray = specifier.Substring(10).Split(';');
            int y = Convert.ToInt32(specifierArray[0]);
            if (y < 16 || y > 235)
                return false;
            int cb = Convert.ToInt32(specifierArray[1]);
            if (cb < 16 || cb > 240)
                return false;
            int cr = Convert.ToInt32(specifierArray[2]);
            if (cr < 16 || cr > 240)
                return false;
            return true;
        }

        /// <summary>
        /// Parses the specifier and returns an instance of <see cref="YCbCrHiVi"/> converted to <see cref="RedGreenBlue"/>
        /// </summary>
        /// <param name="specifier">Specifier of RGB</param>
        /// <param name="settings">Settings to use. Use null for global settings</param>
        /// <returns>An instance of <see cref="RedGreenBlue"/></returns>
        /// <exception cref="ColorException"></exception>
        public static new RedGreenBlue ParseSpecifierToRgb(string specifier, ColorSettings? settings = null)
        {
            var YCbCr = ParseSpecifier(specifier);
            var rgb = ConversionTools.ToRgb(YCbCr);
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
        /// Parses the specifier and returns an instance of <see cref="YCbCrHiVi"/>
        /// </summary>
        /// <param name="specifier">Specifier of YCbCr</param>
        /// <returns>An instance of <see cref="YCbCrHiVi"/></returns>
        /// <exception cref="ColorException"></exception>
        public static YCbCrHiVi ParseSpecifier(string specifier)
        {
            if (!IsSpecifierValid(specifier))
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYCBCRSPECIFIER").FormatString(specifier) + ": ycbcrhivi:<y>;<cb>;<cr>");

            // Split the VT sequence into three parts
            var specifierArray = specifier.Substring(10).Split(';');
            if (specifierArray.Length == 3)
            {
                // We got the YCbCr whole values! First, check to see if we need to filter the color for the color-blind
                double y = Convert.ToDouble(specifierArray[0]);
                if (y < 16 || y > 235)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYCBCRYLEVEL") + $" {y}");
                double cb = Convert.ToDouble(specifierArray[1]);
                if (cb < 16 || cb > 240)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYCBCRCBLEVEL") + $" {cb}");
                double cr = Convert.ToDouble(specifierArray[2]);
                if (cr < 16 || cr > 240)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEYCBCRCRLEVEL") + $" {cr}");

                // First, we need to convert from YCbCr to RGB
                var YCbCr = new YCbCrHiVi(y, cb, cr);
                return YCbCr;
            }
            else
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_MODEL_EXCEPTION_PARSEINVALIDYCBCRSPECIFIEREXCEED").FormatString(specifier) + ": ycbcrhivi:<y>;<cb>;<cr>");
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            Equals((YCbCrHiVi)obj);

        /// <inheritdoc/>
        public bool Equals(YCbCrHiVi other) =>
            other is not null &&
            Y == other.Y &&
            Cb == other.Cb &&
            Cr == other.Cr;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -882565174;
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Cb.GetHashCode();
            hashCode = hashCode * -1521134295 + Cr.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(YCbCrHiVi left, YCbCrHiVi right) =>
            EqualityComparer<YCbCrHiVi>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(YCbCrHiVi left, YCbCrHiVi right) =>
            !(left == right);

        internal YCbCrHiVi(double y, double cb, double pr)
        {
            Y = y;
            Cb = cb;
            Cr = pr;
        }
    }
}
