# drawing-application
## A drawing application implemented in C# using WinForms

Shapes that were implemented are: **square, triangle and circle**. Each shape has: Start and End point, rotation angle and scale factor used for transformations. On top of that each shape is being created with its own axis aligned bounding box that has a custom method that checks whether a point that the mouse clicked on is inside the shape or not, this way the user can select any shape they want on the screen to perform actions on it. 
When it comes to shape creation, rubber-banding was implemented. Upon choosing shape that the user wishes to create, the program tracks the mouse position from the moment a mouse button has been pressed until it’s released. When the mouse is released, and final shape drawn to the screen, it is being added to the list of shapes. The shapes are being drawn in the ‘OnPaint’ function while the mouse is moving so that the drawing process is visible on the screen and also, draws previously created shapes that are in the list of shapes. After creating the shapes, user can left click on any of them to highlight and select it at the same time. To keep track of which shape was selected a shape container in the form of generic shape object is created and when the shape is selected it is being assigned to that “selectedShape” container. Upon right clicking on the chosen shape, a context menu opens with following options:  

  • **Move** – move the shape around the screen by drag-and-drop action,

  • **Rotate** – drag the mouse around to rotate the shape (while the rotation angle is displayed in the right upper corner of the screen),
  
  • **Scale** – drag the mouse to make the shape bigger or smaller, 
  
  • **Delete** – remove the shape from the screen and the memory. 

The only shape where the rotation does not work as it is supposed to be is the circle. Due to the circle’s nature it is not really possible to visibly rotate it, nevertheless this option is still available with the circle. 
In addition, for the right click option – if the user clicks anywhere on the screen without selecting a shape, another context menu shows up that allows the user to create shape. 
When the application first starts, instructions for the functionality of the application are being displayed, they are also available at any time under “Help” tab in the main menu.  
Because of substantial flickering present in the application, it was decided to implement double buffering, but the flickering was still very much visible so as a test Windows Forms’ very own Double Buffering feature was turned on which got rid of the flickering completely, however being aware that we should not rely on any standard library features an option to switch between the two was included so that the user can choose whether they want to see double buffering written programatically or the one provided by Windows Forms.  
 
