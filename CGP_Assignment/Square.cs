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
        // rotation matrix
        //static float[,] matrix = new float[3, 3] { { 0.7071f, 0.7071f, 0.0f }, { -0.7071f, 0.7071f, 0.0f }, { 0.0f, 0.0f, 1.0f } };
        static float[,] matrix = new float[3, 3];


        //This class contains the specific details for a square defined in terms of opposite corners
        //public Point Start, End; // these points identify opposite corners of the square

        PointF A = new PointF();
        PointF B = new PointF();
        PointF C = new PointF();
        PointF D = new PointF();
             
        // axis aligned bounding box
        public Rectangle aabb;

        public Square()
        {

        }

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
            PointF newStart = new PointF(Start.X,Start.Y);
            PointF newEnd = new PointF(End.X, End.Y);

            //this.Scale(ScaleFactor, ref newStart, ref newEnd);
            this.Transform(RotationAngle, ref newStart, ref newEnd);

            // calculate ranges and mid points
            xDiff = newEnd.X - newStart.X;
            yDiff = newEnd.Y - newStart.Y;
            xMid = (newEnd.X + newStart.X) / 2;
            yMid = (newEnd.Y + newStart.Y) / 2;

            A = new PointF(newStart.X, newStart.Y);
            B = new PointF(newEnd.X, newEnd.Y);
            C = new PointF((float)(xMid + yDiff / 2), (float)(yMid - xDiff / 2));
            D = new PointF((float)(xMid - yDiff / 2), (float)(yMid + xDiff / 2));


            // draw square
            g.DrawLine(blackPen, A, C);
            g.DrawLine(blackPen, C, B);
            g.DrawLine(blackPen, B, D);
            g.DrawLine(blackPen, D, A);

            List<Point> points = new List<Point>();
            points.Add(new Point((int)Start.X, (int)Start.Y));
            points.Add(new Point((int)End.X, (int)End.Y));
            points.Add(new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2)));
            points.Add(new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2)));

            int maxX = points.Max(p => p.X);
            int maxY = points.Max(p => p.Y);
            int minX = points.Min(p => p.X);
            int minY = points.Min(p => p.Y);

            // create bounding box
            aabb = new Rectangle(new Point(minX, minY), new Size(maxX - minX, maxY - minY));
            //g.DrawRectangle(blackPen, aabb);
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
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);
            matrix[0, 0] = c;
            matrix[0, 1] = s;
            matrix[1, 0] = -s;
            matrix[1, 1] = c;

            // scale first
            float xMid = (End.X + Start.X) / 2;
            float yMid = (End.Y + Start.Y) / 2;

            float[] vector1 = new float[2] { (int)(Start.X - xMid), (int)(Start.Y - yMid) };
            vector1[0] *= ScaleFactor;
            vector1[1] *= ScaleFactor;
            float[] vector2 = new float[2] { (int)(End.X - xMid), (int)(End.Y - yMid) };
            vector2[0] *= ScaleFactor;
            vector2[1] *= ScaleFactor;

            // then rotate
            float[] newPointKey = new float[2];
            float[] newPointOpp = new float[2];


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


        public override bool contains(Point point)
        {
            //float A1 = area(A.X, A.Y, B.X, B.Y, C.X, C.Y) + area(A.X, A.Y, D.X, D.Y, C.X, C.Y);
            //float PAB = area(point.X, point.Y, A.X, A.Y, B.X, B.Y);
            //float PBC = area(point.X, point.Y, B.X, B.Y, C.X, C.X);
            //float PCD = area(point.X, point.Y, C.X, C.Y, D.X, D.Y);
            //float PAD = area(point.X, point.Y, A.X, A.Y, D.X, D.Y);

            //return (A1 == PAB + PBC + PCD + PAD);

            /*public override bool IsInside(int mouseX, int mouseY) 
            {
            return 
            mouseX >= x && 
            mouseX <= x + width &&
            mouseY >= y &&
            mouseY <= y + height;
            */

            base.contains(point);
            return aabb.Contains(point);


        }

        static float area(int x1, int y1, int x2,
               int y2, int x3, int y3)
        {
            return (float)Math.Abs((x1 * (y2 - y3) +
                                    x2 * (y3 - y1) +
                                    x3 * (y1 - y2)) / 2.0);
        }


    }
}
