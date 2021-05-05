/**************************************************************************************************/
/*!
\file   DungeonHallTile.cs
\author Craig Williams
\par    Last Updated
        2021-03-31

\brief
  A file containing implementation of a single hallway tile in the procedurally generated dungeon.

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
  /// <see cref="TileBasicType.Hallway"/>s.
  /// </summary>
  public class DungeonHallTile : DungeonTile
  {
    /// <summary>
    /// The default constructor for a hall tile, determining that it is a
    /// <see cref="TileBasicType.Hallway"/>.
    /// </summary>
    public DungeonHallTile()
    {
      BasicType = TileBasicType.Hallway;
    }

    /// <summary>
    /// A helper function to determine what kind of doorway a <see cref="DungeonWall"/> is. This
    /// is called if the wall is adjacent to a <see cref="DungeonRoomTile"/>.
    /// </summary>
    /// <param name="currentCardinal">The <see cref="CardinalDirection"/> the wall faces.</param>
    /// <returns>Returns the finalized <see cref="DungeonWall"/>.</returns>
    protected DungeonWall DetermineDoorwayType(int currentCardinal)
    {
      DungeonWall wall; // The wall to create and return the reference to.

      // Get the indexes of the tile's left and right neighbors.
      int leftCardinal = Tools.Math.LoopValueIE(currentCardinal + 1, 0, CardinalCount);
      int rightCardinal = Tools.Math.LoopValueIE(currentCardinal - 1, 0, CardinalCount);

      // Get the adjacent tiles. These might be null/empty!
      DungeonTile leftTile = DungeonManager.GetDungeonTile(TileIndex + NeighborIndexes[leftCardinal]);
      DungeonTile rightTile = DungeonManager.GetDungeonTile(TileIndex + NeighborIndexes[rightCardinal]);

      // These determine if the left and right neighbors are also adjacent to a room.
      bool leftNearRoom = CheckTileAdjacentToRoom(leftTile, currentCardinal);
      bool rightNearRoom = CheckTileAdjacentToRoom(rightTile, currentCardinal);

      // If both neighbors are adjacent, this wall is in the middle of a doorway.
      if (leftNearRoom && rightNearRoom)
        wall = new DungeonWall(WallBasicType.DoorwayMiddle);
      // If just the left neighbor is adjacent, this wall is the right portion of a doorway.
      else if (leftNearRoom)
        wall = new DungeonWall(WallBasicType.DoorwayRight);
      // If just the right neighbor is adjacent, this wall is the left portion of a doorway.
      else if (rightNearRoom)
        wall = new DungeonWall(WallBasicType.DoorwayLeft);
      // If neither neighbor is adjacent, this wall is a full doorway.
      else
        wall = new DungeonWall(WallBasicType.DoorwayFull);

      return wall;
    }

    /// <summary>
    /// A helper function for determining if a tile, adjacent to the current wall being checked,
    /// is adjacent to a <see cref="DungeonRoomTile"/>. This is used if the current wall is also
    /// adjacent to a <see cref="DungeonRoomTile"/>.
    /// </summary>
    /// <param name="tile">The <see cref="DungeonTile"/> to check the neighbor of.</param>
    /// <param name="currentCardinal">The current <see cref="CardinalDirection"/> of the
    /// wall being checked. We check <paramref name="tile"/>'s neighbor in this direction.</param>
    /// <returns>Returns if the <paramref name="tile"/> is also adjacent to a
    /// <see cref="DungeonRoomTile"/> in the same direction.</returns>
    protected bool CheckTileAdjacentToRoom(DungeonTile tile, int currentCardinal)
    {
      // The tile must not be null or empty!
      if (tile != null && tile.BasicType != TileBasicType.Empty)
      {
        // Get the tile type of the tile's neighbor in the same direction as the current cardinal.
        TileBasicType directedNeighborType = DungeonManager.GetTileType(tile.TileIndex + NeighborIndexes[currentCardinal]);

        // Return if the adjacent tile is a room.
        return directedNeighborType == TileBasicType.Room;
      }

      return false; // By default, return false.
    }

    public override void CalculateBaseWalls()
    {
      // Iterate through the four cardinal directions in order.
      for (int currentCardinal = 0; currentCardinal < CardinalCount; currentCardinal++)
      {
        DungeonWall wall = null; // Wall information will be stored here.

        if (TileIndex == new Vector2Int(26, 20))
        {
          TileIndex = TileIndex;
        }

        // Get the index of the neighbor adjacent to the current wall.
        Vector2Int neighborIndex = NeighborIndexes[currentCardinal] + TileIndex;
        // Get the basic type of the neighbor. Invalid neighbors are treated as empty.
        TileBasicType neighborType = DungeonManager.GetTileType(neighborIndex);

        // Switch based on the neighboring tile's type.
        switch (neighborType)
        {
          // If the neighbor is a hallway, this will always be empty.
          case TileBasicType.Hallway:
            wall = new DungeonWall(true);
            break;
          // If the neighbor is a room, determine the wall's type of doorway.
          case TileBasicType.Room:
            wall = DetermineDoorwayType(currentCardinal);
            break;
          // By default, this will always be a solid wall.
          default:
            wall = new DungeonWall(WallBasicType.BasicWall);
            break;
        }

        TileWalls[currentCardinal] = wall; // Set the current wall.
      }
    }
  }
  /************************************************************************************************/
}