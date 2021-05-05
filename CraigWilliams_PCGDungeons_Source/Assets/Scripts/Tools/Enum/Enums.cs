/**************************************************************************************************/
/*!
\file   Enums.cs
\author Craig Williams
\par    Last Updated
        2021-03-31

\brief
  A file containing tools for dealing with enums and their values.

\par Bug List

\par References
*/
/**************************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace PCGDungeon.Tools
{
  /************************************************************************************************/
  public static partial class Enums
  {
    /// <summary>
    /// A function which gets the number of values an Enum has.
    /// </summary>
    /// <typeparam name="TEnum">The type of Enum to get the values of.</typeparam>
    /// <returns>Returns the number of values the Enum has. If <typeparamref name="TEnum"/> is not valid, it returns -1.</returns>
    public static int GetValueCount<TEnum>() where TEnum : System.Enum
    {
      return GetValueCount(typeof(TEnum));
    }

    /// <summary>
    /// A function which gets the number of values an Enum has.
    /// </summary>
    /// <param name="tenum">The type of the Enum to get the values of. Make sure this is an Enum!</param>
    /// <returns>Returns the number of values the Enum has. If <paramref name="tenum"/> is not valid, it returns -1.</returns>
    public static int GetValueCount(System.Type tenum)
    {
      return tenum.IsEnum ? System.Enum.GetValues(tenum).Length : -1;
    }

    /// <summary>
    /// A function which creates an array of an enum's values.
    /// </summary>
    /// <typeparam name="TEnum">The type of enum to get the values of.</typeparam>
    /// <returns>Returns an array of all values in the enum.</returns>
    public static TEnum[] GetEnumValueArray<TEnum>()
    {
      // Return the values of the enum, casted to a TEnum[]
      return (TEnum[])System.Enum.GetValues(typeof(TEnum));
    }

    /// <summary>
    /// A function which creates an array of an enum's values.
    /// This version returns a 'System.Array', not a normal array.
    /// </summary>
    /// <param name="tenum">The type of enum to get the values of. Make sure it's an Enum type!</param>
    /// <returns>Returns an array of all values in the enum.</returns>
    public static System.Array GetEnumValueArray(System.Type tenum)
    {
      // If the type is an enum, return all values.
      if (tenum.IsEnum)
        return System.Enum.GetValues(tenum);

      return null; // Return null if the type is not an enum.
    }

    /// <summary>
    /// A function which creates a list of an enum's values.
    /// </summary>
    /// <typeparam name="TEnum">The type of enum to get the values of.</typeparam>
    /// <returns>Returns a list of all values in the enum.</returns>
    public static List<TEnum> GetEnumValueList<TEnum>() where TEnum : System.Enum
    {
      // Get an array of the values, and convert it to a list.
      return GetEnumValueArray<TEnum>().ToList();
    }

    /// <summary>
    /// A function which gets the names of all values of an Enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of Enum the values will be obtained from.</typeparam>
    /// <returns>Returns an array of string names of the Enum values.</returns>
    public static string[] GetEnumStringNames<TEnum>() where TEnum : System.Enum
    {
      TEnum[] values = (TEnum[])System.Enum.GetValues(typeof(TEnum)); // Get the values in an array.
      string[] names = new string[values.Length]; // Create an array of strings of the same length.

      // For each value, convert it to a string and store into the array.
      for (int i = 0; i < values.Length; i++)
        names[i] = values[i].ToString();

      return names; // Return the array.
    }

    /// <summary>
    /// A function which gets the names of all values of an Enum.
    /// </summary>
    /// <param name="type">The type of Enum the values will be obtained from.</param>
    /// <returns>Returns an array of string names of the Enum values.</returns>
    public static string[] GetEnumStringNames(System.Type type)
    {
      if (!type.IsEnum)
        return null;

      System.Array values = System.Enum.GetValues(type); // Get the values in an array.
      string[] names = new string[values.Length]; // Create an array of strings of the same length.

      // For each value, convert it to a string and store into the array.
      for (int i = 0; i < values.Length; i++)
        names[i] = ((System.Enum)values.GetValue(i)).ToString();

      return names; // Return the array.
    }
  }
  /************************************************************************************************/
}