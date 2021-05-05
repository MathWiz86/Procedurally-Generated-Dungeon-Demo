/**************************************************************************************************/
/*!
\file   DungeonRoomTile.cs
\author Craig Williams
\par    Last Updated
        2021-04-01

\brief
  A file containing implementation of a single room tile in the procedurally generated dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// An extension of <see cref="DungeonTile"/> for tiles that are default
  /// <see cref="TileBasicType.Room"/>s.
  /// </summary>
  public class DungeonRoomTile : DungeonTile
  {
    /// <summary>
    /// The default constructor for a room tile, setting its <see cref="TileBasicType"/>.
    /// </summary>
    public DungeonRoomTile()
    {
      BasicType = TileBasicType.Room;
    }

    /// <summary>
    /// A helper function for determining if a tile must force itself and its neighbors to be
    /// a bridge. This happens if the tile is surrounded by
    /// <see cref="TileEnvironmentType.Water"/> tiles.
    /// </summary>
    public void CheckWaterSurrounding()
    {
      int neighborCount = NeighborIndexes.Length; // Get the neighbor count.

      // Iterate through all neighbors.
      for (int i = 0; i < neighborCount; i++)
      {
        // If any neighbor is not water, it does not have to worry about bridging.
        DungeonTile neighbor = GetNeighborTile(i);
        if (!neighbor || neighbor.EnvironmentType != TileEnvironmentType.Water)
          return;
      }

      // At this point, the tile and its neighbors must be bridged.
      for (int i = 0; i < neighborCount; i++)
        GetNeighborTile(i).SetDecorType(TileDecorType.Bridge); // Set the neighbor as a bridge.

      // Set this tile as a bridge.
      SetDecorType(TileDecorType.Bridge);
    }

    public override void SetEnvironmentType(TileEnvironmentType type)
    {
      base.SetEnvironmentType(type);
      SpreadEnvironment(0, DungeonDecorator.GetRandomEnvironmentSpread(type));
    }

    public override void SetDecorType(TileDecorType type)
    {
      base.SetDecorType(type);
    }

    public override void CalculateBaseWalls()
    {
      // Iterate through the four cardinal directions in order.
      for (int currentCardinal = 0; currentCardinal < CardinalCount; currentCardinal++)
      {
        // Get the index of the neighbor adjacent to the current wall.
        Vector2Int neighborIndex = NeighborIndexes[currentCardinal] + TileIndex;
        // Get the basic type of the neighbor. Invalid neighbors are treated as empty.
        TileBasicType neighborType = DungeonManager.GetTileType(neighborIndex);

        // Set the wall based on the tile type.
        DungeonWall wall = neighborType switch
        {
          // If the neighbor is empty, this will always be a solid wall.
          TileBasicType.Empty => new DungeonWall(WallBasicType.BasicWall),
          // By default, this will always be empty. This is to not get in the way of hall walls.
          _ => new DungeonWall(true),
        };
        TileWalls[currentCardinal] = wall; // Set the current wall.
      }
    }
  }
  /************************************************************************************************/
}