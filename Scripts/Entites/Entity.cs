using System;
using Godot;

namespace Roguelike.Entities
{
	public abstract class Entity : Node
	{
		public EntityBody Body;
		public EntityData Data;

		public Entity()
		{
			Data = new EntityData();
			Body = new EntityBody(Data);

			AddChild(Data.Sprite);
			AddChild(Data.Collider);

			Body.PhysicProcess = PhysicProcess;
			Body.Process = Process;
			Body.Input = HandleInput;
		}

		public void PhysicProcess(float delta)
		{
		}

		public void Process(float delta)
		{
		}

		public void HandleInput(InputEvent ev)
		{
		}

		public void ConnectToNode(Node parent)
		{
			parent.AddChild(Body);
		}

		public void DisconnectFromNode(Node parent)
		{
			parent.RemoveChild(Body);
		}

		public void AddChild(Node child)
		{
			Body.AddChild(child);
		}

		public new void RemoveChild(Node child)
		{
			Body.RemoveChild(child);
		}
	}
}
