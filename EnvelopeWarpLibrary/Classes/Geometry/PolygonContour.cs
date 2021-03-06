// <copyright file="PolygonContour.cs">
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
using System.Text;
using System.Text.RegularExpressions;

namespace EnvelopeWarpLibrary
{
    /// <summary>
    /// The polygon contour class.
    /// </summary>
    /// <seealso cref="EnvelopeWarpLibrary.IGeometry{T}" />
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class PolygonContour
        : IGeometry<PointF>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonContour" /> class.
        /// </summary>
        public PolygonContour()
            : this(new List<PointF>())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonContour" /> class.
        /// </summary>
        /// <param name="polygon">The <paramref name="polygon" />.</param>
        public PolygonContour(PolygonContour polygon)
            : this(polygon.Points)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonContour" /> class.
        /// </summary>
        /// <param name="points">The <paramref name="points" />.</param>
        public PolygonContour(params PointF[] points)
            : this(points.ToList())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonContour" /> class.
        /// </summary>
        /// <param name="points">The <paramref name="points" />.</param>
        public PolygonContour(IEnumerable<PointF> points)
        {
            Points = points as List<PointF> ?? new List<PointF>();
        }
        #endregion Constructors

        #region Enumeration
        /// <summary>
        /// Gets or sets the <see cref="PointF" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PointF" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PointF this[int index] { get { return Points[index]; } set { Points[index] = value; } }

        /// <summary>
        /// Gets or sets the <see cref="PointF" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PointF" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PointF this[Index index] { get { return Points[index]; } set { Points[index] = value; } }

        /// <summary>
        /// Gets the <see cref="Span{PointF}" /> with the specified range.
        /// </summary>
        /// <value>
        /// The <see cref="Span{PointF}" />.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public Span<PointF> this[Range range] => Points.ToArray()[range];

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PointF> GetEnumerator() => Points.GetEnumerator();
        #endregion Enumeration

        #region Properties
        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public List<PointF> Points { get; set; }

        /// <summary>
        /// Gets the number of points.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => Points.Count;
        #endregion Properties

        #region Mutators
        /// <summary>
        /// Add Point.
        /// </summary>
        /// <param name="point">The <paramref name="point" />.</param>
        public void Add(PointF point)
        {
            Points.Add(point);
            //return this;
        }

        /// <summary>
        /// Inserts a point the specified point.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="point">The point.</param>
        public void Insert(int index, PointF point) => Points.Insert(index, point);

        /// <summary>
        /// Removes the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void Remove(PointF point)
        {
            Points.Remove(point);
        }

        /// <summary>
        /// Removes an item at a specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index) => Points.RemoveAt(index);

        /// <summary>
        /// Clears the points of the contour.
        /// </summary>
        /// <remarks>
        /// Clear the elements of the array so that the garbage collector can reclaim the references.
        /// </remarks>
        public void Clear() => Points.Clear();

        /// <summary>
        /// Reverse the Points.
        /// </summary>
        /// <returns>
        /// The <see cref="PolygonContour" />.
        /// </returns>
        public PolygonContour Reverse()
        {
            Points.Reverse();
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
        /// <param name="path">The path.</param>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        public static PolygonContour Translate(PolygonContour path, Vector2 delta)
        {
            var contour = new List<PointF>(path.Points.Count);
            for (var i = 0; i < path.Points.Count; i++)
            {
                contour.Add(new PointF(path[i].X + delta.X, path[i].Y + delta.Y));
            }

            return new PolygonContour(contour);
        }

        /// <summary>
        /// Queries whether the shape includes the specified point in it's geometry.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool Includes(PointF point)
        {
            return Points.Contains(point);
        }

        /// <summary>
        /// The to array.
        /// </summary>
        /// <returns>
        /// The <see cref="T:PointF[]" />.
        /// </returns>
        public PointF[] ToArray() => Points.ToArray();

        /// <summary>
        /// Clone.
        /// </summary>
        /// <returns>
        /// The <see cref="PolygonContour" />.
        /// </returns>
        public PolygonContour Clone() => new(Points.ToArray());

        /// <summary>
        /// Parse the path def string.
        /// </summary>
        /// <param name="pathDefinition">The <paramref name="pathDefinition" />.</param>
        /// <returns>
        /// The <see cref="T:List{PointF}" />.
        /// </returns>
        public static List<PointF> ParsePathDefString(string pathDefinition) => ParsePathDefString(pathDefinition, CultureInfo.InvariantCulture);

        /// <summary>
        /// Parse the path def string.
        /// </summary>
        /// <param name="pathDefinition">The <paramref name="pathDefinition" />.</param>
        /// <param name="provider">The <paramref name="provider" />.</param>
        /// <returns>
        /// The <see cref="T:List{PointF}" />.
        /// </returns>
        /// <exception cref="Exception">Polygon definitions must be in sets of two numbers.</exception>
        public static List<PointF> ParsePathDefString(string pathDefinition, IFormatProvider provider)
        {
            // Discard whitespace and comma but keep the - minus sign.
            var separators = $@"[\s{','}]|(?=-)";

            var poly = new List<PointF>();

            // Split the definition string into shape tokens.
            var list = Regex.Split(pathDefinition, separators).Where(t => !string.IsNullOrEmpty(t)).Select(arg => float.Parse(arg, provider)).ToArray();

            if (list.Length % 2 != 0)
            {
                throw new Exception("Polygon definitions must be in sets of two numbers.");
            }

            for (var i = 0; i < list.Length - 1; i += 2)
            {
                poly.Add(new PointF(list[i], list[i + 1]));
            }

            return poly;
        }

        /// <summary>
        /// The to path def string.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public string ToPathDefString() => ToPathDefString(null, CultureInfo.InvariantCulture);

        /// <summary>
        /// The to path def string.
        /// </summary>
        /// <param name="format">The <paramref name="format" />.</param>
        /// <param name="provider">The <paramref name="provider" />.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public string ToPathDefString(string? format, IFormatProvider provider)
        {
            var output = new StringBuilder();

            const char sep = ',';

            foreach (var item in Points)
            {
                // M is Move to.
                output.Append($"{item.X.ToString(format, provider)}{sep}{item.Y.ToString(format, provider)} ");
            }

            // Minus signs are valid separators in SVG path definitions which can be
            // used in place of commas to shrink the length of the string. 
            output.Replace(",-", "-");
            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Creates a string representation of this <see cref="PolygonContour" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public override string? ToString() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a string representation of this <see cref="PolygonContour" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string? ToString(IFormatProvider formatProvider) => ToString("R" /* format string */, formatProvider);

        /// <summary>
        /// Creates a string representation of this <see cref="PolygonContour" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The provider.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string? ToString(string format, IFormatProvider formatProvider)
        {
            if (this is null)
            {
                return nameof(PolygonContour);
            }

            const char sep = ',';
            return $"{nameof(PolygonContour)}{{{string.Join(sep.ToString(), Points.Select(x => x.ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */)))}}}";
        }

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns></returns>
        private string GetDebuggerDisplay() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */) ?? string.Empty;
        #endregion Methods
    }
}
