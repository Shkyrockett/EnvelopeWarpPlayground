// <copyright file="WinformsExtentions.cs">
//     Copyright © 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using EnvelopeWarpLibrary;
using System.Drawing.Drawing2D;

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// 
    /// </summary>
    public static class WinformsExtentions
    {
        /// <summary>
        /// Converts to graphics path.
        /// </summary>
        /// <returns></returns>
        public static GraphicsPath ToGraphicsPath(this IEnvelope envelope)
        {
            return envelope switch
            {
                LinearEnvelope e => ToGraphicsPath(e),
                QuadraticEnvelope e => ToGraphicsPath(e),
                CubicEnvelope e => ToGraphicsPath(e),
                _ => null,
            };
        }

        /// <summary>
        /// The to GraphicsPath.
        /// </summary>
        /// <returns>
        /// The <see cref="GraphicsPath" />.
        /// </returns>
        public static GraphicsPath ToGraphicsPath(this LinearEnvelope envelope)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddLine(envelope.ControlPointTopLeft, envelope.ControlPointTopRight);
            path.AddLine(envelope.ControlPointTopRight, envelope.ControlPointBottomRight);
            path.AddLine(envelope.ControlPointBottomRight, envelope.ControlPointBottomLeft);
            path.AddLine(envelope.ControlPointBottomLeft, envelope.ControlPointTopLeft);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// The to GraphicsPath.
        /// </summary>
        /// <returns>
        /// The <see cref="GraphicsPath" />.
        /// </returns>
        public static GraphicsPath ToGraphicsPath(this QuadraticEnvelope envelope)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            var (a, b, c, d) = Mathematics.QuadraticBezierToCubicBezier(envelope.ControlPointTopLeft, envelope.ControlHandleTop, envelope.ControlPointTopRight);
            path.AddBezier(a, b, c, d);
            (a, b, c, d) = Mathematics.QuadraticBezierToCubicBezier(envelope.ControlPointTopRight, envelope.ControlHandleRight, envelope.ControlPointBottomRight);
            path.AddBezier(a, b, c, d);
            (a, b, c, d) = Mathematics.QuadraticBezierToCubicBezier(envelope.ControlPointBottomRight, envelope.ControlHandleBottom, envelope.ControlPointBottomLeft);
            path.AddBezier(a, b, c, d);
            (a, b, c, d) = Mathematics.QuadraticBezierToCubicBezier(envelope.ControlPointBottomLeft, envelope.ControlHandleLeft, envelope.ControlPointTopLeft);
            path.AddBezier(a, b, c, d);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// The to GraphicsPath.
        /// </summary>
        /// <returns>
        /// The <see cref="GraphicsPath" />.
        /// </returns>
        public static GraphicsPath ToGraphicsPath(this CubicEnvelope envelope)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddBezier(envelope.ControlPointTopLeft.Point, envelope.ControlPointTopLeft.AnchorAGlobal, envelope.ControlPointTopRight.AnchorAGlobal, envelope.ControlPointTopRight.Point);
            path.AddBezier(envelope.ControlPointTopRight.Point, envelope.ControlPointTopRight.AnchorBGlobal, envelope.ControlPointBottomRight.AnchorBGlobal, envelope.ControlPointBottomRight.Point);
            path.AddBezier(envelope.ControlPointBottomRight.Point, envelope.ControlPointBottomRight.AnchorAGlobal, envelope.ControlPointBottomLeft.AnchorAGlobal, envelope.ControlPointBottomLeft.Point);
            path.AddBezier(envelope.ControlPointBottomLeft.Point, envelope.ControlPointBottomLeft.AnchorBGlobal, envelope.ControlPointTopLeft.AnchorBGlobal, envelope.ControlPointTopLeft.Point);
            path.CloseFigure();
            return path;
        }
    }
}
