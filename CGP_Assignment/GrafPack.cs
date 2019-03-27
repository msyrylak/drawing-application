using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CGP_Assignment
{
    public partial class GrafPack : Form
    {
        // enum to know which shape is currently being drawn/chosen
        enum ShapeSelected
        {
            NONE,
            SQUARE,
            TRIANGLE,
            CIRCLE
        }

        public class MoveInfo
        {
            public PointF StartShapePoint;
            public PointF EndShapePoint;
            public Point StartMoveMousePoint;
        }

        // shape that is currently chosen 
        private  Shape selectedShape = null;
        private MoveInfo Moving = null;
        private MainMenu mainMenu;
        private TextBox txtAngle = new TextBox();
        private Label lblAngle = new Label();
        private ContextMenu PopupMenu = new ContextMenu();
        private ShapeSelected createShape = ShapeSelected.NONE;

        // menu flags
        private bool selectShape = false;
        private bool deleteShape = false;
        private bool moveShape = false;
        private bool rotateShape = false;

        // for rotation angle calculations
        private float totalAngle = 0.0f;
        private float rotationAngle = 0;
        private float startAngle;

        private PointF mDown;
        private PointF mMove;

        private Graphics g;
        private Pen blackpen = new Pen(Color.Black, 2);

        private List<Shape> shapes = new List<Shape>();

        public GrafPack()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.Controls.Add(txtAngle);
            this.Controls.Add(lblAngle);

            lblAngle.Text = "Rotation angle: ";
            lblAngle.Location = new Point(this.Width - 80, 12);
            txtAngle.Location = new Point(this.Width, 10);
            txtAngle.Hide();
            lblAngle.Hide();

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

            selectItem.Click += new System.EventHandler(this.shapeSelect);
            exitItem.Click += new System.EventHandler(this.selectExit);
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);
            deleteItem.Click += new EventHandler(this.deleteItem);
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
            moveShape = false;
            selectShape = false;
            createShape = ShapeSelected.NONE;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
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
                            Refresh();
                            selectedShape = shape;
                        }
                    }
                }
            } else if (e.Button == MouseButtons.Right)
            {
                if (selectShape == true)
                {
                    foreach (var shape in shapes.ToArray())
                    {
                        if (shape.contains(e.Location))
                        {
                            Refresh();
                            selectedShape = shape;
                            PopupMenu.Show(this, e.Location);
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


        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            g = this.CreateGraphics();
            if (e.Button == MouseButtons.Left)
            {
                mMove = e.Location;

                Refresh();
                switch (createShape)
                {
                    case ShapeSelected.SQUARE:
                        Square square = new Square(mDown, mMove, totalAngle);
                        square.draw(g, blackpen);
                        break;

                    case ShapeSelected.TRIANGLE:
                        Triangle triangle = new Triangle(mDown, mMove, totalAngle);
                        triangle.draw(g, blackpen);
                        break;

                    case ShapeSelected.CIRCLE:
                        Circle circle = new Circle(mDown, mMove, totalAngle);
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
                            lblAngle.Show();
                            txtAngle.Show();
                            float dx1 = e.X - ((selectedShape.Start.X + selectedShape.End.X) / 2);
                            float dy1 = e.Y - ((selectedShape.Start.Y + selectedShape.End.Y) / 2);

                            double newAngle = Math.Atan2(dy1, dx1);

                            rotationAngle = (float)(newAngle - startAngle);

                            selectedShape.RotationAngle = rotationAngle;

                            txtAngle.Text = (rotationAngle*= (float)(180/Math.PI)).ToString("0.00") + "°";

                        }
                        break;

                    default:
                        break;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Capture = true;

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

                        float dx = e.X - ((selectedShape.Start.X + selectedShape.End.X) / 2);
                        float dy = e.Y - ((selectedShape.Start.Y + selectedShape.End.Y) / 2);
                        startAngle = (float)Math.Atan2(dy, dx);

                    }
                }
            }

            mDown = e.Location;
            mMove = e.Location;


        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Capture = false;

            selectedShape = null;
            Moving = null;
            totalAngle = rotationAngle;

            if (mDown.X != mMove.X && mDown.Y != mMove.Y)
            {
                switch (createShape)
                {
                    case ShapeSelected.SQUARE:

                        Square square = new Square(mDown, mMove, totalAngle);
                        shapes.Add(square);
                        break;

                    case ShapeSelected.TRIANGLE:

                        Triangle triangle = new Triangle(mDown, mMove, totalAngle);
                        shapes.Add(triangle);
                        break;

                    case ShapeSelected.CIRCLE:
                        Circle circle = new Circle(mDown, mMove, totalAngle);
                        shapes.Add(circle);
                        break;

                    default:
                        break;
                }
            }
        }

    }
}



