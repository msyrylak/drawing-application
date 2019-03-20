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
        //This class contains the specific details for a square defined in terms of opposite corners
        Point keyPt, oppPt; // these points identify opposite corners of the square

        Point A = new Point();
        Point B = new Point();
        Point C = new Point();
        Point D = new Point();

        // axis aligned bounding box
        public Rectangle aabb;

        public Square(Point keyPt, Point oppPt)   // constructor
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
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
            xDiff = oppPt.X - keyPt.X;
            yDiff = oppPt.Y - keyPt.Y;
            xMid = (oppPt.X + keyPt.X) / 2;
            yMid = (oppPt.Y + keyPt.Y) / 2;

            A = new Point(keyPt.X, keyPt.Y);
            B = new Point(oppPt.X, oppPt.Y);
            C = new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            D = new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2));


            // draw square
            g.DrawLine(blackPen, (int)keyPt.X, (int)keyPt.Y, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            g.DrawLine(blackPen, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2), (int)oppPt.X, (int)oppPt.Y);
            g.DrawLine(blackPen, (int)oppPt.X, (int)oppPt.Y, (int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2));
            g.DrawLine(blackPen, (int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2), (int)keyPt.X, (int)keyPt.Y);

            List<Point> points = new List<Point>();
            points.Add(new Point(keyPt.X, keyPt.Y));
            points.Add(new Point(oppPt.X, oppPt.Y));
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

        public Square RotateShape()
        {
            double xMid = (oppPt.X + keyPt.X) / 2;
            double yMid = (oppPt.Y + keyPt.Y) / 2;
            Square newSquaer = new Square(this.keyPt, this.oppPt);
            newSquaer = Transform.Rotate(new Point((int)xMid, (int)yMid), this.keyPt, this.oppPt);

            return newSquaer;

        }

        public override bool contains(Point point)
        {
            //float A1 = area(A.X, A.Y, B.X, B.Y, C.X, C.Y) + area(A.X, A.Y, D.X, D.Y, C.X, C.Y);
            //float PAB = area(point.X, point.Y, A.X, A.Y, B.X, B.Y);
            //float PBC = area(point.X, point.Y, B.X, B.Y, C.X, C.X);
            //float PCD = area(point.X, point.Y, C.X, C.Y, D.X, D.Y);
            //float PAD = area(point.X, point.Y, A.X, A.Y, D.X, D.Y);

            //return (A1 == PAB + PBC + PCD + PAD);

            base.contains(point);
            return aabb.Contains(point);


        }

        public Square Mirror(Shape shape)
        {
            Square newShape = new Square(shape.End, shape.Start);
            return newShape;
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
