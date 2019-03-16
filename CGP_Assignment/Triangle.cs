using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CGP_Assignment
{
    class Triangle : Shape
    {
        private Point startPoint, endPoint;
        public Triangle(Point start, Point end)
        {
            startPoint = start;
            endPoint = end;
        }

        public override void draw(Graphics g, Pen blackPen)
        {
           // g.DrawPolygon(blackPen, );
        }

        //private Point[] CalculateVertices(int startingAngle, Point center)
        //{
        //    double radius = Math.Sqrt(Math.Pow((endPoint.Y - startPoint.Y), 2) + Math.Pow((endPoint.X - startPoint.X), 2));

        //    List<Point> points = new List<Point>();
        //    float step = 360.0f / 3;

        //    PointF xy = new PointF();
        //    double radians = degrees * Math.PI / 180.0;

        //    xy.X = (float)Math.Cos(radians) * radius + origin.X;
        //    xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;


        //    float angle = startingAngle; //starting angle
        //    for (double i = startingAngle; i < startingAngle + 360.0; i += step) //go in a full circle
        //    {
        //        points.Add(DegreesToXY(angle, radius, center)); //code snippet from above
        //        angle += step;
        //    }

        //    return points.ToArray();
        //}

    }
}
