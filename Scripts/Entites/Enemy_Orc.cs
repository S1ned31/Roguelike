using Godot;
using System;
using Roguelike.Entities;

public class Enemy_Orc : KinematicBody2D
{
	[Export] public int Speed = 50;
	[Export] public int MaxHealth = 30;
	private int _currentHealth;
	private Vector2 _velocity = new Vector2();
	private Random _random = new Random();
	private AnimatedSprite _animatedSprite;

	private bool _isDead = false;
	private bool _isTakingDamage = false; // Вказує, чи програється анімація попадання

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_currentHealth = MaxHealth;
		SetRandomDirection();
	}

	public override void _PhysicsProcess(float delta)
	{
		if (_isDead || _isTakingDamage) return;

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

	public void TakeDamage(int damage)
	{
		if (_isDead) return;

		_currentHealth -= damage;
		GD.Print($"Enemy took damage: {damage}. Remaining health: {_currentHealth}");

		if (_currentHealth > 0)
		{
			// Якщо ворог ще живий, то програемо анімацію попадання
			PlayHurtAnimation();
		}
		else
		{
			Die(); // Якщо здоров'я нема - вбиваемо
		}
	}

	private void PlayHurtAnimation()
	{
		_isTakingDamage = true;
		_animatedSprite.Play("Hurt");

		_animatedSprite.Connect("animation_finished", this, nameof(OnHurtAnimationFinished));
	}

	private void OnHurtAnimationFinished()
	{
		_isTakingDamage = false;

		// Отключаем связь, чтобы избежать повторных вызовов
		_animatedSprite.Disconnect("animation_finished", this, nameof(OnHurtAnimationFinished));
	}

	private void Die()
	{
		_isDead = true; // Помічаємо ворога, як мертвого
		_velocity = Vector2.Zero; // Зупняемо його рух 
		_animatedSprite.Play("Death"); // Програемо анімацію смерті

		// Відключаео його
		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		collisionShape.Disabled = true;

		// Видаляємо ворога зі сцени після завершення аніманії
		_animatedSprite.Connect("animation_finished", this, nameof(OnDeathAnimationFinished));
	}

	private void OnDeathAnimationFinished()
	{
		QueueFree();
	}


	private void GameOver()
	{
		GD.Print("Game Over!");
		// Здесь можно добавить логику завершения игры, например, переход на сцену Game Over или остановка игры
		GetTree().Paused = true;
	}
}





