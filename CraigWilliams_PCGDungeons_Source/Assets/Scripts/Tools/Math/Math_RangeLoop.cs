/**************************************************************************************************/
/*!
\file   Math_RangeLoop.cs
\author Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing helper functions for looping values between extremeties.

\par Bug List

\par References
*/
/**************************************************************************************************/

namespace PCGDungeon.Tools
{
  /************************************************************************************************/
  public static partial class Math
  {
    /// <summary>
    /// A function to loop a given value around until it is properly between two extremes.
    /// </summary>
    /// <param name="value">The value to loop.</param>
    /// <param name="min">The inclusive minimum for the <paramref name="value"/>.</param>
    /// <param name="max">The inclusive maximum for the <paramref name="value"/>.</param>
    /// <returns>Returns the looped <paramref name="value"/>.</returns>
    public static int LoopValueII(int value, int min, int max)
    {
      return value switch
      {
        int i when i < min => max - (min - i) % (max - min),
        int i when i > max => min + (i - min) % (max - min),
        _ => value,
      };
    }

    /// <summary>
    /// A function to loop a given value around until it is properly between two extremes.
    /// </summary>
    /// <param name="value">The value to loop.</param>
    /// <param name="min">The inclusive minimum for the <paramref name="value"/>.</param>
    /// <param name="max">The exclusive maximum for the <paramref name="value"/>.</param>
    /// <returns>Returns the looped <paramref name="value"/>.</returns>
    public static int LoopValueIE(int value, int min, int max)
    {
      return value switch
      {
        int i when i < min => max - (min - i) % (max - min),
        int i when i >= max => min + (i - min) % (max - min),
        _ => value,
      };
    }
  }
  /************************************************************************************************/
}
