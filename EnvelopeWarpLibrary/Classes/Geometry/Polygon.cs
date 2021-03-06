// <copyright file="Polygon.cs">
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
using System.Linq;
using System.Numerics;

namespace EnvelopeWarpLibrary
{
    /// <summary>
    /// The polygon class.
    /// </summary>
    /// <seealso cref="EnvelopeWarpLibrary.IGeometry{T}" />
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Polygon
        : IGeometry<PolygonContour>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon()
            : this(new List<PolygonContour>())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="contours">The contours.</param>
        public Polygon(params PolygonContour[] contours)
            : this(contours.ToList())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="contours">The contours.</param>
        public Polygon(IEnumerable<PolygonContour> contours)
        {
            Contours = contours as List<PolygonContour> ?? new List<PolygonContour>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the contours.
        /// </summary>
        /// <value>
        /// The contours.
        /// </value>
        public List<PolygonContour> Contours { get; set; }

        /// <summary>
        /// Gets the number of contours.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => Contours.Count;
        #endregion Properties

        #region Indexers
        /// <summary>
        /// Gets or sets the <see cref="PolygonContour" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PolygonContour" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PolygonContour this[int index] { get { return Contours[index]; } set { Contours[index] = value; } }

        /// <summary>
        /// Gets or sets the <see cref="PolygonContour" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PolygonContour" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PolygonContour this[Index index] { get { return Contours[index]; } set { Contours[index] = value; } }

        /// <summary>
        /// Gets the <see cref="Span{PolygonContour}" /> with the specified range.
        /// </summary>
        /// <value>
        /// The <see cref="Span{PolygonContour}" />.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public Span<PolygonContour> this[Range range] => Contours.ToArray()[range];
        #endregion

        #region Mutators
        /// <summary>
        /// Adds the specified contour.
        /// </summary>
        /// <param name="contour">The contour.</param>
        public void Add(PolygonContour contour)
        {
            Contours.Add(contour);
        }

        /// <summary>
        /// Inserts the specified contour.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="contour">The contour.</param>
        public void Insert(int index, PolygonContour contour) => Contours.Insert(index, contour);

        /// <summary>
        /// Removes the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void Remove(PointF point)
        {
            foreach (var item in Contours)
            {
                if (item.Includes(point))
                {
                    item.Remove(point);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the specified contour.
        /// </summary>
        /// <param name="contour">The contour.</param>
        public void Remove(PolygonContour contour)
        {
            foreach (var item in Contours)
            {
                if (item == contour)
                {
                    Contours.Remove(item);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes an item at a specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index) => Contours.RemoveAt(index);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            foreach (var contour in Contours)
            {
                contour.Clear();
            }

            Contours.Clear();
        }

        /// <summary>
        /// Reverse the Contours.
        /// </summary>
        /// <returns>
        /// The <see cref="PolygonContour" />.
        /// </returns>
        public Polygon Reverse()
        {
            Contours.Reverse();
            return this;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Translates the specified delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        public IGeometry Translate(Vector2 delta) => Translate(this, delta);

        /// <summary>
        /// Translates the specified path.
        /// </summary>
        /// <param name="polygon">The path.</param>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        public static Polygon Translate(Polygon polygon, Vector2 delta)
        {
            var contours = new List<PolygonContour>(polygon.Count);
            foreach (var contour in polygon.Contours)
            {
                contours.Add((PolygonContour)contour.Translate(delta));
            }

            return new Polygon(contours);
        }

        /// <summary>
        /// Queries whether the shape includes the specified point in it's geometry.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool Includes(PointF point)
        {
            foreach (var contour in Contours)
            {
                if (contour.Includes(point))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PolygonContour> GetEnumerator() => Contours.GetEnumerator();

        /// <summary>
        /// Creates a string representation of this <see cref="Polygon" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public override string? ToString() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a string representation of this <see cref="Polygon" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string? ToString(IFormatProvider formatProvider) => ToString("R" /* format string */, formatProvider);

        /// <summary>
        /// Creates a string representation of this <see cref="Polygon" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string? ToString(string format, IFormatProvider formatProvider)
        {
            if (this is null)
            {
                return nameof(Polygon);
            }

            const char sep = ',';
            return $"{nameof(Polygon)}{{{string.Join(sep.ToString(), Contours.Select(x => x.ToString(format, formatProvider)))}}}";
        }

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns></returns>
        private string GetDebuggerDisplay() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */) ?? string.Empty;
        #endregion
    }
}
