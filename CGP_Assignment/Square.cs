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
        static float[,] matrix = new float[3, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 1.0f } };


        //This class contains the specific details for a square defined in terms of opposite corners
        //public Point Start, End; // these points identify opposite corners of the square

        PointF A = new PointF();
        PointF B = new PointF();
        PointF C = new PointF();
        PointF D = new PointF();
             
        // axis aligned bounding box
        public Rectangle aabb;

        public Square(PointF startPoint, PointF endPoint)   // constructor
        {
            this.Start = startPoint;
            this.End = endPoint;
        }

        // You will need a different draw method for each kind of shape. Note the square is drawn
        // from first principles. All other shapes should similarly be drawn from first principles. 
        // Ideally no C# standard library class or method should be used to create, draw or transform a shape
        // and instead should utilse user-developed code.
        public override void draw(Graphics g, Pen blackPen)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            double xDiff, yDiff, xMid, yMid;   // range and mid points of x & y  

            // calculate ranges and mid points
            xDiff = End.X - Start.X;
            yDiff = End.Y - Start.Y;
            xMid = (End.X + Start.X) / 2;
            yMid = (End.Y + Start.Y) / 2;

            A = new PointF(Start.X, Start.Y);
            B = new PointF(End.X, End.Y);
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

        public override void Rotate(double angle)
        {
            base.Rotate(angle);

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
