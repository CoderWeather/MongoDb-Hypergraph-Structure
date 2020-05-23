using System;
using System.Windows;

namespace HyperGraphSharp.Extensions
{
    public static class PointExtensions
    {
        public static double AngleWith(this Point point1, Point point2) =>
            Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);

        public static double DistanceTo(this Point point1, Point point2) =>
            Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
    }
}