/**************************************************************************************************/
/*!
\file   Triangle.cs
\author Bl4ckb0ne, Adapted by Simon Zeni, Adapted by Vazgriz, Adapted by Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing single triangle, used for the Delaunay Triangulation Method.

\par Bug List

\par References
  - https://github.com/Bl4ckb0ne/delaunay-triangulation
  - https://github.com/vazgriz/DungeonGenerator
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Graphs.cs
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Scripts2D/Delaunay2D.cs
  - https://blackpawn.com/texts/pointinpoly/default.html
*/
/**************************************************************************************************/

using UnityEngine.UIElements;

namespace Delaunay
{
  /************************************************************************************************/
  /// <summary>
  /// A single triangle, made of 3 Vertices [See: <see cref="Vertex"/>], used for the Delaunay
  /// Triangulation Method.
  /// </summary>
  public class Triangle : System.IEquatable<Triangle>
  {
    /// <summary>The first <see cref="Vertex"/> that makes up the <see cref="Triangle"/>.</summary>
    public Vertex A { get; private set; }
    /// <summary>The second <see cref="Vertex"/> that makes up the <see cref="Triangle"/>.</summary>
    public Vertex B { get; private set; }
    /// <summary>The third <see cref="Vertex"/> that makes up the <see cref="Triangle"/>.</summary>
    public Vertex C { get; private set; }
    /// <summary>A helper toggle to determine some marking on this <see cref="Triangle"/>.</summary>
    public bool marked = false;

    /// <summary>
    /// A constructor for a Triangle, based on 3 Vertices [See: <see cref="Vertex"/>].
    /// </summary>
    /// <param name="A">The first <see cref="Vertex"/> of the <see cref="Triangle"/>.</param>
    /// <param name="B">The second <see cref="Vertex"/> of the <see cref="Triangle"/>.</param>
    /// <param name="C">The third <see cref="Vertex"/> of the <see cref="Triangle"/>.</param>
    public Triangle(Vertex A, Vertex B, Vertex C)
    {
      this.A = A;
      this.B = B;
      this.C = C;
    }

    /// <summary>
    /// A function determining if a given point is contained within the <see cref="Triangle"/>.
    /// This uses the Barycentric Technique.
    /// </summary>
    /// <param name="position">The position of the point.</param>
    /// <param name="error">The amount of error to allow in calculations.</param>
    /// <returns>Returns if the <paramref name="position"/> is contained.</returns>
    public bool ContainsPosition(System.Numerics.Vector3 position, float error = 0.01f)
    {
      error *= error; // Square error, as we are using squared distance.
      return System.Numerics.Vector3.DistanceSquared(position, A.Position) < error
        || System.Numerics.Vector3.DistanceSquared(position, B.Position) < error
        || System.Numerics.Vector3.DistanceSquared(position, C.Position) < error;

      /*
      // Get the vectors connecting the 3 vertices.
      System.Numerics.Vector3 vec0 = C.Position - A.Position;
      System.Numerics.Vector3 vec1 = B.Position - A.Position;
      System.Numerics.Vector3 vec2 = position - A.Position;

      // Compute dot products between the vectors.
      float dot00 = System.Numerics.Vector3.Dot(vec0, vec0);
      float dot01 = System.Numerics.Vector3.Dot(vec0, vec1);
      float dot02 = System.Numerics.Vector3.Dot(vec0, vec2);
      float dot11 = System.Numerics.Vector3.Dot(vec1, vec1);
      float dot12 = System.Numerics.Vector3.Dot(vec1, vec2);

      // Calculate the final coordinates.
      float inverse = 1 / ((dot00 * dot11) - (dot01 * dot01));
      float finalU = ((dot11 * dot02) - (dot01 * dot12)) * inverse;
      float finalV = ((dot00 * dot12) - (dot01 * dot02)) * inverse;

      // Check if the coordinates are within the plane.
      return (finalU >= 0) && (finalV >= 0) && (finalU + finalV < 1);*/
    }

#if UNITY_5_4_OR_NEWER
    /// <summary>
    /// A function determining if a given point is contained within the <see cref="Triangle"/>.
    /// This uses the Barycentric Technique.
    /// </summary>
    /// <param name="position">The position of the point.</param>
    /// <returns>Returns if the <paramref name="position"/> is contained.</returns>
    public bool ContainsPosition(UnityEngine.Vector3 position)
    {
      return ContainsPosition(new System.Numerics.Vector3(position.x, position.y, position.z));
    }
#endif

    /// <summary>
    /// A function determining if a position is contained in a triangle's circumference, via the
    /// Delaunay Triangulation Method. This is only for the XY plane.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>Returns if the <paramref name="position"/> is within the circumference.</returns>
    public bool CircumferenceContainsXY(System.Numerics.Vector3 position)
    {
      // These are simply holders for A, B, and C positions, as the lines will get very long.
      System.Numerics.Vector3 a = A.Position;
      System.Numerics.Vector3 b = B.Position;
      System.Numerics.Vector3 c = C.Position;

      // Get the square lengths of each magnitude.
      float ab = a.LengthSquared();
      float cd = b.LengthSquared();
      float ef = c.LengthSquared();

      // Get the circumference coordinates.
      float circumX = (ab * (c.Y - b.Y) + cd * (a.Y - c.Y) + ef * (b.Y - a.Y)) / (a.X * (c.Y - b.Y) + b.X * (a.Y - c.Y) + c.X * (b.Y - a.Y));
      float circumY = (ab * (c.X - b.X) + cd * (a.X - c.X) + ef * (b.X - a.X)) / (a.Y * (c.X - b.X) + b.Y * (a.X - c.X) + c.Y * (b.X - a.X));

      // Calculate the radius of the circle, and the distance to the position.
      System.Numerics.Vector3 circumference = new System.Numerics.Vector3(circumX / 2, circumY / 2, 0);
      float radius = (a - circumference).LengthSquared(); // System.Numerics.Vector3.DistanceSquared(a, circumference);
      float distance = (position - circumference).LengthSquared(); // System.Numerics.Vector3.DistanceSquared(position, circumference);

      return distance <= radius; // Check if the distance is within the radius.
    }

    public override bool Equals(object obj)
    {
      return (obj is Triangle t) ? this == t : false;
    }

    public override int GetHashCode()
    {
      return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
    }

    public bool Equals(Triangle other)
    {
      throw new System.NotImplementedException();
    }

    public static bool operator ==(Triangle left, Triangle right)
    {
      // A helper function for matching from each possible orientation.
      static bool Matches(Vertex pos, Triangle tri)
      {
        return pos == tri.A || pos == tri.B || pos == tri.C;
      }

      return Matches(left.A, right) || Matches(left.B, right) || Matches(left.C, right);
    }

    public static bool operator !=(Triangle left, Triangle right)
    {
      return !(left == right);
    }
  }
  /************************************************************************************************/
}