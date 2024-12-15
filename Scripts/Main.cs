using Godot;
using System;
using Roguelike.Entities;
using Roguelike.Dungeon;

namespace Roguelike.Entities 
{
	public class Main : Node
	{
		private Player _Player;
		private Level _Dungeon;
		private PackedScene _EnemyScene;

		public override void _Ready()
		{
			try { 
				_Dungeon = new Level(new Vector2(100, 100), numRooms: 17);
				_Dungeon.ConnectLevel(this);

				_Player = new Player();
				_Player.ConnectToNode(_Dungeon.Walls);
				_Player.SetPosition(_Dungeon.ChunkList[0].Room.Center);

				_EnemyScene = (PackedScene)ResourceLoader.Load("res://Scenes/Enemy.tscn");

				for (int i = 0; i < _Dungeon.ChunkList.Count && i < 17; i++)
				{
					var chunk = _Dungeon.ChunkList[i];
					if (chunk.Room != null)
					{
						var enemyInstance = (Node2D)_EnemyScene.Instance();
						enemyInstance.Position = chunk.Room.Center;
						AddChild(enemyInstance);
						GD.Print("Enemy spawned at: ", chunk.Room.Center);
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomException("An error occurred while initializing the Main node", ex);
			}
		}
	}
}
