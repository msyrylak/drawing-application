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

        private Shape selectedShape = null; // shape that is currently chosen 
        PointF shapeStartPt;
        PointF shapeEndPt;
        //private MoveInfo Moving = null; // object to hold moving info
        private MainMenu mainMenu;
        private TextBox txtAngle = new TextBox(); // text box and a label to show the angle of rotation
        private Label lblAngle = new Label(); //
        private ContextMenuStrip PopupMenu = new ContextMenuStrip();  // popupmenu to interact with the shapes upon right mouse button click
        private CreateShape createShape = CreateShape.NONE; // TODO reset after finished drawing

        // menu flags
        private bool moveShape = false;
        private bool rotateShape = false;
        private bool scaleShape = false;


        // for rotation angle calculations
        private float angle = 0.0f;
        private float rotationAngle = 0.0f;
        private float startAngle; // ?

        // ratio for scaling
        private float scale = 1.0f;


        private PointF mouseDown; 
        private PointF mouseMove;

        private Graphics g;
        private Pen pen = new Pen(Color.Black, 2);

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
            exitItem.Text = "&Exit";
            deleteItem.Text = "&Delete";
            moveItem.Text = "&Move";
            rotateItem.Text = "&Rotate";
            scaleItem.Text = "&Scale";

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            PopupMenu.Items.AddRange(new ToolStripItem[] { moveItem, rotateItem, scaleItem, deleteItem });

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
            this.MouseClick += mouseClick; // not needed?
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
            txtAngle.Hide(); // put in a func? TODO
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


        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Capture = true;
            mouseDown = e.Location;
            mouseMove = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                foreach (var shape in shapes.ToArray())
                {
                    if (shape.contains(e.Location))
                    {
                        createShape = CreateShape.NONE;

                        selectedShape = shape;
                        Refresh();
                        if (selectedShape != null) // && Moving == null)
                        {
                            shapeStartPt = new PointF(selectedShape.Start.X, selectedShape.Start.Y);
                            shapeEndPt = new PointF(selectedShape.End.X, selectedShape.End.Y);

                            float dx = e.X - ((selectedShape.Start.X + selectedShape.End.X) / 2);
                            float dy = e.Y - ((selectedShape.Start.Y + selectedShape.End.Y) / 2);
                            startAngle = (float)Math.Atan2(dy, dx);
                        }
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
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

        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //TODO not draw here? 
            g = this.CreateGraphics();
            if (e.Button == MouseButtons.Left)
            {
                mouseMove = e.Location;

                Refresh();
                switch (createShape)
                {
                    case CreateShape.SQUARE:
                        Square square = new Square(mouseDown, mouseMove, angle, scale);
                        square.draw(g, pen);
                        //this.Invalidate(square);
                        break;

                    case CreateShape.TRIANGLE:
                        Triangle triangle = new Triangle(mouseDown, mouseMove, angle, scale);
                        triangle.draw(g, pen);
                        break;

                    case CreateShape.CIRCLE:
                        Circle circle = new Circle(mouseDown, mouseMove, angle, scale);
                        circle.draw(g, pen);
                        break;

                    case CreateShape.NONE:
                        if (moveShape == true && selectedShape != null)
                        {
                            selectedShape.Start = new PointF(shapeStartPt.X + e.X - mouseDown.X, shapeStartPt.Y + e.Y - mouseDown.Y);
                            selectedShape.End = new PointF(shapeEndPt.X + e.X - mouseDown.X, shapeEndPt.Y + e.Y - mouseDown.Y);
                        }
                        if (rotateShape == true && selectedShape != null)
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
                        if (scaleShape == true && selectedShape != null)
                        {
                            Shape shapeCopy = selectedShape;

                            // middle point
                            float centerX = ((shapeCopy.Start.X + shapeCopy.End.X) / 2);
                            float centerY = ((shapeCopy.End.Y + shapeCopy.End.Y) / 2);

                            // original distance
                            float ogDist = (float)Math.Sqrt(Math.Pow((double)(centerX - mouseDown.X), 2) + Math.Pow((double)(centerY - mouseDown.Y), 2));
                            float currDist = (float)Math.Sqrt(Math.Pow((double)(centerX - e.X), 2) + Math.Pow((double)(centerY - e.Y), 2));
                            scale = (currDist / ogDist);

                            selectedShape.ScaleFactor = scale;
                        }
                        break;

                    default:
                        break;
                }
            }
        }


        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Capture = false;
            //Moving = null;
            angle = rotationAngle;

            if (mouseDown.X != mouseMove.X && mouseDown.Y != mouseMove.Y)
            {
                switch (createShape)
                {
                    case CreateShape.SQUARE:

                        Square square = new Square(mouseDown, mouseMove, angle, scale);
                        shapes.Add(square);
                        break;

                    case CreateShape.TRIANGLE:

                        Triangle triangle = new Triangle(mouseDown, mouseMove, angle, scale);
                        shapes.Add(triangle);
                        break;

                    case CreateShape.CIRCLE:
                        Circle circle = new Circle(mouseDown, mouseMove, angle, scale);
                        shapes.Add(circle);
                        break;

                    default:
                        break;
                }
            }
        }

    }
}



