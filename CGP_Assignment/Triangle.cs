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
        // rotation matrix
        //static float[,] matrix = new float[3, 3] { { 0.7071f, 0.7071f, 0.0f }, { -0.7071f, 0.7071f, 0.0f }, { 0.0f, 0.0f, 1.0f } };
        static float[,] matrix = new float[3, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 1.0f } };


       // public Point startPoint, endPoint;
        public Rectangle aabb;

        public Triangle(PointF start, PointF end, float rotation)
        {
            this.Start = start;
            this.End = end;
            this.RotationAngle = rotation;
        }

        public override void draw(Graphics g, Pen blackPen)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            double xDiff, yDiff, xMid, yMid;   // range and mid points of x & y  

            // calculate ranges and mid points
            xDiff = End.X - Start.X;
            yDiff = End.Y - Start.Y;
            xMid = (End.X + Start.X) / 2;
            yMid = (End.Y + Start.Y) / 2;

            // draw triangle
            g.DrawLine(blackPen, (int)Start.X, (int)Start.Y, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            g.DrawLine(blackPen, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2), (int)End.X, (int)End.Y);
            g.DrawLine(blackPen, (int)End.X, (int)End.Y, (int)Start.X, (int)Start.Y);

            List<Point> points = new List<Point>();
            points.Add(new Point((int)Start.X, (int)Start.Y));
            points.Add(new Point((int)End.X, (int)End.Y));
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


        public override void Rotate(double angle, ref PointF newStart, ref PointF newEnd)
        {
            // prepare the rotation matrix
            matrix[0, 0] = (float)Math.Cos(angle);
            matrix[0, 1] = (float)Math.Sin(angle);
            matrix[0, 2] = 0.0f;
            matrix[1, 0] = -(float)Math.Sin(angle);
            matrix[1, 1] = (float)Math.Cos(angle);
            matrix[1, 2] = 0.0f;


            double xMid = (End.X + Start.X) / 2;
            double yMid = (End.Y + Start.Y) / 2;

            float[] point1 = new float[2] { (int)(Start.X - xMid), (int)(Start.Y - yMid) };
            float[] point2 = new float[2] { (int)(End.X - xMid), (int)(End.Y - yMid) };

            float[] newPointKey = new float[2];
            float[] newPointOpp = new float[2];


            for (int col = 0; col < 2; col++)
            {
                newPointKey[col] = 0.0f;
                for (int index = 0; index < 2; index++)
                {
                    newPointKey[col] += point1[index] * matrix[index, col];
                }
            }

            for (int col = 0; col < 2; col++)
            {
                newPointOpp[col] = 0.0f;
                for (int index = 0; index < 2; index++)
                {
                    newPointOpp[col] += point2[index] * matrix[index, col];
                }
            }

            Start = new Point((int)(newPointKey[0] + xMid), (int)(newPointKey[1] + yMid));
            End = new Point((int)(newPointOpp[0] + xMid), (int)(newPointOpp[1] + yMid));


        }

    }
}
