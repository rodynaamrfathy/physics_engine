using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Update(float deltaTime, float canvasHeight, float canvasWidth)
        {
            foreach (var shape in shapes)
            {
                shape.UpdatePosition(deltaTime, canvasHeight, canvasWidth);
            }
        }
        
    }
}
