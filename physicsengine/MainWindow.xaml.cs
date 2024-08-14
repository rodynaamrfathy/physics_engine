using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using SlimDX;

namespace physicsengine
{

    public partial class MainWindow : Window
    {

        private bool isDragged = false;
        private Ellipse ball;
        private Point currentPosition;
        double s0, v0 = 0, t = 0, a = 9.8;
        double bouncingfactor = 0.7, ballbottom = 0, canvasHeight;
        bool IsMoving;
        Vector3 x = new Vector3 (0,0.1f,0);

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
            Canvas.SetTop(ball, ballcanvas.ActualHeight);
            ball.MouseLeftButtonDown += Ball_MouseLeftButtonDown;
            ball.MouseLeftButtonUp += Ball_MouseLeftButtonUp;
            ballcanvas.MouseMove += Ballcanvas_MouseMove;

        }
        private void Ball_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragged = true;
            v0 = 0;
            t = 0;
            a = 9.8;
            bouncingfactor = 0.7;
            ball.CaptureMouse();
        }
        private void Ball_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragged)
            {
                isDragged = false;
                ball.ReleaseMouseCapture();
      
                s0 = Canvas.GetTop(ball);
               
                IsMoving = true;
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
                Canvas.SetLeft(ball, mousepos.X - currentPosition.X);
                Canvas.SetTop(ball, mousepos.Y - currentPosition.Y);
            }
        }
        private void StartFreeFall()
        {
            while (IsMoving)
            {
                Dispatcher.Invoke(() =>
            {
                double s = s0 + v0 * t + 0.5 * a * t * t;
                double v = v0 + a * t;
                Canvas.SetTop(ball, s);


                ballbottom = s + ball.Height;
                canvasHeight = ballcanvas.ActualHeight;
                // one the ball hits the end of the canvas it will star bouncing
                if (ballbottom >= canvasHeight)
                {
                    v0 = -v * bouncingfactor;
                    s0 = canvasHeight - ball.Height;

                    t = 0.16;

                    // if the abs of v0 is less than 1 stop the ball
                    if (Math.Abs(v) < 1)
                        IsMoving = false;

                }
                else
                    t += 0.16;

            });
                Thread.Sleep(10);
            }
        }

    }
}