using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckCodeManager : MonoBehaviour
{
    public InputField field;
    public Text textBox;

    public void CheckText(string rightCode) {
        if (field.text == rightCode) {
            textBox.text = "You passed the level";
        } else {
            textBox.text = "Wrong code, try again";
        }
    }
}
