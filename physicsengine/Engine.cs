using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

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
            foreach (var shape in shapes)
            {
                shape.UpdatePosition(deltaTime, canvasHeight, canvasWidth, IsMoving);
            }
        }
    }


}
