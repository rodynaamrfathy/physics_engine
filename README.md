
# 🧩 2D Physics Engine

A lightweight **2D physics engine** written in **C# and WPF**, built for learning and experimentation with **collision detection and response** between dynamic objects.

---

## 🧠 Overview

This project implements a **2D physics simulation framework** using principles from classical mechanics.
It supports real-time motion, collisions, and dynamic interactions between rigid bodies — with a focus on **collision handling between two spheres** in a 2D plane, based on academic research.

The rendering layer is powered by **SlimDX**, providing smooth frame updates and visual simulation.

---

## ⚙️ Features

* 🔹 **Rigid body motion** (position, velocity, acceleration updates)
* ⚡ **Collision detection and response** between 2D circles (spheres in a plane)
* 🧮 **Physics formulas derived from research papers** for realistic momentum transfer
* 🪶 **Lightweight and modular architecture** — easy to extend for new shapes or forces
* 🖥️ **SlimDX-based rendering** for real-time visualization
* 🧰 Written in **C# (.NET / WPF)**

---

## 🧾 Reference Paper

The collision response implementation is based on the paper:

> *“Collision Between Two Spheres in a Two-Dimensional Plane”*
> 

This paper was used to derive the mathematical model for elastic collisions, ensuring accurate momentum and energy conservation.

---

## 🚀 Getting Started

### Prerequisites

* [.NET 6 or later](https://dotnet.microsoft.com/download)
* [SlimDX SDK](https://slimdx.org/)
* Visual Studio (recommended) or VS Code

### Installation

```bash
git clone https://github.com/rodynaamrfathy/physics_engine.git
cd physics_engine
```

Open the project in Visual Studio and build the solution.

### Run

Simply execute the compiled project — a 2D simulation window will appear showing sphere motion and collision behavior.

---

## 💡 Example Simulation

Example of how objects behave under the engine:

```csharp
var world = new PhysicsWorld();

var ball1 = new RigidBody(new CircleShape(radius: 20), mass: 2);
ball1.Position = new Vector2(100, 150);
ball1.Velocity = new Vector2(50, 0);

var ball2 = new RigidBody(new CircleShape(radius: 20), mass: 2);
ball2.Position = new Vector2(300, 150);
ball2.Velocity = new Vector2(-50, 0);

world.AddBody(ball1);
world.AddBody(ball2);

while (true)
{
    world.Step(1f / 60f); // update simulation
}
```

---

## 🧩 Architecture

| Module             | Description                                                   |
| ------------------ | ------------------------------------------------------------- |
| **Core**           | Main physics loop (integration, time step, updates)           |
| **Dynamics**       | Rigid body data structures, velocity and acceleration updates |
| **Collision**      | Detection and response algorithms for circles                 |
| **Rendering**      | SlimDX drawing and visualization layer                        |
| **Math Utilities** | Vector operations and helper functions                        |

---

## 🧠 Future Improvements

* Extend to support rectangular and polygonal collisions
* Add constraints and joints
* Implement friction and restitution coefficients
* Integrate a simple WPF GUI for interactive parameter tuning

---

## 🧑‍💻 Contributing

Contributions are welcome!
Feel free to open issues or pull requests for new features, optimizations, or documentation.

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙌 Acknowledgments

* **SlimDX** team — for the graphics and input API
* referenced research paper on 2D sphere collisions
