using UnityEngine;
using System.Collections;

public class ShowDialog : MonoBehaviour
{
     // 200x300 px window will apear in the center of the screen.
     private Rect windowRect = new Rect ((Screen.width - 200)/2, (Screen.height - 300)/2, 200, 300);
     // Only show it if needed.
     private bool show = false;
    private string text = "You reached the gold.";

    void OnGUI () 
    {
        if(show)
            windowRect = GUI.Window (0, windowRect, DialogWindow, "Game Over");
    }

    // This is the actual window.
    void DialogWindow (int windowID)
    {
        float y = 20;
        GUI.Label(new Rect(5,y, windowRect.width, 20), text);

        if(GUI.Button(new Rect(5,y, windowRect.width - 10, 20), "OK"))
        {
           show = false;
        }
    }

    // To open the dialogue from outside of the script.
    public void Open(string dialogText)
    {
        show = true;
        text = dialogText;
    }
}
