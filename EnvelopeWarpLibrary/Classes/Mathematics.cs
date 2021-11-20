// <copyright file="Mathematics.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System.Drawing;
using System.Runtime.CompilerServices;
using static System.MathF;

namespace EnvelopeWarpLibrary;

/// <summary>
/// The Mathematics class.
/// </summary>
public static class Mathematics
{
    #region Constants
    /// <summary>
    /// The one third constant.
    /// </summary>
    public const float OneThird = 1f / 3f;

    /// <summary>
    /// The one half constant.
    /// </summary>
    public const float OneHalf = 1f / 2f;

    /// <summary>
    /// The two thirds constant.
    /// </summary>
    public const float TwoThirds = 2f / 3f;

    /// <summary>
    /// The scale per mouse wheel delta.
    /// </summary>
    public const float scale_per_delta = 0.1f / 120f;
    #endregion Constants

    #region Envelope Methods
    /// <summary>
    /// Distorts the specified group.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="envelope">The envelope.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Group Distort(Group group, RectangleF bounds, IEnvelope envelope)
    {
        if (group is null) return group!;
        var newGroup = new Group();
        foreach (var polygon in group)
        {
            switch (polygon)
            {
                case Group g:
                    newGroup.Add(Distort(g, bounds, envelope));
                    break;
                case Polygon p:
                    newGroup.Add(Distort(p, bounds, envelope));
                    break;
                case PolygonContour c:
                    newGroup.Add(Distort(c, bounds, envelope));
                    break;
                default:
                    break;
            }

            newGroup.Add(new Polygon());
        }

        return newGroup;
    }

    /// <summary>
    /// Distorts the specified polygon.
    /// </summary>
    /// <param name="polygon">The polygon.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="envelope">The envelope.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Polygon Distort(Polygon polygon, RectangleF bounds, IEnvelope envelope)
    {
        if (polygon is null) return polygon!;
        return new Polygon(Distort(polygon.Contours, bounds, envelope));
    }

    /// <summary>
    /// Distorts the specified polygons.
    /// </summary>
    /// <param name="polygons">The polygons.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="envelope">The envelope.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<Polygon> Distort(List<Polygon> polygons, RectangleF bounds, IEnvelope envelope)
    {
        if (polygons is null) return polygons!;
        var list = new List<Polygon>();
        foreach (var polygon in polygons)
        {
            list.Add(new Polygon(Distort(polygon.Contours, bounds, envelope)));
        }

        return list;
    }

    /// <summary>
    /// Distorts the specified polygons.
    /// </summary>
    /// <param name="polygons">The <paramref name="polygons" />.</param>
    /// <param name="bounds">The <paramref name="bounds" />.</param>
    /// <param name="envelope">The <paramref name="envelope" />.</param>
    /// <returns>
    /// The <see cref="T:List{PolygonContour}" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<PolygonContour> Distort(List<PolygonContour> polygons, RectangleF bounds, IEnvelope envelope)
    {
        if (polygons is null) return polygons!;
        var distortions = new List<PolygonContour>();
        foreach (var contour in polygons)
        {
            var distortion = new PolygonContour();

            // Set previous as the last point for closed shapes.
            var previous = contour[^1];
            foreach (var point in contour)
            {
                var dist = 1f / Distance(previous, point) * 2f;
                for (var i = 0f; i < 1; i += dist)
                {
                    distortion.Add(envelope.ProcessPoint(bounds, Lerp(previous, point, i)));
                }

                previous = point;
            }

            distortions.Add(distortion);
        }

        return distortions;
    }

    /// <summary>
    /// Distorts the specified polygon contour.
    /// </summary>
    /// <param name="contour">The contour.</param>
    /// <param name="bounds">The <paramref name="bounds" />.</param>
    /// <param name="envelope">The <paramref name="envelope" />.</param>
    /// <returns>
    /// The <see cref="T:List{PolygonContour}" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PolygonContour Distort(PolygonContour contour, RectangleF bounds, IEnvelope envelope)
    {
        if (contour is null) return contour!;
        var distortion = new PolygonContour();

        // Set previous as the last point for closed shapes.
        var previous = contour[^1];
        foreach (var point in contour)
        {
            var dist = 1f / Distance(previous, point) * 2f;
            for (var i = 0f; i < 1; i += dist)
            {
                distortion.Add(envelope.ProcessPoint(bounds, Lerp(previous, point, i)));
            }

            previous = point;
        }

        return distortion;
    }

    /// <summary>
    /// Warp the shape using Linear Envelope distortion.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="topLeft">The topLeft.</param>
    /// <param name="topRight">The topRight.</param>
    /// <param name="bottomRight">The bottomRight.</param>
    /// <param name="bottomLeft">The bottomLeft.</param>
    /// <returns>
    /// The <see cref="PointF" />.
    /// </returns>
    /// <acknowledgment>
    /// Based roughly on the ideas presented in: https://web.archive.org/web/20160825211055/http://www.neuroproductions.be:80/experiments/envelope-distort-with-actionscript/
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF LinearEnvelope(
        PointF point,
        RectangleF bounds,
        PointF topLeft, PointF topRight, PointF bottomRight, PointF bottomLeft)
    {
        // topLeft          topRight
        //   0-----------------0
        //   |                 |
        //   |                 |
        //   |                 |
        //   |                 |
        //   |                 |
        //   |                 |
        //   0-----------------0
        // bottomLeft   bottomRight
        // 
        // Install "Match Margin" Extension to enable word match highlighting, to help visualize where a variable resides in the ASCI map. 

        var normal = (X: (point.X - bounds.X) / bounds.Width, Y: (point.Y - bounds.Top) / bounds.Height);
        var leftAnchor = Lerp(topLeft, bottomLeft, normal.Y);
        var rightAnchor = Lerp(topRight, bottomRight, normal.Y);
        return Lerp(leftAnchor, rightAnchor, normal.X);
    }

    /// <summary>
    /// Warp the shape using Linear Envelope distortion.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="topLeft">The topLeft.</param>
    /// <param name="topRight">The topRight.</param>
    /// <param name="bottomRight">The bottomRight.</param>
    /// <param name="bottomLeft">The bottomLeft.</param>
    /// <returns>
    /// The <see cref="PointF" />.
    /// </returns>
    /// <acknowledgment>
    /// Based roughly on the ideas presented in: https://web.archive.org/web/20160825211055/http://www.neuroproductions.be:80/experiments/envelope-distort-with-actionscript/
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF LinearEnvelopeOptimized(
        PointF point,
        RectangleF bounds,
        PointF topLeft, PointF topRight, PointF bottomRight, PointF bottomLeft)
    {
        // topLeft          topRight
        //   0-----------------0
        //   |                 |
        //   |                 |
        //   |                 |
        //   |                 |
        //   |                 |
        //   |                 |
        //   0-----------------0
        // bottomLeft   bottomRight
        // 
        // Install "Match Margin" Extension to enable word match highlighting, to help visualize where a variable resides in the ASCI map. 

        var normal = (X: (point.X - bounds.X) / bounds.Width, Y: (point.Y - bounds.Top) / bounds.Height);
        var reverseNormal = (X: 1f - normal.X, Y: 1f - normal.Y);

        // Linear interpolate the left anchor node.
        var leftAnchor = (
            X: (reverseNormal.Y * topLeft.X) + (normal.Y * bottomLeft.X),
            Y: (reverseNormal.Y * topLeft.Y) + (normal.Y * bottomLeft.Y)
        );
        // Linear interpolate the right anchor node.
        var rightAnchor = (
            X: (reverseNormal.Y * topRight.X) + (normal.Y * bottomRight.X),
            Y: (reverseNormal.Y * topRight.Y) + (normal.Y * bottomRight.Y)
        );
        // Linear interpolate the final result.
        return new PointF(
            x: (reverseNormal.X * leftAnchor.X) + (normal.X * rightAnchor.X),
            y: (reverseNormal.X * leftAnchor.Y) + (normal.X * rightAnchor.Y)
        );
    }

    /// <summary>
    /// Warp the shape using Quadratic Bézier Envelope distortion.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="topLeft">The topLeft.</param>
    /// <param name="topHandle">The topLeftH.</param>
    /// <param name="topRight">The topRight.</param>
    /// <param name="rightHandle">The topRightV.</param>
    /// <param name="bottomRight">The bottomRight.</param>
    /// <param name="bottomHandle">The bottomRightH.</param>
    /// <param name="bottomLeft">The bottomLeft.</param>
    /// <param name="leftHandle">The topLeftV.</param>
    /// <returns>
    /// The <see cref="PointF" />.
    /// </returns>
    /// <acknowledgment>
    /// Based roughly on the ideas presented in: https://web.archive.org/web/20160825211055/http://www.neuroproductions.be:80/experiments/envelope-distort-with-actionscript/
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF QuadraticBezierEnvelope(
        PointF point,
        RectangleF bounds,
        PointF topLeft, PointF topHandle, PointF topRight, PointF rightHandle,
        PointF bottomRight, PointF bottomHandle, PointF bottomLeft, PointF leftHandle)
    {
        // topLeft                             topRight
        //   0------------------0------------------0
        //   |              topHandle              |
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   0 leftHandle              rightHandle 0
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   |             bottomHandle            |
        //   0------------------0------------------0
        // bottomLeft                       bottomRight
        // 
        // Install "Match Margin" Extension to enable word match highlighting, to help visualize where a variable resides in the ASCI map. 

        var normal = (X: (point.X - bounds.X) / bounds.Width, Y: (point.Y - bounds.Top) / bounds.Height);
        var leftAnchor = Qerp(topLeft, leftHandle, bottomLeft, normal.Y);
        var handle = Lerp(topHandle, bottomHandle, normal.Y);
        var rightAnchor = Qerp(topRight, rightHandle, bottomRight, normal.Y);
        return Qerp(leftAnchor, handle, rightAnchor, normal.X);
    }

    /// <summary>
    /// Warp the shape using Quadratic Bézier Envelope distortion.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="topLeft">The topLeft.</param>
    /// <param name="topHandle">The topLeftH.</param>
    /// <param name="topRight">The topRight.</param>
    /// <param name="rightHandle">The topRightV.</param>
    /// <param name="bottomRight">The bottomRight.</param>
    /// <param name="bottomHandle">The bottomRightH.</param>
    /// <param name="bottomLeft">The bottomLeft.</param>
    /// <param name="leftHandle">The topLeftV.</param>
    /// <returns>
    /// The <see cref="PointF" />.
    /// </returns>
    /// <acknowledgment>
    /// Based roughly on the ideas presented in: https://web.archive.org/web/20160825211055/http://www.neuroproductions.be:80/experiments/envelope-distort-with-actionscript/
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF QuadraticBezierEnvelopeOptimized(
        PointF point,
        RectangleF bounds,
        PointF topLeft, PointF topHandle, PointF topRight, PointF rightHandle,
        PointF bottomRight, PointF bottomHandle, PointF bottomLeft, PointF leftHandle)
    {
        // topLeft                             topRight
        //   0------------------0------------------0
        //   |              topHandle              |
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   0 leftHandle              rightHandle 0
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   |                                     |
        //   |             bottomHandle            |
        //   0------------------0------------------0
        // bottomLeft                       bottomRight
        // 
        // Install "Match Margin" Extension to enable word match highlighting, to help visualize where a variable resides in the ASCI map. 

        var normal = (X: (point.X - bounds.X) / bounds.Width, Y: (point.Y - bounds.Top) / bounds.Height);
        var normalSquared = (X: normal.X * normal.X, Y: normal.Y * normal.Y);
        var reverseNormal = (X: 1f - normal.X, Y: 1f - normal.Y);
        var reverseNormalSquared = (X: reverseNormal.X * reverseNormal.X, Y: reverseNormal.Y * reverseNormal.Y);

        // Quadratic interpolate the left anchor node.
        var leftAnchor = (
            X: (reverseNormalSquared.Y * topLeft.X) + (2f * reverseNormal.Y * normal.Y * leftHandle.X) + (normalSquared.Y * bottomLeft.X),
            Y: (reverseNormalSquared.Y * topLeft.Y) + (2f * reverseNormal.Y * normal.Y * leftHandle.Y) + (normalSquared.Y * bottomLeft.Y)
        );
        // Linear interpolate the left handle node.
        var handle = (
            X: (reverseNormal.Y * topHandle.X) + (normal.Y * bottomHandle.X),
            Y: (reverseNormal.Y * topHandle.Y) + (normal.Y * bottomHandle.Y)
        );
        // Quadratic interpolate the right anchor node.
        var rightAnchor = (
            X: (reverseNormalSquared.Y * topRight.X) + (2f * reverseNormal.Y * normal.Y * rightHandle.X) + (normalSquared.Y * bottomRight.X),
            Y: (reverseNormalSquared.Y * topRight.Y) + (2f * reverseNormal.Y * normal.Y * rightHandle.Y) + (normalSquared.Y * bottomRight.Y)
        );
        // Quadratic interpolate the final result.
        return new PointF(
            x: (reverseNormalSquared.X * leftAnchor.X) + (2f * reverseNormal.X * normal.X * handle.X) + (normalSquared.X * rightAnchor.X),
            y: (reverseNormalSquared.X * leftAnchor.Y) + (2f * reverseNormal.X * normal.X * handle.Y) + (normalSquared.X * rightAnchor.Y)
        );
    }

    /// <summary>
    /// Warp the shape using Cubic Bézier Envelope distortion.
    /// </summary>
    /// <param name="point">The <paramref name="point" />.</param>
    /// <param name="bounds">The <paramref name="bounds" />.</param>
    /// <param name="topLeft">The <paramref name="topLeft" />.</param>
    /// <param name="topLeftH">The <paramref name="topLeftH" />.</param>
    /// <param name="topLeftV">The <paramref name="topLeftV" />.</param>
    /// <param name="topRight">The <paramref name="topRight" />.</param>
    /// <param name="topRightH">The <paramref name="topRightH" />.</param>
    /// <param name="topRightV">The <paramref name="topRightV" />.</param>
    /// <param name="bottomRight">The <paramref name="bottomRight" />.</param>
    /// <param name="bottomRightH">The <paramref name="bottomRightH" />.</param>
    /// <param name="bottomRightV">The <paramref name="bottomRightV" />.</param>
    /// <param name="bottomLeft">The <paramref name="bottomLeft" />.</param>
    /// <param name="bottomLeftH">The <paramref name="bottomLeftH" />.</param>
    /// <param name="bottomLeftV">The <paramref name="bottomLeftV" />.</param>
    /// <returns>
    /// The <see cref="PointF" />.
    /// </returns>
    /// <acknowledgment>
    /// Based roughly on the ideas presented in: https://web.archive.org/web/20160825211055/http://www.neuroproductions.be:80/experiments/envelope-distort-with-actionscript/
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF CubicBezierEnvelope(
        PointF point,
        RectangleF bounds,
        PointF topLeft, PointF topLeftH, PointF topLeftV,
        PointF topRight, PointF topRightH, PointF topRightV,
        PointF bottomRight, PointF bottomRightH, PointF bottomRightV,
        PointF bottomLeft, PointF bottomLeftH, PointF bottomLeftV)
    {
        // topLeft                             topRight
        //   0--------0                 0----------0
        //   |   topLeftH             topRightH    |
        //   |                                     |
        //   |                                     |
        //   0 topLeftV                  topRightV 0
        //   
        //   
        //   
        //   0 bottomLeftV            bottomRightV 0
        //   |                                     |
        //   |                                     |
        //   |  bottomLeftH         bottomRightH   |
        //   0--------0                 0----------0
        // bottomLeft                       bottomRight
        // 
        // Install "Match Margin" Extension to enable word match highlighting, to help visualize where a variable resides in the ASCI map. 

        var normal = (X: (point.X - bounds.X) / bounds.Width, Y: (point.Y - bounds.Top) / bounds.Height);
        var leftAnchor = Cerp(topLeft, topLeftV, bottomLeftV, bottomLeft, normal.Y);
        var leftHandle = Lerp(topLeftH, bottomLeftH, normal.Y);
        var rightHandle = Lerp(topRightH, bottomRightH, normal.Y);
        var rightAnchor = Cerp(topRight, topRightV, bottomRightV, bottomRight, normal.Y);
        return Cerp(leftAnchor, leftHandle, rightHandle, rightAnchor, normal.X);
    }

    /// <summary>
    /// Warp the shape using Cubic Bézier Envelope distortion.
    /// </summary>
    /// <param name="point">The <paramref name="point" />.</param>
    /// <param name="bounds">The <paramref name="bounds" />.</param>
    /// <param name="topLeft">The <paramref name="topLeft" />.</param>
    /// <param name="topLeftH">The <paramref name="topLeftH" />.</param>
    /// <param name="topLeftV">The <paramref name="topLeftV" />.</param>
    /// <param name="topRight">The <paramref name="topRight" />.</param>
    /// <param name="topRightH">The <paramref name="topRightH" />.</param>
    /// <param name="topRightV">The <paramref name="topRightV" />.</param>
    /// <param name="bottomRight">The <paramref name="bottomRight" />.</param>
    /// <param name="bottomRightH">The <paramref name="bottomRightH" />.</param>
    /// <param name="bottomRightV">The <paramref name="bottomRightV" />.</param>
    /// <param name="bottomLeft">The <paramref name="bottomLeft" />.</param>
    /// <param name="bottomLeftH">The <paramref name="bottomLeftH" />.</param>
    /// <param name="bottomLeftV">The <paramref name="bottomLeftV" />.</param>
    /// <returns>
    /// The <see cref="PointF" />.
    /// </returns>
    /// <remarks>
    /// This is a more optimized version of <see cref="CubicBezierEnvelope(PointF, RectangleF, PointF, PointF, PointF, PointF, PointF, PointF, PointF, PointF, PointF, PointF, PointF, PointF)" />
    /// where the lerping magic is deduplicated and inlined.
    /// </remarks>
    /// <acknowledgment>
    /// Based roughly on the ideas presented in: https://web.archive.org/web/20160825211055/http://www.neuroproductions.be:80/experiments/envelope-distort-with-actionscript/
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF CubicBezierEnvelopeOptimized(
        PointF point,
        RectangleF bounds,
        PointF topLeft, PointF topLeftH, PointF topLeftV,
        PointF topRight, PointF topRightH, PointF topRightV,
        PointF bottomRight, PointF bottomRightH, PointF bottomRightV,
        PointF bottomLeft, PointF bottomLeftH, PointF bottomLeftV)
    {
        // topLeft                             topRight
        //   0--------0                 0----------0
        //   |   topLeftH             topRightH    |
        //   |                                     |
        //   |                                     |
        //   0 topLeftV                  topRightV 0
        //   
        //   
        //   
        //   0 bottomLeftV            bottomRightV 0
        //   |                                     |
        //   |                                     |
        //   |  bottomLeftH         bottomRightH   |
        //   0--------0                 0----------0
        // bottomLeft                       bottomRight
        // 
        // Install "Match Margin" Extension to enable word match highlighting, to help visualize where a variable resides in the ASCI map. 

        var normal = (X: (point.X - bounds.X) / bounds.Width, Y: (point.Y - bounds.Top) / bounds.Height);
        var normalSquared = (X: normal.X * normal.X, Y: normal.Y * normal.Y);
        var normalCubed = (X: normalSquared.X * normal.X, Y: normalSquared.Y * normal.Y);
        var reverseNormal = (X: 1f - normal.X, Y: 1f - normal.Y);
        var reverseNormalSquared = (X: reverseNormal.X * reverseNormal.X, Y: reverseNormal.Y * reverseNormal.Y);
        var reverseNormalCubed = (X: reverseNormalSquared.X * reverseNormal.X, Y: reverseNormalSquared.Y * reverseNormal.Y);

        // Cubic interpolate the left anchor node.
        var leftAnchor = (
            X: (topLeft.X * reverseNormalCubed.Y) + (3f * topLeftV.X * normal.Y * reverseNormalSquared.Y) + (3f * bottomLeftV.X * normalSquared.Y * reverseNormal.Y) + (bottomLeft.X * normalCubed.Y),
            Y: (topLeft.Y * reverseNormalCubed.Y) + (3f * topLeftV.Y * normal.Y * reverseNormalSquared.Y) + (3f * bottomLeftV.Y * normalSquared.Y * reverseNormal.Y) + (bottomLeft.Y * normalCubed.Y)
            );
        // Linear interpolate the left handle node.
        var leftHandle = (
            X: (topLeftH.X * reverseNormal.Y) + (bottomLeftH.X * normal.Y),
            Y: (topLeftH.Y * reverseNormal.Y) + (bottomLeftH.Y * normal.Y)
            );
        // Linear interpolate the right handle node.
        var rightHandle = (
            X: (topRightH.X * reverseNormal.Y) + (bottomRightH.X * normal.Y),
            Y: (topRightH.Y * reverseNormal.Y) + (bottomRightH.Y * normal.Y)
            );
        // Cubic interpolate the right anchor node.
        var rightAnchor = (
            X: (topRight.X * reverseNormalCubed.Y) + (3f * topRightV.X * normal.Y * reverseNormalSquared.Y) + (3f * bottomRightV.X * normalSquared.Y * reverseNormal.Y) + (bottomRight.X * normalCubed.Y),
            Y: (topRight.Y * reverseNormalCubed.Y) + (3f * topRightV.Y * normal.Y * reverseNormalSquared.Y) + (3f * bottomRightV.Y * normalSquared.Y * reverseNormal.Y) + (bottomRight.Y * normalCubed.Y)
            );
        // Cubic interpolate the final result.
        return new PointF(
            x: (leftAnchor.X * reverseNormalCubed.X) + (3f * leftHandle.X * normal.X * reverseNormalSquared.X) + (3f * rightHandle.X * normalSquared.X * reverseNormal.X) + (rightAnchor.X * normalCubed.X),
            y: (leftAnchor.Y * reverseNormalCubed.X) + (3f * leftHandle.Y * normal.X * reverseNormalSquared.X) + (3f * rightHandle.Y * normalSquared.X * reverseNormal.X) + (rightAnchor.Y * normalCubed.X)
            );
    }
    #endregion Envelope Methods

    #region Conversion Methods
    /// <summary>
    /// Converts a Line segment to a Quadratic Bézier curve.
    /// </summary>
    /// <param name="a">The starting point.</param>
    /// <param name="b">The end point.</param>
    /// <returns>
    /// Returns a Quadratic Bézier curve from the properties of a line segment.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (PointF a, PointF b, PointF c) LineSegmentToQuadraticBezier(PointF a, PointF b) => (a, Lerp(a, b, OneHalf), b);

    /// <summary>
    /// Converts a Line segment to a Cubic Bézier curve.
    /// </summary>
    /// <param name="a">The starting point.</param>
    /// <param name="b">The end point.</param>
    /// <returns>
    /// Returns a Cubic Bézier curve from the properties of a line segment.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (PointF a, PointF b, PointF c, PointF d) LineSegmentToCubicBezier(PointF a, PointF b) => (a, Lerp(a, b, OneThird), Lerp(a, b, TwoThirds), b);

    /// <summary>
    /// Raise a Quadratic Bezier to a Cubic Bezier.
    /// </summary>
    /// <param name="a">The starting point.</param>
    /// <param name="b">The handle.</param>
    /// <param name="c">The end point.</param>
    /// <returns>
    /// Returns Quadratic Bézier curve from a cubic curve.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (PointF a, PointF b, PointF c, PointF d) QuadraticBezierToCubicBezier(PointF a, PointF b, PointF c)
        => (new PointF(a.X, a.Y),
            new PointF(a.X + (TwoThirds * (b.X - a.X)), a.Y + (TwoThirds * (b.Y - a.Y))),
            new PointF(c.X + (TwoThirds * (b.X - c.X)), c.Y + (TwoThirds * (b.Y - c.Y))),
            new PointF(c.X, c.Y));
    #endregion Conversion Methods

    #region Interpolation Methods
    /// <summary>
    /// Two control point 2D Linear interpolation for ranges from 0 to 1, start to end of curve.
    /// </summary>
    /// <param name="a">The first anchor point.</param>
    /// <param name="b">The second anchor point.</param>
    /// <param name="t">The <paramref name="t" /> index of the linear curve.</param>
    /// <returns>
    /// Returns a <see cref="PointF" /> representing a point on the linear curve at the t index.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Lerp(PointF a, PointF b, float t) => new(
        ((1f - t) * a.X) + (t * b.X),
        ((1f - t) * a.Y) + (t * b.Y)
    );

    /// <summary>
    /// Three control point 2D Quadratic interpolation for ranges from 0 to 1, start to end of curve.
    /// </summary>
    /// <param name="a">The first anchor point.</param>
    /// <param name="b">The control node handle point.</param>
    /// <param name="c">The second anchor point.</param>
    /// <param name="t">The <paramref name="t" /> index of the quadratic curve.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Qerp(PointF a, PointF b, PointF c, float t) => new(
        ((1f - t) * (1f - t) * a.X) + (2f * (1f - t) * t * b.X) + (t * t * c.X),
        ((1f - t) * (1f - t) * a.Y) + (2f * (1f - t) * t * b.Y) + (t * t * c.Y)
    );

    /// <summary>
    /// Four control point 2D Cubic interpolation for ranges from 0 to 1, start to end of curve.
    /// </summary>
    /// <param name="a">The first anchor point.</param>
    /// <param name="b">The first control node handle point.</param>
    /// <param name="c">The second control node handle point.</param>
    /// <param name="d">The second anchor point.</param>
    /// <param name="t">The <paramref name="t" /> index of the cubic curve.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Cerp(PointF a, PointF b, PointF c, PointF d, float t) => new(
        ((1f - t) * (1f - t) * (1f - t) * a.X) + (3f * ((1f - t) * (1f - t)) * t * b.X) + (3f * (1f - t) * t * t * c.X) + (t * t * t * d.X),
        ((1f - t) * (1f - t) * (1f - t) * a.Y) + (3f * ((1f - t) * (1f - t)) * t * b.Y) + (3f * (1f - t) * t * t * c.Y) + (t * t * t * d.Y)
    );
    #endregion Interpolation Methods

    #region Distance Methods
    /// <summary>
    /// Calculates the distance between two points in 2-dimensional euclidean space.
    /// </summary>
    /// <param name="point1">First point.</param>
    /// <param name="point2">Second point.</param>
    /// <returns>
    /// Returns the distance between two points.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(PointF point1, PointF point2)
    {
        var dx = point2.X - point1.X;
        var dy = point2.Y - point1.Y;
        return Sqrt((dx * dx) + (dy * dy));
    }

    /// <summary>
    /// Calculate the distance squared between two points.
    /// </summary>
    /// <param name="point1">First point.</param>
    /// <param name="point2">Second point.</param>
    /// <returns>
    /// Returns the squared distance between two points.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DistanceSquared(PointF point1, PointF point2)
    {
        var dx = point1.X - point2.X;
        var dy = point1.Y - point2.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    /// Calculate the distance between point pt and the line segment p1 --&gt; p2.
    /// </summary>
    /// <param name="point">The <paramref name="point" />.</param>
    /// <param name="seg1">The <paramref name="seg1" />.</param>
    /// <param name="seg2">The <paramref name="seg2" />.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float Distnce, PointF Point) DistanceToLineSegment(PointF point, PointF seg1, PointF seg2)
    {
        var (dx, dy) = (seg2.X - seg1.X, seg2.Y - seg1.Y);
        if ((dx == 0f) && (dy == 0f))
        {
            // It's a point not a line segment.
            (dx, dy) = (point.X - seg1.X, point.Y - seg1.Y);
            return (Sqrt((dx * dx) + (dy * dy)), seg1);
        }

        // Calculate the t that minimizes the distance.
        var t = (((point.X - seg1.X) * dx) + ((point.Y - seg1.Y) * dy)) / ((dx * dx) + (dy * dy));
        PointF nearest;

        // See if this represents one of the segment's end points or a point in the middle.
        if (t < 0f)
        {
            (dx, dy, nearest) = (point.X - seg1.X, point.Y - seg1.Y, seg1);
        }
        else if (t > 1f)
        {
            (dx, dy, nearest) = (point.X - seg2.X, point.Y - seg2.Y, seg2);
        }
        else
        {
            nearest = new PointF(seg1.X + (t * dx), seg1.Y + (t * dy));
            (dx, dy) = (point.X - nearest.X, point.Y - nearest.Y);
        }

        return (Sqrt((dx * dx) + (dy * dy)), nearest);
    }

    /// <summary>
    /// Calculate the distance squared between point pt and the line segment p1 --&gt; p2.
    /// </summary>
    /// <param name="point">The <paramref name="point" />.</param>
    /// <param name="seg1">The <paramref name="seg1" />.</param>
    /// <param name="seg2">The <paramref name="seg2" />.</param>
    /// <returns>
    /// The <see cref="double" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float Distnce, PointF Point) DistanceToLineSegmentSquared(PointF point, PointF seg1, PointF seg2)
    {
        var (dx, dy) = (seg2.X - seg1.X, seg2.Y - seg1.Y);
        if ((dx == 0f) && (dy == 0f))
        {
            // It's a point not a line segment.
            dx = point.X - seg1.X;
            dy = point.Y - seg1.Y;
            return ((dx * dx) + (dy * dy), seg1);
        }

        // Calculate the t that minimizes the distance.
        var t = (((point.X - seg1.X) * dx) + ((point.Y - seg1.Y) * dy)) / ((dx * dx) + (dy * dy));
        PointF nearest;

        // See if this represents one of the segment's end points or a point in the middle.
        if (t < 0f)
        {
            (dx, dy, nearest) = (point.X - seg1.X, point.Y - seg1.Y, seg1);
        }
        else if (t > 1f)
        {
            (dx, dy, nearest) = (point.X - seg2.X, point.Y - seg2.Y, seg2);
        }
        else
        {
            nearest = new PointF(seg1.X + (t * dx), seg1.Y + (t * dy));
            (dx, dy) = (point.X - nearest.X, point.Y - nearest.Y);
        }

        return ((dx * dx) + (dy * dy), nearest);
    }
    #endregion Distance Methods

    #region Bounds Methods
    /// <summary>
    /// the Group bounds.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF? PolygonBounds(Group group)
    {
        var rect = new RectangleF();
        foreach (var shape in group)
        {
            switch (shape)
            {
                case Group g:
                    {
                        if (PolygonBounds(g) is RectangleF bounds) rect = Union(rect, bounds);
                    }
                    break;
                case Polygon p:
                    {
                        if (PolygonBounds(p) is RectangleF bounds) rect = Union(rect, bounds);
                    }
                    break;
                case PolygonContour c:
                    {
                        if (PolygonBounds(c) is RectangleF bounds) rect = Union(rect, bounds);
                    }
                    break;
                default:
                    break;
            }

        }
        return rect;
    }

    /// <summary>
    /// The polygon bounds.
    /// </summary>
    /// <param name="polygons">The <paramref name="polygons" />.</param>
    /// <returns>
    /// The <see cref="RectangleF" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF? PolygonBounds(List<Polygon> polygons)
    {
        var rect = new RectangleF();
        foreach (var polygon in polygons)
        {
            if (PolygonBounds(polygon.Contours) is RectangleF bounds) rect = Union(rect, bounds);
        }
        return rect;
    }

    /// <summary>
    /// The polygon bounds.
    /// </summary>
    /// <param name="polygon">The polygon.</param>
    /// <returns>
    /// The <see cref="RectangleF" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF? PolygonBounds(Polygon polygon)
    {
        var rect = new RectangleF();
        if (PolygonBounds(polygon.Contours) is RectangleF bounds) rect = Union(rect, bounds);
        return rect;
    }

    /// <summary>
    /// The polygon bounds.
    /// </summary>
    /// <param name="polygons">The <paramref name="polygons" />.</param>
    /// <returns>
    /// The <see cref="RectangleF" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF? PolygonBounds(List<PolygonContour> polygons)
    {
        var rect = new RectangleF();
        foreach (var polygon in polygons)
        {
            if (PolygonBounds(polygon.Points) is RectangleF bounds) rect = Union(rect, bounds);
        }
        return rect;
    }

    /// <summary>
    /// The polygon bounds.
    /// </summary>
    /// <param name="contour">The <paramref name="contour" />.</param>
    /// <returns>
    /// The <see cref="RectangleF" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF? PolygonBounds(PolygonContour contour)
    {
        var rect = new RectangleF();
        if (PolygonBounds(contour.Points) is RectangleF bounds) rect = Union(rect, bounds);
        return rect;
    }

    /// <summary>
    /// Calculates the external Axis Aligned Bounding Box (AABB) rectangle of a Polygon.
    /// </summary>
    /// <param name="polygonPoints">The <paramref name="polygonPoints" />.</param>
    /// <returns>
    /// Returns an Axis Aligned Bounding Box (AABB) <see cref="RectangleF" /> containing the polygon.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF? PolygonBounds(IEnumerable<PointF> polygonPoints)
    {
        if (polygonPoints is List<PointF> points && points.Count >= 1)
        {
            var left = points[0].X;
            var top = points[0].Y;
            var right = points[0].X;
            var bottom = points[0].Y;

            foreach (var point in points)
            {
                // ToDo: Measure performance impact of overwriting each time.
                left = point.X <= left ? point.X : left;
                top = point.Y <= top ? point.Y : top;
                right = point.X >= right ? point.X : right;
                bottom = point.Y >= bottom ? point.Y : bottom;
            }

            return RectangleF.FromLTRB(left, top, right, bottom);
        }
        return null;
    }

    /// <summary>
    /// Return a rectangle that is a union of this and a supplied Rectangle2D.
    /// </summary>
    /// <param name="rectA">The rectA.</param>
    /// <param name="rectB">The rectB.</param>
    /// <returns>
    /// The <see cref="RectangleF" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF Union(this RectangleF rectA, RectangleF rectB)
    {
        var left = Min(rectA.Left, rectB.Left);
        var top = Min(rectA.Top, rectB.Top);

        float width;
        // We need this check so that the math does not result in NaN
        if (double.IsPositiveInfinity(rectB.Width) || double.IsPositiveInfinity(rectA.Width))
        {
            width = float.PositiveInfinity;
        }
        else
        {
            //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
            var maxRight = Max(rectA.Right, rectB.Right);
            width = Max(maxRight - left, 0);
        }

        float height;
        // We need this check so that the math does not result in NaN
        if (double.IsPositiveInfinity(rectB.Height) || double.IsPositiveInfinity(rectA.Height))
        {
            height = float.PositiveInfinity;
        }
        else
        {
            //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
            var maxBottom = Max(rectA.Bottom, rectB.Bottom);
            height = Max(maxBottom - top, 0);
        }

        return new RectangleF(left, top, width, height);
    }
    #endregion Bounds Methods

    #region Contains Methods
    /// <summary>
    /// Determines whether the specified point is contained withing the set of regions defined by this Polygon.
    /// </summary>
    /// <param name="polygon">List of polygons.</param>
    /// <param name="point">The test point.</param>
    /// <param name="epsilon">The epsilon.</param>
    /// <returns>
    /// Returns <see cref="Inclusions.Outside" /> (0) if false, <see cref="Inclusions.Inside" /> (+1) if true, <see cref="Inclusions.Boundary" /> (-1) if the point is on a polygon boundary.
    /// </returns>
    /// <exception cref="ArgumentNullException">polygon</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Inclusions PolygonContainsPoint(List<PolygonContour> polygon, PointF point, float epsilon = float.Epsilon)
    {
        if (polygon is null)
        {
            throw new ArgumentNullException(nameof(polygon));
        }

        var returnValue = Inclusions.Outside;

        foreach (var poly in polygon)
        {
            // Use alternating rule with XOR to determine if the point is in a polygon or a hole.
            // If the point is in an odd number of polygons, it is inside. If even, it is a hole.
            returnValue ^= PolygonContourContainsPoint(poly, point, epsilon);

            // Any point on any boundary is on a boundary.
            if (returnValue == Inclusions.Boundary)
            {
                return Inclusions.Boundary;
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Determines whether the specified point is contained withing the region defined by this <see cref="PolygonContour" />.
    /// </summary>
    /// <param name="points">The points that form the corners of the polygon.</param>
    /// <param name="point">The test point.</param>
    /// <param name="epsilon">The <paramref name="epsilon" /> or minimal value to represent a change.</param>
    /// <returns>
    /// Returns <see cref="Inclusions.Outside" /> (0) if false, <see cref="Inclusions.Inside" /> (+1) if true, <see cref="Inclusions.Boundary" /> (-1) if the point is on a polygon boundary.
    /// </returns>
    /// <acknowledgment>
    /// Adapted from Clipper library: http://www.angusj.com/delphi/clipper.php
    /// See "The Point in Polygon Problem for Arbitrary Polygons" by Hormann and Agathos
    /// http://www.inf.usi.ch/hormann/papers/Hormann.2001.TPI.pdf
    /// </acknowledgment>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Inclusions PolygonContourContainsPoint(PolygonContour points, PointF point, float epsilon = float.Epsilon)
    {
        // Default value is no inclusion.
        var result = Inclusions.Outside;
        if (points is null) return result;

        // Special cases for points and line segments.
        if (points?.Count < 3)
        {
            if (points.Count == 1)
            {
                // If the polygon has 1 point, it is a point and has no interior, but a point can intersect a point.
                return (point.X == points[0].X && point.Y == points[0].Y) ? Inclusions.Boundary : Inclusions.Outside;
            }
            else if (points.Count == 2)
            {
                // If the polygon has 2 points, it is a line and has no interior, but a point can intersect a line.
                return ((point.X == points[0].X) && (point.Y == points[0].Y))
                    || ((point.X == points[1].X) && (point.Y == points[1].Y))
                    || (((point.X > points[0].X) == (point.X < points[1].X))
                    && ((point.Y > points[0].Y) == (point.Y < points[1].Y))
                    && ((point.X - points[0].X) * (points[1].Y - points[0].Y) == (point.Y - points[0].Y) * (points[1].X - points[0].X))) ? Inclusions.Boundary : Inclusions.Outside;
            }
            else
            {
                // Empty geometry.
                return Inclusions.Outside;
            }
        }

        // Loop through each line segment.
        var curPoint = points![0];
        for (var i = 1; i <= points.Count; ++i)
        {
            var nextPoint = i == points.Count ? points[0] : points[i];

            // Special case for horizontal lines. Check whether the point is on one of the ends, or whether the point is on the segment, if the line is horizontal.
            if (curPoint.Y == point.Y && (curPoint.X == point.X || ((nextPoint.Y == point.Y) && ((curPoint.X > point.X) == (nextPoint.X < point.X)))))
            //if ((Abs(nextPoint.Y - pY) < epsilon) && ((Abs(nextPoint.X - pX) < epsilon) || (Abs(curPoint.Y - pY) < epsilon && ((nextPoint.X > pX) == (curPoint.X < pX)))))
            {
                return Inclusions.Boundary;
            }

            // If Point between start and end points horizontally.
            //if ((curPoint.Y < pY) == (nextPoint.Y >= pY))
            if ((nextPoint.Y < point.Y) != (curPoint.Y < point.Y)) // At least one point is below the Y threshold and the other is above or equal
            {
                // Optimization: at least one point must be to the right of the test point
                // If point between start and end points vertically.
                if (nextPoint.X >= point.X)
                {
                    if (curPoint.X > point.X)
                    {
                        result = 1 - result;
                    }
                    else
                    {
                        var determinant = ((nextPoint.X - point.X) * (curPoint.Y - point.Y)) - ((curPoint.X - point.X) * (nextPoint.Y - point.Y));
                        if (Abs(determinant) < epsilon)
                        {
                            return Inclusions.Boundary;
                        }
                        else if ((determinant > 0) == (curPoint.Y > nextPoint.Y))
                        {
                            result = 1 - result;
                        }
                    }
                }
                else if (curPoint.X > point.X)
                {
                    var determinant = ((nextPoint.X - point.X) * (curPoint.Y - point.Y)) - ((curPoint.X - point.X) * (nextPoint.Y - point.Y));
                    if (Abs(determinant) < epsilon)
                    {
                        return Inclusions.Boundary;
                    }

                    if ((determinant > 0) == (curPoint.Y > nextPoint.Y))
                    {
                        result = 1 - result;
                    }
                }
            }

            curPoint = nextPoint;
        }

        return result;
    }
    #endregion Contains Methods

    /// <summary>
    /// Scales the factor.
    /// </summary>
    /// <param name="scale">The scale.</param>
    /// <param name="delta">The delta.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MouseWheelScaleFactor(float scale, int delta)
    {
        scale += delta * scale_per_delta;
        return (scale <= 0) ? 2f * float.Epsilon : scale;
    }

    #region Point Manipulation Methods
    /// <summary>
    /// Inverses the scale point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScreenToObject_(PointF point, float scale)
    {
        var invScale = 1f / scale;
        return new PointF(invScale * point.X, invScale * point.Y);
    }

    /// <summary>
    /// Screens to object.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScreenToObject(PointF point, float scale) => new(point.X / scale, point.Y / scale);

    /// <summary>
    /// Inverses the translation and scale of a point.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="point">The point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScreenToObject_(PointF offset, PointF point, float scale)
    {
        var invScale = 1f / scale;
        return new PointF((point.X - offset.X) * invScale, (point.Y - offset.Y) * invScale);
    }

    /// <summary>
    /// Screens to object. https://stackoverflow.com/a/37269366
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="point">The screen point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScreenToObject(PointF offset, PointF point, float scale) => new((point.X - offset.X) / scale, (point.Y - offset.Y) / scale);

    /// <summary>
    /// Screens to object transposed matrix.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="screenPoint">The screen point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScreenToObjectTransposedMatrix_(PointF offset, PointF screenPoint, float scale)
    {
        var invScale = 1f / scale;
        return new PointF((screenPoint.X * invScale) - offset.X, (screenPoint.Y * invScale) - offset.Y);
    }

    /// <summary>
    /// Screens to object transposed matrix. https://stackoverflow.com/a/37269366
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="screenPoint">The screen point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ScreenToObjectTransposedMatrix(PointF offset, PointF screenPoint, float scale) => new((screenPoint.X / scale) - offset.X, (screenPoint.Y / scale) - offset.Y);

    /// <summary>
    /// Objects to screen.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ObjectToScreen(PointF point, float scale) => new(point.X * scale, point.Y * scale);

    /// <summary>
    /// Objects to screen. https://stackoverflow.com/a/37269366
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="point">The object point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ObjectToScreen(PointF offset, PointF point, float scale) => new(offset.X + (point.X * scale), offset.Y + (point.Y * scale));

    /// <summary>
    /// Objects to screen transposed matrix. https://stackoverflow.com/a/37269366
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="objectPoint">The object point.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ObjectToScreenTransposedMatrix(PointF offset, PointF objectPoint, float scale) => new((offset.X + objectPoint.X) * scale, (offset.Y + objectPoint.Y) * scale);

    /// <summary>
    /// Zooms at. https://stackoverflow.com/a/37269366
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="cursor">The cursor.</param>
    /// <param name="previousScale">The previous scale.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ZoomAt(PointF offset, PointF cursor, float previousScale, float scale)
    {
        var point = ScreenToObject(offset, cursor, previousScale);
        point = ObjectToScreen(offset, point, scale);
        return new PointF(offset.X + ((cursor.X - point.X) / scale), offset.Y + ((cursor.Y - point.Y) / scale));
    }

    /// <summary>
    /// Zooms at for a transposed matrix. https://stackoverflow.com/a/37269366
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="cursor">The cursor.</param>
    /// <param name="previousScale">The previous scale.</param>
    /// <param name="scale">The scale.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ZoomAtTransposedMatrix(PointF offset, PointF cursor, float previousScale, float scale)
    {
        var point = ScreenToObjectTransposedMatrix(offset, cursor, previousScale);
        point = ObjectToScreenTransposedMatrix(offset, point, scale);
        return new PointF(offset.X + ((cursor.X - point.X) / scale), offset.Y + ((cursor.Y - point.Y) / scale));
    }

    #region Subtract Point
    /// <summary>
    /// Subtracts the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Subtract(this PointF minuend, PointF subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

    /// <summary>
    /// Subtracts the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Subtract(this PointF minuend, Point subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

    /// <summary>
    /// Subtracts the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Subtract(this Point minuend, PointF subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

    /// <summary>
    /// Subtracts the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Subtract(this Point minuend, Point subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);
    #endregion

    #region Add Point
    /// <summary>
    /// Adds the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Add(this PointF minuend, PointF subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);

    /// <summary>
    /// Adds the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Add(this PointF minuend, Point subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);

    /// <summary>
    /// Adds the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Add(this Point minuend, PointF subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);

    /// <summary>
    /// Adds the specified subtrahend.
    /// </summary>
    /// <param name="minuend">The minuend.</param>
    /// <param name="subtrahend">The subtrahend.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Add(this Point minuend, Point subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);
    #endregion

    #region Scale Point
    /// <summary>
    /// Scales the specified multiplier.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="scaler">The scaler.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Scale(this PointF multiplicand, float scaler) => new(multiplicand.X * scaler, multiplicand.Y * scaler);

    /// <summary>
    /// Scales the specified multiplier.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="scaler">The scaler.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Scale(this PointF multiplicand, SizeF scaler) => new(multiplicand.X * scaler.Width, multiplicand.Y * scaler.Height);

    /// <summary>
    /// Scales the specified scaler.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="scaler">The scaler.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Scale(this Point multiplicand, float scaler) => new(multiplicand.X * scaler, multiplicand.Y * scaler);

    /// <summary>
    /// Scales the specified scaler.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="scaler">The scaler.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Scale(this Point multiplicand, SizeF scaler) => new(multiplicand.X * scaler.Width, multiplicand.Y * scaler.Height);

    /// <summary>
    /// Scales the specified scaler.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="scaler">The scaler.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Scale(this Point multiplicand, int scaler) => new(multiplicand.X * scaler, multiplicand.Y * scaler);

    /// <summary>
    /// Scales the specified scaler.
    /// </summary>
    /// <param name="multiplicand">The multiplicand.</param>
    /// <param name="scaler">The scaler.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Scale(this Point multiplicand, Size scaler) => new(multiplicand.X * scaler.Width, multiplicand.Y * scaler.Height);
    #endregion Scale Point

    #region Point To String
    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="format">The format.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(this PointF point, string? format, IFormatProvider provider) => $"{{X={point.X.ToString(format, provider)}, Y={point.Y.ToString(format, provider)}}}";

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="format">The format.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(this Point point, string? format, IFormatProvider provider) => $"{{X={point.X.ToString(format, provider)}, Y={point.Y.ToString(format, provider)}}}";
    #endregion Point To String
    #endregion Point Manipulation Methods
}
