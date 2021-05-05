/**************************************************************************************************/
/*!
\file   DemoManager.cs
\author Craig Williams
\par    Last Updated
        2021-04-09

\brief
  A file containing implementation of a demo manager singleton, which handles displaying information
  and editing values for the generated dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// The singleton manager for all demo purposes. This handles coordinating values set by the
  /// user to the other managers, and displaying debug information.
  /// </summary>
  public class DemoManager : MonoBehaviour
  {
    /// <summary>
    /// A small enum for the current mode the demo is displaying.
    /// </summary>
    private enum DemoMode
    {
      /// <summary>Shows <see cref="InfoTile"/>s from an overhead perspective.</summary>
      Information,
      /// <summary>Allows the user to walk around the generated dungeon.</summary>
      Exploration,
    }

    /// <summary>The singleton instance for the <see cref="DemoManager"/>.</summary>
    private static DemoManager singleton;

    /// <summary>The prefab for the <see cref="InfoTile"/>.</summary>
    [Header("Demo Properties")]
    [SerializeField] private InfoTile TilePrefab;
    /// <summary>The area to spawn the <see cref="InfoTile"/>s at.</summary>
    [SerializeField] private Canvas TileZone;
    /// <summary>The <see cref="DemoCamera"/> that displays the information.</summary>
    [SerializeField] private DemoCamera demoCamera;
    /// <summary>The <see cref="DemoController"/> for moving around the generated dungeon.</summary>
    [SerializeField] private DemoController demoController;

    /// <summary>The <see cref="Canvas"/> of the info settings.</summary>
    [Header("Display Properties")]
    [Space(20.0f)]
    [SerializeField] private Canvas InfoDisplay;
    /// <summary>The <see cref="Canvas"/> of the exploration settings.</summary>
    [SerializeField] private Canvas ExploreDisplay;
    /// <summary>A display of the <see cref="InfoTile"/> currently selected.</summary>
    [SerializeField] private CurrentTileDisplay currentTileDisplay;
    /// <summary>The <see cref="GameObject"/> of the button that toggles the HUD.</summary>
    [SerializeField] private GameObject HUDToggle;

    /// <summary>The <see cref="Color"/>s for each <see cref="TileBasicType"/>.</summary>
    [Header("Color Properties")]
    [Space(20.0f)]
    [SerializeField] private Color[] BasicTypeColors = new Color[Tools.Enums.GetValueCount<TileBasicType>()];
    /// <summary>The <see cref="Color"/>s for each <see cref="TileEnvironmentType"/>.</summary>
    [SerializeField] private Color[] EnvironmentTypeColors = new Color[Tools.Enums.GetValueCount<TileEnvironmentType>()];
    /// <summary>The <see cref="Color"/>s for each <see cref="TileDecorType"/>.</summary>
    [SerializeField] private Color[] DecorTypeColors = new Color[Tools.Enums.GetValueCount<TileDecorType>()];
    /// <summary>The <see cref="Color"/>s for each <see cref="WallBasicType"/>.</summary>
    [SerializeField] private Color[] WallTypeColors = new Color[Tools.Enums.GetValueCount<WallBasicType>()];
    /// <summary>The <see cref="Color"/> for an empty <see cref="DungeonWall"/>.</summary>
    [SerializeField] private Color EmptyWallColor = Color.white;

    /// <summary>The <see cref="InfoTile"/>s currently spawned.</summary>
    private Dictionary<Vector2Int, InfoTile> CurrentInfoTiles = new Dictionary<Vector2Int, InfoTile>();
    /// <summary>The <see cref="RectTransform"/> of the <see cref="TileZone"/>.</summary>
    private RectTransform TileZoneRect;
    /// <summary>The current <see cref="DemoMode"/> being displayed.</summary>
    private DemoMode currentMode = DemoMode.Information;

    private void Awake()
    {
      // Declare this as a singleton manager.
      if (!singleton)
      {
        singleton = this;
        TileZoneRect = TileZone.GetComponent<RectTransform>();
      }
        
      else
        Destroy(this.gameObject);
    }

    /// <summary>
    /// A function to clean out all current <see cref="InfoTile"/>s on the map, to prep for a new
    /// dungeon.
    /// </summary>
    public static void CleanTiles()
    {
      if (singleton)
        singleton.InternalCleanTiles();
    }

    /// <summary>
    /// A function to create all <see cref="InfoTile"/>s for the current dungeon at once.
    /// </summary>
    public static void CreateAllTiles()
    {
      if (singleton)
        singleton.InternalCreateAllTiles();
    }

    /// <summary>
    /// A funciton used to force the demo into <see cref="DemoMode.Information"/>.
    /// </summary>
    public static void ForceInformationMode()
    {
      if (singleton)
      {
        singleton.HUDToggle.SetActive(true);
        singleton.InfoDisplay.enabled = true;
        singleton.ExploreDisplay.enabled = false;
        singleton.demoCamera.gameObject.SetActive(true);
        singleton.demoController.gameObject.SetActive(false);
        singleton.currentMode = DemoMode.Information;
      }
    }

    /// <summary>
    /// A funciton used to force the demo into <see cref="DemoMode.Exploration"/>.
    /// </summary>
    public static void ForceExplorationMode()
    {
      if (singleton)
      {
        singleton.HUDToggle.SetActive(false);
        singleton.InfoDisplay.enabled = false;
        singleton.ExploreDisplay.enabled = true;
        singleton.demoController.gameObject.SetActive(true);
        singleton.demoCamera.gameObject.SetActive(false);
        singleton.currentMode = DemoMode.Exploration;
      }
    }

    /// <summary>
    /// A funciton used to swap the current <see cref="DemoMode"/>.
    /// </summary>
    public static void ToggleDemoMode()
    {
      if (singleton && !DungeonManager.GetIsGenerating())
      {
        if (singleton.currentMode == DemoMode.Information)
          ForceExplorationMode();
        else
          ForceInformationMode();
      }
    }

    /// <summary>
    /// A function used to ready the <see cref="demoCamera"/> for display.
    /// </summary>
    public static void InitializeDemoCamera()
    {
      if (singleton)
        singleton.demoCamera.ReadyDemoCamera();
    }

    /// <summary>
    /// A function for initializing the <see cref="demoController"/>.
    /// </summary>
    /// <param name="room">The <see cref="DungeonRoom"/> to start the controller in.</param>
    public static void InitializeDemoController(DungeonRoom room)
    {
      if (singleton && room != null)
        singleton.InternalInitializeDemoController(room);
    }

    /// <summary>
    /// A function for forcing the HUD to be shown or not.
    /// </summary>
    /// <param name="enabled">The enabled state of the HUD.</param>
    public static void ForceHUDDisplay(bool enabled)
    {
      if (singleton)
        singleton.InfoDisplay.enabled = enabled;
    }

    /// <summary>
    /// A function that handles updating what <see cref="InfoTile"/> is currently presented by the
    /// <see cref="currentTileDisplay"/>.
    /// </summary>
    /// <param name="index">The index of the <see cref="InfoTile"/>.</param>
    public static void DisplayCurrentTile(Vector2Int index)
    {
      if (singleton)
      {
        DungeonTile tile = DungeonManager.GetDungeonTile(index); // Get the dungeon tile.

        // If the tile is not null, display the tile on the display board.
        if (tile != null)
          singleton.currentTileDisplay.DisplayTile(tile);
      }
    }

    /// <summary>
    /// A function to update all <see cref="InfoTile"/>s currently on display.
    /// </summary>
    public static void UpdateAllTiles()
    {
      if (singleton)
      {
        foreach (KeyValuePair<Vector2Int, InfoTile> pair in singleton.CurrentInfoTiles)
          pair.Value.UpdateInfoDisplay();
      }
    }

    /// <summary>
    /// A function to update a single <see cref="InfoTile"/>.
    /// </summary>
    /// <param name="index">The index of the tile to update.</param>
    public static void UpdateSingleTile(Vector2Int index)
    {
      if (singleton)
      {
        if (singleton.CurrentInfoTiles.TryGetValue(index, out InfoTile tile))
          tile.UpdateInfoDisplay();
      }
    }

    /// <summary>
    /// A function to create a single <see cref="InfoTile"/>.
    /// </summary>
    /// <param name="index">The index of the tile to create.</param>
    public static void CreateSingleTile(Vector2Int index)
    {
      if (singleton)
        singleton.InternalCreateSingleTile(index);
    }

    /// <summary>
    /// A function for getting the demo <see cref="Color"/> of a <see cref="TileBasicType"/>.
    /// </summary>
    /// <param name="type">The <see cref="TileBasicType"/> to get the <see cref="Color"/>.</param>
    /// <returns>Returns the associated <see cref="Color"/>.</returns>
    public static Color GetBasicTypeColor(TileBasicType type)
    {
      // Return the color, if the singleton exists.
      return singleton ? singleton.BasicTypeColors[(int)type] : Color.white;
    }

    /// <summary>
    /// A function for getting the demo <see cref="Color"/> of a <see cref="TileEnvironmentType"/>.
    /// </summary>
    /// <param name="type">The <see cref="TileEnvironmentType"/> to get the <see cref="Color"/>.</param>
    /// <returns>Returns the associated <see cref="Color"/>.</returns>
    public static Color GetEnvironmentTypeColor(TileEnvironmentType type)
    {
      // Return the color, if the singleton exists.
      return singleton ? singleton.EnvironmentTypeColors[(int)type] : Color.white;
    }

    /// <summary>
    /// A function for getting the demo <see cref="Color"/> of a <see cref="TileDecorType"/>.
    /// </summary>
    /// <param name="type">The <see cref="TileDecorType"/> to get the <see cref="Color"/>.</param>
    /// <returns>Returns the associated <see cref="Color"/>.</returns>
    public static Color GetDecorTypeColor(TileDecorType type)
    {
      // Return the color, if the singleton exists.
      return singleton ? singleton.DecorTypeColors[(int)type] : Color.white;
    }

    /// <summary>
    /// A function for getting the demo <see cref="Color"/> of a <see cref="WallBasicType"/>.
    /// </summary>
    /// <param name="wall">The <see cref="DungeonWall"/> to get the <see cref="Color"/>.</param>
    /// <returns>Returns the associated <see cref="Color"/>.</returns>
    public static Color GetWallTypeColor(DungeonWall wall)
    {
      // Return the color, if the singleton exists.
      return singleton ? (!wall.IsEmpty ? singleton.WallTypeColors[(int)wall.WallType] : singleton.EmptyWallColor) : Color.white;
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.DungeonSize"/>'s horizontal size.
    /// </summary>
    /// <param name="x">The new horizontal size.</param>
    public void Settings_DungeonSizeX(float x)
    {
      Vector2Int size = DungeonManager.GetDungeonSize();
      size.x = (int)x;
      DungeonManager.SetDungeonSize(size);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.DungeonSize"/>'s vertical size.
    /// </summary>
    /// <param name="y">The new vertical size.</param>
    public void Settings_DungeonSizeY(float y)
    {
      Vector2Int size = DungeonManager.GetDungeonSize();
      size.y = (int)y;
      DungeonManager.SetDungeonSize(size);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.MaxRoomSize"/>'s horizontal size.
    /// </summary>
    /// <param name="x">The new horizontal size.</param>
    public void Settings_MaxRoomSizeX(float x)
    {
      Vector2Int size = DungeonManager.GetMaxRoomSize();
      size.x = (int)x;
      DungeonManager.SetMaxRoomSize(size);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.MaxRoomSize"/>'s vertical size.
    /// </summary>
    /// <param name="y">The new vertical size.</param>
    public void Settings_MaxRoomSizeY(float y)
    {
      Vector2Int size = DungeonManager.GetMaxRoomSize();
      size.y = (int)y;
      DungeonManager.SetMaxRoomSize(size);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.MaxRooms"/>.
    /// </summary>
    /// <param name="rooms">The new room count.</param>
    public void Settings_MaxRooms(float rooms)
    {
      DungeonManager.SetMaxRooms((int)rooms);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.MaxRoomAttempts"/>.
    /// </summary>
    /// <param name="attempts">The new attempt count.</param>
    public void Settings_MaxAttempts(float attempts)
    {
      DungeonManager.SetMaxAttempts((int)attempts);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.LoopbackChance"/> to create hallways.
    /// </summary>
    /// <param name="chance">The new loopback chance..</param>
    public void Settings_LoopbackChance(float chance)
    {
      DungeonManager.SetLoopbackChance(chance);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.NoRandomization"/>.
    /// </summary>
    /// <param name="random">The toggle for if randomization is allowed for generation.</param>
    public void Settings_AllowRandomness(bool random)
    {
      DungeonManager.SetAllowRandomness(random);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.ForcedSeed"/>.
    /// </summary>
    /// <param name="seedString">The new forced seed when there is no randomization.</param>
    public void Settings_ForcedSeed(string seedString)
    {
       bool result = int.TryParse(seedString, out int seed);

      if (result)
        DungeonManager.SetForcedSeed(seed);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonManager"/>'s
    /// <see cref="DungeonManager.generateSlow"/>.
    /// </summary>
    /// <param name="slow">The toggle for if generation is step-by-step</param>
    public void Settings_SlowMode(bool slow)
    {
      DungeonManager.SetSlowMode(slow);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="HallwayPather"/>'s
    /// <see cref="HallwayPather.Heuristic"/>.
    /// </summary>
    /// <param name="type">The new <see cref="HallwayPather.HeuristicType"/>.</param>
    public void Settings_ChangeHeuristic(int type)
    {
      HallwayPather.ChangeHeuristic((HallwayPather.HeuristicType)type);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="HallwayPather"/>'s
    /// <see cref="HallwayPather.HeuristicWeight"/>.
    /// </summary>
    /// <param name="weight">The new heuristic weight.</param>
    public void Settings_ChangeHeuristicWeight(float weight)
    {
      HallwayPather.ChangeHeuristicWeight(weight);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="HallwayPather"/>'s
    /// cost for moving to a <see cref="TileBasicType.Empty"/> tile.
    /// </summary>
    /// <param name="cost">The new cost.</param>
    public void Settings_ChangeEmptyTileCost(float cost)
    {
      HallwayPather.ChangeTileTypeCost(TileBasicType.Empty, cost);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="HallwayPather"/>'s
    /// cost for moving to a <see cref="TileBasicType.Room"/> tile.
    /// </summary>
    /// <param name="cost">The new cost.</param>
    public void Settings_ChangeRoomTileCost(float cost)
    {
      HallwayPather.ChangeTileTypeCost(TileBasicType.Room, cost);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="HallwayPather"/>'s
    /// cost for moving to a <see cref="TileBasicType.Hallway"/> tile.
    /// </summary>
    /// <param name="cost">The new cost.</param>
    public void Settings_ChangeHallwayTileCost(float cost)
    {
      HallwayPather.ChangeTileTypeCost(TileBasicType.Hallway, cost);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonDecorator"/>'s
    /// <see cref="DungeonDecorator.RoomEnvironmentTileCount"/>.
    /// </summary>
    /// <param name="min">The new minimum number of tiles.</param>
    /// <param name="max">The new maximum number of tiles.</param>
    public void Settings_ChangeInitialEnvironmentTiles(float min, float max)
    {
      DungeonDecorator.ChangeInitialEnvironmentCount(new Vector2Int((int)min, (int)max));
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonDecorator"/>'s
    /// <see cref="DungeonDecorator.RiverProbability"/>.
    /// </summary>
    /// <param name="chance">The new probability of spawning a river.</param>
    public void Settings_ChangeRiverProbability(float chance)
    {
      DungeonDecorator.ChangeRiverProbability(chance);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonDecorator"/>'s
    /// <see cref="DungeonDecorator.MaxRoomRivers"/>.
    /// </summary>
    /// <param name="rooms">The new max number of rivers in a <see cref="DungeonRoom"/>.</param>
    public void Settings_ChangeMaxRoomRivers(float rooms)
    {
      DungeonDecorator.ChangeMaxRoomRivers((int)rooms);
    }

    /// <summary>
    /// A Settings function for setting the <see cref="DungeonDecorator"/>'s
    /// <see cref="DungeonDecorator.RiverSizeRange"/>.
    /// </summary>
    /// <param name="min">The new minimum number of tiles allowed for a river.</param>
    /// <param name="max">The new maximum number of tiles allowed for a river.</param>
    public void Settings_ChangeRiverSizeRange(float min, float max)
    {
      DungeonDecorator.ChangeRiverSizeRange(new Vector2Int((int)min, (int)max));
    }

    /// <summary>
    /// A Button function for generating a brand new dungeon.
    /// </summary>
    public void Button_GenerateDungeon()
    {
      DungeonManager.GenerateNewDungeon();
    }

    /// <summary>
    /// A Button function for toggling the display of <see cref="InfoTile"/>s.
    /// </summary>
    public void Button_ToggleInfoTiles()
    {
      if (!DungeonManager.GetIsGenerating())
        TileZone.enabled = !TileZone.enabled;
    }

    /// <summary>
    /// A Button function for toggling the demo's hud.
    /// </summary>
    public void Button_ToggleDemoControlsDisplay()
    {
      if (!DungeonManager.GetIsGenerating())
        InfoDisplay.enabled = !InfoDisplay.enabled;
    }

    /// <summary>
    /// A button function for quitting the application.
    /// </summary>
    public void Button_QuitApplication()
    {
      Application.Quit();
    }

    /// <summary>
    /// The internal function for <see cref="CleanTiles"/>.
    /// </summary>
    private void InternalCleanTiles()
    {
      int count = CurrentInfoTiles.Count; // Get the current count.

      // Destroy all current tiles.
#if UNITY_EDITOR
      // In Editor Mode, DestroyImmediate is needed.
      if (!EditorApplication.isPlayingOrWillChangePlaymode)
        foreach (KeyValuePair<Vector2Int, InfoTile> pair in CurrentInfoTiles)
          Destroy(pair.Value.gameObject);
      else
        foreach (KeyValuePair<Vector2Int, InfoTile> pair in CurrentInfoTiles)
          DestroyImmediate(pair.Value.gameObject);
#else
      foreach (KeyValuePair<Vector2Int, InfoTile> pair in CurrentInfoTiles)
        Destroy(pair.Value.gameObject);
#endif

      CurrentInfoTiles.Clear();
    }

    /// <summary>
    /// The internal function for <see cref="CreateAllTiles"/>.
    /// </summary>
    private void InternalCreateAllTiles()
    {
      Vector2Int size = DungeonManager.GetDungeonSize(); // Get the dungeon size.
      TileZoneRect.sizeDelta = size; // Set the size of the canvas to the dungeon size.

      // Iterate through every row and column.
      for (int row = 0; row < size.x; row++)
      {
        for (int col = 0; col < size.y; col++)
        {
          Vector2Int index = new Vector2Int(row, col); // Create the current index.
          InternalCreateSingleTile(index);
        }
      }
    }

    /// <summary>
    /// The internal function for <see cref="CreateSingleTile(Vector2Int)"/>.
    /// </summary>
    /// <param name="index">The index of the tile to create.</param>
    private void InternalCreateSingleTile(Vector2Int index)
    {
      DungeonTile tile = DungeonManager.GetDungeonTile(index); // Get the current tile.

      // Make sure the tile is valid.
      if (tile != null)
      {
        // Instantiate an InfoTile and set its position and information.
        InfoTile itile = Instantiate(TilePrefab, TileZoneRect);
        itile.GetComponent<RectTransform>().anchoredPosition = new Vector2(-index.y, index.x);
        itile.SetIndex(index);
        itile.UpdateInfoDisplay();
        CurrentInfoTiles.Add(index, itile);
      }
    }

    /// <summary>
    /// The internal function for <see cref="InitializeDemoController(DungeonRoom)"/>.
    /// </summary>
    /// <param name="room">The <see cref="DungeonRoom"/> to start the controller in.</param>
    private void InternalInitializeDemoController(DungeonRoom room)
    {
      DungeonTile tile = room.GetRandomTile(); // Get a random room tile.

      // Get the initial rotation amount and number of directions.
      float rotationAmount = 90.0f;
      int directionCount = Tools.Enums.GetValueCount<CardinalDirection>();

      // Rotate until the controller. would be facing an empty area.
      for (int i = 0; i < directionCount; i++)
      {
        DungeonWall wall = tile.TileWalls[i];
        if (wall.IsEmpty || wall.WallType != WallBasicType.BasicWall)
          break;

        rotationAmount -= 90.0f;
      }

      // Update the controller's position and rotation.
      demoController.transform.position = new Vector3(tile.TileIndex.x, 0.3f, tile.TileIndex.y);
      demoController.transform.eulerAngles = new Vector3(0.0f, rotationAmount, 0.0f);
    }
  }
  /************************************************************************************************/
}