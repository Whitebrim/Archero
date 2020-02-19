using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays an int using a UnityEngine.UI.Text.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display Int as Text")]
[RequireComponent(typeof(Text))]
public class UIDisplayIntAsText : MonoBehaviour
{
    private Text text;
    [SerializeField]
    private ScopedInt number = default;


    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        number.onChangeValue.AddResponseAndInvoke(DisplayNumber);
    }

    private void OnDisable()
    {
        number.onChangeValue.RemoveResponse(DisplayNumber);
    }

    private void DisplayNumber(int number)
    {
        text.text = number + "";
    }
}
