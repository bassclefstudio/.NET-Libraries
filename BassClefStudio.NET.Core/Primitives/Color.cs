using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Core.Primitives
{
    /// <summary>
    /// Represents a color in RGBA space.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// The red <see cref="byte"/>.
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// The green <see cref="byte"/>.
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// The blue <see cref="byte"/>.
        /// </summary>
        public byte B { get; }

        /// <summary>
        /// The alpha (transparency) <see cref="byte"/>.
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// Creates a new <see cref="Color"/> value.
        /// </summary>
        /// <param name="r">The red <see cref="byte"/>.</param>
        /// <param name="g">The green <see cref="byte"/>.</param>
        /// <param name="b">The blue <see cref="byte"/>.</param>
        /// <param name="a">The alpha (transparency) <see cref="byte"/>. Defaults to 255 (opaque).</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
