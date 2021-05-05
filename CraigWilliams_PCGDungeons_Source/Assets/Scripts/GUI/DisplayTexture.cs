using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ImGuiNET;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class DisplayTexture : MonoBehaviour
{
  static Image _myImage;
  static RectTransform _myRectTransform;

  static private int _screenSizeMaxDefault = 256;

  public static void SetVisible(bool visible)
  {
    _myImage.enabled = visible;
  }

  public static void SetTexture(Texture2D tex)
  {
    SetTexture(tex, _screenSizeMaxDefault);
  }

  public static void SetTexture(Texture2D tex, int screenSizeMax)
  {
    InitCheck();

    _myImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    // Scale textures under Max x Max to the nearest pixel multiple between Max/2 and Max, otherwise scale to Max px.
    int scale = screenSizeMax / tex.width;
    _myRectTransform.sizeDelta = new Vector2(tex.width * scale, tex.height * scale);
    if (scale == 0)
      _myRectTransform.sizeDelta = new Vector2(screenSizeMax, screenSizeMax * tex.height/tex.width);
  }

  private void OnEnable()
  {
    _myImage = GetComponent<Image>();
    if (!_myImage)
      Debug.LogError("DisplayTexture: Could not find attached image.");
    _myRectTransform = GetComponent<RectTransform>();
    if (!_myRectTransform)
      Debug.LogError("DisplayTexture: Could not find attached RectTransform.");

    UIManager.RegisterDevCallback(OnDevLayout, 40);
  }

  private void OnDisable()
  {
    _myImage = null;
    _myRectTransform = null;
  }

  private static void OnDevLayout()
  {
    if (ImGui.TreeNode("DisplayTexture"))
    {
      bool visible = _myImage.enabled;
      if(ImGui.Checkbox("Display Heightmap", ref visible))
      {
        _myImage.enabled = visible;
      }
      ImGui.DragInt("Screen Size Max Default", ref _screenSizeMaxDefault);
      ImGui.TreePop();
    }
  }

  private static void InitCheck()
  {
    if (!_myImage || !_myRectTransform)
      Debug.LogWarning("DisplayTexture invalid. Make sure it is attached to a UI object with an Image and RectTransform.");
  }
}
