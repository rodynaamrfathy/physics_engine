using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using SlimDX;

namespace physicsengine
{
    internal class Engine
    {
        private List<Shapes> shapes;

        public Engine()
        {
            shapes = new List<Shapes>();
        }

        public void AddShape(Shapes shape)
        {
            shapes.Add(shape);
        }

        public Shapes GetShapeFromDrawing(Shape drawingShape)
        {
            return shapes.FirstOrDefault(s => s.DrawingShape == drawingShape);
        }

        public static void ApplyCollisionResponse(Ball ball, Rec rect, Vector3 collisionPoint)
        {
                // Find the normal of the collision based on the point of collision
                Vector3 normal = Vector3.Zero;

                // Determine which edge of the rectangle the collision occurred with
                Vector3[][] edges = new Vector3[][]
                {
                    new Vector3[] { rect.v1, rect.v2 },
                    new Vector3[] { rect.v2, rect.v3 },
                    new Vector3[] { rect.v3, rect.v4 },
                    new Vector3[] { rect.v4, rect.v1 }
                };

                foreach (var edge in edges)
                {
                    Vector3 edgeNormal = rect.GetEdgeNormal(edge[0], edge[1]);
                    if (Math.Abs(Vector3.Dot(collisionPoint - edge[0], edgeNormal)) < 0.01f)
                    {
                        normal = edgeNormal;
                        break;
                    }
                }

                // Normalize the collision normal
                normal = Vector3.Normalize(normal);

                // Reflect ball's velocity over the normal
                Vector3 velocity = ball.Velocity;
                Vector3 velocityNormal = Vector3.Dot(velocity, normal) * normal;
                Vector3 velocityTangent = velocity - velocityNormal;
                Vector3 reflectedVelocity = velocityTangent - velocityNormal;

                // Apply the bouncing factor to the reflected velocity
                ball.Velocity = reflectedVelocity * ball.BouncingFactor;

                // Adjust position to resolve collision overlap
                Vector3 displacement = normal * (ball.Radius - Vector3.Dot(normal, collisionPoint - ball.Position));
                ball.Position += displacement;
        }


        public void Update(float deltaTime, float canvasHeight, float canvasWidth, bool IsMoving)
        {
            bool AreCollided;
            foreach (var check in shapes)
            {
                foreach (var shape in shapes)
                {
                    shape.UpdatePosition(deltaTime, canvasHeight, canvasWidth, IsMoving);

                    if (check != shape && check is Ball checkBall && shape is Ball shapeBall)
                    {
                        AreCollided = checkBall.AreCollidedBallToBall(shapeBall);
                        if (AreCollided)
                        {
                            checkBall.HandleOverlap(shapeBall);
                            checkBall.ResolveBallToBallCollison(shapeBall);
                        }
                    }
                    else if (check != shape && check is Rec checkRec && shape is Ball shapeBallcolwithrec)
                    {
                        Vector3? collisionPoint = checkRec.CheckCollision(shapeBallcolwithrec);
                        if (collisionPoint.HasValue)
                        {
                            ApplyCollisionResponse(shapeBallcolwithrec, checkRec, collisionPoint.Value);
                        }
                    }
                }
            }
        }
    }
}
