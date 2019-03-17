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

    }
}
