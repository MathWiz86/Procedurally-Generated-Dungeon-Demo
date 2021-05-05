/**************************************************************************************************/
/*!
\file   TileProbabilities.cs
\author Craig Williams
\par    Last Updated
        2021-04-03

\brief
  A file containing implementation of various probability structs used when dealing with
  decorating each tile.

\par Bug List

\par References
*/
/**************************************************************************************************/

using PCGDungeon.UnityEditor;
using UnityEngine;

namespace PCGDungeon
{
  /**********************************************************************************************/
  /// <summary>
  /// A helpful struct for determining the probabilities of various details about each
  /// <see cref="TileEnvironmentType"/>.
  /// </summary>
  [System.Serializable]
  public struct EnvironmentProbability
  {
    /// <summary>The <see cref="TileEnvironmentType"/> represented.</summary>
    public TileEnvironmentType Environment { get { return environment; } }
    /// <summary>The chance of <see cref="Environment"/> being selected.</summary>
    public float TileProbability { get { return tileProbability; } }
    /// <summary>The range that the <see cref="Environment"/> is allowed to spread.</summary>
    public Vector2Int SpreadRange { get { return spreadRange; } }
    /// <summary>The chance that <see cref="Environment"/> will spread to other tiles.</summary>
    public float SpreadProbability { get { return spreadProbability; } }

    /// <summary>The value of <see cref="Environment"/>.</summary>
    [SerializeField] private TileEnvironmentType environment;
    /// <summary>The value of <see cref="TileProbability"/>.</summary>
    [SerializeField] [Range(0.0f, 1.0f)] private float tileProbability;
    /// <summary>The value of <see cref="SpreadRange"/>.</summary>
    [SerializeField] [ValueRange(0, 5)] private Vector2Int spreadRange;
    /// <summary>The value of <see cref="SpreadProbability"/>.</summary>
    [SerializeField] [Range(0.0f, 1.0f)] private float spreadProbability;
  }
  /**********************************************************************************************/
  /**********************************************************************************************/
  /// <summary>
  /// A helpful struct for determining the probabilities of various details about each
  /// <see cref="TileDecorType"/>.
  /// </summary>
  [System.Serializable]
  public struct DecorProbability
  {
    /// <summary>The <see cref="TileDecorType"/> represented.</summary>
    public TileDecorType Decor { get { return decor; } }
    /// <summary>The chance of <see cref="Decor"/> being selected.</summary>
    public float Probability { get { return probability; } }

    /// <summary>The value of <see cref="Decor"/>.</summary>
    [SerializeField] private TileDecorType decor;
    /// <summary>The value of <see cref="Probability"/>.</summary>
    [SerializeField] [Range(0.0f, 1.0f)] private float probability;
  }
  /**********************************************************************************************/
  /**********************************************************************************************/
  /// <summary>
  /// A helpful struct for determining the probabilities of various details about each
  /// <see cref="TileDecorType"/> in relation to each <see cref="TileEnvironmentType"/>.
  /// </summary>
  [System.Serializable]
  public struct EnvironmentDecorProbability
  {
    /// <summary>The <see cref="TileEnvironmentType"/> represented.</summary>
    public TileEnvironmentType Environment { get { return environment; } }
    /// <summary>The <see cref="TileDecorType"/> paired with <see cref="Environment"/>.</summary>
    public DecorProbability[] Decors { get { return decors; } }

    /// <summary>The value of <see cref="Environment"/>.</summary>
    [SerializeField] private TileEnvironmentType environment;
    /// <summary>The value of <see cref="Decors"/>.</summary>
    [SerializeField] private DecorProbability[] decors;
  }
  /**********************************************************************************************/
}