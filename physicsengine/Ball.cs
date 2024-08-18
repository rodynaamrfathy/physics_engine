using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
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
            Velocity = new Vector3(0, 0, 0);
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


        public void UpdatePosition(float deltaTime, float canvasHeight, float canvasWidth)
        {
            Vector3 gravityForce = GravityForce();

            // Update velocity based on gravity
            Velocity += gravityForce * deltaTime / Mass;

            // Update position based on velocity
            Position += Velocity * deltaTime;

            // Check if the ball hits the top of the canvas
            if (Position.Y <= 0)
            {
                Position = new Vector3(Position.X, 0, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);
            }
            // Check if the ball hits the right edge of the canvas
            if (Position.X + Radius * 2 >= canvasWidth)
            {
                Position = new Vector3(canvasWidth - Radius * 2, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
            }
            // Check if the ball hits the left edge of the canvas
            if (Position.X <= 0)
            {
                Position = new Vector3(0, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
            }
            // Update the drawing shape position
            if (DrawingShape != null)
            {
                Canvas.SetTop(DrawingShape, Position.Y);
                Canvas.SetLeft(DrawingShape, Position.X);
            }
        }
    }
}
