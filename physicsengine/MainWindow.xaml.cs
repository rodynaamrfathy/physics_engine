using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using SlimDX;

namespace physicsengine
{
    public partial class MainWindow : Window
    {
        private bool isDragged = false;
        private Ball ball;
        private double ballbottom = 0, canvasHeight;
        private bool IsMoving;
        private Stopwatch stopwatch;

        public MainWindow()
        {
            InitializeComponent();
            CreateBall();
            stopwatch = new Stopwatch();
        }

        private void CreateBall()
        {
            // Create a new Ball object
            ball = new Ball(1.0f, System.Drawing.Color.Red, 50f); // mass = 1.0, radius = 50

            // Add the ball's DrawingShape (Ellipse) to the canvas
            ballcanvas.Children.Add(ball.DrawingShape);

            // events
            ball.DrawingShape.MouseLeftButtonDown += Ball_MouseLeftButtonDown;
            ball.DrawingShape.MouseLeftButtonUp += Ball_MouseLeftButtonUp;
            ballcanvas.MouseMove += Ballcanvas_MouseMove;
        }

        private void Ball_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragged = true;
            ball.DrawingShape.CaptureMouse();
            stopwatch.Reset();
        }

        private void Ball_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragged)
            {
                isDragged = false;
                ball.DrawingShape.ReleaseMouseCapture();
                ball.Velocity = new Vector3(0, 0, 0);
                IsMoving = true;
                stopwatch.Start(); // Start the stopwatch to track deltaTime
                Task.Run(() =>
                {
                    StartFreeFall();
                });
            }
        }

        private void Ballcanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {
                var mousepos = e.GetPosition(ballcanvas);
                ball.Position = new Vector3((float)mousepos.X - ball.Radius, (float)mousepos.Y - ball.Radius, 0);
            }
        }

        private void StartFreeFall()
        {
            while (IsMoving)
            {
                Dispatcher.Invoke(() =>
                {
                    // Calculate deltaTime in seconds
                    float deltaTime = (float)stopwatch.Elapsed.TotalSeconds * 10;
                    stopwatch.Restart(); // Restart the stopwatch for the next frame

                    // Update the position and velocity
                    ball.UpdatePosition(deltaTime, (float)ballcanvas.ActualHeight, (float)ballcanvas.ActualWidth);

                    ballbottom = ball.Position.Y + ball.Radius * 2;
                    canvasHeight = ballcanvas.ActualHeight;

                    // Once the ball hits the bottom of the canvas, make it bounce
                    if (ballbottom >= canvasHeight)
                    {
                        ball.Velocity = new Vector3(ball.Velocity.X, -ball.Velocity.Y * ball.BouncingFactor, ball.Velocity.Z);

                        // s0 hena b top value el fo2 el ball ashan hya dlw2ty lmsa the end of the canvas
                        ball.Position = new Vector3(ball.Position.X, (float)(canvasHeight - ball.Radius * 2), ball.Position.Z);

                        // If the absolute value of velocity is small, stop the ball
                        if (Math.Abs(ball.Velocity.Y) < 1 && Math.Abs(ball.Velocity.X) < 1)
                            IsMoving = false;
                    }
                });

                Thread.Sleep(10); // Small delay to avoid overloading the CPU
            }

            stopwatch.Stop(); // Stop the stopwatch when the simulation ends
        }


    }
}
