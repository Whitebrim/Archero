using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays a float using a UnityEngine.UI.Text.
/// Offers multiple options for rounding the number for display.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display Float as Text")]
[RequireComponent(typeof(Text))]
public class UIDisplayFloatAsText : MonoBehaviour
{
    public enum RoundingMethod
    {
        Floor, Round, Ceil
    }

    private Text text;
    [SerializeField]
    private ScopedFloat number = default;
    [Tooltip("The amount of digits displayed from the fractional part of the float number, if there is any.")]
    [Range(0, 12)]
    public byte fractionalPartLength = 2;
    public RoundingMethod roundingMethod;


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

    private void DisplayNumber(float number)
    {
        // text.text = number.ToString("n" + fractionalPartLength);
        var factor = Mathf.RoundToInt(Mathf.Pow(10, fractionalPartLength));
        number *= factor;
        switch(roundingMethod)
        {
            case RoundingMethod.Floor:
                number = Mathf.Floor(number);
                break;
            case RoundingMethod.Ceil:
                number = Mathf.Ceil(number);
                break;
            default:
                number = Mathf.Round(number);
                break;
        }
        number /= factor;
        text.text = number + "";
    }
}
