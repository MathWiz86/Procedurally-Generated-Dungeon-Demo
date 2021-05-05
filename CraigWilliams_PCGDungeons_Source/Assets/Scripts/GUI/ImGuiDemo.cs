using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;

public class ImGuiDemo : MonoBehaviour
{
  private bool _show = false;

  void toggle()
  {
    _show = !_show;
    if(_show)
      ImGuiUn.Layout += OnLayout;
    else
      ImGuiUn.Layout -= OnLayout;
  }

  public void Update()
  {
    if (Input.GetButtonDown("OpenIMGUIDemo"))
    {
      toggle();
    }
  }
  
  void OnDisable()
  {
    if(_show)
      ImGuiUn.Layout -= OnLayout;
  }

  void OnLayout()
  {
    ImGui.ShowMetricsWindow();
    ImGui.ShowDemoWindow();
  }
}
