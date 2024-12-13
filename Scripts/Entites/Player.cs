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
		private Area2D _AttackArea;
		private bool _IsAttacking;


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

			_AnimatedSprite.Play("Soldier-Idle");

			Body.PhysicProcess += Control;

			_AttackArea = new Area2D();
			AddChild(_AttackArea);
			
			var collisionShape = new CollisionShape2D();
			collisionShape.Shape = new RectangleShape2D() { Extents = new Vector2(20, 20) }; // Встановлюемо розміри зони атаки
			_AttackArea.AddChild(collisionShape);
			
			_AttackArea.Connect("body_entered", this, nameof(OnBodyEntered));

			_AnimatedSprite.Connect("animation_finished", this, nameof(OnAttackAnimationFinished));
	}

		private void Control(float delta)
		{
			GetInputDirection();

			if (Godot.Input.IsActionJustPressed("attack"))
			{
				Attack();
			}

			else if (Data.Velocity.Length() > 0 && !_IsAttacking)
			{
				_AnimatedSprite.Play("Soldier-Walk");
				_AnimatedSprite.FlipH = Data.Velocity.x < 0;
			}
			else if (!_IsAttacking)
			{
				_AnimatedSprite.Play("Soldier-Idle");
			}

		}

		private void GetInputDirection()
		{
			Data.Velocity.x = Godot.Input.GetActionStrength("Rigth") - Godot.Input.GetActionStrength("Left");
			Data.Velocity.y = Godot.Input.GetActionStrength("Down") - Godot.Input.GetActionStrength("Up");
		}

		private void Attack()
		{
			if (_IsAttacking) return;

			_IsAttacking = true;
			_AnimatedSprite.Play("Soldier-Attack");
			_AnimatedSprite.Frame = 0;


			// Включаем зону атаки на время атаки
			_AttackArea.SetDeferred("monitoring", true);
		}

		public void SetPosition(Vector2 pos)
		{
			Body.GlobalPosition = pos;
		}

		private void OnAttackAnimationFinished()
		{
			_IsAttacking = false;

			// Відключаємо зону атаки по завершенні анімації
			_AttackArea.SetDeferred("monitoring", false);
		}

		private void OnBodyEntered(Node body)
		 { 
			if (body is Enemy_Orc && _IsAttacking)
			{
				// Логіка нанесення урона ворогу
				((Enemy_Orc)body).TakeDamage(10);
			}
			else if (body is Player)
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
