/**************************************************************************************************/
/*!
\file   DungeonRoom.cs
\author Craig Williams
\par    Last Updated
        2021-04-09

\brief
  A file containing implementation of a single room in the procedurally generated dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using PCGDungeon.UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A single room procedurally generated in a dungeon. This handles designing and tiling based on
  /// settings in the <see cref="DungeonManager"/>.
  /// </summary>
  public class DungeonRoom : MonoBehaviour
  {
    private static readonly int CardinalDirectionCount = Tools.Enums.GetValueCount<CardinalDirection>();
    /// <summary>The bounds of the room. We use <see cref="int"/> to keep simple.</summary>
    [SerializeField] [ReadOnly] private RectInt roomBounds;
    /// <summary>The <see cref="DungeonRoomTile"/>s that make up this room.</summary>
    [SerializeField] [ReadOnly] private List<DungeonRoomTile> RoomTiles = new List<DungeonRoomTile>();
    /// <summary>The current tiles being used to form a river.</summary>
    private List<DungeonRoomTile> currentRiverTiles = new List<DungeonRoomTile>();

    /// <summary>
    /// A function which initializes the bounds of the room.
    /// </summary>
    /// <param name="bounds">The bounds of the room.</param>
    public void InitializeBounds(RectInt bounds)
    {
      roomBounds = bounds;
    }

    /// <summary>
    /// A function which initializes the bounds of the room.
    /// </summary>
    /// <param name="location">The pivot location of the room.</param>
    /// <param name="size">The size of the room.</param>
    public void InitializeBounds(Vector2Int location, Vector2Int size)
    {
      roomBounds = new RectInt(location, size);
    }

    /// <summary>
    /// A getter for this room's position.
    /// </summary>
    /// <returns>Returns this room's position.</returns>
    public Vector2Int GetPosition()
    {
      return roomBounds.position;
    }

    /// <summary>
    /// A getter for this room's size.
    /// </summary>
    /// <returns>Returns this room's size.</returns>
    public Vector2Int GetSize()
    {
      return roomBounds.size;
    }

    /// <summary>
    /// A getter for this room's bounds.
    /// </summary>
    /// <returns>Returns this room's bounds.</returns>
    public RectInt GetBounds()
    {
      return roomBounds;
    }

    /// <summary>
    /// A function used to check if this room is overlapping with some other bound.
    /// </summary>
    /// <param name="otherBounds">The other bounds to check against.</param>
    /// <returns>Returns if this room and the <paramref name="otherBounds"/> overlap.</returns>
    public bool CheckOverlap(RectInt otherBounds)
    {
      return roomBounds.Overlaps(otherBounds);
    }

    /// <summary>
    /// A function for creating the room tiles that are within this <see cref="DungeonRoom"/>'s
    /// own boundaries.
    /// </summary>
    /// <param name="BaseTile">The tile prefab to use.</param>
    public void CreateRoomTiles(DungeonRoomTile BaseTile)
    {
      // Clear out any old room tiles.
      for (int i = 0; i < RoomTiles.Count; i++)
        Destroy(RoomTiles[i].gameObject);
      RoomTiles.Clear();

      foreach (Vector2Int index in roomBounds.allPositionsWithin)
      {
        // Instantiate the tile, and set its dungeon index.
        DungeonRoomTile tile = Instantiate(BaseTile, new Vector3(index.x, 0, index.y), Quaternion.identity, transform);
        tile.SetIndex(index);
        // tile.SetBasicType(TileBasicType.Room); // Should be unnecessary, assuming Unity constructors work...
        DungeonManager.AddTileToDungeon(tile); // Add the new tile to the manager's references.
        RoomTiles.Add(tile); // Add the new tile to the room's own references.
      }
    }

    /// <summary>
    /// A function for generating the walls of all of this room's <see cref="RoomTiles"/>.
    /// </summary>
    public void GenerateRoomTileWalls()
    {
      int tileCount = RoomTiles.Count;

      // For every room tilee, calculate its base walls.
      for (int i = 0; i < tileCount; i++)
        RoomTiles[i].CalculateBaseWalls();
    }

    /// <summary>
    /// A function for generating the <see cref="TileEnvironmentType"/>s within the room, and
    /// nearby <see cref="DungeonHallTile"/>s.
    /// </summary>
    public void GenerateEnvironment()
    {
      // Get the limits, set by the decorator.
      Vector2Int limits = DungeonDecorator.GetRoomEnvironmentTileRange();
      // Update the limits based on the maximum amount of room tiles.
      int maxTiles = roomBounds.width * roomBounds.height;
      limits.x = Math.Min(limits.x, maxTiles);
      limits.y = Math.Min(limits.y, maxTiles);

      // Get the number of tiles to force an Environment onto.
      int environmentTileCount = DungeonManager.SystemRandomGenerator.Next(limits.x, limits.y);
      
      int environmentCount = Tools.Enums.GetValueCount<TileEnvironmentType>();
      
      // Get the minimum number of tiles to attempt for each environment.
      int tileCount = DungeonManager.SystemRandomGenerator.Next(limits.x, environmentTileCount);
      
      for (int i = 0; i < environmentCount && environmentTileCount >= 0; i++)
      {
        TileEnvironmentType environment = (TileEnvironmentType)i; // Get the current environment.
        
        // Only proceed if there's even a chance of getting the current environment.
        if (DungeonDecorator.GetRoomEnvironmentTileProbability(environment) > 0.0f)
        {
          tileCount = Mathf.Clamp(tileCount, 0, environmentTileCount); // Clamp the tile count.
          // If this is the last environment, give it all remaining tiles to try.
          if (i == environmentCount - 1)
            tileCount = environmentTileCount;

          environmentTileCount -= tileCount; // Reduce the remaining count.

          // Iterate through the number of tiles.
          for (int j = 0; j < tileCount; j++)
          {
            // Get a random room tile.
            DungeonRoomTile tile = RoomTiles[DungeonManager.SystemRandomGenerator.Next(0, RoomTiles.Count)];

            // If the tile is Dirt, and we successfully rolled, set the tile to the environment.
            if (tile.EnvironmentType == TileEnvironmentType.Dirt && DungeonDecorator.DetermineRoomEnvironmentOutcome(environment))
              tile.SetEnvironmentType(environment);
          }
        }
      }
    }

    /// <summary>
    /// A function for generating rivers, done by placing down
    /// <see cref="TileEnvironmentType.Water"/> and <see cref="TileDecorType.Bridge"/>s.
    /// </summary>
    public void GenerateRivers()
    {
      int currentRiver = 0; // Initialize the current amount of rivers.

      // Generate rivers horizontally.
      CreateRiversInDirection(ref currentRiver, roomBounds.width, roomBounds.height, CardinalDirection.West, (int i) => new Vector2Int(i + roomBounds.x, roomBounds.y));
      // Generate rivers vertically.
      CreateRiversInDirection(ref currentRiver, roomBounds.height, roomBounds.width, CardinalDirection.North, (int i) => new Vector2Int(roomBounds.x, roomBounds.y + i));
      // Clean up the hanging bridges. The second pass is for accuracy.
      CleanUpHangingBridges();
      CleanUpHangingBridges();
    }

    /// <summary>
    /// A <see cref="Coroutine"/> for generating rivers, done by placing down
    /// <see cref="TileEnvironmentType.Water"/> and <see cref="TileDecorType.Bridge"/>s.
    /// </summary>
    /// <returns>Returns an <see cref="IEnumerator"/> for use in <see cref="Coroutine"/>s.</returns>
    public IEnumerator SlowGenerateRivers()
    {
      int currentRiver = 0; // Initialize the current amount of rivers.

      // Generate rivers horizontally.
      if (CreateRiversInDirection(ref currentRiver, roomBounds.width, roomBounds.height, CardinalDirection.West, (int i) => new Vector2Int(i + roomBounds.x, roomBounds.y)))
      {
        // SLOW GENERATION
        foreach (Vector2Int index in roomBounds.allPositionsWithin)
          DemoManager.UpdateSingleTile(index);
        yield return new WaitForSecondsRealtime(0.2f);
      }
      
      // Generate rivers vertically.
      if (CreateRiversInDirection(ref currentRiver, roomBounds.height, roomBounds.width, CardinalDirection.North, (int i) => new Vector2Int(roomBounds.x, roomBounds.y + i)))
      {
        // SLOW GENERATION
        foreach (Vector2Int index in roomBounds.allPositionsWithin)
          DemoManager.UpdateSingleTile(index);
        yield return new WaitForSecondsRealtime(0.2f);
      }

      // Clean up the hanging bridges. The second pass is for accuracy.
      CleanUpHangingBridges();
      CleanUpHangingBridges();
      // SLOW GENERATION
      foreach (Vector2Int index in roomBounds.allPositionsWithin)
        DemoManager.UpdateSingleTile(index);
      yield return new WaitForSecondsRealtime(0.2f);
    }

    /// <summary>
    /// A helper function for getting a random <see cref="DungeonTile"/> within the room. This
    /// uses <see cref="UnityEngine"/>'s random functions so as not to affect dungeon generation.
    /// </summary>
    /// <returns>Returns a random <see cref="DungeonTile"/> in the room.</returns>
    public DungeonTile GetRandomTile()
    {
      return RoomTiles[UnityEngine.Random.Range(0, RoomTiles.Count)];
    }

    /// <summary>
    /// A helper function to generate rivers in a given direction.
    /// </summary>
    /// <param name="riverCount">The current amount of rivers made.</param>
    /// <param name="dimensionSize">The size of the dimension to generate rivers in.</param>
    /// <param name="riverSize">The size of the river.</param>
    /// <param name="direction">The <see cref="CardinalDirection"/> to form the river in.</param>
    /// <param name="indexFunction">A function for updating the starting tile index.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CreateRiversInDirection(ref int riverCount, int dimensionSize, int riverSize, CardinalDirection direction, System.Func<int, Vector2Int> indexFunction)
    {
      int originalCount = riverCount;
      // The room's height must be within the river size range.
      if (DungeonDecorator.CheckRiverSize(riverSize))
      {
        // Iterate through every column. This allows for connected rivers.
        for (int i = 0; i < dimensionSize; i++)
        {
          // If past the max amount of rivers, immediately end the function.
          if (riverCount >= DungeonDecorator.MaxRoomRivers)
            return false;

          // Proceed only if this dimension successfully has the chance of getting a river.
          if (!DungeonDecorator.DetermineRiverSpawnOutcome())
            continue;

          // Calculate the starting tile's index.
          Vector2Int index = indexFunction.Invoke(i);

          // Attempt to spawn a river. If successful, increment the number of rivers.
          if (AttemptRiverGeneration(DungeonManager.GetDungeonTile(index) as DungeonRoomTile, direction, riverSize))
            riverCount++;
        }
      }

      return originalCount < riverCount;
    }

    /// <summary>
    /// A helper function for attempting to generate a river from a given
    /// <paramref name="startTile"/> following a given <paramref name="direction"/>.
    /// </summary>
    /// <param name="startTile">The initial <see cref="DungeonTile"/> to generate from.</param>
    /// <param name="direction">The <see cref="CardinalDirection"/> to form the river in.</param>
    /// <param name="riverSize">The size of the river.</param>
    /// <returns>Returns if the river was successfully generated.</returns>
    private bool AttemptRiverGeneration(DungeonRoomTile startTile, CardinalDirection direction, int riverSize)
    {
      DungeonRoomTile currentTile = startTile; // The current tile in the river line.
      currentRiverTiles.Clear(); // Clear out the current river tiles.
      int neighborCount = DungeonTile.NeighborCount; // Get the number of neighbors.

      // Iterate through the entire river line.
      for (int i = 0; i < riverSize; i++)
      {
        // Iterate through each neighbor of the current tile.
        for (int j = 0; j < neighborCount; j++)
        {
          TileBasicType neighborType = DungeonManager.GetTileType(currentTile.TileIndex + DungeonTile.NeighborIndexes[j]);
          // No tile of the river can be adjacent to a hallway.
          if (neighborType == TileBasicType.Hallway)
            return false;
        }

        currentRiverTiles.Add(currentTile); // If the tile passed, add it to the current river.
        // Move on to its next neighbor in the given direction.
        currentTile = currentTile.GetNeighborTile(direction) as DungeonRoomTile;
      }

      // Iterate through all river tiles and set their environment as water.
      for (int i = 0; i < riverSize; i++)
      {
        currentRiverTiles[i].SetEnvironmentType(TileEnvironmentType.Water);
      }

      // Iterate through all river tiles and check if they are completely surrounded by water.
      // If so, both it and its neighbors must become bridges.
      for (int i = 0; i < riverSize; i++)
        currentRiverTiles[i].CheckWaterSurrounding();

      // Create a list of the bridged tiles of the river.
      List<DungeonRoomTile> bridgedTiles = new List<DungeonRoomTile>();

      // Even-sized rivers have two bridges. Odd-sized rivers have one bridge.
      if (riverSize % 2 == 0)
      {
        // Add the two middle tiles of the even-sized river.
        float middle = (riverSize - 1) / 2.0f;
        bridgedTiles.Add(currentRiverTiles[(int)System.Math.Floor(middle)]);
        bridgedTiles.Add(currentRiverTiles[(int)System.Math.Ceiling(middle)]);
      }
      else
      {
        // Otherwise, just add the single middle tile of the odd-sized river.
        bridgedTiles.Add(currentRiverTiles[currentRiverTiles.Count / 2]);
      }

      // Iterate through each bridged tile, and check its nearby tiles.
      for (int i = 0; i < bridgedTiles.Count; i++)
      {
        DungeonRoomTile tile = bridgedTiles[i]; // Get the current tile.

        // Get the two neighbors.
        int neighborIndex = Tools.Math.LoopValueIE((int)direction + 1, 0, CardinalDirectionCount);
        TileBasicType left = tile.GetNeighborBasicType((CardinalDirection)neighborIndex);
        neighborIndex = Tools.Math.LoopValueIE((int)direction - 1, 0, CardinalDirectionCount);
        TileBasicType right = tile.GetNeighborBasicType((CardinalDirection)neighborIndex);

        // Only set this tile as a bridge if it is actively connecting two other room tiles.
        if (left == TileBasicType.Room && right == TileBasicType.Room)
          tile.SetDecorType(TileDecorType.Bridge);
      }

      return true; // A river was generated.
    }

    /// <summary>
    /// A function to cleanup remaining bridges that no longer belong after more rivers
    /// were generated.
    /// </summary>
    private void CleanUpHangingBridges()
    {
      int count = RoomTiles.Count; // Get the room tile count.
      int neighborCount = DungeonTile.NeighborCount; // Get the neighbor count.

      // Iterate through all room tiles, finding the rivers.
      for (int i = 0; i < count; i++)
      {
        DungeonRoomTile tile = RoomTiles[i]; // Get the current tile.

        // Check if the tile is bridged. Only water tiles are bridged.
        if (tile.DecorType == TileDecorType.Bridge)
        {
          int validNeighborCount = 0; // The number of valid neighbors. There must be at least 2.
          int bridgeNeighborCount = 0; // The number of bridge neighbors.

          // Check around all neighbors. All bridges must either be surrounded by two non-water
          // room tiles, or surrounded fully by bridges.
          for (int j = 0; j < neighborCount; j++)
          {
            // Get the current neighbor.
            DungeonTile neighbor = tile.GetNeighborTile((CardinalDirection)j);

            // Make sure the neighbor exists.
            if (neighbor)
            {
              // If hte neighbor isn't water, increment the valid neighbors.
              if (neighbor.EnvironmentType != TileEnvironmentType.Water)
                validNeighborCount++;
              else if(neighbor.DecorType == TileDecorType.Bridge)
              {
                // Else, if it is a bridge, increment the bridge count and check its path.
                bridgeNeighborCount++;
                // Get the next neighbor in the same direction.
                DungeonTile nextNeighbor = neighbor.GetNeighborTile((CardinalDirection)j);
                // Iterate through the current line, checking if it ever hits land.
                while (nextNeighbor)
                {
                  // If the next neighbor is also a bridge, move along the path.
                  if (nextNeighbor.DecorType == TileDecorType.Bridge)
                    nextNeighbor = nextNeighbor.GetNeighborTile((CardinalDirection)j);
                  else 
                  {
                    // If the neighbor is not a bridge, but is solid land, it is a valid neighbor.
                    if (nextNeighbor.EnvironmentType != TileEnvironmentType.Water)
                      validNeighborCount++;
                    break;
                  }
                }
              }
            }
          }

          // If there are not enough valid neighbors, remove the bridge decor.
          if (validNeighborCount < 2 && bridgeNeighborCount != neighborCount)
            tile.SetDecorType(TileDecorType.None);
        }
      }
    }
  }
  /************************************************************************************************/
}