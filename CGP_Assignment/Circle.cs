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
        Point startPoint;
        Point endPoint;

        public Circle(Point start, Point end)
        {
            startPoint = start;
            endPoint = end;

            radius = Math.Sqrt(Math.Pow((end.Y - start.Y), 2) + Math.Pow((end.X - start.X), 2));
        }

        public override void draw(Graphics g, Pen pen)
        {
            g.DrawEllipse(pen, (float)(startPoint.X - radius), (float)(startPoint.Y - radius),
                      (float)(radius + radius), (float)(radius + radius));
        }
    }
}
