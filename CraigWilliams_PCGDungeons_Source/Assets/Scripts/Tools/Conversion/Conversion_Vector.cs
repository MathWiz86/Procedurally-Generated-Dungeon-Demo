/**************************************************************************************************/
/*!
\file   Conversion_Vector.cs
\author Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing implementation of conversions between the built-in Vectors of .NET and the
  Vectors of Unity.

\par Bug List

\par References
*/
/**************************************************************************************************/

namespace PCGDungeon.Tools
{
  /************************************************************************************************/
  public static partial class Conversion
  {
    /// <summary>
    /// A conversion function between a <see cref="System.Numerics.Vector3"/> to a
    /// <see cref="UnityEngine.Vector3"/>.
    /// </summary>
    /// <param name="vector">The <see cref="System.Numerics.Vector3"/>.</param>
    /// <returns>Returns <paramref name="vector"/> as a <see cref="UnityEngine.Vector3"/>.</returns>
    public static UnityEngine.Vector3 ToUnityVector(System.Numerics.Vector3 vector)
    {
      return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
    }

    /// <summary>
    /// A conversion function between a <see cref="UnityEngine.Vector3"/> to a
    /// <see cref="System.Numerics.Vector3"/>.
    /// </summary>
    /// <param name="vector">The <see cref="UnityEngine.Vector3"/>.</param>
    /// <returns>Returns <paramref name="vector"/> as a
    /// <see cref="System.Numerics.Vector3"/>.</returns>
    public static System.Numerics.Vector3 ToStandardVector(UnityEngine.Vector3 vector)
    {
      return new System.Numerics.Vector3(vector.x, vector.y, vector.z);
    }
  }
  /************************************************************************************************/
}