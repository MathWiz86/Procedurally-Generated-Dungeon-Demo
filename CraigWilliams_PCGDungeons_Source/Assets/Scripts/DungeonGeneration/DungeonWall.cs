/**************************************************************************************************/
/*!
\file   DungeonWall.cs
\author Craig Williams
\par    Last Updated
        2021-03-31

\brief
  A file containing implementation of a single wall on a dungeon tile.

\par Bug List

\par References
*/
/**************************************************************************************************/

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A class of data representing a single wall on a <see cref="DungeonTile"/>. Every tile has
  /// four walls, one in each <see cref="CardinalDirection"/>.
  /// </summary>
  [System.Serializable]
  public class DungeonWall
  {
    /// <summary>A check if the wall is empty, or is filled with some model.</summary>
    public /*readonly*/ bool IsEmpty = true;
    /// <summary>The type of wall represented.</summary>
    public WallBasicType WallType = WallBasicType.BasicWall;

    /// <summary>
    /// The default constructor for a <see cref="DungeonWall"/>. This is privatized to force 
    /// initialization of <see cref="IsEmpty"/>.
    /// </summary>
    private DungeonWall() { }

    /// <summary>
    /// The constructor for a <see cref="DungeonWall"/>.
    /// </summary>
    /// <param name="IsEmpty">A check if the wall is empty, or is filled with some model.</param>
    public DungeonWall(bool IsEmpty)
    {
      this.IsEmpty = IsEmpty;
    }

    /// <summary>
    /// The constructor for a <see cref="DungeonWall"/>. This defaults to there being a wall.
    /// </summary>
    /// <param name="WallType">The type of wall represented.</param>
    public DungeonWall(WallBasicType WallType)
    {
      this.IsEmpty = false;
      this.WallType = WallType;
    }
  }
  /************************************************************************************************/
}