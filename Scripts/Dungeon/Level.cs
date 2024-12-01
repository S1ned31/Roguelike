using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;
using Roguelike.Dungeon;

namespace Roguelike.Dungeon
{
	public class Level : Node
	{
		public TileMap Walls { get; private set; }
		public TileMap Floor { get; private set; }

		private int _TileSize = 16;
		private Vector2 _Size;

		public List<Chunk> ChunkList = new List<Chunk>();

		public List<Room> Rooms => ChunkList.Select(chunk => chunk.Room).Where(room => room != null).ToList();

		private int _ChunkSize;
		private int _MinRoomSize = 0;
		private int _MaxRoomSize = 0;
		private int _NumRooms;

		private List<Vector2> _RoomsPos = new List<Vector2>();

		private Random _Rand = new Random();

		private AStar2D _Path = new AStar2D();
		private ArtCanvas _ArtCanvas = new ArtCanvas();

		public Level(Vector2 size, int chunkSize = 20, int minRoomSize = 5, int maxRoomSize = 16, int numRooms = 10)
		{
			_Size = size;
			_ChunkSize = chunkSize;
			_MaxRoomSize = maxRoomSize;
			_MinRoomSize = minRoomSize;
			_NumRooms = numRooms;

			Walls = new TileMap();
			Floor = new TileMap();

			Floor.CellSize = new Vector2(_TileSize, _TileSize);
			Walls.CellSize = new Vector2(_TileSize, _TileSize);
			Walls.CellTileOrigin = TileMap.TileOrigin.Center;
			Walls.CellYSort = true;

			Walls.TileSet = (TileSet)ResourceLoader.Load("res://Resourses/TileSets/Wall.tres");
			Floor.TileSet = (TileSet)ResourceLoader.Load("res://Resourses/TileSets/Floor.tres");

			GenerateDungeon();
		}

		private void GenerateDungeon()
		{
			FillArea(Walls, Vector2.Zero, _Size, 0);
			CreateChunks();
			ShuffleChunk();
			GenerateRooms();
			FindPath();
			GenerateCorridor();
		}

		private void FillArea(TileMap tileMap, Vector2 pos, Vector2 size, int idTile)
		{
			Vector2 tilePos = new Vector2();
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					tilePos.x = pos.x + x;
					tilePos.y = pos.y + y;
					tileMap.SetCellv(tilePos, idTile);
				}
			}
			tileMap.UpdateBitmaskRegion();
		}

		public void ConnectLevel(Node parent)
		{
			parent.AddChild(Floor);
			parent.AddChild(Walls);
			parent.AddChild(_ArtCanvas);
		}

		private void CreateChunks()
		{
			Vector2 chunkPos = Vector2.Zero;

			for (int chunkY = 0; chunkY < _Size.y / _ChunkSize; chunkY++)
			{
				for (int chunkX = 0; chunkX < _Size.x / _ChunkSize; chunkX++)
				{
					chunkPos.x = chunkX * _ChunkSize;
					chunkPos.y = chunkY * _ChunkSize;

					ChunkList.Add(new Chunk(chunkPos));
				}
			}
		}

		// Алгоритм Фішера-Етса
		private void ShuffleChunk()
		{
			for (int i = ChunkList.Count - 1; i >= 1; i--)
			{
				int j = _Rand.Next(i + 1);

				Chunk currentChunk = ChunkList[j];
				ChunkList[j] = ChunkList[i];
				ChunkList[i] = currentChunk;
			}
		}

		private void GenerateRooms()
		{
			for (int i = 0; i < _NumRooms; i++)
			{
				Room room = new Room(Walls, ChunkList[i].Position, _ChunkSize, _MinRoomSize, _MaxRoomSize);
				ChunkList[i].Room = room;
				FillArea(Floor, room.Position, new Vector2(room.Width, room.Height), 0);
				_RoomsPos.Add(room.Center);
			}
		}

		// Алгоритм Прима для з'еднання центрів кімнат коридорами
		private void FindPath()
		{
			Vector2 startPos = _RoomsPos[0];
			_Path.AddPoint(0, startPos);
			_RoomsPos.Remove(startPos);

			foreach (Vector2 posRoom in _RoomsPos)
			{
				int currentPoint = _Path.GetAvailablePointId();
				int nearPoint = _Path.GetClosestPoint(posRoom);
				_Path.AddPoint(currentPoint, posRoom);
				_Path.ConnectPoints(nearPoint, currentPoint);
			}
			//_ArtCanvas.DrawCorridor(_Path);
		}

		private void GenerateCorridor()
		{
			foreach (int point in _Path.GetPoints())
			{
				Vector2 currentPointPos = _Path.GetPointPosition(point) / _TileSize;
				foreach (int connect in _Path.GetPointConnections(point))
				{
					Vector2 connectPointPos = _Path.GetPointPosition(connect) / _TileSize;
					Vector2 length = connectPointPos - currentPointPos;

					int incrementX = Math.Sign(length.x);
					int incrementY = Math.Sign(length.y);


					for (int x = 0; x < Math.Abs(length.x); x++)
					{
						currentPointPos.x += incrementX;
						Walls.SetCellv(currentPointPos, -1);
						Floor.SetCellv(currentPointPos, 0);
					}
					for (int y = 0; y < Math.Abs(length.y); y++)
					{
						currentPointPos.y += incrementY;
						Walls.SetCellv(currentPointPos, -1);
						Floor.SetCellv(currentPointPos, 0);
					}
				}
			}
			Walls.UpdateBitmaskRegion();

		}
	}
}
