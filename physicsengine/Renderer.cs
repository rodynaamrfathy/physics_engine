using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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

        public void UpdateShapePosition(Shapes shape)
        {
            Canvas.SetLeft(shape.DrawingShape, shape.Position.X);
            Canvas.SetTop(shape.DrawingShape, shape.Position.Y);
        }

        public void SetInitialShapePosition(Shapes shape)
        {
            Canvas.SetLeft(shape.DrawingShape, (float) 0.5 * ( canvas.ActualWidth + shape.DrawingShape.Width));
            Canvas.SetTop(shape.DrawingShape, (float) canvas.ActualHeight - shape.DrawingShape.Height);
        }

        public void UpdateCanvas()
        {
            foreach (UIElement child in canvas.Children)
            {
                if (child is Shape drawingShape)
                {
                    var shape = canvas.Children.OfType<Shapes>().FirstOrDefault(s => s.DrawingShape == drawingShape);
                    if (shape != null)
                    {
                        UpdateShapePosition(shape);
                    }
                }
            }
        }
    }



}
