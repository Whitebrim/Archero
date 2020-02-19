using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays an int using a UnityEngine.UI.Image by setting its fillAmount.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display Int with Maximum as Image Fill")]
[RequireComponent(typeof(Image))]
public class UIDisplayIntWithMaximumAsImageFill : MonoBehaviour
{
    private Image image;
    [SerializeField]
    private GlobalIntWithMaximum number = default;


    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        number.onChange.AddResponseAndInvoke(DisplayNumber);
    }

    private void OnDisable()
    {
        number.onChange.RemoveResponse(DisplayNumber);
    }

    private void DisplayNumber(int number)
    {
        image.fillAmount = number / (float)this.number.maximum;
    }
}
