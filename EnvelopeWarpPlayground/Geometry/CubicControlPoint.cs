// <copyright file="CubicControlPoint.cs">
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

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// The control point class.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public struct CubicControlPoint
        : IEquatable<CubicControlPoint>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicControlPoint" /> class.
        /// </summary>
        /// <param name="point">The <paramref name="point" />.</param>
        /// <param name="anchorA">The <paramref name="anchorA" />.</param>
        /// <param name="anchorB">The <paramref name="anchorB" />.</param>
        /// <param name="global">if set to <see langword="true" /> [global].</param>
        public CubicControlPoint(PointF point, PointF anchorA, PointF anchorB, bool global = false)
        {
            Point = point;
            if (global)
            {
                AnchorA = GlobalToLocal(anchorA, Point);
                AnchorB = GlobalToLocal(anchorB, Point);
            }
            else
            {
                AnchorA = anchorA;
                AnchorB = anchorB;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubicControlPoint" /> class.
        /// </summary>
        /// <param name="point">The <paramref name="point" />.</param>
        public CubicControlPoint(PointF point)
        {
            (Point, AnchorA, AnchorB) = (point, point, point);
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the point.
        /// </summary>
        /// <value>
        /// The point.
        /// </value>
        public PointF Point { get; set; }

        /// <summary>
        /// Gets or sets the horizontal anchor.
        /// </summary>
        /// <value>
        /// The anchor a.
        /// </value>
        public PointF AnchorA { get; set; }

        /// <summary>
        /// Gets or sets the vertical anchor.
        /// </summary>
        /// <value>
        /// The anchor b.
        /// </value>
        public PointF AnchorB { get; set; }

        /// <summary>
        /// Gets or sets the global horizontal anchor.
        /// </summary>
        /// <value>
        /// The anchor a global.
        /// </value>
        public PointF AnchorAGlobal { get { return LocalToGlobal(AnchorA, Point); } set { AnchorA = GlobalToLocal(value, Point); } }

        /// <summary>
        /// Gets or sets the global vertical anchor.
        /// </summary>
        /// <value>
        /// The anchor b global.
        /// </value>
        public PointF AnchorBGlobal { get { return LocalToGlobal(AnchorB, Point); } set { AnchorB = GlobalToLocal(value, Point); } }
        #endregion Properties

        #region Enumeration
        /// <summary>
        /// Gets or sets the <see cref="PointF" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PointF" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public PointF this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Point,
                    1 => AnchorAGlobal,
                    2 => AnchorBGlobal,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Point = value;
                        break;
                    case 1:
                        AnchorAGlobal = value;
                        break;
                    case 2:
                        AnchorBGlobal = value;
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
        /// The <see cref="T:IEnumerator{PointF}" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerator<PointF> GetEnumerator()
        {
            yield return Point;
            yield return AnchorAGlobal;
            yield return AnchorBGlobal;
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
        public static bool operator ==(CubicControlPoint left, CubicControlPoint right) => left.Equals(right);

        /// <summary>
        /// The operator !=.
        /// </summary>
        /// <param name="left">The <paramref name="left" />.</param>
        /// <param name="right">The <paramref name="right" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool operator !=(CubicControlPoint left, CubicControlPoint right) => !(left == right);
        #endregion Operators

        #region Methods
        /// <summary>
        /// The local to global method.
        /// </summary>
        /// <param name="point">The <paramref name="point" />.</param>
        /// <param name="reference">The <paramref name="reference" />.</param>
        /// <returns>
        /// The <see cref="PointF" />.
        /// </returns>
        private static PointF LocalToGlobal(PointF point, PointF reference) => new PointF(point.X + reference.X, point.Y + reference.Y);

        /// <summary>
        /// The global to local method.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="reference">The <paramref name="reference" />.</param>
        /// <returns>
        /// The <see cref="PointF" />.
        /// </returns>
        private static PointF GlobalToLocal(PointF point, PointF reference) => new PointF(point.X - reference.X, point.Y - reference.Y);

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public override int GetHashCode() => HashCode.Combine(Point, AnchorA, AnchorB);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">The <paramref name="obj" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object obj) => obj is CubicControlPoint point && Equals(point);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="value">The <paramref name="value" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool Equals(CubicControlPoint value) => Point.Equals(value.Point) && AnchorA.Equals(value.AnchorA) && AnchorB.Equals(value.AnchorB);

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Equals(CubicControlPoint a, CubicControlPoint b) => a.Equals(b);

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="a">The <paramref name="a" />.</param>
        /// <param name="b">The <paramref name="b" />.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool Compare(CubicControlPoint a, CubicControlPoint b) => a.Equals(b);

        /// <summary>
        /// Creates a human-readable <see cref="string" /> that represents this <see cref="CubicControlPoint" />.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public override string ToString() => ToString(null /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a <see cref="string" /> representation of this <see cref="CubicControlPoint" /> struct based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="provider">The <paramref name="provider" />.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string ToString(IFormatProvider provider) => ToString(null /* format string */, provider);

        /// <summary>
        /// Creates a <see cref="string" /> representation of this <see cref="CubicControlPoint" /> class based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <param name="format">The <paramref name="format" />.</param>
        /// <param name="provider">The <paramref name="provider" />.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string ToString(string format, IFormatProvider provider)
        {
            var sep = ',';
            return $"{nameof(CubicControlPoint)}{{{nameof(Point)}={Point.ToString(format, provider)}{sep}{nameof(AnchorA)}={AnchorA.ToString(format, provider)}{sep}{nameof(AnchorB)}={AnchorB.ToString(format, provider)}}}";
        }

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns></returns>
        private string GetDebuggerDisplay() => ToString();
        #endregion Methods
    }
}
