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
//			Data.Sprite.Texture = ImageLoader.LoadTexture("res://Sprites/Entites/Player/Knight.png", true);
			Data.InitBody(4, 6, new Vector2(0, -16));
			Data.InitCollider(3.5f, 3);

			_Camera = new Camera2D();
			_Camera.Current = true;
			_Camera.Zoom = new Vector2(1,1);
			AddChild(_Camera);

			// Створюємо вузол AnimatedSprite2D
			_AnimatedSprite = new AnimatedSprite();
			AddChild(_AnimatedSprite);

			// Завантажуємо анімацію для вузла AnimatedSprite2D
			var animationResource = GD.Load<SpriteFrames>("res://Sprites/Entites/Player/Player_Animation.tres");
			_AnimatedSprite.Frames = animationResource;

			// Встановлюемо початкову анімацію
			_AnimatedSprite.Play("Stay");

			Body.PhysicProcess += Control;
		}
		private void Control(float delta)
		{
			GetInputDirection();

			// Анімація в залежності від руху
			if (Data.Velocity.Length() > 0)
			{
				_AnimatedSprite.Play("Run"); // Анімація бігу
				_AnimatedSprite.FlipH = Data.Velocity.x < 0; // Відзеркалювання вліво
			}
			else
			{
				_AnimatedSprite.Play("Stay"); // Анімація бездіяльності
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
	}
}
