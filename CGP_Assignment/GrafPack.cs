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

        // shapes to draw
        Square square = null;
        Circle circle = null;
        Triangle triangle = null;

        private Shape selectedShape = null; // shape that is currently chosen 
        PointF shapeStartPt;
        PointF shapeEndPt;
        private MainMenu mainMenu;
        private TextBox txtAngle = new TextBox(); // text box and a label to show the angle of rotation
        private Label lblAngle = new Label(); //
        private ContextMenuStrip PopupMenu = new ContextMenuStrip();  // popupmenu to interact with the shapes upon right mouse button click
        private ContextMenuStrip CreatePopupMenu = new ContextMenuStrip();
        private CreateShape createShape = CreateShape.NONE; // TODO reset after finished drawing

        // menu flags
        private bool moveShape = false;
        private bool rotateShape = false;
        private bool scaleShape = false;

        // flag to check if rubber banding is in process
        private bool rubberBanding = false;

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

            // The following approach uses menu items coupled with mouse clicks
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem("Create");
            MenuItem exitItem = new MenuItem("Exit");
            MenuItem squareItem = new MenuItem("Square");
            MenuItem triangleItem = new MenuItem("Triangle");
            MenuItem circleItem = new MenuItem("Circle");
            MenuItem helpItem = new MenuItem("Help");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete");
            ToolStripMenuItem moveItem = new ToolStripMenuItem("Move");
            ToolStripMenuItem rotateItem = new ToolStripMenuItem("Rotate");
            ToolStripMenuItem scaleItem = new ToolStripMenuItem("Scale");
            ToolStripMenuItem createPopUp = new ToolStripMenuItem("Create");
            ToolStripMenuItem squarePopUp = new ToolStripMenuItem("Square");
            ToolStripMenuItem trianglePopUp = new ToolStripMenuItem("Triangle");
            ToolStripMenuItem circlePopUp = new ToolStripMenuItem("Circle");

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(helpItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            PopupMenu.Items.AddRange(new ToolStripItem[] { moveItem, rotateItem, scaleItem, deleteItem });
            CreatePopupMenu.Items.Add(createPopUp);
            createPopUp.DropDownItems.AddRange(new ToolStripItem[] { squarePopUp, trianglePopUp, circlePopUp});

            helpItem.Click += new System.EventHandler(this.DisplayInstructions);
            exitItem.Click += new System.EventHandler(this.selectExit); 
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);
            deleteItem.Click += new EventHandler(this.deleteItem);

            // shape transformation options
            moveItem.Click += new EventHandler(this.moveItem);
            rotateItem.Click += new EventHandler(this.rotateItem);
            scaleItem.Click += new EventHandler(this.scaleItem);

            // popup create options
            squarePopUp.Click += new System.EventHandler(this.createSquare);
            trianglePopUp.Click += new System.EventHandler(this.createTriangle);
            circlePopUp.Click += new System.EventHandler(this.createCircle);


            this.Menu = mainMenu;
        }

        // display instructions when program first runs
        protected override void OnShown(EventArgs e)
        {
            DisplayInstructions(this, e);
        }

        // Generally, all methods of the form are usually private
        private void DisplayInstructions(object sender, EventArgs e)
        {
            string Instructions = "Instructions:\n\n" +
                "1. To create a shape choose one from the \"Create\" tab, press left mouse button and drag the mouse across the screen.\n\n" +
                "2. To select any of the shapes drawn on the screen, click on it with the left mouse button.\n\n" +
                "3. To perform any action on the shape right click to open a Pop Up menu. \n\n" +
                "3a. \"Move\" - click anywhere inside the shape and drag the mouse around to move the shape.\n\n" +
                "3b \"Rotate\" - drag the mouse around to rotate the shape.\n\n" +
                "3c. \"Scale\" - drag the mouse further from the shape to make it bigger and closer to the shape to make it smaller.\n\n" +
                "3d. \"Delete\" - remove the shape from the screen.\n\n" +
                "4. You can also right click in any place on the screen to create a new shape.\n\n " +
                "You can access these instructions under the \"Help\" tab.";

            MessageBox.Show(Instructions);
        }


        private void SetFlags(bool move, bool scale, bool rotate)
        {
            moveShape = move;
            scaleShape = scale;
            rotateShape = rotate;
        }


        private void createSquare(object sender, EventArgs e)
        {
            SetFlags(false, false, false);
            createShape = CreateShape.SQUARE;
        }


        private void createTriangle(object sender, EventArgs e)
        {
            SetFlags(false, false, false);
            createShape = CreateShape.TRIANGLE;
        }


        private void createCircle(object sender, EventArgs e)
        {
            SetFlags(false, false, false);
            createShape = CreateShape.CIRCLE;
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
            SetFlags(false, false, false);
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
            SetFlags(false, true, false);
            createShape = CreateShape.NONE;
        }


        private void moveItem(object sender, EventArgs e)
        {
            txtAngle.Hide();
            lblAngle.Hide();
            SetFlags(true, false, false);
            createShape = CreateShape.NONE;
        }


        private void rotateItem(object sender, EventArgs e)
        {
            SetFlags(false, false, true);
            createShape = CreateShape.NONE;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            g = this.CreateGraphics();

            if (rubberBanding == true)
            {
                switch (createShape)
                {
                    case CreateShape.SQUARE:
                        square.draw(g, pen);
                        break;

                    case CreateShape.TRIANGLE:
                        triangle.draw(g, pen);
                        break;

                    case CreateShape.CIRCLE:
                        circle.draw(g, pen);
                        break;

                    default:
                        break;
                }
            }

            foreach (var shape in shapes)
            {
                var color = shape == selectedShape ? Color.Red : Color.Black;
                var pen = new Pen(color, 2);
                shape.draw(g, pen);
            }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Capture = true;
            mouseDown = e.Location;
            mouseMove = e.Location;
            rubberBanding = !rubberBanding;

            switch (createShape)
            {
                case CreateShape.SQUARE:

                    square = new Square(mouseDown, mouseMove, angle, scale);
                    break;

                case CreateShape.TRIANGLE:

                    triangle = new Triangle(mouseDown, mouseMove, angle, scale);
                    break;

                case CreateShape.CIRCLE:
                    circle = new Circle(mouseDown, mouseMove, angle, scale);
                    break;

                default:
                    break;
            }

            if (e.Button == MouseButtons.Left)
            {
                foreach (var shape in shapes.ToArray())
                {
                    if (shape.contains(e.Location))
                    {
                        createShape = CreateShape.NONE;
                        selectedShape = shape;
                        Refresh();
                        if (selectedShape != null)
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
                if (shapes.Count != 0)
                {
                    foreach (var shape in shapes.ToArray())
                    {
                        if (shape.contains(e.Location))
                        {
                            selectedShape = shape;
                            PopupMenu.Show(this, e.Location);
                        }
                        else if(selectedShape == null || selectedShape.contains(e.Location) == false)
                        {
                            CreatePopupMenu.Show(this, e.Location);
                        }
                    }
                }
                else
                {
                    CreatePopupMenu.Show(this, e.Location);
                }
            }
            Invalidate();
        }


        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //TODO not draw here? 
            g = this.CreateGraphics();
            if (e.Button == MouseButtons.Left)
            {
                if (rubberBanding)
                {
                    mouseMove = e.Location;

                    Invalidate();
                    switch (createShape)
                    {
                        case CreateShape.SQUARE:
                            square.Start = mouseDown;
                            square.End = mouseMove;
                            break;

                        case CreateShape.TRIANGLE:
                            triangle.Start = mouseDown;
                            triangle.End = mouseMove;
                            break;

                        case CreateShape.CIRCLE:
                            circle.Start = mouseDown;
                            circle.End = mouseMove;
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
                                // middle point
                                float centerX = ((selectedShape.Start.X + selectedShape.End.X) / 2);
                                float centerY = ((selectedShape.End.Y + selectedShape.End.Y) / 2);

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
                    Invalidate();
                }
            }
        }


        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.Capture = false;
            angle = rotationAngle;
            rubberBanding = !rubberBanding;

            if (mouseDown.X != mouseMove.X && mouseDown.Y != mouseMove.Y)
            {
                switch (createShape)
                {
                    case CreateShape.SQUARE:
                        shapes.Add(square);
                        break;

                    case CreateShape.TRIANGLE:
                        shapes.Add(triangle);
                        break;

                    case CreateShape.CIRCLE:
                        shapes.Add(circle);
                        break;

                    default:
                        break;
                }
            }
        }

    }
}



