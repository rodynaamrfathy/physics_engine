using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using SlimDX;
using System.Windows.Shapes;

namespace physicsengine
{
    public partial class MainWindow : Window
    {
        private Engine engine = new Engine();
        private Renderer renderer;
        private bool isDragged = false;
        private Stopwatch stopwatch;
        private Shapes draggedShape;


        public MainWindow()
        {
            InitializeComponent();
            renderer = new Renderer(ballcanvas);
            stopwatch = new Stopwatch();


            //CreateShape(new Ball(1.0f, System.Drawing.Color.Red, 20f) { Position = new Vector3(50f,50f,0)});
            CreateShape(new Ball(4.0f, System.Drawing.Color.Blue, 70f) { Position = new Vector3(100f, 100f, 0) });
            CreateShape(new Ball(2.0f, System.Drawing.Color.Green, 50f) { Position = new Vector3(300f, 150f, 0) });

        }

        private void CreateShape(Shapes shape)
        {
            shape.DrawingShape.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
            shape.DrawingShape.MouseLeftButtonUp += Shape_MouseLeftButtonUp;
            ballcanvas.MouseMove += Ballcanvas_MouseMove;

            engine.AddShape(shape);
            renderer.AddShapeToCanvas(shape);

            renderer.UpdateShapePosition(shape);
        }

        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedShape = engine.GetShapeFromDrawing(sender as Shape);
            if (draggedShape != null)
            {
                isDragged = true;
                draggedShape.DrawingShape.CaptureMouse();
                stopwatch.Reset();
            }
        }

        private void Shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedShape != null && isDragged)
            {
                isDragged = false;
                draggedShape.DrawingShape.ReleaseMouseCapture();
                draggedShape.Velocity = Vector3.Zero; // Stop movement
                stopwatch.Start(); // Start the stopwatch for physics update
                Task.Run(() => StartFreeFall());
            }
        }

        private void Ballcanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged && draggedShape != null)
            {
                var mousePos = e.GetPosition(ballcanvas);
                draggedShape.Position = new Vector3((float)(mousePos.X - draggedShape.DrawingShape.Width / 2),
                                                    (float)(mousePos.Y - draggedShape.DrawingShape.Height / 2), 0);
                renderer.UpdateShapePosition(draggedShape); // Update the canvas immediately
            }
        }

        private void StartFreeFall()
        {
            while (draggedShape != null && !draggedShape.IsMoving)
            {
                Dispatcher.Invoke(() =>
                {
                    float deltaTime = (float)stopwatch.Elapsed.TotalSeconds * 10;
                    stopwatch.Restart();
                    // update physics and check for collision
                    engine.Update(deltaTime, (float)ballcanvas.ActualHeight, (float)ballcanvas.ActualWidth, true);
                    renderer.UpdateCanvas(); // Redraw the canvas to reflect changes
                });

                Thread.Sleep(10); // Control update frequency
            }

            stopwatch.Stop(); // Stop when the simulation ends
        }
    }
}
