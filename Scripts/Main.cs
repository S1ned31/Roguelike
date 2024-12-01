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

		public override void _Ready()
		{
			_Dungeon = new Level(new Vector2(100, 100), numRooms: 17);
			_Dungeon.ConnectLevel(this);

			_Player = new Player();
			_Player.ConnectToNode(_Dungeon.Walls);
			_Player.SetPosition(_Dungeon.ChunkList[0].Room.Center);

			EnemySpawner spawner = new EnemySpawner();
			spawner.SpawnEnemies(_Dungeon); 
		}
	}
}
