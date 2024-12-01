using Godot;
using System;
using System.Collections.Generic;
using Roguelike.Dungeon;
using Roguelike.Entities;

namespace Roguelike.Entities
{
    public class Enemy : Roguelike.Entities.Entity
    {
        public Room RoomBounds { get; private set; }
        private AnimatedSprite _AnimatedSprite;

        public Enemy(Room room)
        {
            RoomBounds = room;
            Data.Sprite.Texture = ImageLoader.LoadTexture("res://Sprites/Entites/Player/Orc_Copy.png", true);
            Data.InitBody(4, 6, new Vector2(0, -16));
            Data.InitCollider(3.5f, 3);

            // Добавляем AnimatedSprite
            _AnimatedSprite = new AnimatedSprite();
            AddChild(_AnimatedSprite);
            AddChild(_AnimatedSprite);

            // Загружаем анимации
            var animatedResource = GD.Load<SpriteFrames>("res://Sprites/Entites/Orc/Orc_Animation.tres");
            _AnimatedSprite.Frames = animatedResource;

            // Устанавливаем начальную анимацию
            _AnimatedSprite.Play("Idle");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            KeepWithinBounds();
            UpdateAnimation();
        }

        private void KeepWithinBounds()
        {
            // Убедиться, что враг остаётся в пределах своей комнаты
            Vector2 pos = Body.GlobalPosition;

            if (pos.x < RoomBounds.Bounds.Position.x)
                pos.x = RoomBounds.Bounds.Position.x;
            if (pos.y < RoomBounds.Bounds.Position.y)
                pos.y = RoomBounds.Bounds.Position.y;

            if (pos.x > RoomBounds.Bounds.End.x)
                pos.x = RoomBounds.Bounds.End.x;
            if (pos.y > RoomBounds.Bounds.End.y)
                pos.y = RoomBounds.Bounds.End.y;

            Body.GlobalPosition = pos;
        }

        private void UpdateAnimation()
        {
            // Если враг движется, играем анимацию "Run"
            if (Data.Velocity.Length() > 0)
            {
                _AnimatedSprite.Play("Walk");
                _AnimatedSprite.FlipH = Data.Velocity.x < 0;
            }
            else
            {
                _AnimatedSprite.Play("Idle");
            }
        }
    }

    public class EnemySpawner
    {
        private const int MinEnemies = 15;
        private const int MaxEnemies = 30;
        private const int MaxEnemiesPerRoom = 4;

        private Random _random = new Random();

        public void SpawnEnemies(Level level)
        {
            int totalEnemies = _random.Next(MinEnemies, MaxEnemies + 1);
            List<Room> rooms = level.Rooms;

            foreach (Room room in rooms)
            {
                int enemiesInRoom = _random.Next(1, Math.Min(MaxEnemiesPerRoom, totalEnemies) + 1);

                for (int i = 0; i < enemiesInRoom; i++)
                {
                    Vector2 spawnPosition = GetRandomCorner(room);

                    Enemy enemy = new Enemy(room);
                    enemy.Body.GlobalPosition = spawnPosition;

                    GD.Print($"Добавлен враг в комнату: {room.Bounds.Position} на позицию: {spawnPosition}");

                    level.AddChild(enemy);
                    totalEnemies--;

                    if (totalEnemies <= 0)
                        return;
                }
            }
        }

        private Vector2 GetRandomCorner(Room room)
        {
            Vector2[] corners = new Vector2[4]
            {
                room.Bounds.Position, // Верхний левый угол
                new Vector2(room.Bounds.Position.x, room.Bounds.Position.y + room.Height), // Нижний левый угол
                new Vector2(room.Bounds.Position.x + room.Width, room.Bounds.Position.y), // Верхний правый угол
                new Vector2(room.Bounds.Position.x + room.Width, room.Bounds.Position.y + room.Height) // Нижний правый угол
            };

            return corners[_random.Next(0, corners.Length)];
        }
    }
}
