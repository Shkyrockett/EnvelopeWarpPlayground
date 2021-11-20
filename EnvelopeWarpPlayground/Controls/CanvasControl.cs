// <copyright file="CanvasControl.cs">
//     Copyright © 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using EnvelopeWarpLibrary;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using static EnvelopeWarpLibrary.Mathematics;

namespace EnvelopeWarpPlayground;

/// <summary>
/// 
/// </summary>
/// <seealso cref="UserControl" />
public partial class CanvasControl
    : UserControl
{
    #region Constants
    /// <summary>
    /// The envelope stroke
    /// </summary>
    private static readonly Pen envelopeStroke = Pens.DarkRed;

    /// <summary>
    /// The envelope node fill
    /// </summary>
    private static readonly Brush envelopeNodeFill = Brushes.Salmon;

    /// <summary>
    /// The envelope node stroke
    /// </summary>
    private static readonly Pen envelopeNodeStroke = Pens.DarkRed;

    /// <summary>
    /// The envelope bounds stroke
    /// </summary>
    private static readonly Pen envelopeBoundsStroke = new(Color.Khaki)
    {
        DashPattern = new float[] { 3f, 3f }
    };

    /// <summary>
    /// The new polygon stroke
    /// </summary>
    private static readonly Pen newPolygonStroke = Pens.Green;

    /// <summary>
    /// The new polygon dashed stroke
    /// </summary>
    private static readonly Pen newPolygonDashedStroke = new(Color.Green)
    {
        DashPattern = new float[] { 3f, 3f }
    };

    /// <summary>
    /// The polygon fill
    /// </summary>
    private static readonly Brush polygonFill = Brushes.AliceBlue;

    /// <summary>
    /// The polygon stroke
    /// </summary>
    private static readonly Pen polygonStroke = Pens.LightBlue;

    /// <summary>
    /// The polygon node fill
    /// </summary>
    private static readonly Brush polygonNodeFill = Brushes.LightGoldenrodYellow;

    /// <summary>
    /// The polygon node stroke
    /// </summary>
    private static readonly Pen polygonNodeStroke = Pens.Tan;

    /// <summary>
    /// The warped polygon fill
    /// </summary>
    private static readonly Brush warpedPolygonFill = Brushes.CornflowerBlue;

    /// <summary>
    /// The warped polygon stroke
    /// </summary>
    private static readonly Pen warpedPolygonStroke = Pens.Blue;
    #endregion

    #region Fields
    /// <summary>
    /// The envelope.
    /// </summary>
    private IEnvelope envelope;

    /// <summary>
    /// The polygons.
    /// </summary>
    private Group group;

    /// <summary>
    /// The polygons distorted.
    /// </summary>
    private Group polygonsDistorted;

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

    /// <summary>
    /// The panning
    /// </summary>
    private bool panning;

    /// <summary>
    /// The starting point
    /// </summary>
    private PointF startingPoint = PointF.Empty;

    /// <summary>
    /// The pan point
    /// </summary>
    private PointF panPoint = PointF.Empty;

    /// <summary>
    /// The scale.
    /// </summary>
    private float scale = 1f;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CanvasControl"/> class.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CanvasControl()
    {
        DoubleBuffered = true;
        HandleRadius = 3;
        InitializeComponent();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the ghost polygon pen.
    /// </summary>
    /// <value>
    /// The ghost polygon pen.
    /// </value>
    public Pen GhostPolygonPen { get; set; }

    /// <summary>
    /// Gets or sets the handle radius.
    /// </summary>
    /// <value>
    /// The handle radius.
    /// </value>
    [DefaultValue(3)]
    public int HandleRadius { get; set; }

    /// <summary>
    /// We're over an object if the distance squared
    /// between the mouse and the object is less than this.
    /// </summary>
    private int HandleRadiusSquared => HandleRadius * HandleRadius;

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    /// <value>
    /// The document.
    /// </value>
    public Group Document { get => group; set => group = value; }

    /// <summary>
    /// Gets or sets the distorted document.
    /// </summary>
    /// <value>
    /// The distorted document.
    /// </value>
    public Group DistortedDocument { get => polygonsDistorted; set => polygonsDistorted = value; }

    /// <summary>
    /// Gets or sets the document bounds.
    /// </summary>
    /// <value>
    /// The document bounds.
    /// </value>
    public RectangleF DocumentBounds { get => polygonsBounds; set => polygonsBounds = value; }

    /// <summary>
    /// Gets or sets the envelope.
    /// </summary>
    /// <value>
    /// The envelope.
    /// </value>
    public IEnvelope Envelope { get => envelope; set => envelope = value; }

    /// <summary>
    /// Gets or sets the pan point.
    /// </summary>
    /// <value>
    /// The pan point.
    /// </value>
    public PointF Pan
    {
        get => panPoint; set
        {
            panPoint = value;

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(group, polygonsBounds, envelope);

            // Redraw.
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the scale.
    /// </summary>
    /// <value>
    /// The scale.
    /// </value>
    public float Zoom
    {
        get => scale; set
        {
            scale = value;

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(group, polygonsBounds, envelope);

            // Redraw.
            Invalidate();
        }
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the Load event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_Load(object sender, EventArgs e)
    { }

    /// <summary>
    /// Handles the Resize event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_Resize(object sender, EventArgs e)
    {
        Invalidate();
    }

    /// <summary>
    /// Handles the Paint event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_Paint(object sender, PaintEventArgs e)
    {
        //base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);
        g.ScaleTransform(scale, scale);
        g.TranslateTransform(panPoint.X, panPoint.Y);

        // Draw Polygon Bounds for Envelope Reference
        g.DrawRectangles(envelopeBoundsStroke, new RectangleF[] { polygonsBounds });

        // Draw the old polygons.
        foreach (var polygon in group)
        {
            if (polygon.Count > 0)
            {
                // Draw the polygon.
                polygon.DrawGeometry(g, polygonFill, polygonStroke);

                // Draw the corners.
                polygon.DrawNodes(g, polygonNodeFill, polygonNodeStroke, HandleRadius);
            }
        }

        // Draw the resulting warped polygons.
        foreach (var polygon in polygonsDistorted)
        {
            // Draw the polygon.
            polygon.DrawGeometry(g, warpedPolygonFill, warpedPolygonStroke);
        }

        // Draw Envelope.
        using (var path = envelope.ToGraphicsPath())
        {
            g.DrawPath(envelopeStroke, path);
            foreach (var point in envelope)
            {
                var rect = new RectangleF(
                    point.X - HandleRadius, point.Y - HandleRadius,
                    (2f * HandleRadius) + 1f, (2f * HandleRadius) + 1f);
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

        using var forePen = new Pen(ForeColor);
        g.ResetTransform();
        g.DrawRectangle(forePen, 0, 0, Width - 1, Height - 1);
        TextRenderer.DrawText(g, $"Pan Point: {panPoint}\n\rScale: {scale}\n\r🌎 Mouse Pos: {MousePosition}", Font, new Point(0, 0), ForeColor);
    }

    /// <summary>
    /// Handles the MouseDown event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseDown(object sender, MouseEventArgs e)
    {
        var translateScalePoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);
        var scalePoint = ScreenToObject(e.Location, scale);

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
            else if ((cornerResult = MouseIsOverCornerPoint(translateScalePoint, envelope)).Success)
            {
                // Start dragging this corner.
                MouseMove -= CanvasControl_MouseMove_NotDrawing;
                MouseMove += CanvasControl_MouseMove_MovingCorner;
                MouseUp += CanvasControl_MouseUp_MovingCorner;
                MouseDoubleClick += CanvasControl_MouseDoubleClick_Corner;

                // Remember the polygon and point number.
                movingPolygon = cornerResult.HitPolygon;
                movingPoint = cornerResult.HitPoint;

                // Remember the offset from the mouse to the point.
                offsetX = cornerResult.HitPolygon[cornerResult.HitPoint].X - scalePoint.X;
                offsetY = cornerResult.HitPolygon[cornerResult.HitPoint].Y - scalePoint.Y;
            }
            else if ((cornerResult = MouseIsOverCornerPoint(translateScalePoint, group)).Success)
            {
                // Start dragging this corner.
                MouseMove -= CanvasControl_MouseMove_NotDrawing;
                MouseMove += CanvasControl_MouseMove_MovingCorner;
                MouseUp += CanvasControl_MouseUp_MovingCorner;
                MouseDoubleClick += CanvasControl_MouseDoubleClick_Corner;

                // Remember the polygon and point number.
                movingPolygon = cornerResult.HitPolygon;
                movingPoint = cornerResult.HitPoint;

                // Remember the offset from the mouse to the point.
                offsetX = cornerResult.HitPolygon[cornerResult.HitPoint].X - scalePoint.X;
                offsetY = cornerResult.HitPolygon[cornerResult.HitPoint].Y - scalePoint.Y;
            }
            else if ((edgeResult = MouseIsOverEdge(translateScalePoint, group)).Success)
            {
                // Add a point.
                edgeResult.HitPolygon.Insert(edgeResult.HitPoint1 + 1, edgeResult.NearestPoint);
            }
            else if ((polygonResult = MouseIsOverPolygon(translateScalePoint, group)).Success)
            {
                // Start moving this polygon.
                MouseMove -= CanvasControl_MouseMove_NotDrawing;
                MouseMove += CanvasControl_MouseMove_MovingPolygon;
                MouseUp += CanvasControl_MouseUp_MovingPolygon;
                MouseDoubleClick += CanvasControl_MouseDoubleClick_Polygon;

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
                MouseMove -= CanvasControl_MouseMove_NotDrawing;
                MouseMove += CanvasControl_MouseMove_Drawing;
                MouseUp += CanvasControl_MouseUp_Drawing;
            }
        }
        else if (e.Button == MouseButtons.Middle)
        {
            panning = true;
            startingPoint = translateScalePoint;
            panPoint = ScreenToObjectTransposedMatrix(startingPoint, e.Location, scale);

            // Get ready to work on the new polygon.
            MouseMove -= CanvasControl_MouseMove_NotDrawing;
            MouseMove += CanvasControl_MouseMove_Panning;
            MouseUp += CanvasControl_MouseUp_Panning;
        }
        else if (e.Button == MouseButtons.Right)
        { }
        else if (e.Button == MouseButtons.XButton1)
        { }
        else if (e.Button == MouseButtons.XButton2)
        { }

        // Redraw.
        Invalidate();
    }

    /// <summary>
    /// See if we're over a polygon or corner point.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseMove_NotDrawing(object sender, MouseEventArgs e)
    {
        var mousePoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);
        Cursor =
            MouseIsOverCornerPoint(mousePoint, envelope).Success ? Cursors.Arrow :
            MouseIsOverCornerPoint(mousePoint, group).Success ? Cursors.Arrow :
            MouseIsOverEdge(mousePoint, group).Success ? Cursors.Cross :
            MouseIsOverPolygon(mousePoint, group).Success ? Cursors.Hand :
            Cursors.Cross;
        Invalidate();
    }

    /// <summary>
    /// Move the next point in the new polygon.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseMove_Drawing(object sender, MouseEventArgs e)
    {
        newPoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);
        Invalidate();
    }

    /// <summary>
    /// Finish moving the selected polygon.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseUp_Drawing(object sender, MouseEventArgs e)
    {
        var mousePoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);

        // We are already drawing a polygon.
        // If it's the right mouse button, finish this polygon.
        if (e.Button == MouseButtons.Right)
        {
            // Finish this polygon.
            if (newPolygon?.Count > 2)
            {
                group?.Add(new Polygon(newPolygon));
            }

            newPolygon = null;

            // We no longer are drawing.
            MouseMove += CanvasControl_MouseMove_NotDrawing;
            MouseMove -= CanvasControl_MouseMove_Drawing;
            MouseUp -= CanvasControl_MouseUp_Drawing;
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
        polygonsDistorted = Distort(group, polygonsBounds, envelope);

        // Redraw.
        Invalidate();
    }

    /// <summary>
    /// Move the selected corner.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseMove_MovingCorner(object sender, MouseEventArgs e)
    {
        var mousePoint = ScreenToObject(e.Location, scale);

        // Move the point.
        movingPolygon[movingPoint] = new PointF(mousePoint.X + offsetX, mousePoint.Y + offsetY);

        if (movingPolygon is IEnvelope envelope1)
        {
            envelope = envelope1;
        }

        //polygonsBounds = PolygonBounds(polygons).Value;
        polygonsDistorted = Distort(group, polygonsBounds, envelope);

        // Redraw.
        Invalidate();
    }

    /// <summary>
    /// Finish moving the selected corner.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseUp_MovingCorner(object sender, MouseEventArgs e)
    {
        MouseMove += CanvasControl_MouseMove_NotDrawing;
        MouseMove -= CanvasControl_MouseMove_MovingCorner;
        MouseUp -= CanvasControl_MouseUp_MovingCorner;
        MouseDoubleClick -= CanvasControl_MouseDoubleClick_Corner;
    }

    /// <summary>
    /// Handles the MouseDoubleClick event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseDoubleClick_Corner(object sender, MouseEventArgs e)
    {
        MouseMove += CanvasControl_MouseMove_NotDrawing;
        MouseMove -= CanvasControl_MouseMove_MovingCorner;
        MouseUp -= CanvasControl_MouseUp_MovingCorner;
        MouseDoubleClick -= CanvasControl_MouseDoubleClick_Corner;

        var (Success, movingPolygon, movingPoint) = MouseIsOverCornerPoint(ScreenToObjectTransposedMatrix(panPoint, e.Location, scale), group);

        if (Success)
        {
            movingPolygon.RemoveAt(movingPoint);
            if (movingPolygon.Count == 0)
            {
                group.Remove(movingPolygon as PolygonContour);
            }

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(group, polygonsBounds, envelope);

            // Redraw.
            Invalidate();
        }
    }

    /// <summary>
    /// Move the selected polygon.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseMove_MovingPolygon(object sender, MouseEventArgs e)
    {
        var mousePoint = ScreenToObject(e.Location, scale);

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
        polygonsDistorted = Distort(group, polygonsBounds, envelope);

        // Redraw.
        Invalidate();
    }

    /// <summary>
    /// Finish moving the selected polygon.
    /// </summary>
    /// <param name="sender">The <paramref name="sender" />.</param>
    /// <param name="e">The mouse event arguments.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseUp_MovingPolygon(object sender, MouseEventArgs e)
    {
        MouseMove += CanvasControl_MouseMove_NotDrawing;
        MouseMove -= CanvasControl_MouseMove_MovingPolygon;
        MouseUp -= CanvasControl_MouseUp_MovingPolygon;
        MouseDoubleClick -= CanvasControl_MouseDoubleClick_Polygon;
    }

    /// <summary>
    /// Handles the MouseDoubleClick event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    private void CanvasControl_MouseDoubleClick_Polygon(object sender, MouseEventArgs e)
    {
        var mouse_pt = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);

        MouseMove += CanvasControl_MouseMove_NotDrawing;
        MouseMove -= CanvasControl_MouseMove_MovingPolygon;
        MouseUp -= CanvasControl_MouseUp_MovingPolygon;
        MouseDoubleClick -= CanvasControl_MouseDoubleClick_Polygon;

        var (Success, movingPolygon) = MouseIsOverPolygon(mouse_pt, group);
        if (Success)
        {
            group.Remove(movingPolygon as PolygonContour);

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(group, polygonsBounds, envelope);

            // Redraw.
            Invalidate();
        }
    }

    /// <summary>
    /// Handles the Panning event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseMove_Panning(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Middle && panning)
        {
            panPoint = ScreenToObjectTransposedMatrix(startingPoint, e.Location, scale);

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(group, polygonsBounds, envelope);

            // Redraw.
            Invalidate();
        }
    }

    /// <summary>
    /// Handles the MouseUp Panning event of the Canvas control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseUp_Panning(object sender, MouseEventArgs e)
    {
        MouseMove += CanvasControl_MouseMove_NotDrawing;
        MouseMove -= CanvasControl_MouseMove_Panning;
        MouseUp -= CanvasControl_MouseUp_Panning;

        if (e.Button == MouseButtons.Middle && panning)
        {
            panning = false;

            //polygonsBounds = PolygonBounds(polygons).Value;
            polygonsDistorted = Distort(group, polygonsBounds, envelope);

            // Redraw.
            Invalidate();
        }
    }

    /// <summary>
    /// Handles the MouseWheel event of the Form1 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseWheel(object sender, MouseEventArgs e)
    {
        var previousScale = scale;
        scale = MouseWheelScaleFactor(scale, e.Delta);
        scale = scale < scale_per_delta ? scale_per_delta : scale;

        panPoint = ZoomAtTransposedMatrix(panPoint, e.Location, previousScale, scale);

        //polygonsBounds = PolygonBounds(polygons).Value;
        polygonsDistorted = Distort(group, polygonsBounds, envelope);

        // Redraw.
        Invalidate();
    }

    /// <summary>
    /// Handles the MouseEnter event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseEnter(object sender, EventArgs e)
    { }

    /// <summary>
    /// Handles the MouseHover event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseHover(object sender, EventArgs e)
    { }

    /// <summary>
    /// Handles the MouseLeave event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_MouseLeave(object sender, EventArgs e)
    { }

    /// <summary>
    /// Handles the Enter event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_Enter(object sender, EventArgs e)
    { }

    /// <summary>
    /// Handles the Leave event of the CanvasControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CanvasControl_Leave(object sender, EventArgs e)
    { }
    #endregion

    #region Methods
    /// <summary>
    /// See if the mouse is over a corner point.
    /// </summary>
    /// <param name="mousePoint">The mouse_pt.</param>
    /// <param name="envelope">The envelope.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, IEnvelope envelope)
    {
        // See if we're over one of the Envelope's corner points.
        for (var i = 0; i < envelope.Count; i++)
        {
            // See if we're over this point.
            if (DistanceSquared(envelope[i], mousePoint) < HandleRadiusSquared * 2)
            {
                // We're over this point.
                return (true, envelope, i);
            }
        }

        return (false, null, -1);
    }

    /// <summary>
    /// See if the mouse is over a corner point.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="group">The group.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, Group group)
    {
        if (group is Group shapes)
        {
            // See if we're over a corner point.
            foreach (var shape in shapes)
            {
                switch (shape)
                {
                    case Group g:
                        {
                            var result = MouseIsOverCornerPoint(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    case Polygon g:
                        {
                            var result = MouseIsOverCornerPoint(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    case PolygonContour g:
                        {
                            var result = MouseIsOverCornerPoint(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return (false, null, -1);
    }

    /// <summary>
    /// See if the mouse is over a corner point.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="polygon">The polygon.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, Polygon polygon)
    {
        // See if we're over a corner point.
        foreach (var contour in polygon)
        {
            // See if we're over one of the polygon's corner points.
            for (var i = 0; i < contour.Count; i++)
            {
                // See if we're over this point.
                if (DistanceSquared(contour[i], mousePoint) < HandleRadiusSquared * 2)
                {
                    // We're over this point.
                    return (true, contour, i);
                }
            }
        }

        return (false, null, -1);
    }

    /// <summary>
    /// See if the mouse is over a corner point.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="polygonContour">The polygon contour.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, PolygonContour polygonContour)
    {
        // See if we're over one of the polygon's corner points.
        for (var i = 0; i < polygonContour.Count; i++)
        {
            // See if we're over this point.
            if (DistanceSquared(polygonContour[i], mousePoint) < HandleRadiusSquared * 2)
            {
                // We're over this point.
                return (true, polygonContour, i);
            }
        }

        return (false, null, -1);
    }

    /// <summary>
    /// See if the mouse is over a polygon's edge.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="group">The group.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, Group group)
    {
        if (group is Group shapes)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            for (var polygonIndex = shapes.Count - 1; polygonIndex >= 0; polygonIndex--)
            {
                var polygon = shapes[polygonIndex];
                switch (polygon)
                {
                    case Group g:
                        {
                            var result = MouseIsOverEdge(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    case Polygon g:
                        {
                            var result = MouseIsOverEdge(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    case PolygonContour g:
                        {
                            var result = MouseIsOverEdge(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return (false, null, -1, -1, new PointF(0, 0));
    }

    /// <summary>
    /// See if the mouse is over a polygon's edge.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="polygon">The polygon.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, Polygon polygon)
    {
        // Examine each polygon in reverse order to check the ones on top first.
        for (var polygonIndex = polygon.Count - 1; polygonIndex >= 0; polygonIndex--)
        {
            var contour = polygon[polygonIndex];

            // See if we're over one of the polygon's segments.
            var cursorIndex = contour.Count - 1;
            for (var pointIndex = 0; pointIndex < contour.Count; pointIndex++)
            {
                // See if we're over the segment between these points.
                var query = DistanceToLineSegmentSquared(mousePoint, contour[cursorIndex], contour[pointIndex]);
                if (query.Distnce < HandleRadiusSquared)
                {
                    // We are over this segment.
                    return (true, contour, cursorIndex, pointIndex, query.Point);
                }

                // Get the index of the polygon's next point.
                cursorIndex = pointIndex;
            }
        }

        return (false, null, -1, -1, new PointF(0, 0));
    }

    /// <summary>
    /// See if the mouse is over a polygon's edge.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="contour">The polygon contour.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, PolygonContour contour)
    {
        // See if we're over one of the polygon's segments.
        var cursorIndex = contour.Count - 1;
        for (var pointIndex = 0; pointIndex < contour.Count; pointIndex++)
        {
            // See if we're over the segment between these points.
            var query = DistanceToLineSegmentSquared(mousePoint, contour[cursorIndex], contour[pointIndex]);
            if (query.Distnce < HandleRadiusSquared)
            {
                // We are over this segment.
                return (true, contour, cursorIndex, pointIndex, query.Point);
            }

            // Get the index of the polygon's next point.
            cursorIndex = pointIndex;
        }

        return (false, null, -1, -1, new PointF(0, 0));
    }

    /// <summary>
    /// See if the mouse is over a polygon's body.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="group">The polygons.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, Group group)
    {
        if (group is Group shapes)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            for (var i = shapes.Count - 1; i >= 0; i--)
            {
                var polygon = shapes[i];
                switch (polygon)
                {
                    case Group g:
                        {
                            var result = MouseIsOverPolygon(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    case Polygon g:
                        {
                            var result = MouseIsOverPolygon(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    case PolygonContour g:
                        {
                            var result = MouseIsOverPolygon(mousePoint, g);
                            if (result.Success) return result;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return (false, null);
    }

    /// <summary>
    /// See if the mouse is over a polygon's body.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="polygons">The polygons.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, Polygon polygons)
    {
        // Examine each polygon in reverse order to check the ones on top first.
        for (var i = polygons.Count - 1; i >= 0; i--)
        {
            var inclusions = PolygonContourContainsPoint(polygons[i], mousePoint);
            if (inclusions == Inclusions.Inside || inclusions == Inclusions.Boundary)
            {
                return (true, polygons[i]);
            }
        }

        return (false, null);
    }

    /// <summary>
    /// See if the mouse is over a polygon's body.
    /// </summary>
    /// <param name="mousePoint">The mouse point.</param>
    /// <param name="contour">The polygons.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, PolygonContour contour)
    {
        // Examine each polygon in reverse order to check the ones on top first.
        var inclusions = PolygonContourContainsPoint(contour, mousePoint);
        if (inclusions == Inclusions.Inside || inclusions == Inclusions.Boundary)
        {
            return (true, contour);
        }

        return (false, null);
    }
    #endregion
}
