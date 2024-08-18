using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace physicsengine
{
    internal abstract class Shapes
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float BouncingFactor { get; set; } = 0.7f;
        public Shape DrawingShape { get; set; }
        public float Mass { get; set; }
        public Color Color { get; set; }

        public Shapes(float mass, Color color)
        {
            Mass = mass;
            Color = color;
        }

        public Vector3 GravityForce()
        {
            return new Vector3(0, Mass * 9.8f, 0);
        }

        public Vector3 FrictionForce()
        {
            float frictionCoefficient = 0.1f;
            return new Vector3(-frictionCoefficient * Velocity.X, 0, -frictionCoefficient * Velocity.Z);
        }

        public List<Vector3> TotalForces()
        {
            List<Vector3> forces = new List<Vector3>
            {
                GravityForce(),
                FrictionForce()
            };
            return forces;
        }


        
    }

}
