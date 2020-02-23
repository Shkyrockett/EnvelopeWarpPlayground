// <copyright file="CubicEnvelope.cs">
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
    public struct CubicEnvelope
        : IEnvelope, IEquatable<CubicEnvelope>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicEnvelope"/> struct.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public CubicEnvelope(RectangleF rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubicEnvelope"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public CubicEnvelope(float x, float y, float width, float height)
        {
            var w3 = width * (1f / 3f);
            var h3 = height * (1f / 3f);

            //  Top Left
            ControlPointTopLeft = new CubicControlPoint
            {
                Point = new PointF(x, y),
                AnchorA = new PointF(w3, 0f),
                AnchorB = new PointF(0f, h3)
            };

            //  Top Right
            ControlPointTopRight = new CubicControlPoint
            {
                Point = new PointF(x + width, y),
                AnchorA = new PointF(-w3, 0f),
                AnchorB = new PointF(0f, h3)
            };

            //  Bottom Left
            ControlPointBottomLeft = new CubicControlPoint
            {
                Point = new PointF(x, y + height),
                AnchorA = new PointF(w3, 0f),
                AnchorB = new PointF(0f, -h3)
            };

            //  Bottom Right
            ControlPointBottomRight = new CubicControlPoint
            {
                Point = new PointF(x + width, y + height),
                AnchorA = new PointF(-w3, 0f),
                AnchorB = new PointF(0f, -h3)
            };

            //Update();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubicEnvelope"/> struct.
        /// </summary>
        /// <param name="controlPointTopLeft">The control point top left.</param>
        /// <param name="controlPointTopRight">The control point top right.</param>
        /// <param name="controlPointBottomLeft">The control point bottom left.</param>
        /// <param name="controlPointBottomRight">The control point bottom right.</param>
        public CubicEnvelope(CubicControlPoint controlPointTopLeft, CubicControlPoint controlPointTopRight, CubicControlPoint controlPointBottomLeft, CubicControlPoint controlPointBottomRight)
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
        public CubicControlPoint ControlPointTopLeft { get; set; }

        /// <summary>
        /// Gets or sets the control point top right.
        /// </summary>
        public CubicControlPoint ControlPointTopRight { get; set; }

        /// <summary>
        /// Gets or sets the control point bottom left.
        /// </summary>
        public CubicControlPoint ControlPointBottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the control point bottom right.
        /// </summary>
        public CubicControlPoint ControlPointBottomRight { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => 12;
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
                    0 => ControlPointTopLeft.Point,
                    1 => ControlPointTopLeft.AnchorAGlobal,
                    2 => ControlPointTopRight.AnchorAGlobal,
                    3 => ControlPointTopRight.Point,
                    4 => ControlPointTopRight.AnchorBGlobal,
                    5 => ControlPointBottomLeft.AnchorBGlobal,
                    6 => ControlPointBottomLeft.Point,
                    7 => ControlPointBottomLeft.AnchorAGlobal,
                    8 => ControlPointBottomRight.AnchorAGlobal,
                    9 => ControlPointBottomRight.Point,
                    10 => ControlPointBottomRight.AnchorBGlobal,
                    11 => ControlPointTopLeft.AnchorBGlobal,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        ControlPointTopLeft = new CubicControlPoint(value, ControlPointTopLeft.AnchorA, ControlPointTopLeft.AnchorB, false);
                        break;
                    case 1:
                        ControlPointTopLeft = new CubicControlPoint(ControlPointTopLeft.Point, value, ControlPointTopLeft.AnchorBGlobal, true);
                        break;
                    case 2:
                        ControlPointTopRight = new CubicControlPoint(ControlPointTopRight.Point, value, ControlPointTopRight.AnchorBGlobal, true);
                        break;
                    case 3:
                        ControlPointTopRight = new CubicControlPoint(value, ControlPointTopRight.AnchorA, ControlPointTopRight.AnchorB, false);
                        break;
                    case 4:
                        ControlPointTopRight = new CubicControlPoint(ControlPointTopRight.Point, ControlPointTopRight.AnchorAGlobal, value, true);
                        break;
                    case 5:
                        ControlPointBottomLeft = new CubicControlPoint(ControlPointBottomLeft.Point, ControlPointBottomLeft.AnchorAGlobal, value, true);
                        break;
                    case 6:
                        ControlPointBottomLeft = new CubicControlPoint(value, ControlPointBottomLeft.AnchorA, ControlPointBottomLeft.AnchorB, false);
                        break;
                    case 7:
                        ControlPointBottomLeft = new CubicControlPoint(ControlPointBottomLeft.Point, value, ControlPointBottomLeft.AnchorBGlobal, true);
                        break;
                    case 8:
                        ControlPointBottomRight = new CubicControlPoint(ControlPointBottomRight.Point, value, ControlPointBottomRight.AnchorBGlobal, true);
                        break;
                    case 9:
                        ControlPointBottomRight = new CubicControlPoint(value, ControlPointBottomRight.AnchorA, ControlPointBottomRight.AnchorB, false);
                        break;
                    case 10:
                        ControlPointBottomRight = new CubicControlPoint(ControlPointBottomRight.Point, ControlPointBottomRight.AnchorAGlobal, value, true);
                        break;
                    case 11:
                        ControlPointTopLeft = new CubicControlPoint(ControlPointTopLeft.Point, ControlPointTopLeft.AnchorAGlobal, value, true);
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
            yield return ControlPointTopLeft.Point;
            yield return ControlPointTopLeft.AnchorAGlobal;
            yield return ControlPointTopRight.AnchorAGlobal;
            yield return ControlPointTopRight.Point;
            yield return ControlPointTopRight.AnchorBGlobal;
            yield return ControlPointBottomLeft.AnchorBGlobal;
            yield return ControlPointBottomLeft.Point;
            yield return ControlPointBottomLeft.AnchorAGlobal;
            yield return ControlPointBottomRight.AnchorAGlobal;
            yield return ControlPointBottomRight.Point;
            yield return ControlPointBottomRight.AnchorBGlobal;
            yield return ControlPointTopLeft.AnchorBGlobal;
        }
        #endregion Enumeration

        #region Operators
        /// <summary>
        /// The operator ==.
        /// </summary>
        /// <param name="left">The <paramref name="left"/>.</param>
        /// <param name="right">The <paramref name="right"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool operator ==(CubicEnvelope left, CubicEnvelope right) => left.Equals(right);

        /// <summary>
        /// The operator !=.
        /// </summary>
        /// <param name="left">The <paramref name="left"/>.</param>
        /// <param name="right">The <paramref name="right"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool operator !=(CubicEnvelope left, CubicEnvelope right) => !(left == right);
        #endregion Operators

        #region Methods
        /// <summary>
        /// Processes the point.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointF ProcessPoint(RectangleF bounds, PointF point) => Mathematics.CubicBezierEnvelopeOptimized(
            point,
            bounds,
            ControlPointTopLeft.Point, ControlPointTopLeft.AnchorAGlobal, ControlPointTopLeft.AnchorBGlobal,
            ControlPointTopRight.Point, ControlPointTopRight.AnchorAGlobal, ControlPointTopRight.AnchorBGlobal,
            ControlPointBottomRight.Point, ControlPointBottomRight.AnchorAGlobal, ControlPointBottomRight.AnchorBGlobal,
            ControlPointBottomLeft.Point, ControlPointBottomLeft.AnchorAGlobal, ControlPointBottomLeft.AnchorBGlobal
        );

        /// <summary>
        /// The to GraphicsPath.
        /// </summary>
        /// <returns>The <see cref="GraphicsPath"/>.</returns>
        public GraphicsPath ToGraphicsPath()
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddBezier(ControlPointTopLeft.Point, ControlPointTopLeft.AnchorAGlobal, ControlPointTopRight.AnchorAGlobal, ControlPointTopRight.Point);
            path.AddBezier(ControlPointTopRight.Point, ControlPointTopRight.AnchorBGlobal, ControlPointBottomRight.AnchorBGlobal, ControlPointBottomRight.Point);
            path.AddBezier(ControlPointBottomRight.Point, ControlPointBottomRight.AnchorAGlobal, ControlPointBottomLeft.AnchorAGlobal, ControlPointBottomLeft.Point);
            path.AddBezier(ControlPointBottomLeft.Point, ControlPointBottomLeft.AnchorBGlobal, ControlPointTopLeft.AnchorBGlobal, ControlPointTopLeft.Point);
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
        public override bool Equals(object obj) => obj is CubicEnvelope && Equals((CubicEnvelope)obj);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="envelope">The <paramref name="envelope"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Equals(CubicEnvelope envelope) => ControlPointTopLeft.Equals(envelope.ControlPointTopLeft) && ControlPointTopRight.Equals(envelope.ControlPointTopRight) && ControlPointBottomLeft.Equals(envelope.ControlPointBottomLeft) && ControlPointBottomRight.Equals(envelope.ControlPointBottomRight);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Equals(CubicEnvelope a, CubicEnvelope b) => a.Equals(b);

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Compare(CubicEnvelope a, CubicEnvelope b) => a.Equals(b);

        /// <summary>
        /// Creates a human-readable string that represents this <see cref="CubicEnvelope"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(null /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="CubicEnvelope"/> struct based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="provider">The <paramref name="provider"/>.</param>
        /// <returns>A <see cref="string"/> representation of this object.</returns>
        public string ToString(IFormatProvider provider) => ToString(null /* format string */, provider);

        /// <summary>
        /// Creates a <see cref="string"/> representation of this <see cref="CubicEnvelope"/> class based on the format string
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
            return $"{nameof(CubicEnvelope)}{{{nameof(ControlPointTopLeft)}={ControlPointTopLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointTopRight)}={ControlPointTopRight.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointBottomLeft)}={ControlPointBottomLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointBottomRight)}={ControlPointBottomRight.ToString(format, provider)}}}";
        }
        #endregion Methods
    }
}
