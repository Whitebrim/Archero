using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays an int using a UnityEngine.UI.Image by setting its fillAmount.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display Int as Image Fill")]
[RequireComponent(typeof(Image))]
public class UIDisplayIntAsImageFill : MonoBehaviour
{
    private Image image;
    [SerializeField]
    private ScopedInt number = default;
    [SerializeField]
    private ScopedInt maximum = default;


    private void Reset()
    {
        maximum = new ScopedInt(1);
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

    private void DisplayNumber(int number)
    {
        image.fillAmount = number / (float)maximum.value;
    }
}
