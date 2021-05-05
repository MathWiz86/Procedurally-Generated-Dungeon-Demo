/**************************************************************************************************/
/*!
\file   ValueSlider.cs
\author Craig Williams
\par    Last Updated
        2021-04-09

\brief
  A file containing implementation of a UI Slider that shows its current value.

\par Bug List

\par References
*/
/**************************************************************************************************/

using PCGDungeon.UnityEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PCGDungeon.UI
{
  /************************************************************************************************/
  /// <summary>
  /// A wrapper around a <see cref="Slider"/> that displays its current value.
  /// </summary>
  public class ValueSlider : MonoBehaviour
  {
    /// <summary>The attached <see cref="Slider"/>.</summary>
    [SerializeField] [ReadOnly(true)] private Slider slider = null;
    /// <summary>The text object to show the value on.</summary>
    [SerializeField] [ReadOnly(true)] private TextMeshProUGUI tmpValue = null;
    /// <summary>The formatting for the value string.</summary>
    [SerializeField] private string valueFormat = "D2";

    private void Awake()
    {
      // Attach a listener for value changing and update the initial display.
      slider.onValueChanged.AddListener(UpdateValue);
      UpdateValue(slider.value);
    }

    /// <summary>
    /// An event function for updating the display of the <see cref="slider"/>'s value.
    /// </summary>
    /// <param name="value">The new slider value.</param>
    private void UpdateValue(float value)
    {
      // Update based on if the value is an int or a float.
      tmpValue.text = slider.wholeNumbers ? ((int)(value)).ToString(valueFormat) : value.ToString(valueFormat);
    }
  }
  /************************************************************************************************/
}