using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays a float using a UnityEngine.UI.Image by setting its fillAmount.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display Float as Image Fill")]
[RequireComponent(typeof(Image))]
public class UIDisplayFloatAsImageFill : MonoBehaviour
{
    private Image image;
    [SerializeField]
    private ScopedFloat number = default;
    [SerializeField]
    private ScopedFloat maximum = default;


    private void Reset()
    {
        maximum = new ScopedFloat(1);
    }

    private void Awake()
    {
        image = GetComponent<Image>();
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
        image.fillAmount = number / maximum.value;
    }
}
