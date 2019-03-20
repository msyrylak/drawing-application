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
        ContextMenu PopupMenu = new ContextMenu();
        private ShapeSelected createShape = ShapeSelected.NONE;
        private bool selectedShape = false;
        private bool shapeFlag = false;
        private bool deleteShape = false;
        private bool transformShape = false;
        private bool moveShape = false;
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
            MenuItem deleteItem = new MenuItem();
            MenuItem transformItem = new MenuItem();
            MenuItem moveItem = new MenuItem();
            MenuItem rotateItem = new MenuItem();

            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            selectItem.Text = "&Select";
            exitItem.Text = "&Exit";
            deleteItem.Text = "&Delete";
            transformItem.Text = "&Transform";
            moveItem.Text = "&Move";
            rotateItem.Text = "&Rotate";

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            PopupMenu.MenuItems.Add(transformItem);
            PopupMenu.MenuItems.Add(deleteItem);
            transformItem.MenuItems.Add(moveItem);
            transformItem.MenuItems.Add(rotateItem);

            selectItem.Click += new System.EventHandler(this.selectShape);
            exitItem.Click += new System.EventHandler(this.selectExit);
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);
            deleteItem.Click += new EventHandler(this.deleteItem);
            transformItem.Click += new EventHandler(this.transformItem);
            moveItem.Click += new EventHandler(this.moveItem);

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
            MessageBox.Show("Right click on any of the shapes to transform or delete them");
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

        private void deleteItem(object sender, EventArgs e)
        {
            deleteShape = true;
        }

        private void transformItem(object sender, EventArgs e)
        {
            transformShape = true;
        }

        private void moveItem(object sender, EventArgs e)
        {
            moveShape = true;
        }

        private void rotateItem(object sender, EventArgs e)
        {

        }


        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            //g = this.CreateGraphics();

            if (selectedShape == true)
            {
                foreach (var shape in shapes.ToArray())
                {
                    if (shape.contains(e.Location))
                    {
                        PopupMenu.Show(this, e.Location);
                        if (deleteShape == true)
                        {
                            shapes.Remove(shape);
                            this.Invalidate();
                            deleteShape = false;
                        }
                        else if(moveShape == true)
                        {
                            //this.Invalidate();
                            //shape.RotateShape();
                            Refresh();
                        }
                    }
                }
            }
        }

        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            g = this.CreateGraphics();

            base.OnMouseMove(e);
            mMove = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                Refresh();
                switch (createShape)
                {
                    case ShapeSelected.SQUARE:

                        Square square = new Square(mDown, mMove);
                        square.draw(g, blackpen);
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

            Square square = new Square(new Point(100, 100), new Point(200, 200));
            square.draw(g, blackpen);
            if (moveShape == true)
            {
               Square dupa = square.RotateShape();
               dupa.draw(g, blackpen);
            }

            foreach (var shape in shapes)
            {
                if(shapeFlag == true)
                {
                    shape.draw(g, Pens.Red);
                }
                else
                {
                    shape.draw(g, Pens.Black);

                }
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
            g = this.CreateGraphics();

            if (mDown.X != mMove.X && mDown.Y != mMove.Y)
            {
                switch (createShape)
                {
                    case ShapeSelected.SQUARE:

                        Square square = new Square(mDown, mMove);
                        shapes.Add(square);
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


