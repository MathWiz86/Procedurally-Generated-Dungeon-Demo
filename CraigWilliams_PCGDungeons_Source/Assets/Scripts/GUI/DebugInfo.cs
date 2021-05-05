using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ImGuiNET;

public class DebugInfo : MonoBehaviour
{

  const int savedValues = 480;
  float[] fps = new float[savedValues];
  int currentIndex = 0;
  float average = 0;

  void OnDevLayout()
  {
    float framerate = 1.0f / Time.deltaTime;
    average -= fps[currentIndex] / savedValues;
    fps[currentIndex++] = framerate;
    if (currentIndex >= savedValues)
      currentIndex = 0;
    average += framerate / savedValues;

    ImGui.PlotLines("Frame Graph: ", ref fps[0], savedValues, currentIndex, "average: " + average, 0.0f, 300.0f, new Vector2(0, 80.0f));

    string s = "Frametime: " + Time.deltaTime;
    ImGui.Text(s);
  }

  private void OnEnable()
  {
    for (int i = 0; i < savedValues; i++)
      fps[i] = 0;
    UIManager.RegisterDevCallback(OnDevLayout, 10);
  }

  private void Start()
  {
  }
}
