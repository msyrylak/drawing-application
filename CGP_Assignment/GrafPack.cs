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

        Shape selectedShape = null;
        private MainMenu mainMenu;
        ContextMenu PopupMenu = new ContextMenu();
        private ShapeSelected createShape = ShapeSelected.NONE;
        private bool selectShape = false;
        private bool deleteShape = false;
        //private bool transformShape = false;
        private bool moveShape = false;
        private bool rotateShape = false;

        private PointF mDown;
        private PointF mMove;
        private PointF movementStart;

        Graphics g;
        Pen blackpen = new Pen(Color.Black);

        private List<Shape> shapes = new List<Shape>();

        public GrafPack()
        {
            InitializeComponent();
            //this.DoubleBuffered = true;
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
            mainMenu.MenuItems.Add(moveItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            PopupMenu.MenuItems.Add(transformItem);
            PopupMenu.MenuItems.Add(deleteItem);
            //transformItem.MenuItems.Add(moveItem);
            transformItem.MenuItems.Add(rotateItem);

            selectItem.Click += new System.EventHandler(this.shapeSelect);
            exitItem.Click += new System.EventHandler(this.selectExit);
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);
            deleteItem.Click += new EventHandler(this.deleteItem);
            //transformItem.Click += new EventHandler(this.transformItem);
            moveItem.Click += new EventHandler(this.moveItem);
            rotateItem.Click += new EventHandler(this.rotateItem);

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

        private void shapeSelect(object sender, EventArgs e)
        {
            MessageBox.Show("Right click on any of the shapes to transform or delete them");
            selectShape = true;
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

        private void moveItem(object sender, EventArgs e)
        {
            moveShape = true;
            createShape = ShapeSelected.NONE;

        }

        private void rotateItem(object sender, EventArgs e)
        {
            rotateShape = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            g = this.CreateGraphics();

            foreach (var shape in shapes)
            {
                var color = shape == selectedShape ? Color.Red : Color.Black;
                var pen = new Pen(color);
                shape.draw(g, pen);
            }
        }

        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            //g = this.CreateGraphics();

        }

        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            g = this.CreateGraphics();

            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
               mMove = e.Location;

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

                    case ShapeSelected.NONE:
                        if (moveShape == true)
                        {
                            //Refresh();
                            foreach (var shape in shapes.ToArray())
                            {
                                if (shape.contains(e.Location))
                                {
                                    selectedShape = shape;

                                    mDown = new PointF(shape.Start.X + e.X - movementStart.X, shape.Start.Y + e.Y - movementStart.Y);
                                    mMove = new PointF(shape.End.X + e.X - movementStart.X, shape.End.Y + e.Y - movementStart.Y);
                                    shape.Start = mDown;
                                    shape.End = mMove;
                                    shape.draw(g, blackpen);
                                    //this.Invalidate();

                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
                //Refresh();
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            movementStart = e.Location;
            mDown = e.Location;
            mMove = e.Location;

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            g = this.CreateGraphics();

            //moveShape = false;

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

            if (selectShape == true)
            {
                foreach (var shape in shapes.ToArray())
                {
                    if (shape.contains(e.Location))
                    {
                        selectedShape = shape;
                        Refresh();
                        PopupMenu.Show(this, e.Location);
                        if (deleteShape == true)
                        {
                            shapes.Remove(shape);
                            //this.Invalidate();
                            deleteShape = false;
                            this.Refresh();
                        }
                        else if (rotateShape == true)
                        {
                            shape.Rotate(90);
                            rotateShape = false;
                            this.Invalidate();
                        }
                    }
                }
            }

        }
    }
}


