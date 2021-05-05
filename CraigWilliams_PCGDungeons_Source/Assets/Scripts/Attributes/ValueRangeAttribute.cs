/**************************************************************************************************/
/*!
\file   ValueRangeAttribute.cs
\author Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing implementation of an attribute for Unity, which displays a range for the
  various types that support it, including Vectors.

\par Bug List

\par References
*/
/**************************************************************************************************/

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

namespace PCGDungeon.UnityEditor
{
  /// <summary>
  /// An upgraded range attribute, which works with any type of Vector.
  /// </summary>
  public class ValueRangeAttribute : PropertyAttribute
  {
    /// <summary>The minimum value allowed, inclusive.</summary>
    public float min;
    /// <summary>The maximum value allowed, inclusive.</summary>
    public float max;
    /// <summary>A check for if the range is valid. <see cref="min"/> must be less than
    /// <see cref="max"/>.</summary>
    public bool isValid;

    /// <summary>
    /// The constructor for a <see cref="ValueRangeAttribute"/>.
    /// </summary>
    /// <param name="min">The minimum value allowed, inclusive.</param>
    /// <param name="max">The maximum value allowed, inclusive.</param>
    public ValueRangeAttribute(float min, float max)
    {
      this.min = min;
      this.max = max;

      isValid = min < max;
    }
  }

#if UNITY_EDITOR
  /// <summary>
  /// The custom drawer for a <see cref="ValueRangeAttribute"/>.
  /// </summary>
  [CustomPropertyDrawer(typeof(ValueRangeAttribute))]
  public sealed class ValueRangeDrawer : PropertyDrawer
  {
    /// <summary>A holder for the final min value.</summary>
    float minValue = 0.0f;
    /// <summary>A holder for the final max value.</summary>
    float maxValue = 0.0f;

    /// <summary>
    /// A version of the attribute GUI for float properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyFloat(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      EditorGUI.Slider(position, property, minValue, maxValue, label); // Draw the slider.
      EditorGUI.EndProperty(); // End the property.
    }

    /// <summary>
    /// A version of the attribute GUI for int properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyInt(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      EditorGUI.IntSlider(position, property, (int)minValue, (int)maxValue, label); // Draw the slider.
      EditorGUI.EndProperty(); // End the property.
    }

    /// <summary>
    /// A version of the attribute GUI for Vector2 properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyVector2(Rect position, SerializedProperty property, GUIContent label)
    {
      // Note: Unity EditorGUI is very buggy and boilerplate. There existed a bug that caused the label
      // to change text every time an EditorGUI function was called, even though label never got passed.
      // Please bear with the poorly created code in an attempt to make the MinMaxSlider actually have numbers.

      Rect fullPos = position;
      string text = label.text;
      string tooltip = label.tooltip;
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      Vector2 value = property.vector2Value; // Make a copy of the vector value.

      EditorGUI.indentLevel += 3;
      position.y += 20.0f;
      position.height = 20.0f;
      EditorGUI.PrefixLabel(position, new GUIContent("Min"));

      position.x += 30.0f;
      position.width = 100.0f;
      EditorGUI.BeginChangeCheck();
      value.x = EditorGUI.FloatField(position, value.x);

      position.x += 80.0f;
      position.width = fullPos.width;
      EditorGUI.PrefixLabel(position, new GUIContent("Max"));

      position.x += 30.0f;
      position.width = 100.0f;
      value.y = EditorGUI.FloatField(position, value.y);

      // Finish clamping values.
      if (EditorGUI.EndChangeCheck())
      {
        value.x = Mathf.Clamp(value.x, minValue, maxValue);
        value.y = Mathf.Clamp(value.y, minValue, maxValue);
        value.x = Mathf.Clamp(value.x, minValue, value.y);
        value.y = Mathf.Clamp(value.y, value.x, maxValue);
      }

      position = fullPos;
      EditorGUI.indentLevel -= 3;
      EditorGUI.MinMaxSlider(fullPos, new GUIContent(text, tooltip), ref value.x, ref value.y, minValue, maxValue); // Draw the slider.
      property.vector2Value = value; // Set the value.
      EditorGUI.EndProperty(); // End the property.
    }

    /// <summary>
    /// A version of the attribute GUI for Vector2Int properties.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyVector2Int(Rect position, SerializedProperty property, GUIContent label)
    {
      // Note: Unity EditorGUI is very buggy and boilerplate. There existed a bug that caused the label
      // to change text every time an EditorGUI function was called, even though label never got passed.
      // Please bear with the poorly created code in an attempt to make the MinMaxSlider actually have numbers.

      Rect fullPos = position;
      string text = label.text;
      string tooltip = label.tooltip;
      EditorGUI.BeginProperty(position, label, property); // Begin the Editor Property.
      Vector2 value = property.vector2IntValue; // Make a copy of the vector value.

      EditorGUI.indentLevel += 3;
      position.y += 20.0f;
      position.height = 20.0f;
      EditorGUI.PrefixLabel(position, new GUIContent("Min"));

      position.x += 30.0f;
      position.width = 100.0f;
      EditorGUI.BeginChangeCheck();
      value.x = EditorGUI.FloatField(position, value.x);

      position.x += 80.0f;
      position.width = fullPos.width;
      EditorGUI.PrefixLabel(position, new GUIContent("Max"));

      position.x += 30.0f;
      position.width = 100.0f;
      value.y = EditorGUI.FloatField(position, value.y);

      // Finish clamping values.
      if (EditorGUI.EndChangeCheck())
      {
        value.x = Mathf.Clamp((int)value.x, minValue, maxValue);
        value.y = Mathf.Clamp((int)value.y, minValue, maxValue);
        value.x = Mathf.Clamp((int)value.x, minValue, value.y);
        value.y = Mathf.Clamp((int)value.y, value.x, maxValue);
      }

      position = fullPos;
      EditorGUI.indentLevel -= 3;
      EditorGUI.MinMaxSlider(fullPos, new GUIContent(text, tooltip), ref value.x, ref value.y, minValue, maxValue); // Draw the slider.
      property.vector2IntValue = new Vector2Int((int)value.x, (int)value.y); // Set the value.
      EditorGUI.EndProperty(); // End the property.
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      ValueRangeAttribute ValueAttribute = this.attribute as ValueRangeAttribute;

      if (!ValueAttribute.isValid)
        EditorGUI.PropertyField(position, property, label);
      else
      {
        minValue = ValueAttribute.min;
        maxValue = ValueAttribute.max;

        switch (property.propertyType)
        {
          case SerializedPropertyType.Integer:
            OnPropertyInt(position, property, label);
            return;
          case SerializedPropertyType.Float:
            OnPropertyFloat(position, property, label);
            return;
          case SerializedPropertyType.Vector2:
            OnPropertyVector2(position, property, label);
            return;
          case SerializedPropertyType.Vector2Int:
            OnPropertyVector2Int(position, property, label);
            return;
          default:
            EditorGUI.PropertyField(position, property, label);
            return;
        }
      }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      if (property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector2Int)
        return base.GetPropertyHeight(property, label) + 25.0f;

      return base.GetPropertyHeight(property, label);
    }
  }
#endif
}