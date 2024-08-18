using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Shapes;
namespace physicsengine
{
    internal class Ball : Shapes
    {
        public float Radius { get; set; }

        public Ball(float mass, System.Drawing.Color color, float radius,float canvasHeight) : base(mass, color)
        {
            // Set the initial position of the ball
            Position = new Vector3(0, canvasHeight + radius * 2, 0);
            Radius = radius;
            DrawingShape = new Ellipse()
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)),
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
    }
}