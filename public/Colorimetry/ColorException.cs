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
using Colorimetry.Languages;
using Textify.General;

namespace Colorimetry
{
    /// <summary>
    /// Color exception
    /// </summary>
    public class ColorException : Exception
    {
        /// <summary>
        /// Makes an empty <see cref="ColorException"/> exception instance with the default message
        /// </summary>
        public ColorException() :
            base(LanguageTools.GetLocalized("COLORIMETRY_EXCEPTION_UNKNOWNERROR1"))
        { }

        /// <summary>
        /// Makes an empty <see cref="ColorException"/> exception instance with the default message
        /// </summary>
        /// <param name="innerException">An inner exception to specify</param>
        public ColorException(Exception innerException) :
            base(LanguageTools.GetLocalized("COLORIMETRY_EXCEPTION_UNKNOWNERROR1"), innerException)
        { }

        /// <summary>
        /// Makes a <see cref="ColorException"/> exception instance
        /// </summary>
        /// <param name="message">A message to specify</param>
        public ColorException(string message) :
            base(message)
        { }

        /// <summary>
        /// Makes a <see cref="ColorException"/> exception instance
        /// </summary>
        /// <param name="message">A message to specify</param>
        /// <param name="vars">List of variables to format the message with</param>
        public ColorException(string message, params object[] vars) :
            base(TextTools.FormatString(message, vars))
        { }

        /// <summary>
        /// Makes a <see cref="ColorException"/> exception instance
        /// </summary>
        /// <param name="message">A message to specify</param>
        /// <param name="innerException">An inner exception to specify</param>
        public ColorException(string message, Exception innerException) :
            base(message, innerException)
        { }

        /// <summary>
        /// Makes a <see cref="ColorException"/> exception instance
        /// </summary>
        /// <param name="message">A message to specify</param>
        /// <param name="innerException">An inner exception to specify</param>
        /// <param name="vars">List of variables to format the message with</param>
        public ColorException(string message, Exception innerException, params object[] vars) :
            base(TextTools.FormatString(message, vars), innerException)
        { }
    }
}
