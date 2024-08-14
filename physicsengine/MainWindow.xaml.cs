using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SlimDX;

namespace physicsengine
{
    public partial class MainWindow : Window
    {
        private bool isDragged = false;
        private Ellipse ball;
        private Vector3 position;
        private Vector3 velocity = new Vector3(0, 0, 0);
        private Vector3 gravity = new Vector3(0, 9.8f, 0);
        private float bounceFactor = 0.7f;
        private bool isMoving;
        private Point clickPosition;
        private double canvasHeight;

        public MainWindow()
        {
            InitializeComponent();
            CreateBall();
        }

        private void CreateBall()
        {
            ball = new Ellipse()
            {
                Width = 100,
                Height = 100,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };
            ballcanvas.Children.Add(ball);
            canvasHeight = ballcanvas.ActualHeight;
            position = new Vector3((float)(ballcanvas.ActualWidth / 2 - ball.Width / 2), (float)(canvasHeight - ball.Height), 0);
            Canvas.SetLeft(ball, position.X);
            Canvas.SetTop(ball, position.Y);
            ball.MouseLeftButtonDown += Ball_MouseLeftButtonDown;
            ball.MouseLeftButtonUp += Ball_MouseLeftButtonUp;
            ballcanvas.MouseMove += Ballcanvas_MouseMove;
        }

        private void Ball_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragged = true;
            velocity = new Vector3(0, 0, 0); // Reset velocity
            clickPosition = e.GetPosition(ball); // Store the position where the ball was clicked
            ball.CaptureMouse();
        }

        private void Ball_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragged)
            {
                isDragged = false;
                ball.ReleaseMouseCapture();
                isMoving = true;
                Task.Run(StartFreeFall);
            }
        }

        private void Ballcanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {
                var mousePos = e.GetPosition(ballcanvas);
                position.X = (float)(mousePos.X - clickPosition.X);
                position.Y = (float)(mousePos.Y - clickPosition.Y);
                Canvas.SetLeft(ball, position.X);
                Canvas.SetTop(ball, position.Y);
            }
        }

        private void StartFreeFall()
        {
            const float dt = 0.016f; // Time step (~16ms for 60 FPS)
            while (isMoving)
            {
                Dispatcher.Invoke(() =>
                {
                    // Update velocity and position
                    velocity += gravity * dt;
                    position += velocity * dt;

                    // Check for collision with the bottom of the canvas
                    if (position.Y + ball.Height >= canvasHeight)
                    {
                        // Bounce
                        velocity.Y = -velocity.Y * bounceFactor;
                        position.Y = (float)(canvasHeight - ball.Height); // Reset position just above the bottom

                        // Stop if the bounce is negligible
                        if (Math.Abs(velocity.Y) < 1)
                            isMoving = false;
                    }

                    // Apply the new position to the ball
                    Canvas.SetLeft(ball, position.X);
                    Canvas.SetTop(ball, position.Y);
                });

                Thread.Sleep((int)(dt * 1000)); // Maintain frame rate
            }
        }
    }
}