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
        private ShapeSelected createShape = ShapeSelected.NONE;
        private bool selectedShape = false;
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
            MenuItem exitItem = new MenuItem();
            MenuItem squareItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();
            MenuItem circleItem = new MenuItem();


            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            selectItem.Text = "&Select";
            exitItem.Text = "&Exit";

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            //selectItem.MenuItems.Add(new MenuItem("Transform"));

            selectItem.Click += new System.EventHandler(this.selectShape);
            exitItem.Click += new System.EventHandler(this.selectExit);
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);

            this.Menu = mainMenu;
            this.MouseClick += mouseClick;
        }

        // Generally, all methods of the form are usually private
        private void createSquare(object sender, EventArgs e)
        {
            createShape = ShapeSelected.SQUARE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a square.");
        }

        private void createTriangle(object sender, EventArgs e)
        {
            createShape = ShapeSelected.TRIANGLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a triangle.");
        }

        private void createCircle(object sender, EventArgs e)
        {
            createShape = ShapeSelected.CIRCLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a circle.");
        }

        private void selectShape(object sender, EventArgs e)
        {
            MessageBox.Show("You selected the Select option...");
            selectedShape = true;
            createShape = ShapeSelected.NONE;
        }

        private void selectExit(object sender, EventArgs e)
        {
            string title = "Exit program";
            string message = "Are you sure you want to Exit?";
            DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                this.Dispose();
            }
        }


        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (selectedShape == true && e.Button == MouseButtons.Left)
            {
                foreach (var shape in shapes)
                {
                    if (shape.contains(e.Location))
                    {
                        MessageBox.Show("Hit!");
                    }
                }
            }

        }

        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mMove = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                Refresh();
                switch (createShape)
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
            base.OnPaint(e);
            g = this.CreateGraphics();

            foreach (var shape in shapes)
            {
                shape.draw(g, blackpen);
            }
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
                switch (createShape)
                {
                    case ShapeSelected.SQUARE:

                        Square aShape = new Square(mDown, mMove);
                        shapes.Add(aShape);
                        break;

                    case ShapeSelected.TRIANGLE:

                        Triangle triangle = new Triangle(mDown, mMove);
                        shapes.Add(triangle);
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


