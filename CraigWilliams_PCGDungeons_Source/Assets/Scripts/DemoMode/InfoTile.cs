/**************************************************************************************************/
/*!
\file   InfoTile.cs
\author Craig Williams
\par    Last Updated
        2021-04-07

\brief
  A file containing implementation of a demo tile, which is used to display information about a
  corresponding tile in the dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using PCGDungeon.UnityEditor;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A special tile used for displaying information about a specified <see cref="DungeonTile"/>.
  /// </summary>
  public class InfoTile : MonoBehaviour, IPointerClickHandler
  {
    /// <summary></summary>
    /// <summary>The index of the <see cref="DungeonTile"/> to display information about.</summary>
    [SerializeField] [ReadOnly] private Vector2Int TileIndex;

    /// <summary>The <see cref="CanvasGroup"/> for all tile colors.</summary>
    [Header("Tile Color Properties")]
    [Space(20.0f)]
    [SerializeField] private CanvasGroup TileColorGroup;
    /// <summary>The <see cref="Image"/> for the <see cref="TileBasicType"/>.</summary>
    [SerializeField] private Image imgBasicType;
    /// <summary>The <see cref="Image"/> for the <see cref="TileEnvironmentType"/>.</summary>
    [SerializeField] private Image imgEnvironmentType;
    /// <summary>The <see cref="Image"/> for the <see cref="TileDecorType"/>.</summary>
    [SerializeField] private Image imgDecorType;

    /// <summary>The <see cref="CanvasGroup"/> for all wall colors.</summary>
    [Header("Wall Color Properties")]
    [Space(20.0f)]
    [SerializeField] private CanvasGroup WallColorGroup;
    /// <summary>The <see cref="Image"/> for the east <see cref="DungeonWall"/>.</summary>
    [SerializeField] private Image imgEastWall;
    /// <summary>The <see cref="Image"/> for the north <see cref="DungeonWall"/>.</summary>
    [SerializeField] private Image imgNorthWall;
    /// <summary>The <see cref="Image"/> for the west <see cref="DungeonWall"/>.</summary>
    [SerializeField] private Image imgWestWall;
    /// <summary>The <see cref="Image"/> for the south <see cref="DungeonWall"/>.</summary>
    [SerializeField] private Image imgSouthWall;

    /// <summary>The <see cref="CanvasGroup"/> for all tile text.</summary>
    [Header("Tile Text Properties")]
    [Space(20.0f)]
    [SerializeField] private CanvasGroup TileTextGroup;
    /// <summary>The text for the <see cref="TileBasicType"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpBasicType;
    /// <summary>The text for the <see cref="TileEnvironmentType"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpEnvironmentType;
    /// <summary>The text for the <see cref="TileDecorType"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpDecorType;

    /// <summary>The <see cref="CanvasGroup"/> for all wall text.</summary>
    [Header("Wall Text Properties")]
    [Space(20.0f)]
    [SerializeField] private CanvasGroup WallTextGroup;
    /// <summary>The text for the east <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpEastWall;
    /// <summary>The text for the north <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpNorthWall;
    /// <summary>The text for the west <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpWestWall;
    /// <summary>The text for the south <see cref="DungeonWall"/>.</summary>
    [SerializeField] private TextMeshProUGUI tmpSouthWall;

    /// <summary>The text to display when a <see cref="DungeonWall"/> is empty.</summary>
    private string EmptyWallText = "EMPTY";
    /// <summary>A basic <see cref="Color"/> to use when the corresponding tile is null.</summary>
    private Color NulledColor = Color.white;

    /// <summary>
    /// A function for setting this tile's <see cref="TileIndex"/>.
    /// </summary>
    /// <param name="index">The index of the <see cref="DungeonTile"/> represented.</param>
    public void SetIndex(Vector2Int index)
    {
      TileIndex = index;
    }

    /// <summary>
    /// A function to get 
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetIndex()
    {
      return TileIndex;
    }

    /// <summary>
    /// A function for toggling the view of all tile colors.
    /// </summary>
    /// <param name="enabled">A bool representing if the colors are visible.</param>
    public void ToggleTileColor(bool enabled)
    {
      TileColorGroup.alpha = enabled ? 1.0f : 0.0f; // Toggle the alpha, based on the enable state.
    }

    /// <summary>
    /// A function for toggling the view of the <see cref="imgBasicType"/>.
    /// </summary>
    /// <param name="enabled">A bool representing if the color is visible.</param>
    public void ToggleBasicTileColor(bool enabled)
    {
      imgBasicType.enabled = enabled; // Set the enable state.
    }

    /// <summary>
    /// A function for toggling the view of the <see cref="imgEnvironmentType"/>.
    /// </summary>
    /// <param name="enabled">A bool representing if the color is visible.</param>
    public void ToggleEnvironmentTileColor(bool enabled)
    {
      imgEnvironmentType.enabled = enabled; // Set the enable state.
    }

    /// <summary>
    /// A function for toggling the view of the <see cref="imgDecorType"/>.
    /// </summary>
    /// <param name="enabled">A bool representing if the color is visible.</param>
    public void ToggleDecorTileColor(bool enabled)
    {
      imgDecorType.enabled = enabled; // Set the enable state.
    }

    /// <summary>
    /// A function for toggling the view of all wall colors.
    /// </summary>
    /// <param name="enabled">A bool representing if the colors are visible.</param>
    public void ToggleWallColor(bool enabled)
    {
      WallColorGroup.alpha = enabled ? 1.0f : 0.0f; // Toggle the alpha, based on the enable state.
    }

    /// <summary>
    /// A function for toggling the view of all tile text.
    /// </summary>
    /// <param name="enabled">A bool representing if the text is visible.</param>
    public void ToggleTileText(bool enabled)
    {
      TileTextGroup.alpha = enabled ? 1.0f : 0.0f; // Toggle the alpha, based on the enable state.
    }

    /// <summary>
    /// A function for toggling the view of all wall text
    /// </summary>
    /// <param name="enabled">A bool representing if the text is visible.</param>
    public void ToggleWallText(bool enabled)
    {
      WallTextGroup.alpha = enabled ? 1.0f : 0.0f; // Toggle the alpha, based on the enable state.
    }

    /// <summary>
    /// A function for updating the displayed information of this tile's corresponding
    /// <see cref="DungeonTile"/>.
    /// </summary>
    public void UpdateInfoDisplay()
    {
      DungeonTile tile = DungeonManager.GetDungeonTile(TileIndex);

      if (tile != null)
      {
        UpdateTileColors(tile);
        UpdateWallColors(tile);
        UpdateTileText(tile);
        UpdateWallText(tile);
      }
      else
        NullifyDisplay();
    }

    /// <summary>
    /// A function for updating the displayed colors for a <see cref="DungeonTile"/>'s types.
    /// </summary>
    /// <param name="tile">The <see cref="DungeonTile"/> represented.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTileColors(DungeonTile tile)
    {
      // Get the colors from the demo manager.
      imgBasicType.color = DemoManager.GetBasicTypeColor(tile.BasicType);
      imgEnvironmentType.color = DemoManager.GetEnvironmentTypeColor(tile.EnvironmentType);
      imgDecorType.color = DemoManager.GetDecorTypeColor(tile.DecorType);
    }

    /// <summary>
    /// A function for updating the displayed colors for a <see cref="DungeonTile"/>'s walls.
    /// </summary>
    /// <param name="tile">The <see cref="DungeonTile"/> represented.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateWallColors(DungeonTile tile)
    {
      // Get the colors from the demo manager.
      DungeonWall[] walls = tile.TileWalls;
      imgEastWall.color = DemoManager.GetWallTypeColor(walls[(int)CardinalDirection.East]);
      imgNorthWall.color = DemoManager.GetWallTypeColor(walls[(int)CardinalDirection.North]);
      imgWestWall.color = DemoManager.GetWallTypeColor(walls[(int)CardinalDirection.West]);
      imgSouthWall.color = DemoManager.GetWallTypeColor(walls[(int)CardinalDirection.South]);
    }

    /// <summary>
    /// A function for updating the displayed text for a <see cref="DungeonTile"/>'s types.
    /// </summary>
    /// <param name="tile">The <see cref="DungeonTile"/> represented.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTileText(DungeonTile tile)
    {
      // Set the text based on the enum.
      tmpBasicType.text = tile.BasicType.ToString();
      tmpEnvironmentType.text = tile.EnvironmentType.ToString();
      tmpDecorType.text = tile.DecorType.ToString();
    }

    /// <summary>
    /// A function for updating the displayed text for a <see cref="DungeonTile"/>'s walls.
    /// </summary>
    /// <param name="tile">The <see cref="DungeonTile"/> represented.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateWallText(DungeonTile tile)
    {
      // Set the text based on the enum and emptiness.
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

    /// <summary>
    /// A function for nullifying the display if the corresponding <see cref="DungeonTile"/> does
    /// not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NullifyDisplay()
    {
      imgBasicType.color = NulledColor;
      imgEnvironmentType.color = NulledColor;
      imgDecorType.color = NulledColor;

      imgEastWall.color = NulledColor;
      imgNorthWall.color = NulledColor;
      imgWestWall.color = NulledColor;
      imgSouthWall.color = NulledColor;

      tmpBasicType.text = string.Empty;
      tmpEnvironmentType.text = "NULL TILE";
      tmpDecorType.text = string.Empty;

      tmpEastWall.text = string.Empty;
      tmpNorthWall.text = string.Empty;
      tmpWestWall.text = string.Empty;
      tmpSouthWall.text = string.Empty;
    }

    /// <summary>
    /// An event function for clicking on the tile.
    /// </summary>
    /// <param name="eventData">The data of the pointer when clicked on.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
      DemoManager.DisplayCurrentTile(TileIndex); // Dipslay the current tile.
    }
  }
  /************************************************************************************************/
}