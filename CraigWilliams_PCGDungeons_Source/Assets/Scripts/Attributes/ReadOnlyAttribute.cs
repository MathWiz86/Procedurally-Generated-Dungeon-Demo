/**************************************************************************************************/
/*!
\file   ReadOnlyAttribute.cs
\author Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing implementation of an attribute for Unity, which makes the property unable to be
  edited in the Inspector window.

\par Bug List

\par References
*/
/**************************************************************************************************/

using UnityEngine;
using UnityEditor;

namespace PCGDungeon.UnityEditor
{
  /************************************************************************************************/
  /// <summary>
  /// An attribute for making a value uneditable when in the Unity Editor.
  /// </summary>
  public class ReadOnlyAttribute : PropertyAttribute
  {
    /// <summary>A toggle for allowing the variable to be edited when not in play mode.</summary>
    public bool isOnlyDuringGameplay = false;

    /// <summary>
    /// A constructor for a <see cref="ReadOnlyAttribute"/>.
    /// </summary>
    /// <param name="isOnlyDuringGameplay">A toggle for allowing the variable to be edited when not
    /// playing the game. Defaults to false.</param>
    public ReadOnlyAttribute(bool isOnlyDuringGameplay = false)
    {
      this.isOnlyDuringGameplay = isOnlyDuringGameplay;
    }
  }
  /************************************************************************************************/
#if UNITY_EDITOR
  /************************************************************************************************/
  /// <summary>
  /// The custom drawer for a <see cref="ReadOnlyAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      bool currentEnabled = GUI.enabled;
      bool isOnlyDuringGameplay = (this.attribute as ReadOnlyAttribute).isOnlyDuringGameplay;

      // The GUI should either be disabled, or only disabled if playing the game.
      if (isOnlyDuringGameplay)
        GUI.enabled = !(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused);
      else
        GUI.enabled = false;

      // Draw the property, and reset the GUI.
      EditorGUI.PropertyField(position, property, label, true);
      GUI.enabled = currentEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }
  }
  /************************************************************************************************/
#endif
}