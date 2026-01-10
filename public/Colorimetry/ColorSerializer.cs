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

using Newtonsoft.Json;
using System;
using Colorimetry.Languages;

namespace Colorimetry
{
    /// <summary>
    /// Color serializer
    /// </summary>
    public class ColorSerializer : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(Color);

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            Color color;
            if (reader.Value is string colorValueString)
                color = new Color(colorValueString);
            else if (reader.Value is long colorValueLong)
                color = new Color((int)colorValueLong);
            else
            {
                if (reader.ValueType?.Name is not null)
                    throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_SERIALIZER_EXCEPTION_READERROR1"), reader.TokenType, reader.ValueType?.Name ?? "");
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_SERIALIZER_EXCEPTION_READERROR2"), reader.TokenType);
            }
            return color;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var color = value as Color ??
                throw new ColorException(LanguageTools.GetLocalized("COLORIMETRY_SERIALIZER_EXCEPTION_WRITEERROR"));
            serializer.Serialize(writer, color.ToString());
        }
    }
}
