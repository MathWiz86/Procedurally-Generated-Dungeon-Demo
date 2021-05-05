using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ImGuiNET;

public class UIManager : MonoBehaviour
{
  // WINDOW CONTROL //
  private bool _showUserWindow = true;
  private bool _showDevWindow = false;
  private bool _showUI = true;
  private GameObject _canvas = null;

  // CALLBACK MANAGEMENT //
  public delegate void LayoutCallback();
  private static event LayoutCallback _userLayout;
  private static event LayoutCallback _devLayout;

  // When callbacks are added through the public functions, they are
  // added to these two lists, and on start, they are added to the actual
  // events in an order determined by their key.
  private static List<KeyValuePair<int, LayoutCallback>> _userCallbacks = new List<KeyValuePair<int, LayoutCallback>>();
  private static List<KeyValuePair<int, LayoutCallback>> _devCallbacks = new List<KeyValuePair<int, LayoutCallback>>();
  private static bool _startCalled = false;

  private void Awake()
  {
    _userCallbacks = new List<KeyValuePair<int, LayoutCallback>>();
    _devCallbacks = new List<KeyValuePair<int, LayoutCallback>>();
    _canvas = GameObject.Find("Canvas");
  }

  void Start()
  {
    if (!_startCalled)
      ImGuiUn.Layout += OnLayout;

    _startCalled = true;
    processLists();

  }
  
  void Update()
  {
    handleInput();
  }

  void OnLayout()
  {
    renderWindows();
  }

  private void renderWindows()
  {
    if (!_showUI)
      return;

    if (_userLayout != null)
    {
      if(ImGui.Begin("Options", ref _showUserWindow))
      {
        _userLayout();
      }
      ImGui.End();
    }
    if (UnityEngine.Debug.isDebugBuild == true && _devLayout != null)
    {
      if(ImGui.Begin("Debug", ref _showDevWindow))
      {
        _devLayout();
      }
      ImGui.End();
    }
  }

  private void handleInput()
  {
    if (Input.GetButtonDown("OpenDebugMenu"))
      _showDevWindow = !_showDevWindow;
    if (Input.GetButtonDown("ToggleUI"))
    {
      _showUI = !_showUI;
      if(_canvas)
        _canvas.SetActive(_showUI);
    }
  }
  // *Call from OnEnable*
  // Register a callback for adding UI to the user window. Order only matters if
  // called before start.
  public static void RegisterUserCallback(LayoutCallback callback, int order)
  {
    if (_startCalled)
    {
      //_userLayout += callback;
      return;
    }

    _userCallbacks.Add(new KeyValuePair<int, LayoutCallback>(order, callback));
  }

  // *Call from OnEnable*
  // Register a callback for adding UI to the dev window. Order only matters if
  // called before start.
  public static void RegisterDevCallback(LayoutCallback callback, int order)
  {
    // If start was already called, just add the callback to the end of the event
    if (_startCalled)
    {
      //_devLayout += callback;
      return;
    }

    _devCallbacks.Add(new KeyValuePair<int, LayoutCallback>(order, callback));
  }

  // To be called on start to add everything in the callback lists to the events.
  // Sorts the lists, adds them to the corresponding events, and destroys them.
  // This could be done so much cleaner if you could pass events.
  private void processLists()
  {
    _userCallbacks.Sort(callbackListCompare);
    foreach (KeyValuePair<int, LayoutCallback> listItem in _userCallbacks)
    {
      _userLayout += listItem.Value;
    }
    _userCallbacks = null;

    _devCallbacks.Sort(callbackListCompare);
    foreach (KeyValuePair<int, LayoutCallback> listItem in _devCallbacks)
    {
      _devLayout += listItem.Value;
    }
    _devCallbacks = null;
  }

  private int callbackListCompare(KeyValuePair<int, LayoutCallback> itemA, KeyValuePair<int, LayoutCallback> itemB)
  {
    if (itemA.Key < itemB.Key)
      return -1;
    if (itemA.Key > itemB.Key)
      return 1;
    return 0;
  }

  // Call on start to test event functions
  private void eventTest()
  {
    print("UIManager eventTest: Should print 0-5 in order.");
    RegisterUserCallback(() => { print(0); }, 0);
    RegisterUserCallback(() => { print(2); }, 2);
    RegisterUserCallback(() => { print(1); }, 1);
    RegisterDevCallback(() => { print(3); }, 3);
    RegisterDevCallback(() => { print(5); }, 5);
    RegisterDevCallback(() => { print(4); }, 4);
    processLists();
    _startCalled = true;
    _userLayout();
    _devLayout();
  }

  // Call on awake or start to test menus
  private void windowTest()
  {
    RegisterDevCallback(() => { ImGui.Text("This is the dev window"); }, 0);
    RegisterUserCallback(() => { ImGui.Text("This is the user window"); }, 0);
  }
}
