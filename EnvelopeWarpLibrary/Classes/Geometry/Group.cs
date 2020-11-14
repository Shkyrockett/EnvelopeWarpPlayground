// <copyright file="Group.cs">
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
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace EnvelopeWarpLibrary
{
    /// <summary>
    /// The Group class.
    /// </summary>
    /// <seealso cref="PolygonLibrary.IGeometry{T}" />
    /// <seealso cref="PolygonLibrary.IGeometry" />
    public class Group
        : IGeometry<IGeometry>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group()
            : this(new List<IGeometry>())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="shapes">The shapes.</param>
        public Group(params IGeometry[] shapes)
            : this(shapes.ToList())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="shapes">The shapes.</param>
        public Group(IEnumerable<IGeometry> shapes)
        {
            Shapes = shapes as List<IGeometry> ?? new List<IGeometry>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the shapes.
        /// </summary>
        /// <value>
        /// The contours.
        /// </value>
        public List<IGeometry> Shapes { get; set; }

        /// <summary>
        /// Gets the shapes count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => Shapes.Count;
        #endregion

        #region Indexers
        /// <summary>
        /// Gets or sets the <see cref="IGeometry" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="IGeometry" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IGeometry this[int index] { get { return Shapes[index]; } set { Shapes[index] = value; } }

        /// <summary>
        /// Gets or sets the <see cref="IGeometry" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="IGeometry" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IGeometry this[Index index] { get { return Shapes[index]; } set { Shapes[index] = value; } }

        /// <summary>
        /// Gets the <see cref="Span{IGeometry}" /> with the specified range.
        /// </summary>
        /// <value>
        /// The <see cref="Span{IGeometry}" />.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public Span<IGeometry> this[Range range] => Shapes.ToArray()[range];
        #endregion

        #region Mutators
        /// <summary>
        /// Adds the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void Add(Group group)
        {
            Shapes.Add(group);
        }

        /// <summary>
        /// Adds the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        public void Add(Polygon polygon)
        {
            Shapes.Add(polygon);
        }

        /// <summary>
        /// Adds the specified contour.
        /// </summary>
        /// <param name="contour">The contour.</param>
        public void Add(PolygonContour contour)
        {
            Shapes.Add(contour);
        }

        /// <summary>
        /// Inserts the specified contour.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="group">The group.</param>
        public void Insert(int index, Group group) => Shapes.Insert(index, group);

        /// <summary>
        /// Inserts the specified contour.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="polygon">The polygon.</param>
        public void Insert(int index, Polygon polygon) => Shapes.Insert(index, polygon);

        /// <summary>
        /// Inserts the specified contour.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="contour">The contour.</param>
        public void Insert(int index, PolygonContour contour) => Shapes.Insert(index, contour);

        /// <summary>
        /// Removes the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void Remove(PointF point)
        {
            foreach (var item in Shapes)
            {
                if (item.Includes(point))
                {
                    item.Remove(point);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void Remove(Group group)
        {
            foreach (var item in Shapes)
            {
                switch (item)
                {
                    case Group g:
                        if (g == group)
                        {
                            Shapes.Remove(g);
                            return;
                        }
                        else
                        {
                            g.Remove(group);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Removes the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        public void Remove(Polygon polygon)
        {
            foreach (var item in Shapes)
            {
                switch (item)
                {
                    case Group g:
                        g.Remove(polygon);
                        break;
                    case Polygon p:
                        if (p == polygon)
                        {
                            Shapes.Remove(p);
                            return;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Removes the specified contour.
        /// </summary>
        /// <param name="contour">The contour.</param>
        public void Remove(PolygonContour contour)
        {
            foreach (var item in Shapes)
            {
                switch (item)
                {
                    case Group g:
                        g.Remove(contour);
                        break;
                    case Polygon p:
                        p.Remove(contour);
                        break;
                    case PolygonContour c:
                        if (c == contour)
                        {
                            Shapes.Remove(c);
                            return;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Removes an item at a specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index) => Shapes.RemoveAt(index);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            foreach (var shape in Shapes)
            {
                shape.Clear();
            }

            Shapes.Clear();
        }

        /// <summary>
        /// Reverse the Shapes.
        /// </summary>
        /// <returns>
        /// The <see cref="PolygonContour" />.
        /// </returns>
        public Group Reverse()
        {
            Shapes.Reverse();
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
        /// <param name="group">The path.</param>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        public static Group Translate(Group group, Vector2 delta)
        {
            var shapes = new List<IGeometry>(group.Count);
            foreach (var shape in group.Shapes)
            {
                shapes.Add(shape.Translate(delta));
            }

            return new Group(shapes);
        }

        /// <summary>
        /// Queries whether the shape includes the specified point in it's geometry.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool Includes(PointF point)
        {
            foreach (var item in Shapes)
            {
                if (item.Includes(point))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IGeometry> GetEnumerator() => Shapes.GetEnumerator();

        /// <summary>
        /// Creates a string representation of this <see cref="Group" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public override string? ToString() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */);

        /// <summary>
        /// Creates a string representation of this <see cref="Group" /> struct based on the format string and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string" /> representation of this object.
        /// </returns>
        public string? ToString(IFormatProvider formatProvider) => ToString("R" /* format string */, formatProvider);

        /// <summary>
        /// Creates a string representation of this <see cref="Group" /> struct based on the format string and IFormatProvider passed in.
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
                return nameof(Group);
            }

            var sep = ',';
            return $"{nameof(Group)}{{{string.Join(sep.ToString(), Shapes.Select(x => x.ToString(format, formatProvider)))}}}";
        }

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns></returns>
        private string GetDebuggerDisplay() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */) ?? string.Empty;
        #endregion
    }
}
