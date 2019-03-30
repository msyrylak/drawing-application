using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CGP_Assignment
{
    abstract class Shape
    {

        // This is the base class for Shapes in the application. It should allow an array or LL
        // to be created containing different kinds of shapes.

        public PointF Start { get; set; }
        public PointF End { get; set; }
        public float RotationAngle { get; set; }
        public float ScaleFactor { get; set; }

        public Shape()   // constructor
        {
        }

        public virtual void draw(Graphics g, Pen p)
        {

        }

        public virtual bool contains(Point point)
        {
            return false;
        }

        public virtual void Move(Point one, Point two)
        {

        }
        public virtual void Transform(double angle, ref PointF newStart, ref PointF newEnd)
        {

        }
        public virtual void Scale(float scaleFactor, ref PointF newStart, ref PointF newEnd)
        {

        }

    }
}
