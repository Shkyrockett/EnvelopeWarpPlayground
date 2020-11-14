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
using System.Drawing;
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

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this IGeometry geometry, Graphics graphics, Brush brush, Pen pen)
        {
            switch (geometry)
            {
                case Group g:
                    DrawGeometry(g, graphics, brush, pen);
                    break;
                case Polygon g:
                    DrawGeometry(g, graphics, brush, pen);
                    break;
                case PolygonContour g:
                    DrawGeometry(g, graphics, brush, pen);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this Group geometry, Graphics graphics, Brush brush, Pen pen)
        {
            foreach (var shape in geometry)
            {
                shape.DrawGeometry(graphics, brush, pen);
            }
        }

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this Polygon geometry, Graphics graphics, Brush brush, Pen pen)
        {
            foreach (var shape in geometry)
            {
                shape.DrawGeometry(graphics, brush, pen);
            }
        }

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this PolygonContour geometry, Graphics graphics, Brush brush, Pen pen)
        {
            if (geometry.Count > 0)
            {
                if (geometry.Count > 2)
                {
                    if (brush is Brush b && b != Brushes.Transparent) graphics.FillPolygon(b, geometry.Points.ToArray());
                    if (pen is Pen p && p != Pens.Transparent) graphics.DrawPolygon(p, geometry.Points.ToArray());
                }
                else if (geometry.Count > 1)
                {
                    if (pen is Pen p && p != Pens.Transparent) graphics.DrawLines(pen, geometry.ToArray());
                }
                else
                {
                    // Draw Point here.
                }
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this IGeometry geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            switch (geometry)
            {
                case Group g:
                    DrawNodes(g, graphics, brush, pen, radius);
                    break;
                case Polygon g:
                    DrawNodes(g, graphics, brush, pen, radius);
                    break;
                case PolygonContour g:
                    DrawNodes(g, graphics, brush, pen, radius);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this Group geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            foreach (var shape in geometry)
            {
                shape.DrawNodes(graphics, brush, pen, radius);
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this Polygon geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            foreach (var shape in geometry)
            {
                shape.DrawNodes(graphics, brush, pen, radius);
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this PolygonContour geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            if (geometry.Count > 0)
            {
                foreach (var corner in geometry)
                {
                    var rect = new RectangleF(corner.X - radius, corner.Y - radius, (2f * radius) + 1f, (2f * radius) + 1f);
                    graphics.FillEllipse(brush, rect);
                    graphics.DrawEllipse(pen, rect);
                }
            }
        }
    }
}
