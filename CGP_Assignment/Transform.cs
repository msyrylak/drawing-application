using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CGP_Assignment
{
    class Transform
    {
        static float[,] matrix = new float[3, 3] { { 0.7071f, 0.7071f, 0.0f }, { -0.7071f, 0.7071f, 0.0f }, { 0.0f, 0.0f, 1.0f } };


        public static Square Rotate(Point center, Point point1, Point point2)
        {
            float[] point = new float[2] { point1.X - center.X, point1.Y - center.Y };
            float[] point3 = new float[2] { point2.X - center.X, point2.Y - center.X };

            float[] newPoint = new float[2];
            float[] newPoint2 = new float[2];


            for (int col = 0; col < 2; col++)
            {
                newPoint[col] = 0.0f;
                for (int index = 0; index < 2; index++)
                {
                    newPoint[col] += point[index] * matrix[index, col];
                }
            }

            for (int col = 0; col < 2; col++)
            {
                newPoint2[col] = 0.0f;
                for (int index = 0; index < 2; index++)
                {
                    newPoint2[col] += point3[index] * matrix[index, col];
                }
            }

            Point transfPoint = new Point((int)newPoint[0] + center.X, (int)newPoint[1] + center.Y);
            Point transfPoint2 = new Point((int)newPoint2[0] + center.X, (int)newPoint2[1] + center.Y);

            return new Square(transfPoint, transfPoint2);
        }

    }
}
