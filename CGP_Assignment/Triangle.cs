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
        public Rectangle aabb;

        public Triangle(Point start, Point end)
        {
            startPoint = start;
            endPoint = end;
        }

        public override void draw(Graphics g, Pen blackPen)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            double xDiff, yDiff, xMid, yMid;   // range and mid points of x & y  

            // calculate ranges and mid points
            xDiff = endPoint.X - startPoint.X;
            yDiff = endPoint.Y - startPoint.Y;
            xMid = (endPoint.X + startPoint.X) / 2;
            yMid = (endPoint.Y + startPoint.Y) / 2;

            // draw triangle
            g.DrawLine(blackPen, (int)startPoint.X, (int)startPoint.Y, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            g.DrawLine(blackPen, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2), (int)endPoint.X, (int)endPoint.Y);
            g.DrawLine(blackPen, (int)endPoint.X, (int)endPoint.Y, (int)startPoint.X, (int)startPoint.Y);

            List<Point> points = new List<Point>();
            points.Add(new Point(startPoint.X, startPoint.Y));
            points.Add(new Point(endPoint.X, endPoint.Y));
            points.Add(new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2)));
            //points.Add(new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2)));

            int maxX = points.Max(p => p.X);
            int maxY = points.Max(p => p.Y);
            int minX = points.Min(p => p.X);
            int minY = points.Min(p => p.Y);

            // create bounding box
            aabb = new Rectangle(new Point(minX, minY), new Size(maxX - minX, maxY - minY));
            //g.DrawRectangle(Pens.Aquamarine, aabb);
        }

        public override bool contains(Point point)
        {
            base.contains(point);
            return aabb.Contains(point);
        }
    }
}
