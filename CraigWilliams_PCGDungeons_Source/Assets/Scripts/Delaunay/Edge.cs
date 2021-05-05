/**************************************************************************************************/
/*!
\file   Edge.cs
\author Simon Zeni, Adapted by Vazgriz, Adapted by Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing implementation of a singular edge, made of two Vertices.

\par Bug List

\par References
  - https://github.com/vazgriz/DungeonGenerator
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Graphs.cs
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Prim.cs
*/
/**************************************************************************************************/

namespace Delaunay
{
  /************************************************************************************************/
  /// <summary>
  /// singular edge, made of two Vertices [See: <see cref="Vertex"/>], used for making a
  /// triangulated map.
  /// </summary>
  public class Edge : System.IEquatable<Edge>
  {
    /// <summary>The first <see cref="Vertex"/> making up the edge.</summary>
    public Vertex U { get; private set; }
    /// <summary>The second <see cref="Vertex"/> making up the edge.</summary>
    public Vertex V { get; private set; }
    /// <summary>The squared distance between <see cref="U"/> and <see cref="V"/>.</summary>
    public float SquareDistance { get; private set; }
    /// <summary>A helper toggle to determine some marking on this <see cref="Edge"/>.</summary>
    public bool marked = false;

    /// <summary>
    /// A constructor for an edge, based on two vertices [See: <see cref="Vertex"/>].
    /// </summary>
    /// <param name="U">The first <see cref="Vertex"/> making up the edge.</param>
    /// <param name="V">The second <see cref="Vertex"/> making up the edge.</param>
    public Edge(Vertex U, Vertex V)
    {
      this.U = U;
      this.V = V;
      // Calculate the square distance, as it is faster.
      SquareDistance = System.Numerics.Vector3.DistanceSquared(U.Position, V.Position);
    }

    /// <summary>
    /// A helper function for checking if two <see cref="Edge"/>s are almost equal.
    /// This is only for checking on the XY plane.
    /// </summary>
    /// <param name="left">The first <see cref="Edge"/> to check.</param>
    /// <param name="right">The second <see cref="Edge"/> to check.</param>
    /// <returns>Returns if the values are almost equal.</returns>
    public static bool AlmostEqualXY(Edge left, Edge right)
    {
      return (Vertex.AlmostEqualXY(left.U, right.U) && Vertex.AlmostEqualXY(left.V, right.V)) || (Vertex.AlmostEqualXY(left.U, right.V) && Vertex.AlmostEqualXY(left.V, right.U));
    }

    public override bool Equals(object obj)
    {
      return (obj is Edge e) ? this == e : false;
    }

    public override int GetHashCode()
    {
      return U.GetHashCode() ^ V.GetHashCode();
    }

    public bool Equals(Edge other)
    {
      return this == other;
    }

    public static bool operator ==(Edge left, Edge right)
    {
      return (left.U == right.U || left.U == right.V) && (left.V == right.U || left.V == right.V);
    }

    public static bool operator !=(Edge left, Edge right)
    {
      return !(left == right);
    }
  }
  /************************************************************************************************/
}