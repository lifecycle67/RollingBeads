using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RollingBeads.Models;

public class BeadCollection
{
    private List<Bead> _firstBeads = new List<Bead>();
    private List<Bead> _secondBeads = new List<Bead>();
    private double _tiltUnit = 0;

    public ReadOnlyCollection<Bead> Beads => new ReadOnlyCollection<Bead>(_firstBeads.Union(_secondBeads).ToList());

    public BeadCollection(int beadCount, double oneCycleSeconds, Point firstBeadPoint, Point secondBeadPoint)
    {
        _tiltUnit = 180.0 / beadCount;
        var originPoint = new Point((firstBeadPoint.X + secondBeadPoint.X) / 2, (firstBeadPoint.Y + secondBeadPoint.Y) / 2);

        for (int i = 0; i < beadCount; i++)
        {
            bool isOtherOrthogonal = false;

            var beadTilt = (i * _tiltUnit) % 360;

            var p1x = PointCalculator.PointX(firstBeadPoint.X, firstBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt);
            var p1y = PointCalculator.PointY(firstBeadPoint.X, firstBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt);
            var p2x = PointCalculator.PointX(secondBeadPoint.X, secondBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt);
            var p2y = PointCalculator.PointY(secondBeadPoint.X, secondBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt);

            var rightPoint = new Point(PointCalculator.PointX(firstBeadPoint.X,
                                                          firstBeadPoint.Y,
                                                          originPoint.X,
                                                          originPoint.Y,
                                                          _tiltUnit * i),
                                   PointCalculator.PointY(firstBeadPoint.X,
                                                          firstBeadPoint.Y,
                                                          originPoint.X,
                                                          originPoint.Y,
                                                          _tiltUnit * i));

            var leftPoint = new Point(PointCalculator.PointX(secondBeadPoint.X,
                                                          secondBeadPoint.Y,
                                                          originPoint.X,
                                                          originPoint.Y,
                                                          _tiltUnit * i),
                                   PointCalculator.PointY(secondBeadPoint.X,
                                                          secondBeadPoint.Y,
                                                          originPoint.X,
                                                          originPoint.Y,
                                                          _tiltUnit * i));

            double halfCycleDuration = 1000.0 * Convert.ToDouble(oneCycleSeconds / 2);
            double frameMs = halfCycleDuration / Convert.ToDouble(oneCycleSeconds / 2 * 60);
            double delayFrame = Convert.ToDouble(oneCycleSeconds / 2) * 60 / (double)(beadCount + 1);
            double delayMs = frameMs * delayFrame * i + 20;

            _firstBeads.Add(new Bead(p1x,
                                p1y,
                                rightPoint,
                                leftPoint,
                                beadTilt,
                                originPoint,
                                delayMs,
                                oneCycleSeconds * 60));

            if (beadTilt == 0)
                isOtherOrthogonal = true;

            _secondBeads.Add(new Bead(p2x,
                                p2y,
                                rightPoint,
                                leftPoint,
                                beadTilt,
                                originPoint,
                                delayMs,
                                oneCycleSeconds * 60,
                                isOtherOrthogonal));
        }

        Debug.WriteLine("FIRST " + Environment.NewLine + string.Join(Environment.NewLine, _firstBeads.Select(b => $"X:{b.XPoint} Y:{b.YPoint} A:{b.TiltAngle} D:{b.DelayMilliseconds}")));
        Debug.WriteLine("SECOND " + Environment.NewLine + string.Join(Environment.NewLine, _secondBeads.Select(b => $"X:{b.XPoint} Y:{b.YPoint} A:{b.TiltAngle} D:{b.DelayMilliseconds}")));
    }
}
