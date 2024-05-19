using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollingBeads.Models;

public class PointCalculator
{
    public static double PointX(double x, double y, double originX, double originY, double tileAngle)
    {
        return (x - originX) * Math.Cos(GetRadian(tileAngle)) - (y - originY) * Math.Sin(GetRadian(tileAngle)) + originX;
    }

    public static double PointY(double x, double y, double originX, double originY, double tileAngle)
    {
        return (x - originX) * Math.Sin(GetRadian(tileAngle)) - (y - originY) * Math.Cos(GetRadian(tileAngle)) + originY;
    }

    public static double AngleBetweenPoint(Point p1, Point p2)
    {
        var radian = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
        return radian * 180 / Math.PI;
    }

    private static double GetRadian(double angle)
    {
        return Math.PI * angle / 180.0;
    }
}
