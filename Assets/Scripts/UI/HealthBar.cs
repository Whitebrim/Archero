using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] Image bar;
	[SerializeField] Entity entity;
	private void Update() // Можно написать лучше структуру, которая будет работать на событиях по изменению hp
	{
		bar.fillAmount = Mathf.Lerp(bar.fillAmount, entity.Hp / entity.MaxHp, 0.2f);
	}
}
