using Godot;
using System;
using Roguelike.Entities;

public class Enemy_Orc : KinematicBody2D
{
	[Export] public int Speed = 50;
	private Vector2 _velocity = new Vector2();
	private Random _random = new Random();
	private AnimatedSprite _animatedSprite;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		SetRandomDirection();
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 motion = _velocity * delta;
		KinematicCollision2D collision = MoveAndCollide(motion);

		if (collision != null)
		{
			if (collision.Collider is Player) // Проверяем столкновение с игроком
			{
				GameOver();
			}
			else
			{
				_velocity = Vector2.Zero;
				SetRandomDirection();
			}
		}

		if (_velocity != Vector2.Zero)
		{
			_animatedSprite.Play("Walk");
		}
		else
		{
			_animatedSprite.Play("Stay");
		}
	}

	private void SetRandomDirection()
	{
		float angle = (float)(_random.NextDouble() * 2 * Math.PI);
		_velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Speed;
	}

	private void GameOver()
	{
		GD.Print("Game Over!");
		// Здесь можно добавить логику завершения игры, например, переход на сцену Game Over или остановка игры
		GetTree().Paused = true;
	}
}





