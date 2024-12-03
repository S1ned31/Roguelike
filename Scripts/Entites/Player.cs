using Godot;
using System;
using Roguelike.Entities;
using Roguelike.Dungeon;

namespace Roguelike.Entities
{
	public class Player : Entity
	{
		private Camera2D _Camera;
		private AnimatedSprite _AnimatedSprite;


		public Player()
		{
			Data.InitBody(4, 6, new Vector2(0, -16));
			Data.InitCollider(3.5f, 3);

			_Camera = new Camera2D();
			_Camera.Current = true;
			_Camera.Zoom = new Vector2(1, 1);
			AddChild(_Camera);

			_AnimatedSprite = new AnimatedSprite();
			AddChild(_AnimatedSprite);

			var animationResource = GD.Load<SpriteFrames>("res://Sprites/Entites/Player/Player_Animation.tres");
			_AnimatedSprite.Frames = animationResource;

			_AnimatedSprite.Play("Stay");

			Body.PhysicProcess += Control;

			var area = new Area2D();
			AddChild(area);
			
			var collisionShape = new CollisionShape2D();
			area.AddChild(collisionShape);
			
			area.Connect("body_entered", this, nameof(OnBodyEntered));
		}

		private void Control(float delta)
		{
			GetInputDirection();

			if (Data.Velocity.Length() > 0)
			{
				_AnimatedSprite.Play("Run");
				_AnimatedSprite.FlipH = Data.Velocity.x < 0;
			}
			else
			{
				_AnimatedSprite.Play("Stay");
			}

		}

		private void GetInputDirection()
		{
			Data.Velocity.x = Godot.Input.GetActionStrength("Rigth") - Godot.Input.GetActionStrength("Left");
			Data.Velocity.y = Godot.Input.GetActionStrength("Down") - Godot.Input.GetActionStrength("Up");
		}

		public void SetPosition(Vector2 pos)
		{
			Body.GlobalPosition = pos;
		}

		private void OnBodyEntered(Node body)
		 { 
			if (body is Enemy_Orc)
			{ 
				GameOver();
			}
		}
				
			private void GameOver() 
			{ 
				GD.Print("Game Over!");
				GetTree().Paused = true; // Здесь можно добавить любую дополнительную логику окончания игры
			}
	}
}
