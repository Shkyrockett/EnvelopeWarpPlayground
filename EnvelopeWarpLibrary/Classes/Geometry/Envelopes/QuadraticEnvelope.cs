// <copyright file="QuadraticEnvelope.cs">
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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace EnvelopeWarpLibrary
{
    /// <summary>
    /// The envelope distort class.
    /// </summary>
    /// <seealso cref="IEnvelope" />
    /// <seealso cref="IEquatable{T}" />
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public struct QuadraticEnvelope
        : IEnvelope, IEquatable<QuadraticEnvelope>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticEnvelope" /> struct.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        public QuadraticEnvelope(RectangleF rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticEnvelope" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public QuadraticEnvelope(float x, float y, float width, float height)
        {
            var w2 = width * (1f / 2f);
            var h2 = height * (1f / 2f);

            ControlPointTopLeft = new PointF(x, y);
            ControlHandleTop = new PointF(x + w2, y);
            ControlPointTopRight = new PointF(x + width, y);
            ControlHandleRight = new PointF(x + width, y + h2);
            ControlPointBottomLeft = new PointF(x, y + height);
            ControlHandleLeft = new PointF(x, y + h2);
            ControlPointBottomRight = new PointF(x + width, y + height);
            ControlHandleBottom = new PointF(x + w2, y + height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraticEnvelope" /> struct.
        /// </summary>
        /// <param name="controlPointTopLeft">The control point top left.</param>
        /// <param name="controlHandleTop">The control handle top.</param>
        /// <param name="controlPointTopRight">The control point top right.</param>
        /// <param name="controlHandleRight">The control handle right.</param>
        /// <param name="controlPointBottomLeft">The control point bottom left.</param>
        /// <param name="controlHandleLeft">The control handle left.</param>
        /// <param name="controlPointBottomRight">The control point bottom right.</param>
        /// <param name="controlHandleBottom">The control handle bottom.</param>
        public QuadraticEnvelope(PointF controlPointTopLeft, PointF controlHandleTop, PointF controlPointTopRight, PointF controlHandleRight, PointF controlPointBottomLeft, PointF controlHandleLeft, PointF controlPointBottomRight, PointF controlHandleBottom)
        {
            (ControlPointTopLeft, ControlHandleTop, ControlPointTopRight, ControlHandleRight, ControlPointBottomLeft, ControlHandleLeft, ControlPointBottomRight, ControlHandleBottom) = (controlPointTopLeft, controlHandleTop, controlPointTopRight, controlHandleRight, controlPointBottomLeft, controlHandleLeft, controlPointBottomRight, controlHandleBottom);
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the control point top left.
        /// </summary>
        /// <value>
        /// The control point top left.
        /// </value>
        public PointF ControlPointTopLeft { get; set; }

        /// <summary>
        /// Gets or sets the control handle top.
        /// </summary>
        /// <value>
        /// The control handle top.
        /// </value>
        public PointF ControlHandleTop { get; set; }

        /// <summary>
        /// Gets or sets the control point top right.
        /// </summary>
        /// <value>
        /// The control point top right.
        /// </value>
        public PointF ControlPointTopRight { get; set; }

        /// <summary>
        /// Gets or sets the control handle right.
        /// </summary>
        /// <value>
        /// The control handle right.
        /// </value>
        public PointF ControlHandleRight { get; set; }

        /// <summary>
        /// Gets or sets the control point bottom left.
        /// </summary>
        /// <value>
        /// The control point bottom left.
        /// </value>
        public PointF ControlPointBottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the control handle left.
        /// </summary>
        /// <value>
        /// The control handle left.
        /// </value>
        public PointF ControlHandleLeft { get; set; }

        /// <summary>
        /// Gets or sets the control point bottom right.
        /// </summary>
        /// <value>
        /// The control point bottom right.
        /// </value>
        public PointF ControlPointBottomRight { get; set; }

        /// <summary>
        /// Gets or sets the control handle bottom.
        /// </summary>
        /// <value>
        /// The control handle bottom.
        /// </value>
        public PointF ControlHandleBottom { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => 8;
        #endregion Properties

        #region Enumeration
        /// <summary>
        /// The Indexer.
        /// </summary>
        /// <value>
        /// The <see cref="PointF"/>.
        /// </value>
        /// <param name="index">The <paramref name="index" /> index.</param>
        /// <returns>
        /// One element of type PointF?.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public PointF this[int index]
        {
            get
            {
                return index switch
                {
                    0 => ControlPointTopLeft,
                    1 => ControlHandleTop,
                    2 => ControlPointTopRight,
                    3 => ControlHandleRight,
                    4 => ControlPointBottomLeft,
                    5 => ControlHandleLeft,
                    6 => ControlPointBottomRight,
                    7 => ControlHandleBottom,
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
                        ControlHandleTop = value;
                        break;
                    case 2:
                        ControlPointTopRight = value;
                        break;
                    case 3:
                        ControlHandleRight = value;
                        break;
                    case 4:
                        ControlPointBottomLeft = value;
                        break;
                    case 5:
                        ControlHandleLeft = value;
                        break;
                    case 6:
                        ControlPointBottomRight = value;
                        break;
                    case 7:
                        ControlHandleBottom = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PointF" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PointF" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PointF this[Index index] { get { return this[index.Value]; } set { this[index.Value] = value; } }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="T:IEnumerator{CubicControlPoint}" />.
        /// </returns>
        public IEnumerator<PointF> GetEnumerator()
        {
            yield return ControlPointTopLeft;
            yield return ControlHandleTop;
            yield return ControlPointTopRight;
            yield return ControlHandleRight;
            yield return ControlPointBottomLeft;
            yield return ControlHandleLeft;
            yield return ControlPointBottomRight;
            yield return ControlHandleBottom;
        }
        #endregion Enumeration

        #region Operators
        /// <summary>
        /// The operator ==.
        /// </summary>
        /// <param name="left">The <paramref name="left" />.</param>
        /// <param name="right">The <paramref name="right" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool operator ==(QuadraticEnvelope left, QuadraticEnvelope right) => left.Equals(right);

        /// <summary>
        /// The operator !=.
        /// </summary>
        /// <param name="left">The <paramref name="left" />.</param>
        /// <param name="right">The <paramref name="right" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool operator !=(QuadraticEnvelope left, QuadraticEnvelope right) => !(left == right);
        #endregion Operators

        #region Methods
        /// <summary>
        /// Processes the point.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PointF ProcessPoint(RectangleF bounds, PointF point) => Mathematics.QuadraticBezierEnvelopeOptimized(
            point,
            bounds,
            ControlPointTopLeft, ControlHandleTop, ControlPointTopRight, ControlHandleRight,
            ControlPointBottomRight, ControlHandleBottom, ControlPointBottomLeft, ControlHandleLeft
        );

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Clear() => throw new NotImplementedException();

        /// <summary>
        /// Removes the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Remove(PointF point) => throw new NotImplementedException();

        /// <summary>
        /// Translates the specified delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IGeometry Translate(Vector2 delta) => throw new NotImplementedException();

        /// <summary>
        /// Queries whether the shape includes the specified point in it's geometry.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Includes(PointF point) => throw new NotImplementedException();

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public override int GetHashCode() => HashCode.Combine(ControlPointTopLeft, ControlHandleTop, ControlPointTopRight, ControlHandleRight, ControlPointBottomLeft, ControlHandleLeft, ControlPointBottomRight, ControlHandleBottom);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">The <paramref name="obj" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object? obj) => obj is QuadraticEnvelope envelope && Equals(envelope);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="envelope">The <paramref name="envelope" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool Equals(QuadraticEnvelope envelope) =>
            ControlPointTopLeft.Equals(envelope.ControlPointTopLeft)
            && ControlHandleTop.Equals(envelope.ControlHandleTop)
            && ControlPointTopRight.Equals(envelope.ControlPointTopRight)
            && ControlHandleRight.Equals(envelope.ControlHandleRight)
            && ControlPointBottomLeft.Equals(envelope.ControlPointBottomLeft)
            && ControlHandleLeft.Equals(envelope.ControlHandleLeft)
            && ControlPointBottomRight.Equals(envelope.ControlPointBottomRight)
            && ControlHandleBottom.Equals(envelope.ControlHandleBottom);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Equals(QuadraticEnvelope a, QuadraticEnvelope b) => a.Equals(b);

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Compare(QuadraticEnvelope a, QuadraticEnvelope b) => a.Equals(b);

        /// <summary>
        /// Creates a human-readable string that represents this <see cref="QuadraticEnvelope" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => ToString(null /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a <see cref="string" /> representation of this <see cref="QuadraticEnvelope" /> struct based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="provider">The <paramref name="provider" />.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string ToString(IFormatProvider provider) => ToString(null /* format string */, provider);

        /// <summary>
        /// Creates a <see cref="string" /> representation of this <see cref="QuadraticEnvelope" /> class based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string ToString(string? format, IFormatProvider provider)
        {
            var sep = ',';
            return $"{nameof(QuadraticEnvelope)}{{{nameof(ControlPointTopLeft)}={ControlPointTopLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlHandleTop)}={ControlHandleTop.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointTopRight)}={ControlPointTopRight.ToString(format, provider)}" +
                $"{sep}{nameof(ControlHandleRight)}={ControlHandleRight.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointBottomLeft)}={ControlPointBottomLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlHandleLeft)}={ControlHandleLeft.ToString(format, provider)}" +
                $"{sep}{nameof(ControlPointBottomRight)}={ControlPointBottomRight.ToString(format, provider)}" +
                $"{sep}{nameof(ControlHandleBottom)}={ControlHandleBottom.ToString(format, provider)}}}";
        }

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns></returns>
        private string GetDebuggerDisplay() => ToString();
        #endregion Methods
    }
}
