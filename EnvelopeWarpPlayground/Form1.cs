// <copyright file="Form1.cs">
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static EnvelopeWarpPlayground.Mathematics;

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// The form1 class.
    /// </summary>
    public partial class Form1
        : Form
    {
        #region Constants
        /// <summary>
        /// The "size" of an object for mouse over purposes.
        /// </summary>
        private const int objectRadius = 3;

        /// <summary>
        /// We're over an object if the distance squared
        /// between the mouse and the object is less than this.
        /// </summary>
        private const int objectRadiusSquared = objectRadius * objectRadius;

        private static readonly Pen envelopeStroke = Pens.DarkRed;
        private static readonly Brush envelopeNodeFill = Brushes.Salmon;
        private static readonly Pen envelopeNodeStroke = Pens.DarkRed;
        private static readonly Pen envelopeBoundsStroke = new Pen(Color.Khaki)
        {
            DashPattern = new float[] { 3f, 3f }
        };

        private static readonly Pen newPolygonStroke = Pens.Green;
        private static readonly Pen newPolygonDashedStroke = new Pen(Color.Green)
        {
            DashPattern = new float[] { 3f, 3f }
        };

        private static readonly Brush polygonFill = Brushes.AliceBlue;
        private static readonly Pen polygonStroke = Pens.LightBlue;
        private static readonly Brush polygonNodeFill = Brushes.LightGoldenrodYellow;
        private static readonly Pen polygonNodeStroke = Pens.Tan;

        private static readonly Brush warpedPolygonFill = Brushes.CornflowerBlue;
        private static readonly Pen warpedPolygonStroke = Pens.Blue;
        #endregion Constants

        #region Fields
        /// <summary>
        /// The envelope.
        /// </summary>
        private IEnvelope envelope;

        /// <summary>
        /// The polygons.
        /// </summary>
        private List<PolygonContour> polygons;

        /// <summary>
        /// The polygons distorted.
        /// </summary>
        private List<PolygonContour> polygonsDistorted;

        /// <summary>
        /// The bounds.
        /// </summary>
        private RectangleF polygonsBounds;

        /// <summary>
        /// The new polygon.
        /// </summary>
        private PolygonContour newPolygon;

        /// <summary>
        /// The new point.
        /// </summary>
        private PointF newPoint;

        /// <summary>
        /// The polygon and index of the corner we are moving.
        /// </summary>
        private IGeometry<PointF> movingPolygon;

        /// <summary>
        /// The moving point.
        /// </summary>
        private int movingPoint = -1;

        /// <summary>
        /// The offset x.
        /// </summary>
        private float offsetX;

        /// <summary>
        /// The offset y.
        /// </summary>
        private float offsetY;
        private bool panning;
        private PointF startingPoint;
        private PointF panPoint;
        private float scale = 1f;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            TypeDescriptor.AddAttributes(typeof(PointF));

            InitializeComponent();
        }
        #endregion Constructors

        #region Event Handlers
        /// <summary>
        /// The form1 load.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The event arguments.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            var (left, top, width, height) = (100f, 100f, 300f, 200f);
            var (hStroke, vStroke, hOffset, vOffset, hMargin, vMargin) = (width / 6f, height / 4f, width, 0f, 25f, 0f);

            polygons = new List<PolygonContour>
            {
                // H.
                new PolygonContour(
                new PointF(left, top),
                new PointF(left + hStroke, top),
                new PointF(left + hStroke, top + 1.5f * vStroke),
                new PointF(left + 2f * hStroke, top + 1.5f * vStroke),
                new PointF(left + 2f * hStroke, top),
                new PointF(left + 3f * hStroke, top),
                new PointF(left + 3f * hStroke, top + height),
                new PointF(left + 2f * hStroke, top + height),
                new PointF(left + 2f * hStroke, top + 2.5f * vStroke),
                new PointF(left + hStroke, top + 2.5f * vStroke),
                new PointF(left + hStroke, top + height),
                new PointF(left, top + height)
                ),
                // I.
                new PolygonContour(
                new PointF(left + 3f * hStroke, top),
                new PointF(left + width, top),
                new PointF(left + width, top + vStroke),
                new PointF(left + 5f * hStroke, top + vStroke),
                new PointF(left + 5f * hStroke, top + 3f * vStroke),
                new PointF(left + width, top + 3f * vStroke),
                new PointF(left + width, top + height),
                new PointF(left + 3f * hStroke, top + height),
                new PointF(left + 3f * hStroke, top + 3f * vStroke),
                new PointF(left + 4f * hStroke, top + 3f * vStroke),
                new PointF(left + 4f * hStroke, top + vStroke),
                new PointF(left + 3f * hStroke, top + vStroke)
                )
            };

            //envelope = new LinearEnvelope(left + width, top, width, height);
            //envelope = new QuadraticEnvelope(left + width, top, width, height);
            envelope = new CubicEnvelope(left + hOffset + hMargin, top + vOffset + vMargin, width, height);

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsBounds = new RectangleF(left, top, width, height);
            polygonsDistorted = Distort(polygons, polygonsBounds, envelope);
        }

        /// <summary>
        /// The pic canvas paint.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The paint event arguments.</param>
        private void PicCanvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(picCanvas.BackColor);
            g.ResetTransform();
            g.TranslateTransform(panPoint.X, panPoint.Y);
            g.ScaleTransform(scale, scale);

            // Draw Polygon Bounds for Envelope Reference
            g.DrawRectangles(envelopeBoundsStroke, new RectangleF[] { polygonsBounds });

            // Draw the old polygons.
            foreach (var polygon in polygons)
            {
                if (polygon.Count > 0)
                {
                    if (polygon.Count > 2)
                    {
                        // Draw the polygon.
                        g.FillPolygon(polygonFill, polygon.ToArray());
                        g.DrawPolygon(polygonStroke, polygon.ToArray());
                    }
                    else if (polygon.Count > 1)
                    {
                        g.DrawLines(polygonStroke, polygon.ToArray());
                    }

                    // Draw the corners.
                    foreach (var corner in polygon)
                    {
                        var rect = new RectangleF(
                            corner.X - objectRadius, corner.Y - objectRadius,
                            (2f * objectRadius) + 1f, (2f * objectRadius) + 1f);
                        g.FillEllipse(polygonNodeFill, rect);
                        g.DrawEllipse(polygonNodeStroke, rect);
                    }
                }
            }

            // Draw the resulting warped polygons.
            foreach (var polygon in polygonsDistorted)
            {
                if (polygon.Count > 2)
                {
                    // Draw the polygon.
                    g.FillPolygon(warpedPolygonFill, polygon.ToArray());
                    g.DrawPolygon(warpedPolygonStroke, polygon.ToArray());
                }
                else if (polygon.Count > 1)
                {
                    g.DrawLines(warpedPolygonStroke, polygon.ToArray());
                }
            }

            // Draw Envelope.
            using (var path = envelope.ToGraphicsPath())
            {
                g.DrawPath(envelopeStroke, path);
                foreach (var point in envelope)
                {
                    var rect = new RectangleF(
                        point.X - objectRadius, point.Y - objectRadius,
                        (2f * objectRadius) + 1f, (2f * objectRadius) + 1f);
                    g.FillEllipse(envelopeNodeFill, rect);
                    g.DrawEllipse(envelopeNodeStroke, rect);
                }
            }

            // Draw the new polygon.
            if (newPolygon != null)
            {
                if (newPolygon.Count > 1)
                {
                    // Draw the new polygon.
                    g.DrawLines(newPolygonStroke, newPolygon.ToArray());
                }
                if (newPolygon.Count > 0)
                {
                    // Draw the newest edge.
                    g.DrawLine(newPolygonDashedStroke, newPolygon[^1], newPoint);
                }
            }
        }

        /// <summary>
        /// The pic canvas mouse down.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            var translateScalePoint = InverseTranslateScalePoint(e.Location, panPoint, scale);
            var scalePoint = InverseScalePoint(e.Location, scale);

            // See what we're over.
            if (e.Button == MouseButtons.Left)
            {
                (bool Success, IGeometry<PointF> HitPolygon) polygonResult;
                (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) cornerResult;
                (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) edgeResult;

                if (newPolygon != null)
                {
                    newPoint = translateScalePoint;
                }
                else if ((cornerResult = MouseIsOverCornerPoint(translateScalePoint, polygons, envelope)).Success)
                {
                    // Start dragging this corner.
                    picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                    picCanvas.MouseMove += PicCanvas_MouseMove_MovingCorner;
                    picCanvas.MouseUp += PicCanvas_MouseUp_MovingCorner;
                    picCanvas.MouseDoubleClick += PicCanvas_MouseDoubleClick_Corner;

                    // Remember the polygon and point number.
                    movingPolygon = cornerResult.HitPolygon;
                    movingPoint = cornerResult.HitPoint;

                    // Remember the offset from the mouse to the point.
                    offsetX = cornerResult.HitPolygon[cornerResult.HitPoint].X - scalePoint.X;
                    offsetY = cornerResult.HitPolygon[cornerResult.HitPoint].Y - scalePoint.Y;
                }
                else if ((edgeResult = MouseIsOverEdge(translateScalePoint, polygons)).Success)
                {
                    // Add a point.
                    edgeResult.HitPolygon.Insert(edgeResult.HitPoint1 + 1, edgeResult.NearestPoint);
                }
                else if ((polygonResult = MouseIsOverPolygon(translateScalePoint, polygons)).Success)
                {
                    // Start moving this polygon.
                    picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                    picCanvas.MouseMove += PicCanvas_MouseMove_MovingPolygon;
                    picCanvas.MouseUp += PicCanvas_MouseUp_MovingPolygon;
                    picCanvas.MouseDoubleClick += PicCanvas_MouseDoubleClick_Polygon;

                    // Remember the polygon.
                    movingPolygon = (PolygonContour)polygonResult.HitPolygon;

                    // Remember the offset from the mouse to the segment's first point.
                    offsetX = polygonResult.HitPolygon[0].X - scalePoint.X;
                    offsetY = polygonResult.HitPolygon[0].Y - scalePoint.Y;
                }
                else
                {
                    // Start a new polygon.
                    newPolygon = new PolygonContour();
                    newPoint = translateScalePoint;
                    newPolygon.Add(translateScalePoint);

                    // Get ready to work on the new polygon.
                    picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                    picCanvas.MouseMove += PicCanvas_MouseMove_Drawing;
                    picCanvas.MouseUp += PicCanvas_MouseUp_Drawing;
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                panning = true;
                startingPoint = PanAt(panPoint, e.Location);

                // Get ready to work on the new polygon.
                picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                picCanvas.MouseMove += PicCanvas_MouseMove_Panning;
                picCanvas.MouseUp += PicCanvas_MouseUp_Panning;
            }

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// See if we're over a polygon or corner point.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_NotDrawing(object sender, MouseEventArgs e)
        {
            var mousePoint = InverseTranslateScalePoint(e.Location, panPoint, scale);
            picCanvas.Cursor =
                MouseIsOverCornerPoint(mousePoint, polygons, envelope).Success ? Cursors.Arrow :
                MouseIsOverEdge(mousePoint, polygons).Success ? Cursors.Cross :
                MouseIsOverPolygon(mousePoint, polygons).Success ? Cursors.Hand :
                Cursors.Cross;
        }

        /// <summary>
        /// Move the next point in the new polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_Drawing(object sender, MouseEventArgs e)
        {
            newPoint = InverseTranslateScalePoint(e.Location, panPoint, scale);
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Finish moving the selected polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseUp_Drawing(object sender, MouseEventArgs e)
        {
            var mousePoint = InverseTranslateScalePoint(e.Location, panPoint, scale);

            // We are already drawing a polygon.
            // If it's the right mouse button, finish this polygon.
            if (e.Button == MouseButtons.Right)
            {
                // Finish this polygon.
                if (newPolygon?.Count > 2)
                {
                    polygons?.Add(newPolygon);
                }

                newPolygon = null;

                // We no longer are drawing.
                picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
                picCanvas.MouseMove -= PicCanvas_MouseMove_Drawing;
                picCanvas.MouseUp -= PicCanvas_MouseUp_Drawing;
            }
            else
            {
                // Add a point to this polygon.
                if (newPolygon?[^1] != mousePoint)
                {
                    newPolygon.Add(mousePoint);
                }
            }

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Move the selected corner.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_MovingCorner(object sender, MouseEventArgs e)
        {
            var mousePoint = InverseScalePoint(e.Location, scale);

            // Move the point.
            movingPolygon[movingPoint] = new PointF(mousePoint.X + offsetX, mousePoint.Y + offsetY);

            if (movingPolygon is IEnvelope)
            {
                envelope = (IEnvelope)movingPolygon;
            }

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Finish moving the selected corner.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseUp_MovingCorner(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingCorner;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingCorner;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Corner;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the PicCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PicCanvas_MouseDoubleClick_Corner(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingCorner;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingCorner;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Corner;

            var (Success, movingPolygon, movingPoint) = MouseIsOverCornerPoint(InverseTranslateScalePoint(e.Location, panPoint, scale), polygons, envelope);

            if (Success)
            {
                movingPolygon.RemoveAt(movingPoint);
                if (movingPolygon.Count == 0)
                {
                    polygons.Remove(movingPolygon as PolygonContour);
                }

                //polygonsBounds = PolygonBounds(polygons).Value;
                polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Move the selected polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_MovingPolygon(object sender, MouseEventArgs e)
        {
            var mousePoint = InverseScalePoint(e.Location, scale);

            // See how far the first point will move.
            var dx = mousePoint.X + offsetX - movingPolygon[0].X;
            var dy = mousePoint.Y + offsetY - movingPolygon[0].Y;

            if (dx == 0 && dy == 0)
            {
                return;
            }

            // Move the polygon.
            for (var i = 0; i < movingPolygon.Count; i++)
            {
                movingPolygon[i] = new PointF(movingPolygon[i].X + dx, movingPolygon[i].Y + dy);
            }

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Finish moving the selected polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/>.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseUp_MovingPolygon(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingPolygon;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingPolygon;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Polygon;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the PicCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PicCanvas_MouseDoubleClick_Polygon(object sender, MouseEventArgs e)
        {
            var mouse_pt = InverseTranslateScalePoint(e.Location, panPoint, scale);

            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingPolygon;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingPolygon;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Polygon;

            var (Success, movingPolygon) = MouseIsOverPolygon(mouse_pt, polygons);
            if (Success)
            {
                polygons.Remove(movingPolygon as PolygonContour);

                //polygonsBounds = PolygonBounds(polygons).Value;
                polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Handles the Panning event of the PicCanvas_MouseMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PicCanvas_MouseMove_Panning(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && panning)
            {
                panPoint = PanAt(startingPoint, e.Location);

                //polygonsBounds = PolygonBounds(polygons).Value;
                polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Handles the Panning event of the PicCanvas_MouseUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PicCanvas_MouseUp_Panning(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_Panning;
            picCanvas.MouseUp -= PicCanvas_MouseUp_Panning;

            if (e.Button == MouseButtons.Middle && panning)
            {
                panning = false;

                //polygonsBounds = PolygonBounds(polygons).Value;
                polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Handles the MouseWheel event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PicCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            scale = MouseWheelScaleFactor(scale, e.Delta);
            scale = scale < scale_per_delta ? scale_per_delta : scale;
            //movingPoint = ScrollTo(movingPoint, PointToClient(e.Location), scale);

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }
        #endregion Event Handlers

        #region Methods          
        /// <summary>
        /// See if the mouse is over a corner point.
        /// </summary>
        /// <param name="mousePoint">The mouse_pt.</param>
        /// <param name="polygons">The polygons.</param>
        /// <param name="envelope">The envelope.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, List<PolygonContour> polygons, IEnvelope envelope)
        {
            // See if we're over one of the Envelope's corner points.
            for (var i = 0; i < envelope.Count; i++)
            {
                // See if we're over this point.
                if (DistanceSquared(envelope[i], mousePoint) < objectRadiusSquared * 2)
                {
                    // We're over this point.
                    return (true, envelope, i);
                }
            }

            // See if we're over a corner point.
            foreach (var polygon in polygons)
            {
                // See if we're over one of the polygon's corner points.
                for (var i = 0; i < polygon.Count; i++)
                {
                    // See if we're over this point.
                    if (DistanceSquared(polygon[i], mousePoint) < objectRadiusSquared * 2)
                    {
                        // We're over this point.
                        return (true, polygon, i);
                    }
                }
            }

            return (false, null, -1);
        }

        /// <summary>
        /// See if the mouse is over a polygon's edge.
        /// </summary>
        /// <param name="mousePoint">The mouse_pt.</param>
        /// <param name="polygons">The polygons.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, List<PolygonContour> polygons)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            for (var polygonIndex = polygons.Count - 1; polygonIndex >= 0; polygonIndex--)
            {
                var polygon = polygons[polygonIndex];

                // See if we're over one of the polygon's segments.
                var cursorIndex = polygon.Count - 1;
                for (var pointIndex = 0; pointIndex < polygon.Count; pointIndex++)
                {
                    // See if we're over the segment between these points.
                    var query = DistanceToLineSegmentSquared(mousePoint, polygon[cursorIndex], polygon[pointIndex]);
                    if (query.Distnce < objectRadiusSquared)
                    {
                        // We are over this segment.
                        return (true, polygon, cursorIndex, pointIndex, query.Point);
                    }

                    // Get the index of the polygon's next point.
                    cursorIndex = pointIndex;
                }
            }

            return (false, null, -1, -1, new PointF(0, 0));
        }

        /// <summary>
        /// See if the mouse is over a polygon's body.
        /// </summary>
        /// <param name="mousePoint">The mouse_pt.</param>
        /// <param name="polygons">The polygons.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, List<PolygonContour> polygons)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            for (var i = polygons.Count - 1; i >= 0; i--)
            {
                var inclusions = PolygonContourContainsPoint(polygons[i], mousePoint);
                if (inclusions == Inclusions.Inside || inclusions == Inclusions.Boundary)
                {
                    return (true, polygons[i]);
                }

                //// Make a GraphicsPath representing the polygon.
                //using var path = new GraphicsPath();
                //path.AddPolygon(polygons[i].ToArray());

                //// See if the point is inside the GraphicsPath.
                //if (path.IsVisible(mousePoint))
                //{
                //    return (true, polygons[i]);
                //}
            }

            return (false, null);
        }
        #endregion Methods
    }
}
