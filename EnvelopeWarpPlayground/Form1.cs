// <copyright file="Form1.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using EnvelopeWarpLibrary;
using System.Runtime.CompilerServices;
using static EnvelopeWarpLibrary.Mathematics;

namespace EnvelopeWarpPlayground;

/// <summary>
/// The form1 class.
/// </summary>
/// <seealso cref="Form" />
public partial class Form1
    : Form
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Form1" /> class.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Form1()
    {
        InitializeComponent();
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles the Load event of the Form1 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Form1_Load(object sender, EventArgs e)
    {
        var (left, top, width, height) = (100f, 100f, 300f, 200f);
        var (hStroke, vStroke, hOffset, vOffset, hMargin, vMargin) = (width / 6f, height / 4f, width, 0f, 25f, 0f);

        // H.
        var h = new PolygonContour(
            new PointF(left, top),
            new PointF(left + hStroke, top),
            new PointF(left + hStroke, top + (1.5f * vStroke)),
            new PointF(left + (2f * hStroke), top + (1.5f * vStroke)),
            new PointF(left + (2f * hStroke), top),
            new PointF(left + (3f * hStroke), top),
            new PointF(left + (3f * hStroke), top + height),
            new PointF(left + (2f * hStroke), top + height),
            new PointF(left + (2f * hStroke), top + (2.5f * vStroke)),
            new PointF(left + hStroke, top + (2.5f * vStroke)),
            new PointF(left + hStroke, top + height),
            new PointF(left, top + height)
        );

        // I.
        var i = new PolygonContour(
            new PointF(left + (3f * hStroke), top),
            new PointF(left + width, top),
            new PointF(left + width, top + vStroke),
            new PointF(left + (5f * hStroke), top + vStroke),
            new PointF(left + (5f * hStroke), top + (3f * vStroke)),
            new PointF(left + width, top + (3f * vStroke)),
            new PointF(left + width, top + height),
            new PointF(left + (3f * hStroke), top + height),
            new PointF(left + (3f * hStroke), top + (3f * vStroke)),
            new PointF(left + (4f * hStroke), top + (3f * vStroke)),
            new PointF(left + (4f * hStroke), top + vStroke),
            new PointF(left + (3f * hStroke), top + vStroke)
        );

        canvasControl.Document = new Group(new Polygon(h, i));

        //canvasControl.Envelope = new LinearEnvelope(left + width, top, width, height);
        //canvasControl.Envelope = new QuadraticEnvelope(left + width, top, width, height);
        canvasControl.Envelope = new CubicEnvelope(left + hOffset + hMargin, top + vOffset + vMargin, width, height);

        //canvasControl.DocumentBounds = PolygonBounds(polygons).Value;
        canvasControl.DocumentBounds = new RectangleF(left, top, width, height);
        canvasControl.DistortedDocument = Distort(canvasControl.Document, canvasControl.DocumentBounds, canvasControl.Envelope);
    }

    /// <summary>
    /// Handles the Click event of the ButtonResetPan control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void ButtonResetPan_Click(object sender, EventArgs e)
    {
        canvasControl.Pan = new PointF(0f, 0f);
    }

    /// <summary>
    /// Handles the Click event of the ButtonResetScale control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void ButtonResetScale_Click(object sender, EventArgs e)
    {
        canvasControl.Zoom = 1;
    }
    #endregion Event Handlers
}
