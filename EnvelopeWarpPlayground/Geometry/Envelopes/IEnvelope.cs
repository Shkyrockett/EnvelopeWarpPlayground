﻿// <copyright file="IEnvelope.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System.Drawing;
using System.Drawing.Drawing2D;

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// Interface for Envelopes.
    /// </summary>
    public interface IEnvelope :
        IGeometry<PointF>
    {
        /// <summary>
        /// Processes the point.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        PointF ProcessPoint(RectangleF bounds, PointF point);

        /// <summary>
        /// Converts to graphics path.
        /// </summary>
        /// <returns></returns>
        GraphicsPath ToGraphicsPath();
    }
}
