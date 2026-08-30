using System;
using Windows.Foundation;

namespace RollingBeads.Models;

/// <summary>
/// 직선 위를 단순 조화 운동(사인파)으로 왕복하는 구슬.
/// 위치는 프레임 번호의 함수로 직접 계산하므로 오차가 누적되지 않으며,
/// 선의 기울기와 같은 위상을 주면 전체 구슬이 정확한 원 위에 놓인다(Tusi couple).
/// startDelayFrame을 주면 그 시점의 궤도 위치(선의 끝점이 되도록 선택)에
/// 정지해 있다가 해당 프레임부터 연속적으로 운동에 합류한다.
/// </summary>
public class Bead
{
    private readonly double _centerX;
    private readonly double _centerY;
    private readonly double _directionX;
    private readonly double _directionY;
    private readonly double _amplitude;
    private readonly double _phaseRadian;
    private readonly double _oneCycleFrame;
    private readonly double _startDelayFrame;

    private long _frameCount = 0;
    private double _xPoint;
    private double _yPoint;

    public double TiltAngle { get; private set; }
    public double MaxXPoint { get; private set; }
    public double MinXPoint { get; private set; }
    public double MinYPoint { get; private set; }
    public double MaxYPoint { get; private set; }

    public double XPoint => _xPoint;
    public double YPoint => _yPoint;

    public Bead(Point lineStart,
                Point lineEnd,
                double tiltAngle,
                double phaseDegree,
                double oneCycleFrame = 120.0,
                double startDelayFrame = 0)
    {
        TiltAngle = tiltAngle % 360;
        _oneCycleFrame = oneCycleFrame;
        _phaseRadian = Math.PI * phaseDegree / 180.0;
        _startDelayFrame = startDelayFrame;

        MinXPoint = lineStart.X;
        MinYPoint = lineStart.Y;
        MaxXPoint = lineEnd.X;
        MaxYPoint = lineEnd.Y;

        _centerX = (lineStart.X + lineEnd.X) / 2;
        _centerY = (lineStart.Y + lineEnd.Y) / 2;

        double lengthX = lineEnd.X - lineStart.X;
        double lengthY = lineEnd.Y - lineStart.Y;
        double length = Math.Sqrt(lengthX * lengthX + lengthY * lengthY);

        _amplitude = length / 2;
        _directionX = length == 0 ? 0 : lengthX / length;
        _directionY = length == 0 ? 0 : lengthY / length;

        UpdatePosition();
    }

    public Task Move()
    {
        _frameCount++;
        UpdatePosition();
        return Task.CompletedTask;
    }

    private void UpdatePosition()
    {
        // 시작 지연 전에는 지연 시점의 궤도 위치(선의 끝점)에 정지해 있으므로,
        // 지연이 끝나는 순간 위치가 연속적으로 이어진다.
        double effectiveFrame = Math.Max(_frameCount, _startDelayFrame);
        double angle = 2 * Math.PI * effectiveFrame / _oneCycleFrame + _phaseRadian;
        double offset = _amplitude * Math.Cos(angle);

        _xPoint = _centerX + _directionX * offset;
        _yPoint = _centerY + _directionY * offset;
    }
}
