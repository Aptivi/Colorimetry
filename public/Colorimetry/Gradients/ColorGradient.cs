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

using System.Diagnostics;

namespace Colorimetry.Gradients
{
    /// <summary>
    /// A color gradient enumeration class
    /// </summary>
    [DebuggerDisplay("[{Step}] {IntermediateColor}")]
    public class ColorGradient
    {
        /// <summary>
        /// The step number for this intermediate color
        /// </summary>
        public int Step { get; private set; }
        /// <summary>
        /// The intermediate color that transitions from the start to the finish
        /// </summary>
        public Color IntermediateColor { get; private set; }

        /// <summary>
        /// Gets a color string of this gradient
        /// </summary>
        public override string ToString() =>
            IntermediateColor.ToString();

        internal ColorGradient(int step, Color intermediateColor)
        {
            Step = step;
            IntermediateColor = intermediateColor ??
                throw new ColorException(nameof(intermediateColor));
        }
    }
}
