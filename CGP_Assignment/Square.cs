using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CGP_Assignment
{
    class Square : Shape
    {
        // transformation matrix
        static float[,] matrix = new float[3, 3];


        //This class contains the specific details for a square defined in terms of opposite corners


        // points for the axis aligned bounding box
        PointF upperLeft = new PointF();
        PointF upperRight = new PointF();
        PointF lowerRight = new PointF();
        PointF lowerLeft  = new PointF();

        public Square(PointF startPoint, PointF endPoint, float rotationAngle, float scaleFactor)   // constructor
        {
            this.Start = startPoint;
            this.End = endPoint;
            this.ScaleFactor = scaleFactor;
            this.RotationAngle = rotationAngle;
        }


        // You will need a different draw method for each kind of shape. Note the square is drawn
        // from first principles. All other shapes should similarly be drawn from first principles. 
        // Ideally no C# standard library class or method should be used to create, draw or transform a shape
        // and instead should utilse user-developed code.
        public override void draw(Graphics g, Pen blackPen)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            double xDiff, yDiff, xMid, yMid;   // range and mid points of x & y  

            // create copies of the start and end points to modify in transformations
            PointF newStart = new PointF(Start.X,Start.Y);
            PointF newEnd = new PointF(End.X, End.Y);

            // apply transformations 
            this.Transform(RotationAngle, ref newStart, ref newEnd);

            // calculate ranges and mid points
            xDiff = newEnd.X - newStart.X;
            yDiff = newEnd.Y - newStart.Y;
            xMid = (newEnd.X + newStart.X) / 2;
            yMid = (newEnd.Y + newStart.Y) / 2;


            // draw square
            g.DrawLine(blackPen, new PointF(newStart.X, newStart.Y), new PointF((float)(xMid + yDiff / 2), (float)(yMid - xDiff / 2)));
            g.DrawLine(blackPen, new PointF((float)(xMid + yDiff / 2), (float)(yMid - xDiff / 2)), new PointF(newEnd.X, newEnd.Y));
            g.DrawLine(blackPen, new PointF(newEnd.X, newEnd.Y), new PointF((float)(xMid - yDiff / 2), (float)(yMid + xDiff / 2)));
            g.DrawLine(blackPen, new PointF((float)(xMid - yDiff / 2), (float)(yMid + xDiff / 2)), new PointF(newStart.X, newStart.Y));

            // put the square points in a list
            List<Point> points = new List<Point>();
            points.Add(new Point((int)Start.X, (int)Start.Y));
            points.Add(new Point((int)End.X, (int)End.Y));
            points.Add(new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2)));
            points.Add(new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2)));

            int maxX = points.Max(p => p.X);
            int maxY = points.Max(p => p.Y);
            int minX = points.Min(p => p.X);
            int minY = points.Min(p => p.Y);

            // calculate points for the aabb
            upperLeft = new PointF(minX, minY);
            upperRight = new PointF(minX + (maxX - minX), minY);
            lowerRight = new PointF(minX + (maxX - minX), minY + (maxY - minY));
            lowerLeft = new PointF(minX, minY + (maxY - minY));
            
            // create bounding box
            //g.DrawLine(blackPen, upperLeft, upperRight);
            //g.DrawLine(blackPen, upperRight, lowerRight);
            //g.DrawLine(blackPen, lowerRight, lowerLeft);
            //g.DrawLine(blackPen, lowerLeft, upperLeft);
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


        // function to check if a point is inside the square or not
        public override bool contains(Point point)
        {
            float height = (float)Math.Sqrt(Math.Pow((lowerRight.Y - lowerLeft.Y), 2) + Math.Pow((lowerRight.X - lowerLeft.X), 2));
            float width = (float)Math.Sqrt(Math.Pow((upperRight.Y - upperLeft.Y), 2) + Math.Pow((upperRight.X - upperLeft.X), 2));

            return 
            point.X >= upperLeft.X && 
            point.X <= upperLeft.X + width &&
            point.Y >= upperLeft.Y &&
            point.Y <= upperLeft.Y + height;
        }
    }
}
