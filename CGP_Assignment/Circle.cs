using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CGP_Assignment
{
    class Circle : Shape
    {
        double radius;
        static float[,] matrix = new float[3, 3];

        public Circle(PointF start, PointF end, float rotation, float scaleFactor)
        {
            this.Start = start;
            this.End = end;
            this.RotationAngle = rotation;
            this.ScaleFactor = scaleFactor;

        }

        public override void draw(Graphics g, Pen pen)
        {
            // create copies of the start and end points to modify in transformations
            PointF newStart = new PointF(Start.X, Start.Y);
            PointF newEnd = new PointF(End.X, End.Y);

            // apply transformations
            this.Transform(RotationAngle, ref newStart, ref newEnd);

            // calculate circle's radius based on transformed points 
            radius = Math.Sqrt(Math.Pow((newEnd.Y - newStart.Y), 2) + Math.Pow((newEnd.X - newStart.X), 2));

            // draw circle
            g.DrawEllipse(pen, (float)(newStart.X - radius), (float)(newStart.Y - radius),
                      (float)(radius + radius), (float)(radius + radius));
        }

        public override void Transform(double angle, ref PointF newStart, ref PointF newEnd)
        {
            // generate identity matrix
            matrix[0, 0] = 1.0f;
            matrix[0, 1] = 0.0f;
            matrix[0, 2] = 0.0f;
            matrix[1, 0] = 0.0f;
            matrix[1, 1] = 1.0f;
            matrix[1, 2] = 0.0f;
            matrix[2, 0] = 0.0f;
            matrix[2, 1] = 0.0f;
            matrix[2, 2] = 1.0f;


            // prepare the rotation matrix
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 1] = sin;
            matrix[1, 0] = -sin;
            matrix[1, 1] = cos;

            // scale first
            float xMid = (End.X + Start.X) / 2;
            float yMid = (End.Y + Start.Y) / 2;

            // multiply movement vector for start and end by the scale factor
            float[] vector1 = new float[2] { (int)(Start.X - xMid), (int)(Start.Y - yMid) };
            vector1[0] *= ScaleFactor;
            vector1[1] *= ScaleFactor;
            float[] vector2 = new float[2] { (int)(End.X - xMid), (int)(End.Y - yMid) };
            vector2[0] *= ScaleFactor;
            vector2[1] *= ScaleFactor;

            // then rotate
            float[] newPointKey = new float[2];
            float[] newPointOpp = new float[2];

            // apply rotation matrix to the start and end points
            for (int col = 0; col < 2; col++)
            {
                newPointKey[col] = 0.0f;
                for (int index = 0; index < 2; index++)
                {
                    newPointKey[col] += vector1[index] * matrix[index, col];
                }
            }

            for (int col = 0; col < 2; col++)
            {
                newPointOpp[col] = 0.0f;
                for (int index = 0; index < 2; index++)
                {
                    newPointOpp[col] += vector2[index] * matrix[index, col];
                }
            }

            newStart = new PointF(newPointKey[0] + xMid, newPointKey[1] + yMid);
            newEnd = new PointF(newPointOpp[0] + xMid, newPointOpp[1] + yMid);
        }

        // function to check if a point is inside the circle or not
        public override bool contains(Point point)
        {
            base.contains(point);
            
            double d = Math.Sqrt(Math.Pow(point.X - Start.X, 2) + Math.Pow(point.Y - Start.Y, 2));

            if (d <= radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
