/**************************************************************************************************/
/*!
\file   CurrentTileDisplay.cs
\author Craig Williams
\par    Last Updated
        2021-04-09

\brief
  A file containing implementation of a display for the information stored in an InfoTile.

\par Bug List

\par References
*/
/**************************************************************************************************/

using TMPro;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  public class CurrentTileDisplay : MonoBehaviour
  {
    /// <summary>The text to display when a <see cref="DungeonWall"/> is empty.</summary>
    private static readonly string EmptyWallText = "EMPTY";

    /// <summary>The text object for the <see cref="DungeonTile.TileIndex"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpIndex;
    /// <summary>The text object for the <see cref="DungeonTile.BasicType"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpBasicType;
    /// <summary>The text object for the <see cref="DungeonTile.EnvironmentType"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpEnvironmentType;
    /// <summary>The text object for the <see cref="DungeonTile.DecorType"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpDecorType;
    /// <summary>The text object for the tile's eastern <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpEastWall;
    /// <summary>The text object for the tile's northern <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpNorthWall;
    /// <summary>The text object for the tile's western <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpWestWall;
    /// <summary>The text object for the tile's southern <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpSouthWall;

    /// <summary>
    /// A function used to display the information of a given <see cref="DungeonTile"/>.
    /// </summary>
    /// <param name="tile">The <see cref="DungeonTile"/> to display information for. It is assumed
    /// by this point that a null check has been performed.</param>
    public void DisplayTile(DungeonTile tile)
    {
      // Display the index.
      tmpIndex.text = string.Format("({0}, {1})", tile.TileIndex.x, tile.TileIndex.y);

      // Display the tile types.
      tmpBasicType.text = tile.BasicType.ToString();
      tmpEnvironmentType.text = tile.EnvironmentType.ToString();
      tmpDecorType.text = tile.DecorType.ToString();

      // Get the walls and display either their type, or if they are empty.
      DungeonWall[] walls = tile.TileWalls;

      DungeonWall wall = walls[(int)CardinalDirection.East];
      tmpEastWall.text = wall.IsEmpty ? EmptyWallText : wall.WallType.ToString();

      wall = walls[(int)CardinalDirection.North];
      tmpNorthWall.text = wall.IsEmpty ? EmptyWallText : wall.WallType.ToString();

      wall = walls[(int)CardinalDirection.West];
      tmpWestWall.text = wall.IsEmpty ? EmptyWallText : wall.WallType.ToString();

      wall = walls[(int)CardinalDirection.South];
      tmpSouthWall.text = wall.IsEmpty ? EmptyWallText : wall.WallType.ToString();
    }
  }
  /************************************************************************************************/
}