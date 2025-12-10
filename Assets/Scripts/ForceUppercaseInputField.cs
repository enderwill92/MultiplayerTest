using TMPro;
using UnityEngine;
using UnityEngine.UI; // Required for InputField

public class ForceUppercaseInputField : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the InputField

    void Start()
    {
        // Get the InputField component if not already assigned
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }

        // Add a listener to the onValueChanged event
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }
    }

    void OnInputFieldValueChanged(string newText)
    {
        // Convert the new text to uppercase
        string upperText = newText.ToUpper();

        // If the text has changed (meaning it was not already uppercase),
        // update the InputField's text to the uppercase version
        if (newText != upperText)
        {
            inputField.text = upperText;
            // Optionally, set the caret position to the end for a better user experience
            inputField.caretPosition = upperText.Length;
        }
    }
}