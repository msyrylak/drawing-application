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

        public Circle()
        {

        }

        public Circle(PointF start, PointF end, float rotation)
        {
            Start = start;
            End = end;
            this.RotationAngle = rotation;

            radius = Math.Sqrt(Math.Pow((end.Y - start.Y), 2) + Math.Pow((end.X - start.X), 2));
        }

        public override void draw(Graphics g, Pen pen)
        {
            g.DrawEllipse(pen, (float)(Start.X - radius), (float)(Start.Y - radius),
                      (float)(radius + radius), (float)(radius + radius));
        }

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
