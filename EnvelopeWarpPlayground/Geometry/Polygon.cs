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

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// The polygon class.
    /// </summary>
    /// <seealso cref="IGeometry{T}" />
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Polygon
        : IGeometry<PolygonContour>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the contours.
        /// </summary>
        /// <value>
        /// The contours.
        /// </value>
        public List<PolygonContour> Contours { get; set; }

        /// <summary>
        /// Gets the points count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => Contours.Count;
        #endregion Properties

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

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PolygonContour> GetEnumerator() => Contours.GetEnumerator();

        /// <summary>
        /// Gets the debugger display.
        /// </summary>
        /// <returns></returns>
        private string GetDebuggerDisplay() => ToString();
    }
}
