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
                            // Debug.WriteLine("collided");
                            checkBall.ResolveBallToBallCollison(shapeBall);
                            // Handle collision here
                        }
                    }
                    else if (check != shape && check is Rec checkRec && shape is Ball shapeBallcolwithrec)
                    {
                        Vector3? collisionpoint = checkRec.CheckCollision(shapeBallcolwithrec);
                    }

                }
            }
        }
    }
}
