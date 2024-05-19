using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RollingBeads.Models;
public class Bead
{
    private double _xPoint = 0;
    private double _yPoint = 0;
    private QuadrantType _quadrant = QuadrantType.FirstQuadrant;

    private double _movingDistanceXPerFrame = 0;
    private double _movingDistanceYPerFrame = 0;

    private int _frameCount = 0;
    private int _initiateCount = 0;

    private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
    private double _delayMilliseconds = 0;
    private double _halfCycleFrame = 60.0;

    public double TiltAngle { get; private set; }
    public double MaxXPoint { get; private set; }
    public double MinXPoint { get; private set; }
    public double MinYPoint { get; private set; }
    public double MaxYPoint { get; private set; }

    public double OriginX { get; private set; }
    public double XPoint => _xPoint;
    public double YPoint => _yPoint;
    public QuadrantType Quadrant => _quadrant;
    public double DelayMilliseconds => _delayMilliseconds;

    public Bead(double x,
                double y,
                Point rightEndPoint,
                Point leftEndPoint,
                double tiltAngle,
                Point origin,
                double delayMilliSeconds,
                double oneCycleFrame = 120.0,
                bool isOtherOrthogonal = false)
    {
        _halfCycleFrame = oneCycleFrame / 2;
        _delayMilliseconds = delayMilliSeconds;

        TiltAngle = tiltAngle % 360;

        if (x >= origin.X && y > origin.Y)
            _quadrant = QuadrantType.FirstQuadrant;
        else if (x < origin.X && y >= origin.Y)
            _quadrant = QuadrantType.SecondQuadrant;
        else if (x < origin.X && y <= origin.Y)
            _quadrant = QuadrantType.ThirdQuadrant;
        else if (x > origin.X && y <= origin.Y)
            _quadrant = QuadrantType.FourthQuadrant;

        _xPoint = x;
        _yPoint = y;
        MinXPoint = leftEndPoint.X;
        MaxXPoint = rightEndPoint.X;
        MinYPoint = leftEndPoint.Y;
        MaxYPoint = rightEndPoint.Y;

        double largeX = MaxXPoint > MinXPoint ? MaxXPoint : MinXPoint;
        double smallX = MaxXPoint < MinXPoint ? MaxXPoint : MinXPoint;
        double largeY = MaxYPoint > MinYPoint ? MaxYPoint : MinYPoint;
        double smallY = MaxYPoint < MinYPoint ? MaxYPoint : MinYPoint;

        _movingDistanceXPerFrame = Math.Abs(largeX - smallX) / _halfCycleFrame;
        _movingDistanceYPerFrame = Math.Abs(largeY - smallY) / _halfCycleFrame;

        if (isOtherOrthogonal == true)
            _quadrant = QuadrantType.ThirdQuadrant;

        Debug.WriteLine($"x : {x} y : {y} init count : {_initiateCount}  tilt : {TiltAngle} quadrant : {_quadrant.ToString()}");
    }

    public async Task Move()
    {
        if (await _semaphoreSlim.WaitAsync(TimeSpan.FromMilliseconds(_delayMilliseconds)))
            return;

        if (((_initiateCount == 0 && _frameCount == 0) || _frameCount >= _halfCycleFrame))
        {
            _frameCount = _frameCount % (int)_halfCycleFrame;

            if (_quadrant == QuadrantType.FirstQuadrant)
                _quadrant = QuadrantType.ThirdQuadrant;
            else if (_quadrant == QuadrantType.SecondQuadrant)
                _quadrant = QuadrantType.FourthQuadrant;
            else if (_quadrant == QuadrantType.ThirdQuadrant)
                _quadrant = QuadrantType.FirstQuadrant;
            else if (_quadrant == QuadrantType.FourthQuadrant)
                _quadrant = QuadrantType.SecondQuadrant;
        }

        switch (_quadrant)
        {
            case QuadrantType.FirstQuadrant:
                _xPoint += _movingDistanceXPerFrame;
                _yPoint += _movingDistanceYPerFrame;
                break;
            case QuadrantType.SecondQuadrant:
                _xPoint -= _movingDistanceXPerFrame;
                _yPoint += _movingDistanceYPerFrame;
                break;
            case QuadrantType.ThirdQuadrant:
                _xPoint -= _movingDistanceXPerFrame;
                _yPoint -= _movingDistanceYPerFrame;
                break;
            case QuadrantType.FourthQuadrant:
                _xPoint += _movingDistanceXPerFrame;
                _yPoint -= _movingDistanceYPerFrame;
                break;
            default:
                break;
        }

        if (_initiateCount > 0)
            _initiateCount--;
        else
            _frameCount++;
    }

    public enum QuadrantType
    {
        FirstQuadrant,
        SecondQuadrant,
        ThirdQuadrant,
        FourthQuadrant
    }
}
