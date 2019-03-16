using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CGP_Assignment
{
    public partial class GrafPack : Form
    {

        enum ShapeSelected
        {
            NONE,
            SQUARE,
            TRIANGLE,
            CIRCLE
        }

        private MainMenu mainMenu;
        private ShapeSelected selectedShape = ShapeSelected.NONE;
        private Point mDown;
        private Point mMove;
        Graphics g;
        Pen blackpen = new Pen(Color.Black);

        private List<Shape> shapes = new List<Shape>();

        public GrafPack()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // The following approach uses menu items coupled with mouse clicks
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem();
            MenuItem selectItem = new MenuItem();
            MenuItem squareItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();
            MenuItem circleItem = new MenuItem();


            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            selectItem.Text = "&Select";

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);

            selectItem.Click += new System.EventHandler(this.selectShape);
            squareItem.Click += new System.EventHandler(this.selectSquare);
            triangleItem.Click += new System.EventHandler(this.selectTriangle);
            circleItem.Click += new System.EventHandler(this.selectCircle);

            this.Menu = mainMenu;
            this.MouseClick += mouseClick;
        }

        // Generally, all methods of the form are usually private
        private void selectSquare(object sender, EventArgs e)
        {
            selectedShape = ShapeSelected.SQUARE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a square.");
        }

        private void selectTriangle(object sender, EventArgs e)
        {
            selectedShape = ShapeSelected.TRIANGLE;
        }

        private void selectCircle(object sender, EventArgs e)
        {
            selectedShape = ShapeSelected.CIRCLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a circle.");
        }

        private void selectShape(object sender, EventArgs e)
        {
            MessageBox.Show("You selected the Select option...");
        }

        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {


        }

        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mMove = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                Refresh();
                switch (selectedShape)
                {
                    case ShapeSelected.SQUARE:

                        Square aShape = new Square(mDown, mMove);
                        aShape.draw(g, blackpen);
                        break;

                    case ShapeSelected.TRIANGLE:

                        Triangle triangle = new Triangle(mDown, mMove);
                        triangle.draw(g, blackpen);

                        break;

                    case ShapeSelected.CIRCLE:
                        Circle circle = new Circle(mDown, mMove);
                        circle.draw(g, blackpen);
                        break;

                    default:
                        break;
                }
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bitmap = new Bitmap(this.Width, this.Height);

            base.OnPaint(e);
            g = this.CreateGraphics();
            Graphics gBuffer = Graphics.FromImage(bitmap);

            foreach (var shape in shapes)
            {
                shape.draw(g, blackpen);
            }

            //gBuffer.DrawImage(bitmap, 0, 0);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mDown = e.Location;
            mMove = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (mDown.X != mMove.X && mDown.Y != mMove.Y)
            {
                switch (selectedShape)
                {
                    case ShapeSelected.SQUARE:

                        Square aShape = new Square(mDown, mMove);
                        shapes.Add(aShape);
                        break;

                    case ShapeSelected.TRIANGLE:

                        Triangle triangle = new Triangle(mDown, mMove);

                        break;

                    case ShapeSelected.CIRCLE:
                        Circle circle = new Circle(mDown, mMove);
                        shapes.Add(circle);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}


