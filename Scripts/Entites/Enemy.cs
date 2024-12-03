using Godot;
using System;
using Roguelike.Entities;

namespace Roguelike.Entities
{
	public class Enemy : Entity
	{
		private Sprite _Sprite;

		public Enemy()
		{
			Data.InitBody(1, 1, Vector2.Zero); // Устанавливаем один кадр для тела
			Data.InitCollider(3.5f, 3);

			// Создаем узел Sprite
			_Sprite = new Sprite();
			AddChild(_Sprite);

			// Загружаем текстуру для узла Sprite
			var texture = GD.Load<Texture>("res://Sprites/Entites/Player/Knight_copy.png");
			if (texture == null)
			{
				GD.Print("Failed to load enemy texture resource.");
			}
			else
			{
				_Sprite.Texture = texture;
			}

			Body.PhysicProcess += Control;
		}

		private void Control(float delta)
		{
			// Поведение врага можно запрограммировать здесь
		}

		public void SetPosition(Vector2 pos)
		{
			Body.GlobalPosition = pos;
			GD.Print("Enemy Position: ", pos);
		}
	}
}
