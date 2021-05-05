/**************************************************************************************************/
/*!
\file   DungeonTile.cs
\author Craig Williams
\par    Last Updated
        2021-04-03

\brief
  A file containing implementation of a single tile in the procedurally generated dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using System.Runtime.CompilerServices;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A single tile in the procedurally generated dungeon. This contains all information about its
  /// placement, type, and decorations.
  /// </summary>
  public class DungeonTile : MonoBehaviour
  {
    /// <summary>An index used to make sure <see cref="TileIndex"/> is only set once.</summary>
    protected static readonly Vector2Int InvalidIndex = new Vector2Int(-1, -1);
    /// <summary>The number of <see cref="CardinalDirection"/>s.</summary>
    protected static readonly int CardinalCount = Tools.Enums.GetValueCount<CardinalDirection>();
    /// <summary>The additional indexes to each neighbor in cardinal indexes.</summary>
    public static readonly Vector2Int[] NeighborIndexes;
    /// <summary>The count of <see cref="NeighborIndexes"/>.</summary>
    public static readonly int NeighborCount;

    /// <summary>The index of this tile in the generated dungeon.</summary>
    public Vector2Int TileIndex { get; protected set; } = InvalidIndex;
    /// <summary>The basic type of this <see cref="DungeonTile"/>.</summary>
    public TileBasicType BasicType { get; protected set; } = TileBasicType.Empty;
    /// <summary>The environment type of this <see cref="DungeonTile"/>.</summary>
    public TileEnvironmentType EnvironmentType { get; protected set; } = TileEnvironmentType.Dirt;
    /// <summary>The decoration type of this <see cref="DungeonTile"/>.</summary>
    public TileDecorType DecorType { get; protected set; } = TileDecorType.None;
    /// <summary>The four walls, in order of <see cref="CardinalDirection"/>.</summary>
    public DungeonWall[] TileWalls /*{ get; protected set; }*/ = new DungeonWall[CardinalCount];

    /// <summary>
    /// The static constructor for a <see cref="DungeonTile"/>.
    /// </summary>
    static DungeonTile()
    {
      // Set the neighbor indexes and count in the constructor to make sure the count is correct.
      NeighborIndexes = new Vector2Int[]
      {
        // NOTE: Correct Orientation is Z+ Left, X+ Forward!!
        new Vector2Int(0, -1),    // East
        new Vector2Int(1, 0),     // North
        new Vector2Int(0, 1),     // West
        new Vector2Int(-1, 0),    // South
      };

      NeighborCount = NeighborIndexes.Length;
    }

    /// <summary>
    /// An initialization function for the <see cref="TileIndex"/>. This only allows setting
    /// the value once.
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(Vector2Int index)
    {
      // If the current index is the initial invalid one, allow setting the new index.
      if (TileIndex == InvalidIndex)
        TileIndex = index;
    }

    /// <summary>
    /// A helper function for getting the <see cref="TileBasicType"/> of this
    /// <see cref="DungeonTile"/>'s neighbor.
    /// </summary>
    /// <param name="direction">The direction of the neighbor.</param>
    /// <returns>Returns the neighbor's <see cref="TileBasicType"/>.</returns>
    public TileBasicType GetNeighborBasicType(CardinalDirection direction)
    {
      return DungeonManager.GetTileType(TileIndex + NeighborIndexes[(int)direction]);
    }

    /// <summary>
    /// A helper function for getting this <see cref="DungeonTile"/>'s neighbor in a given
    /// <see cref="CardinalDirection"/>.
    /// </summary>
    /// <param name="direction">The <see cref="CardinalDirection"/> of the neighbor.</param>
    /// <returns>Returns the neighboring <see cref="DungeonTile"/>, if it exists.</returns>
    public DungeonTile GetNeighborTile(CardinalDirection direction)
    {
      return DungeonManager.GetDungeonTile(TileIndex + NeighborIndexes[(int)direction]);
    }

    /// <summary>
    /// A helper function for getting this <see cref="DungeonTile"/>'s neighbor in a given
    /// <see cref="CardinalDirection"/>.
    /// </summary>
    /// <param name="direction">The <see cref="CardinalDirection"/> of the neighbor.</param>
    /// <returns>Returns the neighboring <see cref="DungeonTile"/>, if it exists.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected DungeonTile GetNeighborTile(int direction)
    {
      return DungeonManager.GetDungeonTile(TileIndex + NeighborIndexes[direction]);
    }

    /// <summary>
    /// An initialization function for the <see cref="BasicType"/>. This only allows setting
    /// the value of a <see cref="TileBasicType.Empty"/> tile.
    /// </summary>
    /// <param name="type">The basic type of this <see cref="DungeonTile"/>.</param>
    public virtual void SetBasicType(TileBasicType type)
    {
      // Only set if the tile is empty, as this can only be set once.
      if (BasicType == TileBasicType.Empty)
        BasicType = type;
    }

    /// <summary>
    /// An initialization function for the <see cref="EnvironmentType"/>.
    /// </summary>
    /// <param name="type">The environment type of this <see cref="DungeonTile"/>.</param>
    public virtual void SetEnvironmentType(TileEnvironmentType type)
    {
      EnvironmentType = type;
      // Do more things here...
    }

    /// <summary>
    /// An initialization function for the <see cref="DecorType"/>.
    /// </summary>
    /// <param name="type">The decoration type of this <see cref="DungeonTile"/>.</param>
    public virtual void SetDecorType(TileDecorType type)
    {
      DecorType = type;
      // Do more things here...
    }

    /// <summary>
    /// A function to calculate the <see cref="WallBasicType"/>s of this tile's four
    /// <see cref="DungeonWall"/>s.
    /// </summary>
    public virtual void CalculateBaseWalls() { }

    /// <summary>
    /// A function that spreads this <see cref="DungeonTile"/>'s <see cref="EnvironmentType"/>
    /// to neighboring <see cref="DungeonTile"/>s, as long as it can.
    /// </summary>
    /// <param name="currentSpread">The current spread index.</param>
    /// <param name="maxSpread">The maximum amount of spread for the environment.</param>
    protected void SpreadEnvironment(int currentSpread, int maxSpread)
    {

      // Only proceed if we are still able to spread.
      if (currentSpread >= maxSpread)
        return;

      // Iterate through all neighbors.
      for (int i = 0; i < NeighborCount; i++)
      {
        // Proceed if we successfully spread.
        if (DungeonDecorator.DetermineEnvironmentSpreadOutcome(EnvironmentType))
        {
          DungeonTile neighbor = GetNeighborTile(i); // Get the neighboring tile.

          // Spread the environment, so long as the neighbor is valid.
          if (neighbor && neighbor.BasicType != TileBasicType.Empty && neighbor.EnvironmentType == TileEnvironmentType.Dirt)
          {
            neighbor.EnvironmentType = EnvironmentType; // Set the environment.
            neighbor.SpreadEnvironment(currentSpread + 1, maxSpread); // Continue the spread.
          }
        }
      }
    }
  }
  /************************************************************************************************/
}