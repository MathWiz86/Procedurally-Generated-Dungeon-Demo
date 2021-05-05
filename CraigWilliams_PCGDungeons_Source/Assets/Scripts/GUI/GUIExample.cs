using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET; // IMGui widget calls require this include

public class GUIExample : MonoBehaviour
{
    static bool checkboxState = false;
    static int counter = 0;
    static float dragFloat = 0.0f;

    private void Awake() // Use Awake, not Start for registering callbacks
    {
        // Registering a void fn(void) function will cause it to be called
        // when it is its turn to render. The number is the order in which it will
        // appear. Larger numbers will show up later in the UI.
        // User callbacks are for things intended to be used in the output demo.
        UIManager.RegisterUserCallback(OnUserLayout, 20);
        // This will be rendered at the bottom of the window
        UIManager.RegisterUserCallback(OnUserLayoutLate, 999);
        // Dev callbacks will not be called in non-debug builds running outside of the editor.
        UIManager.RegisterDevCallback(OnDevLayout, 5);
    }

    void OnUserLayout()
    {
        if(ImGui.TreeNode("TreeNode"))
        {
            ImGui.Text("Putting all your widgets for a system in a tree node can make the window much cleaner");

            // It's way too easy to forget this line, it's important you dont
            ImGui.TreePop();
        }

        ImGui.Text("Hello!");
        ImGui.Checkbox("I'm a checkbox", ref checkboxState);
        ImGui.DragFloat("Drag Float:", ref dragFloat);

        if (ImGui.Button("Total clicks: " + counter.ToString()))
        {
            counter++;
        }
    }

    void OnUserLayoutLate()
    {
        ImGui.Text("I'm at the bottom because my callback has a very large order value.");
    }

    void OnDevLayout()
    {
        ImGui.Text("Bad Estimation At Framerate: " + (1.0f / Time.deltaTime).ToString());
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        // System logic that probably uses the variables controlled by ImGui widgets
    }
}
