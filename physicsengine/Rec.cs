using SlimDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
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

            Vector3[][] edges = new Vector3[][]
            {
                    new Vector3[] { v1, v2 },
                    new Vector3[] { v2, v3 },
                    new Vector3[] { v3, v4 },
                    new Vector3[] { v4, v1 }
            };

            bool[] edgescollided = new bool[4];

            int i = 0;

            foreach (var edge in edges)
            {
                edgescollided[i] = DetectCollisionWithEdge(ball, edge[0], edge[1], out collisionPoint);
                i++;
            }

            return collisionPoint;
        }

        public Vector3 GetEdgeNormal(Vector3 edgeStart, Vector3 edgeEnd)
        {
            // Calculate the edge vector
            Vector3 edge = edgeEnd - edgeStart;

            // Find the perpendicular vector (normal) to the edge
            Vector3 normal = new Vector3(-edge.Y, edge.X, 0);

            // Normalize the normal vector
            return Vector3.Normalize(normal);
        }

        public void RectangleProjection(Vector3 axis, out float min, out float max) // out means passed by reference
        {
            //// Normalize the axis
            //axis = Vector3.Normalize(axis);

            // Project each corner of the rectangle onto the axis
            float[] projections = new float[]
            {
                Vector3.Dot(v1, axis),
                Vector3.Dot(v2, axis),
                Vector3.Dot(v3, axis),
                Vector3.Dot(v4, axis)
            };

            // Find the minimum and maximum projections
            min = Math.Min(Math.Min(projections[0], projections[1]), Math.Min(projections[2], projections[3]));
            max = Math.Max(Math.Max(projections[0], projections[1]), Math.Max(projections[2], projections[3]));
        }

        public bool DetectCollisionWithEdge(Ball ball, Vector3 edgeStart, Vector3 edgeEnd, out Vector3? CollisionPoint)
        {
            Vector3 normalizedAxis = GetEdgeNormal(edgeStart, edgeEnd);
            float minRec, maxRec;
            float minBall, maxBall;

            RectangleProjection(normalizedAxis, out minRec, out maxRec);
            ball.BallProjection(normalizedAxis, out minBall, out maxBall);

            float A = edgeEnd.Y - edgeStart.Y;
            float B = edgeStart.X - edgeEnd.X;
            float C = (edgeEnd.X * edgeStart.Y) - (edgeStart.X * edgeEnd.Y);

            float Distance = (float)(Math.Abs(A * ball.Position.X + B * ball.Position.Y + C) / Math.Sqrt(A * A + B * B));

            if (!(maxBall < minRec || minBall > maxRec))
            {
                if (Distance <= ball.Radius)
                {
                    // Calculate the closest point on the line
                    float denom = A * A + B * B;
                    float t = ((ball.Position.X * A + ball.Position.Y * B + C) / denom);
                    Vector3 closestPoint = new Vector3(ball.Position.X - A * t / denom, ball.Position.Y - B * t / denom, 0);

                    // Check if the closestPoint lies within the edge segment bounds , el 3 vector are colliner
                    if (IsPointOnSegment(closestPoint, edgeStart, edgeEnd))
                    {
                        CollisionPoint = closestPoint;
                        return true;
                    }
                }
            }

            CollisionPoint = null;
            return false;
        }

        private bool IsPointOnSegment(Vector3 point, Vector3 segStart, Vector3 segEnd)
        {
            // Calculate the vectors for the segment and from segment start to point
            Vector3 segmentVector = segEnd - segStart;
            Vector3 pointVector = point - segStart;

            // Calculate dot products
            float segmentLengthSquared = Vector3.Dot(segmentVector, segmentVector);
            float projection = Vector3.Dot(pointVector, segmentVector);

            // Check if the point projection falls on the segment
            if (projection < 0 || projection > segmentLengthSquared)
            {
                return false;
            }

            // Check if the point is within the bounding box of the segment
            float minX = Math.Min(segStart.X, segEnd.X);
            float maxX = Math.Max(segStart.X, segEnd.X);
            float minY = Math.Min(segStart.Y, segEnd.Y);
            float maxY = Math.Max(segStart.Y, segEnd.Y);

            return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
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
