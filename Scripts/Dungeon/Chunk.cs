using Godot;
using System;
using System.Collections.Generic;

namespace Roguelike.Dungeon
{
	public class Chunk
	{
		public Vector2 Position { get; private set; }
		public Room Room{ get; set; }

		public Chunk(Vector2 pos)
		{
			Position = pos;
		}
	}
}
