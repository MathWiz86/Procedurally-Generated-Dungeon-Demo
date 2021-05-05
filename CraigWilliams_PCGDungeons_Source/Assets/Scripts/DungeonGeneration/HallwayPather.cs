/**************************************************************************************************/
/*!
\file   HallwayPather.cs
\author Craig Williams
\par    Last Updated
        2021-03-31

\brief
  A file containing implementation of a pathfinder for generating hallway tiles in the
  procedurally generated dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using Priority_Queue;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// An A* Pathfinder for generating <see cref="DungeonHallTile"/>s in the dungeon. This
  /// goes through the <see cref="DungeonManager.MSTEdges"/> tree in order to determine the
  /// grid routes to each room.
  /// </summary>
  public class HallwayPather : MonoBehaviour
  {
    /**********************************************************************************************/
    /// <summary>
    /// An enem for determining what kind of heuristic calculation to use.
    /// </summary>
    public enum HeuristicType
    {
      /// <summary>Heuristic based on overall distance.</summary>
      Euclidian,
      /// <summary>Heuristic based on a distance dependent on the max coordinate.</summary>
      Octile,
      /// <summary>Heuristic based on the maximum coordiante distance.</summary>
      Chebyshev,
      /// <summary>Heuristic based on adding the coordinate distances.</summary>
      Manhattan,
    }
    /**********************************************************************************************/
    /**********************************************************************************************/
    /// <summary>
    /// A helper class for storing traversal data about each tile in the generated dungeon.
    /// </summary>
    private class DungeonNode
    {
      /// <summary>The index of this node in relation to the generated dungeon.</summary>
      public Vector2Int Index { get; private set; }
      /// <summary>The full cost. <see cref="GivenCost"/> + <see cref="HeuristicCost"/>.</summary>
      public double FullCost { get { return GivenCost + HeuristicCost; } }

      /// <summary>The given cost from the <see cref="Parent"/> <see cref="DungeonNode"/>.</summary>
      public double GivenCost = double.MaxValue;
      /// <summary>The heuristical cost of the node.</summary>
      public double HeuristicCost = double.MaxValue;
      /// <summary>The parent node connected to this node.</summary>
      public DungeonNode Parent = null;

      /// <summary>
      /// The default constructor for a <see cref="DungeonNode"/>. This is privatized to force
      /// creation via an index.
      /// </summary>
      private DungeonNode() { }

      /// <summary>
      /// A constructor for a <see cref="DungeonNode"/>, via a grid index.
      /// </summary>
      /// <param name="Index">The index of this node in relation to the generated dungeon.</param>
      public DungeonNode(Vector2Int Index)
      {
        this.Index = Index;
      }

      /// <summary>
      /// A constructor for a <see cref="DungeonNode"/>, via grid coordinates.
      /// </summary>
      /// <param name="x">The row of the node.</param>
      /// <param name="y">The column of the node.</param>
      public DungeonNode(int x, int y)
      {
        this.Index = new Vector2Int(x, y);
      }

      /// <summary>
      /// A helper function for resetting a node back to its initial values.
      /// </summary>
      public void Reset()
      {
        GivenCost = double.MaxValue;
        HeuristicCost = double.MaxValue;
        Parent = null;
      }
    }
    /**********************************************************************************************/

    /// <summary>A pre-calculated square root of 2, for heuristic calculations.</summary>
    private static readonly double SqrtOfTwo = System.Math.Sqrt(2.0f);
    /// <summary>The additional indexes to each neighbor in cardinal indexes.</summary>
    private static readonly Vector2Int[] neighborIndexes =
    {
      // NOTE: Correct Orientation is Z+ Left, X+ Forward!! For some reason, this order ensures
      // that there will not be a floating point error.
      new Vector2Int(1, 0),
      new Vector2Int(-1, 0),
      new Vector2Int(0, 1),
      new Vector2Int(0, -1),
    };

    /// <summary>The singleton instance for the </summary>
    private static HallwayPather singleton = null;

    /// <summary>The selected heuristic for the pathfinder.</summary>
    [SerializeField] private HeuristicType Heuristic = HeuristicType.Euclidian;
    /// <summary>The weight of the heuristic cost when calculating the best path.</summary>
    [SerializeField] [Range(0.0f, 10.0f)] private float HeuristicWeight = 1.0f;
    /// <summary>The costs in relation to the tile types.</summary>
    [SerializeField] private float[] TileTypeCosts = new float[Tools.Enums.GetValueCount<TileBasicType>()];

    /// <summary>A grid of the individual nodes to represent the generated dungeon.</summary>
    private DungeonNode[,] dungeonNodes = null;
    /// <summary>The size of the dungeon. Copied from <see cref="DungeonManager"/>.</summary>
    private Vector2Int dungeonSize = Vector2Int.zero;
    /// <summary>The open list for A* pathfinding the dungeon.</summary>
    private SimplePriorityQueue<DungeonNode, double> openList = new SimplePriorityQueue<DungeonNode, double>();
    /// <summary>The closed list for A* pathfinding the dungeon, disallowing duplicates.</summary>
    private HashSet<DungeonNode> closedList = new HashSet<DungeonNode>();

    private void Awake()
    {
      // Declare this as a singleton manager.
      if (!singleton)
        singleton = this;
      else
        Destroy(this.gameObject);
    }

    public static void ChangeHeuristic(HeuristicType type)
    {
      if (singleton)
        singleton.Heuristic = type;
    }

    public static void ChangeHeuristicWeight(float weight)
    {
      if (singleton)
        singleton.HeuristicWeight = weight;
    }

    public static void ChangeTileTypeCost(TileBasicType type, float cost)
    {
      if (singleton)
        singleton.TileTypeCosts[(int)type] = cost;
    }

    /// <summary>
    /// A function for initializing the pather, based on the size of the dungeon. This works
    /// directly with <see cref="DungeonManager"/>.
    /// </summary>
    /// <param name="DungeonSize"></param>
    public static void InitializePather(Vector2Int DungeonSize)
    {
      if (singleton)
      {
        // Initialize the dungeon size and the nodes.
        singleton.dungeonNodes = new DungeonNode[DungeonSize.x, DungeonSize.y];
        singleton.dungeonSize = DungeonSize;

        // Create a new node for each dungeon tile.
        for (int row = 0; row < DungeonSize.x; row++)
        {
          for (int col = 0; col < DungeonSize.y; col++)
            singleton.dungeonNodes[row, col] = new DungeonNode(row, col);
        }
      }
    }

    /// <summary>
    /// A function for finding a path between one grid space and another grid space in the
    /// randomly generated dungeon.
    /// </summary>
    /// <param name="start">The starting index of the path.</param>
    /// <param name="end">The ending index of the path.</param>
    /// <returns>Returns a list of indexes representing the tiles used to form the path.</returns>
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
      return singleton ? singleton.InternalFindPath(start, end) : null;
    }

    /// <summary>
    /// An internal function for finding a path between one grid space and another grid space in the
    /// randomly generated dungeon.
    /// </summary>
    /// <param name="start">The starting index of the path.</param>
    /// <param name="end">The ending index of the path.</param>
    /// <returns>Returns a list of indexes representing the tiles used to form the path.</returns>
    private List<Vector2Int> InternalFindPath(Vector2Int start, Vector2Int end)
    {
      ResetPather(); // Reset the pather entirely.
      int neighborCount = neighborIndexes.Length;

      // Initialize the open list with the starting node.
      dungeonNodes[start.x, start.y].GivenCost = 0.0f;
      dungeonNodes[start.x, start.y].HeuristicCost = 0.0f;
      openList.Enqueue(dungeonNodes[start.x, start.y], 0.0f);

      // Iterate while there are still nodes on the open list.
      while (openList.Count > 0)
      {
        DungeonNode parent = openList.Dequeue(); // Pop off the top node.
        closedList.Add(parent); // Add the parent to the closed list.

        // If the parent's index matches the last node, we have completed the path.
        if (parent.Index == end)
        {
          List<Vector2Int> finalPath = new List<Vector2Int>();

          // Push the final path.
          while (parent != null)
          {
            finalPath.Add(parent.Index);
            parent = parent.Parent;
          }
          // Reverse the order and return.
          finalPath.Reverse();
          return finalPath;
        }

        for (int i = 0; i < neighborCount; i++)
        {
          Vector2Int neighborIndex = parent.Index + neighborIndexes[i]; // Get the current neighbor

          // Make sure that the neighbor is valid.
          if (DungeonManager.IsValidTileIndex(neighborIndex))
          {
            DungeonNode neighbor = dungeonNodes[neighborIndex.x, neighborIndex.y];

            // Make sure the neighbor is not already on the closed list.
            if (!closedList.Contains(neighbor))
            {
              // Calculate the given and heuristic costs.
              double given = GetNodeGivenCost(neighbor);
              double heuristic = GetNodeHeuristicCost(neighbor, end);

              // The parent cost is added to the heuristic and not the given due to floating point
              //errors. A solution to this is to change to doubles, but it's just not worth it.
              heuristic += parent.FullCost;

              // If the total cost is less than the neighbor's full cost, add it to the open list.
              if (given + heuristic < neighbor.FullCost)
              {
                neighbor.GivenCost = given;
                neighbor.HeuristicCost = heuristic;
                neighbor.Parent = parent;

                // Update priority in the queue, via functionality by BlueRaja.
                if (openList.TryGetPriority(parent, out double _))
                  openList.UpdatePriority(parent, neighbor.FullCost);
                else
                  openList.Enqueue(neighbor, neighbor.FullCost);
              }
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// A helper function for resetting all the nodes in the pather's grid.
    /// </summary>
    private void ResetPather()
    {
      // Reset the dungeon nodes.
      for (int row = 0; row < dungeonSize.x; row++)
      {
        for (int col = 0; col < dungeonSize.y; col++)
          dungeonNodes[row, col].Reset();
      }

      // Clear the lists.
      openList.Clear();
      closedList.Clear();
    }

    /// <summary>
    /// A helper function for getting the heuristic cost of a given neighbor node.
    /// </summary>
    /// <param name="neighbor">The <see cref="DungeonNode"/> to check.</param>
    /// <param name="end">The index of the goal tile.</param>
    /// <returns>Returns the final heuristic cost.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetNodeHeuristicCost(DungeonNode neighbor, Vector2Int end)
    {
      // Calculate the absolute difference on the X and Y coordinates.
      float xdiff = System.Math.Abs(end.x - neighbor.Index.x);
      float ydiff = System.Math.Abs(end.y - neighbor.Index.y);

      switch (Heuristic)
      {
        case HeuristicType.Octile:
          float min = System.Math.Min(xdiff, ydiff);
          float max = System.Math.Max(xdiff, ydiff);
          return HeuristicWeight * ((min * SqrtOfTwo + max - min) * 1.001f);
        case HeuristicType.Chebyshev:
          return HeuristicWeight * System.Math.Max(xdiff, ydiff);
        case HeuristicType.Manhattan:
          return HeuristicWeight * (xdiff + ydiff);
        default:
          return HeuristicWeight * (float)System.Math.Sqrt((xdiff * xdiff) + (ydiff * ydiff));
      }
    }

    /// <summary>
    /// A helper function for getting the given cost of a node, via its <see cref="TileBasicType"/>.
    /// </summary>
    /// <param name="neighbor">The <see cref="DungeonNode"/> to check.</param>
    /// <returns>Returns the given cost of the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetNodeGivenCost(DungeonNode neighbor)
    {
      // Get the neighbor's tile type.
      TileBasicType tileType = DungeonManager.GetTileType(neighbor.Index);
      // Parent used to be here. But due to floating point errors, its cost is added later.
      return /*parent.FullCost + */TileTypeCosts[(int)tileType];
    }
  }
  /************************************************************************************************/
}