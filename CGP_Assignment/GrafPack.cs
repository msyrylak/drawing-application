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
        enum CreateShape
        {
            NONE,
            SQUARE,
            TRIANGLE,
            CIRCLE
        }

        // a container for information regarding the shape's start and end point and mouse location upon pressing a button down ( TODO - REFACTOR)
        public class MoveInfo
        {
            public PointF StartShapePoint;
            public PointF EndShapePoint;
            public Point StartMoveMousePoint;
        }

        private Shape selectedShape = null; // shape that is currently chosen 
        private MoveInfo Moving = null; // object to hold moving info
        private MainMenu mainMenu;
        private TextBox txtAngle = new TextBox(); // text box and a label to show the angle of rotation
        private Label lblAngle = new Label(); //
        private ContextMenuStrip PopupMenu = new ContextMenuStrip();  // popupmenu to interact with the shapes upon right mouse button click
        private CreateShape createShape = CreateShape.NONE;

        // menu flags
        private bool selectShape = false;
        private bool moveShape = false;
        private bool rotateShape = false;
        private bool scaleShape = false;


        // for rotation angle calculations
        private float totalAngle = 0.0f;
        private float rotationAngle = 0.0f;
        private float startAngle; // ?
        private float scaleFactor = 1.0f;

        private PointF mDown; // this is kind of the same as startmovemouse in moving info (maybe move info not needed)
        private PointF mMove;

        private Graphics g;
        private Pen blackpen = new Pen(Color.Black, 2);

        private List<Shape> shapes = new List<Shape>(); // list of shapes to be drawn

        public GrafPack()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.Controls.Add(txtAngle);
            this.Controls.Add(lblAngle);

            // rotation angle info to display when rotating
            lblAngle.Text = "Rotation angle: ";
            lblAngle.Location = new Point(this.Width - 80, 12);
            txtAngle.Location = new Point(this.Width, 10);
            txtAngle.Hide();
            lblAngle.Hide();

            // The following approach uses menu items coupled with mouse clicks (TODO - see if i can do it differently)
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem();
            MenuItem selectItem = new MenuItem();
            MenuItem exitItem = new MenuItem();
            MenuItem squareItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();
            MenuItem circleItem = new MenuItem();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem();
            ToolStripMenuItem moveItem = new ToolStripMenuItem();
            ToolStripMenuItem rotateItem = new ToolStripMenuItem();
            ToolStripMenuItem scaleItem = new ToolStripMenuItem();

            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            selectItem.Text = "&Select";
            exitItem.Text = "&Exit";
            deleteItem.Text = "&Delete";
            moveItem.Text = "&Move";
            rotateItem.Text = "&Rotate";
            scaleItem.Text = "&Scale";

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            PopupMenu.Items.AddRange(new ToolStripItem[] { moveItem, rotateItem, scaleItem, deleteItem });

            selectItem.Click += new System.EventHandler(this.shapeSelect); // not needed?
            exitItem.Click += new System.EventHandler(this.selectExit); // maybe esc?
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);
            deleteItem.Click += new EventHandler(this.deleteItem);

            // shape transformation options
            moveItem.Click += new EventHandler(this.moveItem);
            rotateItem.Click += new EventHandler(this.rotateItem);
            scaleItem.Click += new EventHandler(this.scaleItem);


            this.Menu = mainMenu;
            this.MouseClick += mouseClick;
        }

        // Generally, all methods of the form are usually private
        private void createSquare(object sender, EventArgs e)
        {
            createShape = CreateShape.SQUARE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a square.");
        }

        private void createTriangle(object sender, EventArgs e)
        {
            createShape = CreateShape.TRIANGLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a triangle.");
        }

        private void createCircle(object sender, EventArgs e)
        {
            createShape = CreateShape.CIRCLE;
            MessageBox.Show("Click OK and then click and drag the mouse across the screen to create a circle.");
        }

        private void shapeSelect(object sender, EventArgs e) // not needed
        {
            MessageBox.Show("Left click to select one of the shapes and Right click on any of them to transform or delete.");
            selectShape = true;
            moveShape = false;
            rotateShape = false;

            createShape = CreateShape.NONE;
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
            txtAngle.Hide();
            lblAngle.Hide();
            shapes.Remove(selectedShape);
            selectedShape = null;
            Refresh();
        }

        private void scaleItem(object sender, EventArgs e)
        {
            txtAngle.Hide();
            lblAngle.Hide();
            scaleShape = true;
            moveShape = false;
            rotateShape = false;
        }

        private void moveItem(object sender, EventArgs e)
        {
            txtAngle.Hide();
            lblAngle.Hide();

            moveShape = true;
            rotateShape = false;
            createShape = CreateShape.NONE;
        }

        private void rotateItem(object sender, EventArgs e)
        {
            rotateShape = true;
            moveShape = false;
            createShape = CreateShape.NONE;
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
            }
        }


        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
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
                    case CreateShape.SQUARE:
                        Square square = new Square(mDown, mMove, totalAngle, scaleFactor);
                        square.draw(g, blackpen);
                        break;

                    case CreateShape.TRIANGLE:
                        Triangle triangle = new Triangle(mDown, mMove, totalAngle);
                        triangle.draw(g, blackpen);
                        break;

                    case CreateShape.CIRCLE:
                        Circle circle = new Circle(mDown, mMove, totalAngle);
                        circle.draw(g, blackpen);
                        break;

                    case CreateShape.NONE:
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

                            txtAngle.Text = (rotationAngle *= (float)(180 / Math.PI)).ToString("0.00") + "°";

                        }
                        else if (scaleShape == true && selectedShape != null)
                        {
                            Shape shapeCopy = selectedShape;
                            
                            // middle point
                            float centerX = ((shapeCopy.Start.X + shapeCopy.End.X) / 2);
                            float centerY = ((shapeCopy.Start.Y + shapeCopy.End.Y) / 2);

                            // original distance
                            float ogDist = (float)Math.Sqrt(Math.Pow((double)(centerX - Moving.StartMoveMousePoint.X), 2) + Math.Pow((double)(centerY - Moving.StartMoveMousePoint.Y), 2));
                            float currDist = (float)Math.Sqrt(Math.Pow((double)(centerX - e.X), 2) + Math.Pow((double)(centerY - e.Y), 2));
                            scaleFactor = (currDist/ogDist);

                            selectedShape.ScaleFactor = scaleFactor;

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

            if (e.Button == MouseButtons.Left)
            {
                if (selectShape == true)
                {
                    foreach (var shape in shapes.ToArray())
                    {
                        if (shape.contains(e.Location))
                        {
                            selectedShape = shape;
                            Refresh();
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
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (selectShape == true)
                {
                    PopupMenu.Show(this, e.Location);

                    foreach (var shape in shapes.ToArray())
                    {
                        if (shape.contains(e.Location))
                        {
                            selectedShape = shape;
                            Refresh();
                        }
                    }
                }
            }

            mDown = e.Location;
            mMove = e.Location;


        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Capture = false;
            Moving = null;
            totalAngle = rotationAngle;

            if (mDown.X != mMove.X && mDown.Y != mMove.Y)
            {
                switch (createShape)
                {
                    case CreateShape.SQUARE:

                        Square square = new Square(mDown, mMove, totalAngle, scaleFactor);
                        shapes.Add(square);
                        break;

                    case CreateShape.TRIANGLE:

                        Triangle triangle = new Triangle(mDown, mMove, totalAngle);
                        shapes.Add(triangle);
                        break;

                    case CreateShape.CIRCLE:
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



