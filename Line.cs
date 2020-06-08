using System;
using Microsoft.Xna.Framework;

namespace CodeABitLitGame
{
    public class Line
    {
        public Point point1, point2;

        public Line(Point point1, Point point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        public override string ToString()
        {
            string output = "X1 : " + point1.X + " Y1 : " + point1.Y + " X2 : " + point2.X + " Y2 : " + point2.Y + " Length : " + Length();

            return output;
        }

        public float Length()
        {
            return (point1 - point2).ToVector2().Length();
        }

        public static float Length(Line line)
        {
            return line.Length();
        }

        public float angle()
        {
            return (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        }

        public static float angle(Line line)
        {
            return line.angle();
        }

        public static Line Empty = new Line(Point.Zero, Point.Zero);
    };

    static class LineHelperMethods
    {
        public static bool checkLineRectangleIntersection(Line line, Rectangle rectangle)
        {
            Line rectangeLeft = new Line(rectangle.Location, rectangle.Location + new Point(0, rectangle.Height));
            Line rectangeRight = new Line(rectangle.Location + new Point(rectangle.Width, 0), rectangle.Location + new Point(rectangle.Width, rectangle.Height));
            Line rectangeTop = new Line(rectangle.Location, rectangle.Location + new Point(rectangle.Width, 0));
            Line rectangeBottom = new Line(rectangle.Location + new Point(0, rectangle.Height), rectangle.Location + new Point(rectangle.Width, rectangle.Height));

            bool left = lineToLineIntersection(line, rectangeLeft);
            bool right = lineToLineIntersection(line, rectangeRight);
            bool top = lineToLineIntersection(line, rectangeTop);
            bool bottom = lineToLineIntersection(line, rectangeBottom);

            if (left || right || top || bottom)
            {
                return true;
            }

            return false;
        }

        public static bool lineToLineIntersection(Line line1, Line line2)
        {
            float x1 = line1.point1.X;
            float y1 = line1.point1.Y;
            float x2 = line1.point2.X;
            float y2 = line1.point2.Y;

            float x3 = line2.point1.X;
            float y3 = line2.point1.Y;
            float x4 = line2.point2.X;
            float y4 = line2.point2.Y;

            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }

            return false;
        }
    }
}
