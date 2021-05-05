/**************************************************************************************************/
/*!
\file   InspectorFunctionAttribute.cs
\author Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing implementation of an attribute for Unity, which allows a function to be
  called via a button on the inspector.

\par Bug List

\par References
*/
/**************************************************************************************************/

using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace PCGDungeon.UnityEditor
{
  /************************************************************************************************/
  /// <summary>
  /// An attribute for making a button that calls a named funciton.
  /// </summary>
  public class OnChangeAttribute : PropertyAttribute
  {
    /// <summary>The name of the function.</summary>
    public string FunctionName = string.Empty;
    /// <summary>The parameters to pass to the function.</summary>
    public object[] Parameters = null;
    /// <summary>A toggle for determining if the button can only be used in Play Mode.</summary>
    public bool EditModeOnly = false;



    /// <summary>
    /// A constructor for a <see cref="OnChangeAttribute"/>.
    /// </summary>
    /// <param name="FunctionName">The name of the function.</param>
    /// <param name="EditModeOnly">A toggle for determining if the button is Play Mode only.</param>
    /// <param name="Parameters">The parameters to pass to the function.</param>
    public OnChangeAttribute(string FunctionName, bool EditModeOnly = false, params object[] Parameters)
    {
      this.FunctionName = FunctionName;
      this.EditModeOnly = EditModeOnly;
      this.Parameters = Parameters;
    }
  }
  /************************************************************************************************/
#if UNITY_EDITOR
  /************************************************************************************************/
  /// <summary>
  /// The custom drawer for a <see cref="OnChangeAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(OnChangeAttribute))]
  public class OnChangeDrawer : PropertyDrawer
  {
    private MethodInfo method = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      OnChangeAttribute atr = attribute as OnChangeAttribute;
      if (method == null)
      {
        System.Type type = property.serializedObject.targetObject.GetType();
        string methodName = atr.FunctionName;
        method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      }

      if (method != null && ((!atr.EditModeOnly) || (atr.EditModeOnly && !EditorApplication.isPlayingOrWillChangePlaymode)))
      {
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label, true);

        if (EditorGUI.EndChangeCheck())
          method.Invoke(property.serializedObject.targetObject, atr.Parameters);
      }
      else
        EditorGUI.PropertyField(position, property, label, true);

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }
  }
  /************************************************************************************************/
#endif
}
