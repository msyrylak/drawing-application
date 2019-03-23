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

        public class MoveInfo
        {
            //public Shape shape;
            public PointF StartShapePoint;
            public PointF EndShapePoint;
            public Point StartMoveMousePoint;
        }

        Shape selectedShape = null;
        MoveInfo Moving = null;
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
        //private PointF movementStart;

        Graphics g;
        Pen blackpen = new Pen(Color.Black, 2);

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
            mainMenu.MenuItems.Add(rotateItem);
            mainMenu.MenuItems.Add(moveItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            PopupMenu.MenuItems.Add(transformItem);
            PopupMenu.MenuItems.Add(deleteItem);
            //transformItem.MenuItems.Add(moveItem);
            //transformItem.MenuItems.Add(rotateItem);

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
            selectShape = false;
            moveShape = false;
            createShape = ShapeSelected.SQUARE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a square.");
        }

        private void createTriangle(object sender, EventArgs e)
        {
            selectShape = false;
            moveShape = false;
            createShape = ShapeSelected.TRIANGLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a triangle.");
        }

        private void createCircle(object sender, EventArgs e)
        {
            selectShape = false;
            moveShape = false;
            createShape = ShapeSelected.CIRCLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a circle.");
        }

        private void shapeSelect(object sender, EventArgs e)
        {
            MessageBox.Show("Right click on any of the shapes to transform or delete them");
            selectShape = true;
            moveShape = false;
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
            selectShape = false;
            createShape = ShapeSelected.NONE;
        }

        private void rotateItem(object sender, EventArgs e)
        {
            rotateShape = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //Bitmap offScr = new Bitmap(this.Width, this.Height);

            g = this.CreateGraphics();
            //Graphics g2 = Graphics.FromImage(offScr);

            foreach (var shape in shapes)
            {
                var color = shape == selectedShape ? Color.Red : Color.Black;
                var pen = new Pen(color, 2);
                shape.draw(g, pen);
                //g.DrawImage(offScr, 0, 0);

            }
        }

        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectShape == true)
                {
                    foreach (var shape in shapes.ToArray())
                    {
                        if (shape.contains(e.Location))
                        {
                            selectedShape = shape;
                            //PopupMenu.Show(this, e.Location);
                            if (deleteShape == true)
                            {
                                shapes.Remove(shape);
                                Refresh();
                                deleteShape = false;
                            }
                        }
                    }
                }
            }
        }

        private float StartAngle;
        private float CurrentAngle;
        private float TotalAngle;
        private bool DragInProgress;


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
                            if (Moving != null)
                            {
                                selectedShape.Start = new PointF(Moving.StartShapePoint.X + e.X - Moving.StartMoveMousePoint.X, Moving.StartShapePoint.Y + e.Y - Moving.StartMoveMousePoint.Y);
                                selectedShape.End = new PointF(Moving.EndShapePoint.X + e.X - Moving.StartMoveMousePoint.X, Moving.EndShapePoint.Y + e.Y - Moving.StartMoveMousePoint.Y);
                            }
                        }
                        else if (rotateShape == true && selectedShape != null)
                        {
                            // Get the angle from horizontal to the
                            // vector between the center and the current point.
                            float dx = e.X - ((selectedShape.End.X + selectedShape.Start.X) / 2);
                            float dy = e.Y - ((selectedShape.End.Y + selectedShape.Start.Y) / 2);
                            float new_angle = (float)Math.Atan2(dy, dx);

                            // Calculate the change in angle.
                            CurrentAngle = new_angle - StartAngle;

                            // Convert to degrees.
                            CurrentAngle *= 180 / (float)Math.PI;

                            // Add to the previous total angle rotated.
                            CurrentAngle += TotalAngle;

                            selectedShape.Rotate(CurrentAngle);
                            //Refresh();
                            //rotateShape = false;
                        }
                        break;

                    default:
                        break;
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            foreach (var shape in shapes.ToArray())
            {
                if (shape.contains(e.Location))
                {
                    selectedShape = shape;

                    if (selectedShape != null && Moving == null)
                    {
                        Moving = new MoveInfo
                        {
                            StartShapePoint = selectedShape.Start,
                            EndShapePoint = selectedShape.End,
                            StartMoveMousePoint = e.Location
                        };
                    }
                }
            }

            if (selectedShape != null)
            {
                // Get the initial angle from horizontal to the
                // vector between the center and the drag start point.
                DragInProgress = true;
                float dx = e.X - ((selectedShape.End.X + selectedShape.Start.X) / 2);
                float dy = e.Y - ((selectedShape.End.Y + selectedShape.Start.Y) / 2);
                StartAngle = (float)Math.Atan2(dy, dx);
            }

            mDown = e.Location;
            mMove = e.Location;

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            g = this.CreateGraphics();

            selectedShape = null;
            Moving = null;

            DragInProgress = false;
            // Save the new total angle of rotation.
            TotalAngle = CurrentAngle;

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



