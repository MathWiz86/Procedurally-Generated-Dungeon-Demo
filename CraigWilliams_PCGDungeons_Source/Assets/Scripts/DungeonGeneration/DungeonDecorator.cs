/**************************************************************************************************/
/*!
\file   DungeonDecorator.cs
\author Craig Williams
\par    Last Updated
        2021-04-03

\brief
  A file containing implementation of a dungeon decorator singleton, which handles settings for
  decorating a dungeon with environments and decor.

\par Bug List

\par References
*/
/**************************************************************************************************/

using PCGDungeon.Tools;
using PCGDungeon.UnityEditor;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A singleton class handling anything related to decorating <see cref="DungeonTile"/>s,
  /// including environments and decor.
  /// </summary>
  public class DungeonDecorator : MonoBehaviour
  {
    /// <summary>The number of <see cref="TileEnvironmentType"/> values.</summary>
    public static readonly int EnvironmentCount = Enums.GetValueCount<TileEnvironmentType>();
    /// <summary>The number of <see cref="TileDecorType"/> values.</summary>
    public static readonly int DecorCount = Enums.GetValueCount<TileDecorType>();

    /// <summary>The chance of adding any particular river to a <see cref="DungeonRoom"/>.</summary>
    public static float RiverProbability { get { return singleton ? singleton.riverProbability : 0; } }
    /// <summary>The maximum number of rivers a <see cref="DungeonRoom"/> can have.</summary>
    public static int MaxRoomRivers { get { return singleton ? singleton.maxRoomRivers : 0; } }
    /// <summary>The singleton instance for the dungeon decorator.</summary>
    private static DungeonDecorator singleton = null;

    /// <summary>The range of tiles that must get a <see cref="TileEnvironmentType"/>.</summary>
    [Header("Room Environment Properties")]
    [SerializeField] [ValueRange(1, 10)] private Vector2Int RoomEnvironmentTileCount = Vector2Int.one;
    /// <summary>The <see cref="TileEnvironmentType"/>s that room tiles can have.</summary>
    [SerializeField] private EnvironmentProbability[] RoomEnvironments = null;

    /// <summary>The inner value of <see cref="RiverProbability"/>.</summary>
    [Header("River Properties")]
    [Space(20.0f)]
    [SerializeField] [Range(0.0f, 1.0f)] private float riverProbability = 0.5f;
    /// <summary>The inner value of <see cref="MaxRoomRivers"/>.</summary>
    [SerializeField] [Range(0, 10)] private int maxRoomRivers = 5;
    /// <summary>The range of size that a river can be.</summary>
    [SerializeField] [ValueRange(1, 10)] private Vector2Int RiverSizeRange = Vector2Int.one;

    /// <summary>The probabilities of the <see cref="TileDecorType"/>s.</summary>
    [Header("Decor Properties")]
    [Space(20.0f)]
    [SerializeField] private EnvironmentDecorProbability[] EnvironmentDecors = null;

    private void Awake()
    {
      // Declare this as a singleton manager.
      if (!singleton)
        singleton = this;
      else
        Destroy(this.gameObject);
    }

    /// <summary>
    /// A function for getting the probability of a given <see cref="TileEnvironmentType"/> being
    /// placed onto a <see cref="DungeonRoomTile"/>.
    /// </summary>
    /// <param name="environment">The <see cref="TileEnvironmentType"/> to check.</param>
    /// <returns>Returns the probability of the <paramref name="environment"/> being placed.
    /// If the <paramref name="environment"/> is not allowed for rooms, 0 is returned.</returns>
    public static float GetRoomEnvironmentTileProbability(TileEnvironmentType environment)
    {
      // Make sure the singleton exists.
      if (singleton)
      {
        int count = singleton.RoomEnvironments.Length; // Get the count of environments allowed.

        // Iterate through all allowed environments.
        for (int i = 0; i < count; i++)
        {
          // If the current environment matches, return the probability.
          EnvironmentProbability ep = singleton.RoomEnvironments[i];
          if (ep.Environment == environment)
            return ep.TileProbability;
        }
      }

      return 0; // By default, return 0.
    }

    /// <summary>
    /// A function for randomly deciding if a given <see cref="TileEnvironmentType"/> is selected
    /// to be placed onto a <see cref="DungeonRoomTile"/>, based on its probability in the
    /// <see cref="DungeonDecorator"/>'s <see cref="RoomEnvironments"/>.
    /// </summary>
    /// <param name="environment">The <see cref="TileEnvironmentType"/> to check.</param>
    /// <returns>Returns if the <paramref name="environment"/> was selected.</returns>
    public static bool DetermineRoomEnvironmentOutcome(TileEnvironmentType environment)
    {
      // Get a random float and check it against the environment's tile probability.
      float random = (float)DungeonManager.SystemRandomGenerator.NextDouble();
      float probability = GetRoomEnvironmentTileProbability(environment);
      return random < probability;
    }

    /// <summary>
    /// A function to get a random amount of spreading that the given
    /// <see cref="TileEnvironmentType"/> should do. The <paramref name="environment"/> must be
    /// in the <see cref="DungeonDecorator"/>'s <see cref="RoomEnvironments"/>.
    /// </summary>
    /// <param name="environment">The <see cref="TileEnvironmentType"/> to check.</param>
    /// <returns>Returns the spread that the <paramref name="environment"/> should do.</returns>
    public static int GetRandomEnvironmentSpread(TileEnvironmentType environment)
    {
      if (singleton)
      {
        int count = singleton.RoomEnvironments.Length; // Get the number of environments

        // Iterate through each environment.
        for (int i = 0; i < count; i++)
        {
          // If the environments match, return a randomly selected amount of spread.
          EnvironmentProbability ep = singleton.RoomEnvironments[i];
          if (ep.Environment == environment)
            return DungeonManager.SystemRandomGenerator.Next(ep.SpreadRange.x, ep.SpreadRange.y + 1);
        }
      }

      return 0; // By default, return 0.
    }

    /// <summary>
    /// A function for getting the probability of a given <see cref="TileEnvironmentType"/> being
    /// spread to an adjacent <see cref="DungeonTile"/>.
    /// </summary>
    /// <param name="environment">The <see cref="TileEnvironmentType"/> to check.</param>
    /// <returns>Returns the probability of the <paramref name="environment"/> being spread.
    /// If the <paramref name="environment"/> is not allowed for rooms, 0 is returned.</returns>
    public static float GetEnvironmentSpreadProbability(TileEnvironmentType environment)
    {
      // Make sure the singleton exists.
      if (singleton)
      {
        int count = singleton.RoomEnvironments.Length; // Get the count of environments allowed.

        // Iterate through all allowed environments.
        for (int i = 0; i < count; i++)
        {
          // If the current environment matches, return the spread probability.
          EnvironmentProbability ep = singleton.RoomEnvironments[i];
          if (ep.Environment == environment)
            return ep.SpreadProbability;
        }
      }

      return 0; // By default, return 0.
    }

    /// <summary>
    /// A function for randomly deciding if a given <see cref="TileEnvironmentType"/> is selected
    /// to be spread onto an adjacent <see cref="DungeonTile"/>, based on its probability in the
    /// <see cref="DungeonDecorator"/>'s <see cref="RoomEnvironments"/>.
    /// </summary>
    /// <param name="environment">The <see cref="TileEnvironmentType"/> to check.</param>
    /// <returns>Returns if the <paramref name="environment"/> was spread.</returns>
    public static bool DetermineEnvironmentSpreadOutcome(TileEnvironmentType environment)
    {
      // Get a random float and check it against the environment's spread probability.
      float chance = (float)DungeonManager.SystemRandomGenerator.NextDouble();
      float probability = GetEnvironmentSpreadProbability(environment);
      return chance < probability;

    }

    /// <summary>
    /// A function for quickly checking if a river can spawn or not, based on the
    /// <see cref="RiverProbability"/>.
    /// </summary>
    /// <returns>Returns if a river can spawn.</returns>
    public static bool DetermineRiverSpawnOutcome()
    {
      if (singleton)
      {
        // Get a random float and check it against the environment's spread probability.
        float chance = (float)DungeonManager.SystemRandomGenerator.NextDouble();
        return chance < singleton.riverProbability;
      }

      return false;
    }

    /// <summary>
    /// A function for checking if a given dimension is within the allowed boundaries for the size
    /// of a river.
    /// </summary>
    /// <param name="dimensionSize">The size to check.</param>
    /// <returns>Returns if the <paramref name="dimensionSize"/> is valid for a river.</returns>
    public static bool CheckRiverSize(int dimensionSize)
    {
      // Return if the dimension size is within the range.
      return singleton ? singleton.RiverSizeRange.x <= dimensionSize && singleton.RiverSizeRange.y >= dimensionSize : false;
    }

    /// <summary>
    /// A function to get the range of tiles to force a <see cref="TileEnvironmentType"/> onto in
    /// order to begin environment spread.
    /// </summary>
    /// <returns>Returns the range of tiles to give a <see cref="TileEnvironmentType"/>.</returns>
    public static Vector2Int GetRoomEnvironmentTileRange()
    {
      return singleton ? singleton.RoomEnvironmentTileCount : Vector2Int.one;
    }

    /// <summary>
    /// A function for getting a random <see cref="TileDecorType"/> based on a given
    /// <see cref="TileEnvironmentType"/>.
    /// </summary>
    /// <param name="environment">The <see cref="TileEnvironmentType"/> to check.</param>
    /// <returns>Returns a random <see cref="TileDecorType"/>. By default, returns
    /// <see cref="TileDecorType.None"/>.</returns>
    public static TileDecorType GetRandomEnvironmentDecor(TileEnvironmentType environment)
    {
      // Make sure the singleton exists.
      if (singleton)
      {
        int count = singleton.EnvironmentDecors.Length; // Get the count of environments allowed.

        // Iterate through all allowed environments.
        for (int i = 0; i < count; i++)
        {
          // If the current environment matches, return a random decor.
          EnvironmentDecorProbability edp = singleton.EnvironmentDecors[i];
          if (edp.Environment == environment)
          {
            DecorProbability[] decors = edp.Decors;
            int decorCount = decors.Length;
            // Iterate through all possible decors.
            for (int j = 0; j < decorCount; j++)
            {
              // If the chance passes, return the current decor.
              float chance = (float)DungeonManager.SystemRandomGenerator.NextDouble();
              if (chance < decors[j].Probability)
                return decors[j].Decor;
            }
          }
        }
      }

      return TileDecorType.None;
    }

    /// <summary>
    /// A function for changing the <see cref="RoomEnvironmentTileCount"/>. This should only
    /// be used by the <see cref="DemoManager"/>.
    /// </summary>
    /// <param name="count">The new range of <see cref="RoomEnvironmentTileCount"/>.</param>
    public static void ChangeInitialEnvironmentCount(Vector2Int count)
    {
      if (singleton && count.x > 0 && count.y > 0 && count.x <= count.y)
        singleton.RoomEnvironmentTileCount = count;
    }

    /// <summary>
    /// A function for changing the <see cref="RiverProbability"/>. This should only
    /// be used by the <see cref="DemoManager"/>.
    /// </summary>
    /// <param name="chance">The new <see cref="RiverProbability"/>.</param>
    public static void ChangeRiverProbability(float chance)
    {
      if (singleton)
        singleton.riverProbability = Mathf.Clamp(chance, 0, 1);
    }

    /// <summary>
    /// A function for changing the <see cref="MaxRoomRivers"/>. This should only
    /// be used by the <see cref="DemoManager"/>.
    /// </summary>
    /// <param name="rivers">The new <see cref="MaxRoomRivers"/>.</param>
    public static void ChangeMaxRoomRivers(int rivers)
    {
      if (singleton)
        singleton.maxRoomRivers = Mathf.Clamp(rivers, 0, 10);
    }

    /// <summary>
    /// A function for changing the <see cref="RiverSizeRange"/>. This should only
    /// be used by the <see cref="DemoManager"/>.
    /// </summary>
    /// <param name="range">The new <see cref="RiverSizeRange"/>.</param>
    public static void ChangeRiverSizeRange(Vector2Int range)
    {
      if (singleton && range.x > 0 && range.y > 0 && range.x <= range.y)
        singleton.RiverSizeRange = range;
    }
  }
  /************************************************************************************************/
}