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
        public float AngularVelocity;
        public float Angle;
        private RotateTransform rotateTransform;
        public Matrix RotMat;
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;
        public Vector3 v4;
        public Vector3 Center;

        public Rec(float mass, System.Drawing.Color color, float width, float height, Vector3 Pos) : base(mass, color)
        {
            Angle = 0;
            AngularVelocity = 0;
            // Set the initial position of the ball
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

            v1 = new Vector3(-(float)(Center.X + (DrawingShape.Width / 2)), (float)(Center.Y + (DrawingShape.Height / 2)), 0);
            v2 = new Vector3((float)(Center.X + (DrawingShape.Width / 2)), (float)(Center.Y + (DrawingShape.Height / 2)), 0);
            v3 = new Vector3((float)(Center.X + (DrawingShape.Width / 2)), -(float)(Center.Y + (DrawingShape.Height / 2)), 0);
            v4 = new Vector3(-(float)(Center.X + (DrawingShape.Width / 2)), -(float)(Center.Y + (DrawingShape.Height / 2)), 0);


            rotateTransform = new RotateTransform();
            DrawingShape.RenderTransform = rotateTransform;
            DrawingShape.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5); // Rotate around the center
        }

        public Vector3 AreCollidedRecToBall(Ball ball)
        {
            Vector3 CoillisionPoint = new Vector3();
            if (ball != null)
            {
                return CoillisionPoint;
            }
            // check the distance between the borders of the circle with the rectangle
            // each time the ball moves it should check the the distance from the center
            // of the ball to the center of the rectangle + distance to the broder of the rectangle
            // where the ball collided with the rectangle

            // calc the corner vectors of the rectangle
            Center = new Vector3((float)(Position.X - (DrawingShape.Width / 2)), (float)(Position.Y - (DrawingShape.Height / 2)), 0);

            v1 = new Vector3(-(float)(Center.X + (DrawingShape.Width / 2)), (float)(Center.Y + (DrawingShape.Height / 2)), 0);
            v2 = new Vector3((float)(Center.X + (DrawingShape.Width / 2)), (float)(Center.Y + (DrawingShape.Height / 2)), 0);
            v3 = new Vector3((float)(Center.X + (DrawingShape.Width / 2)), -(float)(Center.Y + (DrawingShape.Height / 2)), 0);
            v4 = new Vector3(-(float)(Center.X + (DrawingShape.Width / 2)), -(float)(Center.Y + (DrawingShape.Height / 2)), 0);

            float XCenterball = ball.Position.X + ball.Radius;
            float YCenterball = ball.Position.Y + ball.Radius;

            Vector3 Ballpos = new Vector3((float)Canvas.GetRight(ball.DrawingShape), YCenterball, 0);
            for (Vector3 LeftSide = v1; LeftSide.Y >= v4.Y; LeftSide = new Vector3(LeftSide.X, LeftSide.Y - 1, LeftSide.Z))
            {
                if (LeftSide.X == Ballpos.X && YCenterball <= v1.Y && YCenterball > v4.Y)
                {
                    Debug.WriteLine("Left side collision");
                    CoillisionPoint = Ballpos;
                    return CoillisionPoint;
                }
            }
            Ballpos = new Vector3((float)Canvas.GetLeft(ball.DrawingShape), YCenterball, 0);
            for (Vector3 RightSide = v2; RightSide.Y >= v4.Y; RightSide = new Vector3(RightSide.X, RightSide.Y - 1, RightSide.Z))
            {
                if (RightSide.X == Ballpos.X && YCenterball <= v2.Y && YCenterball > v3.Y)
                {
                    Debug.WriteLine("Right side collision");
                    CoillisionPoint = Ballpos;
                    return CoillisionPoint;
                }
            }
            Ballpos = new Vector3((float)Canvas.GetBottom(ball.DrawingShape), YCenterball, 0);
            for (Vector3 UpperSide = v1; UpperSide.X <= v2.X; UpperSide = new Vector3(UpperSide.X + 1, UpperSide.Y, UpperSide.Z))
            {
                if (UpperSide.Y == Ballpos.Y && XCenterball <= v2.X && XCenterball > v1.X)
                {
                    Debug.WriteLine("upper side collision");
                    CoillisionPoint = Ballpos;
                    return CoillisionPoint;
                }
            }
            Ballpos = new Vector3((float)Canvas.GetTop(ball.DrawingShape), YCenterball, 0);
            for (Vector3 LowerSide = v4; LowerSide.X <= v3.X; LowerSide = new Vector3(LowerSide.X + 1, LowerSide.Y, LowerSide.Z))
            {
                if (LowerSide.Y == Ballpos.Y && XCenterball <= v3.X && XCenterball > v4.X)
                {
                    Debug.WriteLine("Lower side collision");
                    CoillisionPoint = Ballpos;
                    return CoillisionPoint;
                }
            }

            return CoillisionPoint;
        }

        public void ResolveBallToBallCollison(Ball ball)
        {

        }

        public override void UpdatePosition(float deltaTime, float canvasHeight, float canvasWidth, bool IsMoving)
        {


        }
    }
}

