/**************************************************************************************************/
/*!
\file   DungeonTypes.cs
\author Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing various enum types that are used to associate certain tiles with certain
  properties.

\par Bug List

\par References
*/
/**************************************************************************************************/

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// An enum for the standard cardinal directions, in clockwise order. The values of this type
  /// are extremely important, and are thus directly assigned.
  /// </summary>
  public enum CardinalDirection
  {
    /// <summary>Left</summary>
    East = 0,
    /// <summary>Up</summary>
    North = 1,
    /// <summary>Right</summary>
    West = 2,
    /// <summary>Down</summary>
    South = 3,
  }
  /************************************************************************************************/
  /************************************************************************************************/
  /// <summary>
  /// An enum for the basic type a <see cref="DungeonTile"/> on a dungeon is.
  /// </summary>
  public enum TileBasicType
  {
    /// <summary>There is no tile.</summary>
    Empty,
    /// <summary>The tile is part of a <see cref="DungeonRoom"/>.</summary>
    Room,
    /// <summary>The tile is part of a hallway.</summary>
    Hallway,
  }
  /************************************************************************************************/
  /************************************************************************************************/
  /// <summary>
  /// An enum for the environment texture of a <see cref="DungeonTile"/>.
  /// </summary>
  public enum TileEnvironmentType
  {
    /// <summary>The tile is basic dirt.</summary>
    Dirt,
    /// <summary>The tile is uncrossable water. Not generated the same as the rest.</summary>
    Water,
    /// <summary>The tile is hard stone.</summary>
    Stone,
    /// <summary>The tile is completely grassy.</summary>
    Grass,
  }
  /************************************************************************************************/
  /************************************************************************************************/
  /// <summary>
  /// An enum for the decoration placed on top of a <see cref="DungeonTile"/>.
  /// </summary>
  public enum TileDecorType
  {
    /// <summary>There is no decoration.</summary>
    None,
    /// <summary>There is a bridge. Paired with <see cref="TileEnvironmentType.Water"/>.</summary>
    Bridge,
    /// <summary>There are some crossable pebbles.</summary>
    Pebbles,
    /// <summary>There are some strands of grass.</summary>
    Grass,
  }
  /************************************************************************************************/
  /************************************************************************************************/
  /// <summary>
  /// An enum for the basic type a <see cref="DungeonWall"/> on a <see cref="DungeonTile"/> is.
  /// </summary>
  public enum WallBasicType
  {
    /// <summary>There is some form of a wall.</summary>
    BasicWall,
    /// <summary>There is a full doorway. Used by walls surrounded by basic walls.</summary>
    DoorwayFull,
    /// <summary>There is a left portion of a doorway. Used by walls starting an entrance.</summary>
    DoorwayLeft,
    /// <summary>There is a middle portion of a doorway. Used by walls of an entrance.</summary>
    DoorwayMiddle,
    /// <summary>There is a right portion of a doorway. Used by walls ending an entrance.</summary>
    DoorwayRight,
  }
  /************************************************************************************************/
}