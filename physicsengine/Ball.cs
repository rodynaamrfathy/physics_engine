using SlimDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace physicsengine
{
    internal class Ball : Shapes
    {
        public float Radius { get; set; }

        public Ball(float mass, System.Drawing.Color color, float radius, Vector3 Pos) : base(mass, color)
        {
            // Set the initial position of the ball
            Velocity = new Vector3(0, 0, 0);
            Radius = radius;
            Position = Pos;
            DrawingShape = new Ellipse()
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)),
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }

        public bool AreCollidedBallToBall(Ball ball2)
        {
            if (ball2 == null) return false;

            // Compute the center of each ball
            float XCenterball1 = this.Position.X + this.Radius;
            float YCenterball1 = this.Position.Y + this.Radius;
            float XCenterball2 = ball2.Position.X + ball2.Radius;
            float YCenterball2 = ball2.Position.Y + ball2.Radius;

            float diffX = XCenterball1 - XCenterball2;
            float diffY = YCenterball1 - YCenterball2;
            float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);
            float r1 = this.Radius;
            float r2 = ball2.Radius;

            return distance <= (r1 + r2);
        }

        public void HandleOverlap(Ball ball2)
        {
            if (ball2 == null) return;

            // Compute the center of each ball
            float XCenterball1 = this.Position.X + this.Radius;
            float YCenterball1 = this.Position.Y + this.Radius;
            float XCenterball2 = ball2.Position.X + ball2.Radius;
            float YCenterball2 = ball2.Position.Y + ball2.Radius;

            Vector3 overlapVector = new Vector3(XCenterball1 - XCenterball2, YCenterball1 - YCenterball2, 0);
            float distance = overlapVector.Length();
            float overlap = (this.Radius + ball2.Radius) - distance;

            if (overlap > 0)
            {
                Vector3 separationDirection = Vector3.Normalize(overlapVector);
                Vector3 displacement = separationDirection * (overlap / 2.0f);

                this.Position += displacement;
                ball2.Position -= displacement;
            }
        }


        public void ResolveBallToBallCollison(Ball ball)
        {
            Vector3 Normal = new Vector3(this.Position.X - ball.Position.X, this.Position.Y - ball.Position.Y, 0);
            float NormNormal = (float) Math.Sqrt(Math.Pow(Normal.X, 2) + Math.Pow(Normal.Y, 2));
            Vector3 UnitNormal = new Vector3(Normal.X/NormNormal, Normal.Y / NormNormal, 0);
            Vector3 UnitTangent = new Vector3(- UnitNormal.Y, UnitNormal.X, 0);

            float V1n = Vector3.Dot(UnitNormal, ball.Velocity) * ball.BouncingFactor;
            float V1t = Vector3.Dot(UnitTangent, ball.Velocity); // unchanged before and after collision 
            float V2n = Vector3.Dot(UnitNormal, this.Velocity) * this.BouncingFactor;
            float V2t = Vector3.Dot(UnitTangent, this.Velocity); // unchanged before and after collision 

            float MassSum = this.Mass + ball.Mass;
            float MassDiff = ball.Mass - this.Mass;

            float V1nAfterCollision = ((V1n * MassDiff) +(2 * this.Mass * V2n))/ MassSum;
            float V2nAfterCollision = ((V2n * MassDiff) + (2 * ball.Mass * V1n)) / MassSum;

            Vector3 V1nAfterCollisionVector = new Vector3(UnitNormal.X * V1nAfterCollision, UnitNormal.Y * V1nAfterCollision, UnitNormal.Z * V1nAfterCollision);
            Vector3 V2nAfterCollisionVector = new Vector3(UnitNormal.X * V2nAfterCollision, UnitNormal.Y * V2nAfterCollision, UnitNormal.Z * V2nAfterCollision);
            Vector3 V1tVector = new Vector3(UnitTangent.X * V1t, UnitTangent.Y * V1t, UnitTangent.Z * V1t);
            Vector3 V2tVector = new Vector3(UnitTangent.X * V2t, UnitTangent.Y * V2t, UnitTangent.Z * V2t);

            // update final velocities

            this.Velocity = new Vector3(V2nAfterCollisionVector.X + V2tVector.X, V2nAfterCollisionVector.Y + V2tVector.Y, V2nAfterCollisionVector.Z + V2tVector.Z);
            ball.Velocity = new Vector3(V1nAfterCollisionVector.X + V1tVector.X, V1nAfterCollisionVector.Y + V1tVector.Y, V1nAfterCollisionVector.Z + V1tVector.Z);

        }

        public override void UpdatePosition(float deltaTime, float canvasHeight, float canvasWidth, bool IsMoving)
        {
            // Calculate the gravity force
            Vector3 gravityForce = GravityForce();

            // Update velocity based on gravity
            Velocity += gravityForce * deltaTime / Mass;

            // Update position based on velocity
            Position += Velocity * deltaTime;

            // Check if the shape hits the top of the canvas
            if (Position.Y <= 0)
            {
                Position = new Vector3(Position.X, 0, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);
            }

            // Check if the shape hits the right edge of the canvas
            if (Position.X + Radius * 2 >= canvasWidth)
            {
                Position = new Vector3(canvasWidth - Radius * 2, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
            }

            // Check if the shape hits the left edge of the canvas
            if (Position.X <= 0)
            {
                Position = new Vector3(0, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
            }

            // Check if the shape hits the bottom of the canvas
            if (Position.Y + Radius * 2 >= canvasHeight)
            {
                Position = new Vector3(Position.X, canvasHeight - Radius * 2, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);

                // If the absolute value of velocity is small, stop the shape
                if (Math.Abs(Velocity.Y) < 1 && Math.Abs(Velocity.X) < 1)
                {
                    IsMoving = false; // Assuming there's an IsMoving property in Shapes
                    Velocity = Vector3.Zero;
                }
            }

            // Update the drawing shape position if it's not null
            if (this.DrawingShape != null)
            {
                Canvas.SetTop(DrawingShape, Position.Y);
                Canvas.SetLeft(DrawingShape, Position.X);
            }
        }
    }
}
