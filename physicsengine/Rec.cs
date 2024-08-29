using SlimDX;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Matrix = SlimDX.Matrix;

namespace physicsengine
{
    internal class Rec : Shapes
    {
        public float AngularVelocity;
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
            Center = Position;

            UpdateCorners();

            rotateTransform = new RotateTransform();
            DrawingShape.RenderTransform = rotateTransform;
            DrawingShape.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5); // Rotate around the center
        }

        public void UpdateCorners()
        {
            float halfWidth = (float)DrawingShape.Width / 2;
            float halfHeight = (float)DrawingShape.Height / 2;

            // Define the local corners before rotation
            v1 = new Vector3(-halfWidth, -halfHeight, 0);
            v2 = new Vector3(halfWidth, -halfHeight, 0);
            v3 = new Vector3(halfWidth, halfHeight, 0);
            v4 = new Vector3(-halfWidth, halfHeight, 0);

            ApplyRotation();

            // Translate the rotated corners to the rectangle's position
            v1 += Center;
            v2 += Center;
            v3 += Center;
            v4 += Center;
        }

        private void ApplyRotation()
        {
            // Create a rotation matrix
            RotMat = Matrix.RotationZ(Angle);

            // Apply rotation to each corner relative to the center (local space)
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

            bool[] edgesCollided = new bool[4];
            int i = 0;

            foreach (var edge in edges)
            {
                edgesCollided[i] = DetectCollisionWithEdge(ball, edge[0], edge[1], out collisionPoint);
                i++;
            }

            return collisionPoint;
        }

        public Vector3 GetEdgeNormal(Vector3 edgeStart, Vector3 edgeEnd)
        {
            Vector3 edge = edgeEnd - edgeStart;
            Vector3 normal = new Vector3(-edge.Y, edge.X, 0);
            return Vector3.Normalize(normal);
        }

        public void RectangleProjection(Vector3 axis, out float min, out float max)
        {
            float[] projections = new float[]
            {
                Vector3.Dot(v1, axis),
                Vector3.Dot(v2, axis),
                Vector3.Dot(v3, axis),
                Vector3.Dot(v4, axis)
            };

            min = Math.Min(Math.Min(projections[0], projections[1]), Math.Min(projections[2], projections[3]));
            max = Math.Max(Math.Max(projections[0], projections[1]), Math.Max(projections[2], projections[3]));
        }

        public bool DetectCollisionWithEdge(Ball ball, Vector3 edgeStart, Vector3 edgeEnd, out Vector3? collisionPoint)
        {
            collisionPoint = null;

            Vector3 edge = edgeEnd - edgeStart;
            Vector3 edgeNormal = GetEdgeNormal(edgeStart, edgeEnd);

            RectangleProjection(edgeNormal, out float minRec, out float maxRec);
            ball.BallProjection(edgeNormal, out float minBall, out float maxBall);

            if (maxBall < minRec || minBall > maxRec)
            {
                return false; // No collision
            }

            float A = edge.Y;
            float B = -edge.X;
            float C = (edge.X * edgeStart.Y) - (edge.Y * edgeStart.X);

            float distance = Math.Abs(A * ball.Position.X + B * ball.Position.Y + C) / (float)Math.Sqrt(A * A + B * B);

            if (distance <= ball.Radius)
            {
                float denom = A * A + B * B;
                float t = ((ball.Position.X * A + ball.Position.Y * B + C) / denom);

                Vector3 closestPoint = new Vector3(
                    ball.Position.X - A * t / denom,
                    ball.Position.Y - B * t / denom,
                    0
                );

                if (IsPointOnSegment(closestPoint, edgeStart, edgeEnd))
                {
                    collisionPoint = closestPoint;
                    return true;
                }
            }

            return false;
        }

        private bool IsPointOnSegment(Vector3 point, Vector3 segStart, Vector3 segEnd)
        {
            Vector3 segmentVector = segEnd - segStart;
            Vector3 pointVector = point - segStart;

            float segmentLengthSquared = Vector3.Dot(segmentVector, segmentVector);
            float projection = Vector3.Dot(pointVector, segmentVector);

            if (projection < 0 || projection > segmentLengthSquared)
            {
                return false;
            }

            float minX = Math.Min(segStart.X, segEnd.X);
            float maxX = Math.Max(segStart.X, segEnd.X);
            float minY = Math.Min(segStart.Y, segEnd.Y);
            float maxY = Math.Max(segStart.Y, segEnd.Y);

            return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
        }

        public override void UpdatePosition(float deltaTime, float canvasHeight, float canvasWidth, bool IsMoving)
        {
            Vector3 gravityForce = GravityForce();
            Velocity += gravityForce * deltaTime / Mass;
            Position += Velocity * deltaTime;

            Angle += AngularVelocity * deltaTime;

            if (rotateTransform != null)
            {
                rotateTransform.Angle = Angle;
            }

            UpdateCorners();

            float halfWidth = (float)DrawingShape.Width / 2;
            float halfHeight = (float)DrawingShape.Height / 2;

            if (Position.X - halfWidth < 0)
            {
                Position = new Vector3(halfWidth, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
                AngularVelocity = (-Velocity.X * BouncingFactor / Mass) / deltaTime / 10;
            }
            if (Position.X + halfWidth > canvasWidth)
            {
                Position = new Vector3(canvasWidth - halfWidth, Position.Y, Position.Z);
                Velocity = new Vector3(-Velocity.X * BouncingFactor, Velocity.Y, Velocity.Z);
                AngularVelocity = (-Velocity.X * BouncingFactor / Mass) / deltaTime / 10;
            }
            if (Position.Y - halfHeight < 0)
            {
                Position = new Vector3(Position.X, halfHeight, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);
                AngularVelocity = (-Velocity.X * BouncingFactor / Mass) / deltaTime / 10;
            }
            if (Position.Y + halfHeight > canvasHeight)
            {
                Position = new Vector3(Position.X, canvasHeight - halfHeight, Position.Z);
                Velocity = new Vector3(Velocity.X, -Velocity.Y * BouncingFactor, Velocity.Z);
                AngularVelocity = (-Velocity.X * BouncingFactor / Mass) / deltaTime / 10;
            }

            if (DrawingShape != null)
            {
                Canvas.SetLeft(DrawingShape, Position.X - halfWidth);
                Canvas.SetTop(DrawingShape, Position.Y - halfHeight);
            }
        }
    }
}
