using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckDirectionCode : MonoBehaviour
{
    public Button upButton;

    public Button downButton;

    public Button leftButton;

    public Button rightButton;

    private const string correctCode = "312131303";

    private string enteredCode = "";

    public Text textBox;
    // Start is called before the first frame update
    void Start()
    {
        upButton.onClick.AddListener(UpOnClick);
        downButton.onClick.AddListener(DownOnClick);
        leftButton.onClick.AddListener(LeftOnClick);
        rightButton.onClick.AddListener(RightOnClick);
    }

    // Update is called once per frame
    void UpOnClick()
    {
        enteredCode += "0";
        if (enteredCode.Length == 9) {
            if (correctCode.Equals(enteredCode)) {
                textBox.text = "You passed the level";
            } else {
                textBox.text = "You entered wrong code, try again.";
                enteredCode = "";
            }
        } else {
            textBox.text = 9 - enteredCode.Length + " directions left";
        }
    }

        void DownOnClick()
    {
        enteredCode += "1";
        if (enteredCode.Length == 9) {
            if (correctCode.Equals(enteredCode)) {
                textBox.text = "You passed the level";
            } else {
                textBox.text = "You entered wrong code, try again.";
                enteredCode = "";
            }
        } else {
            textBox.text = 9 - enteredCode.Length + " directions left";
        }
    }

        void LeftOnClick()
    {
        enteredCode += "2";
        if (enteredCode.Length == 9) {
            if (correctCode.Equals(enteredCode)) {
                textBox.text = "You passed the level";
            } else {
                textBox.text = "You entered wrong code, try again.";
                enteredCode = "";
            }
        } else {
            textBox.text = 9 - enteredCode.Length + " directions left";
        }
    }

        void RightOnClick()
    {
        enteredCode += "3";
        if (enteredCode.Length == 9) {
            if (correctCode.Equals(enteredCode)) {
                textBox.text = "You passed the level";
            } else {
                textBox.text = "You entered wrong code, try again.";
                enteredCode = "";
            }
        } else {
            textBox.text = 9 - enteredCode.Length + " directions left";
        }
    }
}
