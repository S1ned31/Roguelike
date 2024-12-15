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
			try
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
				collisionShape.Shape = new RectangleShape2D() { Extents = new Vector2(20, 20) }; // Установите размеры зоны атаки
				_AttackArea.AddChild(collisionShape);

				_AttackArea.Connect("body_entered", this, nameof(OnBodyEntered));

				_AnimatedSprite.Connect("animation_finished", this, nameof(OnAttackAnimationFinished));
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while initializing the Player entity.", ex);
			}
		}

		private void Control(float delta)
		{
			try
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
			catch (Exception ex)
			{
				throw new CustomException("An error occurred during the control process.", ex);
			}
		}

		private void GetInputDirection()
		{
			try
			{
				Data.Velocity.x = Godot.Input.GetActionStrength("Rigth") - Godot.Input.GetActionStrength("Left");
				Data.Velocity.y = Godot.Input.GetActionStrength("Down") - Godot.Input.GetActionStrength("Up");
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while getting input direction.", ex);
			}
		}

		private void Attack()
		{
			try
			{
				if (_IsAttacking) return;

				_IsAttacking = true;
				_AnimatedSprite.Play("Soldier-Attack");
				_AnimatedSprite.Frame = 0;

				// Включаем зону атаки на время атаки
				_AttackArea.SetDeferred("monitoring", true);
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while performing an attack.", ex);
			}
		}

		public void SetPosition(Vector2 pos)
		{
			try
			{
				Body.GlobalPosition = pos;
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while setting the player's position.", ex);
			}
		}

		private void OnAttackAnimationFinished()
		{
			try
			{
				_IsAttacking = false;

				// Отключаем зону атаки по завершении анимации
				_AttackArea.SetDeferred("monitoring", false);
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while finishing the attack animation.", ex);
			}
		}

		private void OnBodyEntered(Node body)
		{
			try
			{
				if (body is Enemy_Orc && _IsAttacking)
				{
					// Логика нанесения урона врагу
					((Enemy_Orc)body).TakeDamage(10);
				}
				else if (body is Player)
				{
					GameOver();
				}
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while processing body entered event.", ex);
			}
		}

		private void GameOver()
		{
			try
			{
				GD.Print("Game Over!");
				GetTree().Paused = true; // Здесь можно добавить любую дополнительную логику окончания игры
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred during game over.", ex);
			}
		}
	}
}
