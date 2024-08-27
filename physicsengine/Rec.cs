using SlimDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Matrix = SlimDX.Matrix;

namespace physicsengine
{
    internal class Rec : Shapes
    {
        public float Angle;
        private RotateTransform rotateTransform;
        public Matrix RotMat;
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;
        public Vector3 v4;
        public Vector3 Center { get; private set; }

        public Rec(float mass, System.Drawing.Color color, float width, float height, Vector3 Pos) : base(mass, color)
        {
            Angle = 0;
            // Set the initial position of the rectangle
            Velocity = new Vector3(0, 0, 0);
            RotMat = new Matrix();

            DrawingShape = new System.Windows.Shapes.Rectangle()
            {
                Width = width,
                Height = height,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B)),
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Position = Pos;
            Center = new Vector3((float)(Position.X - (DrawingShape.Width / 2)), (float)(Position.Y - (DrawingShape.Height / 2)), 0);

            // Initialize corners
            UpdateCorners();

            rotateTransform = new RotateTransform();
            DrawingShape.RenderTransform = rotateTransform;
            DrawingShape.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5); // Rotate around the center
        }

        public void UpdateCorners()
        {
            float halfWidth = (float)DrawingShape.Width / 2;
            float halfHeight = (float)DrawingShape.Height / 2;

            v1 = new Vector3(Center.X - halfWidth, Center.Y - halfHeight, 0);
            v2 = new Vector3(Center.X + halfWidth, Center.Y - halfHeight, 0);
            v3 = new Vector3(Center.X + halfWidth, Center.Y + halfHeight, 0);
            v4 = new Vector3(Center.X - halfWidth, Center.Y + halfHeight, 0);

            ApplyRotation();
        }

        private void ApplyRotation()
        {
            // Create a rotation matrix
            RotMat = Matrix.RotationZ(Angle);

            // Apply rotation to each corner
            v1 = Vector3.TransformCoordinate(v1, RotMat);
            v2 = Vector3.TransformCoordinate(v2, RotMat);
            v3 = Vector3.TransformCoordinate(v3, RotMat);
            v4 = Vector3.TransformCoordinate(v4, RotMat);
        }

        public Vector3? CheckCollision(Ball ball)
        {
            if (ball == null) return null;
            Vector3? collisionPoint = null;

            return collisionPoint;
        }

        public void GetEdgeNormal(Vector3 edgeStart, Vector3 edgeEnd)
        {
            Vector3 edge = new Vector3 (edgeEnd.X - edgeStart.X, edgeEnd.Y - edgeStart.Y, edgeEnd.Z - edgeStart.Z);

        }

        public void RectangleProjection(Vector3 axis)
        {

        }

        public void BallProjection(Vector3 axis, Ball ball)
        {
            if (ball == null) return;
        }

        public void DetectCollisionWithEdge(Ball ball)
        {

        }

        public void ResolveCollision()
        {

        }

        public override void UpdatePosition(float deltaTime, float canvasHeight, float canvasWidth, bool IsMoving)
        {
            // Update the position based on velocity and time step
            Position += Velocity * deltaTime;
            UpdateCorners(); // Update corner positions after the position change

            // Define the boundaries of the simulation area
            double minX = 0;
            double minY = 0;
            double maxX = canvasWidth;
            double maxY = canvasHeight;

            // Define the rectangle's bounds in the canvas
            float halfWidth = (float)DrawingShape.Width / 2;
            float halfHeight = (float)DrawingShape.Height / 2;

            // Check for collisions with the simulation boundaries and adjust velocity accordingly
            if (Position.X - halfWidth < minX)
            {
                Position = new Vector3((float)minX + halfWidth, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
            }
            if (Position.X + halfWidth > maxX)
            {
                Position = new Vector3((float)maxX - halfWidth, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
            }
            if (Position.Y - halfHeight < minY)
            {
                Position = new Vector3(Position.X, (float)minY + halfHeight, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);
            }
            if (Position.Y + halfHeight > maxY)
            {
                Position = new Vector3(Position.X, (float)maxY - halfHeight, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);
            }

            // Update the drawing shape position
            if (this.DrawingShape != null)
            {
                Canvas.SetLeft(DrawingShape, Position.X - halfWidth);
                Canvas.SetTop(DrawingShape, Position.Y - halfHeight);
            }

            // Update the rotation of the drawing shape
            rotateTransform.Angle = Angle;
        }

        public void ApplyTorque(float force, float deltaTime)
        {
            float Torque = force;
            float AngularAcceleration = Torque / Mass;
            Angle += AngularAcceleration * deltaTime;
        }
    }
}
