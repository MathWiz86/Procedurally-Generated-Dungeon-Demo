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
  public class InspectorFunctionAttribute : PropertyAttribute
  {
    /// <summary>The name of the function.</summary>
    public string FunctionName = string.Empty;
    /// <summary>The parameters to pass to the function.</summary>
    public object[] Parameters = null;
    /// <summary>A toggle for determining if the button can only be used in Play Mode.</summary>
    public bool PlayModeOnly = false;



    /// <summary>
    /// A constructor for a <see cref="InspectorFunctionAttribute"/>.
    /// </summary>
    /// <param name="FunctionName">The name of the function.</param>
    /// <param name="PlayModeOnly">A toggle for determining if the button is Play Mode only.</param>
    /// <param name="Parameters">The parameters to pass to the function.</param>
    public InspectorFunctionAttribute(string FunctionName, bool PlayModeOnly = false, params object[] Parameters)
    {
      this.FunctionName = FunctionName;
      this.PlayModeOnly = PlayModeOnly;
      this.Parameters = Parameters;
    }
  }
  /************************************************************************************************/
#if UNITY_EDITOR
  /************************************************************************************************/
  /// <summary>
  /// The custom drawer for a <see cref="InspectorFunctionAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(InspectorFunctionAttribute))]
  public class InspectorFunctionDrawer : PropertyDrawer
  {
    private MethodInfo method = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      InspectorFunctionAttribute atr = attribute as InspectorFunctionAttribute;
      if (method == null)
      {
        System.Type type = property.serializedObject.targetObject.GetType();
        string methodName = atr.FunctionName;
        method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      }
      
      if (method != null)
      {
        bool enabled = GUI.enabled;
        GUI.enabled = (!atr.PlayModeOnly) || (atr.PlayModeOnly && EditorApplication.isPlaying);
        Rect rect = new Rect(position.x * 0.5f, position.y, position.width - 10, position.height);
        if (GUI.Button(rect, label.text))
        {
          method.Invoke(property.serializedObject.targetObject, atr.Parameters);
        }
        GUI.enabled = enabled;
      }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return method != null ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
    }
  }
  /************************************************************************************************/
#endif
}