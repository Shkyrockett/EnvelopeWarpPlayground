// <copyright file="LinearEnvelope.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// The envelope distort class.
    /// </summary>
    /// <seealso cref="IEnvelope" />
    /// <seealso cref="IEquatable{T}" />
    public struct LinearEnvelope
        : IEnvelope, IEquatable<LinearEnvelope>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearEnvelope"/> struct.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public LinearEnvelope(RectangleF rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearEnvelope"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public LinearEnvelope(float x, float y, float width, float height)
        {
            ControlPointTopLeft = new PointF(x, y);
            ControlPointTopRight = new PointF(x + width, y);
            ControlPointBottomLeft = new PointF(x, y + height);
            ControlPointBottomRight = new PointF(x + width, y + height);

            //Update();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearEnvelope"/> struct.
        /// </summary>
        /// <param name="controlPointTopLeft">The control point top left.</param>
        /// <param name="controlPointTopRight">The control point top right.</param>
        /// <param name="controlPointBottomLeft">The control point bottom left.</param>
        /// <param name="controlPointBottomRight">The control point bottom right.</param>
        public LinearEnvelope(PointF controlPointTopLeft, PointF controlPointTopRight, PointF controlPointBottomLeft, PointF controlPointBottomRight)
        {
            ControlPointTopLeft = controlPointTopLeft;
            ControlPointTopRight = controlPointTopRight;
            ControlPointBottomLeft = controlPointBottomLeft;
            ControlPointBottomRight = controlPointBottomRight;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the control point top left.
        /// </summary>
        public PointF ControlPointTopLeft { get; set; }

        /// <summary>
        /// Gets or sets the control point top right.
        /// </summary>
        public PointF ControlPointTopRight { get; set; }

        /// <summary>
        /// Gets or sets the control point bottom left.
        /// </summary>
        public PointF ControlPointBottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the control point bottom right.
        /// </summary>
        public PointF ControlPointBottomRight { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => 4;
        #endregion Properties

        #region Enumeration
        /// <summary>
        /// The Indexer.
        /// </summary>
        /// <param name="index">The <paramref name="index"/> index.</param>
        /// <returns>One element of type PointF?.</returns>
        public PointF this[int index]
        {
            get
            {
                return index switch
                {
                    0 => ControlPointTopLeft,
                    1 => ControlPointTopRight,
                    2 => ControlPointBottomLeft,
                    3 => ControlPointBottomRight,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        ControlPointTopLeft = value;
                        break;
                    case 1:
                        ControlPointTopRight = value;
                        break;
                    case 2:
                        ControlPointBottomLeft = value;
                        break;
                    case 3:
                        ControlPointBottomRight = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PointF"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PointF"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PointF this[Index index] { get { return this[index.Value]; } set { this[index.Value] = value; } }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The <see cref="T:IEnumerator{CubicControlPoint}"/>.</returns>
        public IEnumerator<PointF> GetEnumerator()
        {
            yield return ControlPointTopLeft;
            yield return ControlPointTopRight;
            yield return ControlPointBottomLeft;
            yield return ControlPointBottomRight;
        }
        #endregion Enumeration

        #region Operators
        /// <summary>
        /// The operator ==.
        /// </summary>
        /// <param name="left">The <paramref name="left"/>.</param>
        /// <param name="right">The <paramref name="right"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool operator ==(LinearEnvelope left, LinearEnvelope right) => left.Equals(right);

        /// <summary>
        /// The operator !=.
        /// </summary>
        /// <param name="left">The <paramref name="left"/>.</param>
        /// <param name="right">The <paramref name="right"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool operator !=(LinearEnvelope left, LinearEnvelope right) => !(left == right);
        #endregion Operators

        #region Methods
        /// <summary>
        /// Processes the point.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointF ProcessPoint(RectangleF bounds, PointF point) => Mathematics.LinearEnvelopeOptimized(
            point,
            bounds,
            ControlPointTopLeft, ControlPointTopRight, ControlPointBottomRight, ControlPointBottomLeft
        );

        /// <summary>
        /// The to GraphicsPath.
        /// </summary>
        /// <returns>The <see cref="GraphicsPath"/>.</returns>
        public GraphicsPath ToGraphicsPath()
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddLine(ControlPointTopLeft, ControlPointTopRight);
            path.AddLine(ControlPointTopRight, ControlPointBottomRight);
            path.AddLine(ControlPointBottomRight, ControlPointBottomLeft);
            path.AddLine(ControlPointBottomLeft, ControlPointTopLeft);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override int GetHashCode() => HashCode.Combine(ControlPointTopLeft, ControlPointTopRight, ControlPointBottomLeft, ControlPointBottomRight);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">The <paramref name="obj"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Equals(object obj) => obj is LinearEnvelope envelope && Equals(envelope);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="envelope">The <paramref name="envelope"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Equals(LinearEnvelope envelope) =>
            ControlPointTopLeft.Equals(envelope.ControlPointTopLeft)
            && ControlPointTopRight.Equals(envelope.ControlPointTopRight)
            && ControlPointBottomLeft.Equals(envelope.ControlPointBottomLeft)
            && ControlPointBottomRight.Equals(envelope.ControlPointBottomRight);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Equals(LinearEnvelope a, LinearEnvelope b) => a.Equals(b);

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Compare(LinearEnvelope a, LinearEnvelope b) => a.Equals(b);

        /// <summary>
        /// Creates a human-readable string that represents this <see cref="LinearEnvelope"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(null /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="LinearEnvelope"/> struct based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="provider">The <paramref name="provider"/>.</param>
        /// <returns>A <see cref="string"/> representation of this object.</returns>
        public string ToString(IFormatProvider provider) => ToString(null /* format string */, provider);

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="LinearEnvelope"/> class based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>A <see cref="string"/> representation of this object.</returns>
        public string ToString(string format, IFormatProvider provider)
        {
            var sep = ',';
            return $"{nameof(LinearEnvelope)}{{{nameof(ControlPointTopLeft)}={ControlPointTopLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointTopRight)}={ControlPointTopRight.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointBottomLeft)}={ControlPointBottomLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointBottomRight)}={ControlPointBottomRight.ToString(format, provider)}}}";
        }
        #endregion Methods
    }
}
