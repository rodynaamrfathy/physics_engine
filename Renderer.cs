using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace physicsengine
{
    internal class Renderer
    {
        private Canvas canvas;

        public Renderer(Canvas canvas)
        {
            this.canvas = canvas; 
        }

        public void AddShapeToCanvas(Shapes shape)
        {
            canvas.Children.Add(shape.DrawingShape);
        }
        //public void UpdateCanvas()
        //{

        //}

    }
}
