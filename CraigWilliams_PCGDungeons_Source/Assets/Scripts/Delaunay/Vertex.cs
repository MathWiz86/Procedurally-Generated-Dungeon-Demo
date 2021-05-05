/**************************************************************************************************/
/*!
\file   Vertex.cs
\author Simon Zeni, Adapted by Vazgriz
\par    Last Updated
        2021-03-27

\brief
  A file containing singular vertex point, used for making a triangulated map.

\par Bug List

\par References
  - https://github.com/vazgriz/DungeonGenerator
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Graphs.cs
*/
/**************************************************************************************************/

using System;
using System.Security.Cryptography.X509Certificates;

namespace Delaunay
{
  /************************************************************************************************/
  /// <summary>
  /// A singular vertex point, used for making a triangulated map.
  /// </summary>
  public class Vertex : System.IEquatable<Vertex>
  {
    /// <summary>The position of the vertex.</summary>
    public System.Numerics.Vector3 Position { get; private set; }

    /// <summary>
    /// A constructor for a vertex, based on a given <paramref name="Position"/>.
    /// </summary>
    /// <param name="Position">The position of the vertex.</param>
    public Vertex(System.Numerics.Vector3 Position)
    {
      this.Position = Position;
    }

#if UNITY_5_4_OR_NEWER
    /// <summary>
    /// A constructor for a vertex, based on a given <paramref name="Position"/>.
    /// </summary>
    /// <param name="Position">The position of the vertex.</param>
    public Vertex(UnityEngine.Vector3 Position)
    {
      this.Position = new System.Numerics.Vector3(Position.x, Position.y, Position.z);
    }
#endif

    /// <summary>
    /// A helper function for checking if two Vertices [See: <see cref="Vertex"/>] are almost equal.
    /// This is only for checking on the XY plane.
    /// </summary>
    /// <param name="a">The first <see cref="Vertex"/> to check.</param>
    /// <param name="b">The second <see cref="Vertex"/> to check.</param>
    /// <returns>Returns if the values are almost equal.</returns>
    public static bool AlmostEqualXY(Vertex a, Vertex b)
    {
      return AlmostEqual(a.Position.X, b.Position.X) && AlmostEqual(a.Position.Y, b.Position.Y);
    }

    /// <summary>
    /// A helper function for checking if two values are almost equal.
    /// </summary>
    /// <param name="a">The first <see cref="float"/> to check.</param>
    /// <param name="b">The second <see cref="float"/> to check.</param>
    /// <returns>Returns if the values are almost equal.</returns>
    private static bool AlmostEqual(float a, float b)
    {
      return Math.Abs(a - b) <= float.Epsilon * Math.Abs(a + b) * 2 || Math.Abs(a - b) < float.MinValue;
    }

    public override bool Equals(object obj)
    {
      if (obj is Vertex v)
        return this == v;

      return false;
    }

    public override int GetHashCode()
    {
      return Position.GetHashCode();
    }

    public bool Equals(Vertex other)
    {
      return Position == other.Position;
    }
  }
  /************************************************************************************************/
  /************************************************************************************************/
  /// <summary>
  /// A singular vertex, in relation to some type. This is used to make a triangulated map.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Vertex<T> : Vertex
  {
    /// <summary>The item which this vertex relates to.</summary>
    public T Item { get; private set; }

    /// <summary>
    /// A constructor for a singular vertex.
    /// </summary>
    /// <param name="Position">The position of the vertex.</param>
    /// <param name="Item">The item which this vertex relates to.</param>
    public Vertex(System.Numerics.Vector3 Position, T Item) : base(Position)
    {
      this.Item = Item;
    }

#if UNITY_5_4_OR_NEWER
    /// <summary>
    /// A constructor for a singular vertex.
    /// </summary>
    /// <param name="Position">The position of the vertex.</param>
    /// <param name="Item">The item which this vertex relates to.</param>
    public Vertex(UnityEngine.Vector3 Position, T Item) : base(Position)
    {
      this.Item = Item;
    }
#endif
  }
  /************************************************************************************************/
}