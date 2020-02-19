using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays a string using a UnityEngine.UI.Text.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display String")]
[RequireComponent(typeof(Text))]
public class UIDisplayString : MonoBehaviour
{
    private Text text;
    [SerializeField]
    private ScopedString stringValue = default;


    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        stringValue.onChangeValue.AddResponseAndInvoke(DisplayString);
    }

    private void OnDisable()
    {
        stringValue.onChangeValue.RemoveResponse(DisplayString);
    }

    private void DisplayString(string s)
    {
        text.text = s;
    }
}
