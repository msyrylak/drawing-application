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
        PointF selectedShapeStartPt;
        PointF selectedShapeEndPt;

        private Label angleVal = new Label(); // text box and a label to show the angle of rotation
        private Label lblAngle = new Label();
        private ContextMenuStrip PopupMenu = new ContextMenuStrip();  // popupmenu to interact with the shapes upon right mouse button click
        private ContextMenuStrip CreatePopupMenu = new ContextMenuStrip();
        private CreateShape createShape = CreateShape.NONE;
        MenuItem manualDBItem = new MenuItem("Manual Double Buffer");


        // menu flags
        private bool moveShape = false;
        private bool rotateShape = false;
        private bool scaleShape = false;

        // flag to check if rubber banding is in process
        private bool rubberBanding = false;

        // for rotation angle calculations
        private float angle = 0.0f;
        private float rotationAngle = 0.0f;
        private float startAngle; 

        // ratio for scaling
        private float scale = 1.0f;

        // points to record mouse movement
        private PointF mouseDown;
        private PointF mouseMove;

        private Pen pen = new Pen(Color.Black, 2);

        private List<Shape> shapes = new List<Shape>(); // list of shapes to be drawn

        static bool manualDB = false;
        Bitmap offScr;

        public GrafPack()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.Controls.Add(angleVal);
            this.Controls.Add(lblAngle);

            // rotation angle info to display when rotating
            lblAngle.Text = "Rotation angle: ";
            lblAngle.Location = new Point(this.Width + this.Height - 80, 7);
            angleVal.Location = new Point(this.Width + this.Height, 7);
            angleVal.Hide();
            lblAngle.Hide();

            // The following approach uses menu items coupled with mouse clicks
            // main menu
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem("Create");
            MenuItem exitItem = new MenuItem("Exit");
            MenuItem squareItem = new MenuItem("Square");
            MenuItem triangleItem = new MenuItem("Triangle");
            MenuItem circleItem = new MenuItem("Circle");
            MenuItem helpItem = new MenuItem("Help");
            MenuItem optionsItem = new MenuItem("Options");



            // right click pop up menu
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete");
            ToolStripMenuItem moveItem = new ToolStripMenuItem("Move");
            ToolStripMenuItem rotateItem = new ToolStripMenuItem("Rotate");
            ToolStripMenuItem scaleItem = new ToolStripMenuItem("Scale");
            ToolStripMenuItem createPopUp = new ToolStripMenuItem("Create");
            ToolStripMenuItem squarePopUp = new ToolStripMenuItem("Square");
            ToolStripMenuItem trianglePopUp = new ToolStripMenuItem("Triangle");
            ToolStripMenuItem circlePopUp = new ToolStripMenuItem("Circle");

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(optionsItem);
            mainMenu.MenuItems.Add(helpItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            optionsItem.MenuItems.Add(manualDBItem);
            PopupMenu.Items.AddRange(new ToolStripItem[] { moveItem, rotateItem, scaleItem, deleteItem });
            CreatePopupMenu.Items.Add(createPopUp);
            createPopUp.DropDownItems.AddRange(new ToolStripItem[] { squarePopUp, trianglePopUp, circlePopUp});

            helpItem.Click += new System.EventHandler(this.DisplayInstructions);
            exitItem.Click += new System.EventHandler(this.selectExit); 
            squareItem.Click += new System.EventHandler(this.createSquare);
            triangleItem.Click += new System.EventHandler(this.createTriangle);
            circleItem.Click += new System.EventHandler(this.createCircle);
            deleteItem.Click += new EventHandler(this.deleteItem);
            manualDBItem.Click += new EventHandler(this.manualDoubleBuffering);

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
                "3. To perform any action on the shape right click to open a Pop-up menu. \n\n" +
                "3a. \"Move\" - click anywhere inside the shape and drag the mouse around to move the shape.\n\n" +
                "3b \"Rotate\" - drag the mouse around to rotate the shape.\n\n" +
                "3c. \"Scale\" - drag the mouse further from the shape to make it bigger and closer to the shape to make it smaller.\n\n" +
                "3d. \"Delete\" - remove the shape from the screen.\n\n" +
                "4. You can also right click in any place on the screen to create a new shape.\n\n " +
                "You can access these instructions under the \"Help\" tab.";

            MessageBox.Show(Instructions);
        }


        // sets functionality flags
        private void SetFlags(bool move, bool scale, bool rotate)
        {
            moveShape = move;
            scaleShape = scale;
            rotateShape = rotate;
        }

        private void manualDoubleBuffering(object sender, EventArgs e)
        {
            if (sender == manualDBItem)
            {
                manualDBItem.Checked = true;
                manualDB = true;
            }

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
            angleVal.Hide(); 
            lblAngle.Hide();
            shapes.Remove(selectedShape);
            selectedShape = null;
            Refresh();
        }


        private void scaleItem(object sender, EventArgs e)
        {
            angleVal.Hide();
            lblAngle.Hide();
            SetFlags(false, true, false);
            createShape = CreateShape.NONE;
        }


        private void moveItem(object sender, EventArgs e)
        {
            angleVal.Hide();
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
            Graphics g;

            if (manualDB)
            {
                this.DoubleBuffered = false;
                offScr = new Bitmap(this.Width, this.Height);
                g = Graphics.FromImage(offScr);
                g.Clear(Color.White);

            }
            else
            {
                this.DoubleBuffered = true;
                g = e.Graphics;
            }


            // implement double buffering


            // draw previously created shapes that are in the list
            foreach (var shape in shapes)
            {
                // depending on whether the shape was clicked on (selected) assign the color of the pen
                var color = shape == selectedShape ? Color.Red : Color.Black;
                var pen = new Pen(color, 2);
                shape.draw(g, pen);
            }

            // if rubber banding is in process, draw shapes as they are being created
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
            if(manualDB)
            {
                e.Graphics.DrawImageUnscaled(offScr, 0, 0);
            }
        }



        protected override void OnMouseDown(MouseEventArgs e)
        {
            // track mouse position when down
            this.Capture = true;
            mouseDown = e.Location;
            mouseMove = e.Location;

            // reset rubberbanding flag
            rubberBanding = !rubberBanding;

            // if any of the shapes is chosen to be drawn, create new shape object with 
            // start and end points as well as angle for rotation and scaling factor
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
                // if left mouse button has been pressed check if it clicked anywhere inside a shape
                // from the shape list, if yes assign it to selectedShape object
                foreach (var shape in shapes.ToArray())
                {
                    if (shape.contains(e.Location))
                    {
                        createShape = CreateShape.NONE;
                        selectedShape = shape;
                        if (selectedShape != null)
                        {
                            // record where the select shape has start and end points
                            selectedShapeStartPt = new PointF(selectedShape.Start.X, selectedShape.Start.Y);
                            selectedShapeEndPt = new PointF(selectedShape.End.X, selectedShape.End.Y);

                            // calculate starting angle for rotation
                            float dx = e.X - ((selectedShape.Start.X + selectedShape.End.X) / 2);
                            float dy = e.Y - ((selectedShape.Start.Y + selectedShape.End.Y) / 2);
                            startAngle = (float)Math.Atan2(dy, dx);
                        }
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // if right button has been pressed, show a context menu to perform any action the shape
                // or if it was clicked somewhere else and not on any of the shapes, show context menu for shape creation
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
            //Invalidate();
        }


        // draw shapes using rubber banding
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (rubberBanding)
                {
                    // if rubber banding is on, modify shapes start and end points according to the mouse movement location

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

                            // if none of the shapes is chosen for the creation, the enum switches to none
                            // that means the user can perform actions on selected shape (move, rotate or scale)
                            if (moveShape == true && selectedShape != null)
                            {
                                selectedShape.Start = new PointF(selectedShapeStartPt.X + e.X - mouseDown.X, selectedShapeStartPt.Y + e.Y - mouseDown.Y);
                                selectedShape.End = new PointF(selectedShapeEndPt.X + e.X - mouseDown.X, selectedShapeEndPt.Y + e.Y - mouseDown.Y);
                            }

                            if (rotateShape == true && selectedShape != null)
                            {
                                lblAngle.Show();
                                angleVal.Show();
                                float dx1 = e.X - ((selectedShape.Start.X + selectedShape.End.X) / 2);
                                float dy1 = e.Y - ((selectedShape.Start.Y + selectedShape.End.Y) / 2);

                                double newAngle = Math.Atan2(dy1, dx1);

                                rotationAngle = (float)(newAngle - startAngle);

                                selectedShape.RotationAngle = rotationAngle;

                                angleVal.Text = (rotationAngle *= (float)(180 / Math.PI)).ToString("0.00") + "°";

                            }

                            if (scaleShape == true && selectedShape != null)
                            {
                                // middle point
                                float centerX = ((selectedShape.Start.X + selectedShape.End.X) / 2);
                                float centerY = ((selectedShape.End.Y + selectedShape.End.Y) / 2);

                                // original distance from the center of the shape to mouse position
                                float ogDist = (float)Math.Sqrt(Math.Pow((double)(centerX - mouseDown.X), 2) + Math.Pow((double)(centerY - mouseDown.Y), 2));
                                // current distance
                                float currDist = (float)Math.Sqrt(Math.Pow((double)(centerX - e.X), 2) + Math.Pow((double)(centerY - e.Y), 2));
                                
                                // calculate scale factor
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
            // save angle of last rotation
            angle = rotationAngle;

            // reset rubber banding
            rubberBanding = !rubberBanding;

            // if mouse postions between when it was first pressed and moved are different, add shapes to the list
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



