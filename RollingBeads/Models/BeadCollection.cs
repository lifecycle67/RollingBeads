using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;

namespace RollingBeads.Models;

public class BeadCollection
{
    private List<Bead> _beads = new List<Bead>();

    public ReadOnlyCollection<Bead> Beads => new ReadOnlyCollection<Bead>(_beads);

    public BeadCollection(int beadCount, double oneCycleSeconds, Point firstBeadPoint, Point secondBeadPoint)
    {
        double tiltUnit = 180.0 / beadCount;
        double oneCycleFrame = oneCycleSeconds * 60;
        var originPoint = new Point((firstBeadPoint.X + secondBeadPoint.X) / 2, (firstBeadPoint.Y + secondBeadPoint.Y) / 2);

        for (int i = 0; i < beadCount; i++)
        {
            var beadTilt = (i * tiltUnit) % 360;

            var lineStart = new Point(PointCalculator.PointX(firstBeadPoint.X, firstBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt),
                                      PointCalculator.PointY(firstBeadPoint.X, firstBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt));

            var lineEnd = new Point(PointCalculator.PointX(secondBeadPoint.X, secondBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt),
                                    PointCalculator.PointY(secondBeadPoint.X, secondBeadPoint.Y, originPoint.X, originPoint.Y, beadTilt));

            // 각 선의 위상을 선의 기울기와 일치시키면 구슬들이 정확한 원 위에 배열된다.
            // 같은 선 위의 두 구슬은 반대 위상(180도 차이)으로 움직인다.
            // 시작 지연은 각 구슬의 궤도가 선의 끝점(cos = ±1)을 지나는 시각으로 잡아,
            // 초기에는 모든 구슬이 선의 양 끝에 배치되고 그 지점에서 연속적으로 출발한다.
            double startDelayFrame = oneCycleFrame * (180.0 - beadTilt) / 360.0;
            _beads.Add(new Bead(lineStart, lineEnd, beadTilt, beadTilt, oneCycleFrame, startDelayFrame));
            _beads.Add(new Bead(lineStart, lineEnd, beadTilt, beadTilt + 180, oneCycleFrame, startDelayFrame));
        }
    }
}
