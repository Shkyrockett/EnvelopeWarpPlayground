// <copyright file="IGeometry.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace EnvelopeWarpLibrary;

/// <summary>
/// The IGeometry interface.
/// </summary>
public interface IGeometry
{
    #region Properties
    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    int Count { get; }
    #endregion Properties

    #region Mutators
    /// <summary>
    /// Clears this instance.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    void Remove(PointF point);
    #endregion

    #region Methods
    /// <summary>
    /// Translates the specified delta.
    /// </summary>
    /// <param name="delta">The delta.</param>
    /// <returns></returns>
    IGeometry Translate(Vector2 delta);

    /// <summary>
    /// Queries whether the shape includes the specified point in it's geometry.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns></returns>
    bool Includes(PointF point);

    /// <summary>
    /// Creates a string representation of this <see cref="IGeometry" /> struct based on the format string and IFormatProvider passed in.
    /// If the provider is null, the CurrentCulture is used.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> representation of this object.
    /// </returns>
    public string? ToString() => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */);

    /// <summary>
    /// Creates a string representation of this <see cref="IGeometry" /> struct based on the format string and IFormatProvider passed in.
    /// If the provider is null, the CurrentCulture is used.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns></returns>
    public string? ToString(IFormatProvider formatProvider) => ToString("R" /* format string */, CultureInfo.InvariantCulture /* format provider */);

    /// <summary>
    /// Creates a string representation of this <see cref="IGeometry" /> struct based on the format string and IFormatProvider passed in.
    /// If the provider is null, the CurrentCulture is used.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns></returns>
    string? ToString(string format, IFormatProvider formatProvider);
    #endregion
}

/// <summary>
/// The IGeometry interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGeometry<T>
    : IGeometry
{
    #region Enumeration
    /// <summary>
    /// Gets or sets the <see cref="PointF" /> at the specified index.
    /// </summary>
    /// <value>
    /// The <see cref="PointF" />.
    /// </value>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    T this[int index] { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="PointF" /> at the specified index.
    /// </summary>
    /// <value>
    /// The <see cref="PointF" />.
    /// </value>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    T this[Index index] { get { return this[index.Value]; } set { this[index.Value] = value; } }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns></returns>
    IEnumerator<T> GetEnumerator();
    #endregion Enumeration

    #region Mutators
    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    void Add(T item) { }

    /// <summary>
    /// Inserts a point the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="item">The item.</param>
    void Insert(int index, T item) { }

    /// <summary>
    /// Removes an item at a specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    void RemoveAt(int index) { }
    #endregion Methods
}
