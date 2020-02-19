using UnityEngine;
using ThirteenPixels.Soda;

public class WASDInput : MonoBehaviour
{
	[SerializeField] private GlobalVector2 input;
	Vector2 oldwasdInput, wasdInput;

#if UNITY_EDITOR
	private void Update()
	{
		/// Я не использую GetAxis потому что у него есть эффект затухания, когда клавиша отпущена, а мне нужен мгновенный переход в 0
		/// Это лютый говнокод, но основное перемещение реализовано через наэкранный джойстик
		#region Down
		if (Input.GetKeyDown(KeyCode.A))
		{
			wasdInput.x = -1;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			wasdInput.x = 1;
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			wasdInput.y = 1;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			wasdInput.y = -1;
		}
        #endregion
        #region Up
        if (Input.GetKeyUp(KeyCode.A))
		{
			if(Input.GetKey(KeyCode.D))
				wasdInput.x = 1;
			else
				wasdInput.x = 0;
		}
		if (Input.GetKeyUp(KeyCode.D))
		{
			if (Input.GetKey(KeyCode.A))
				wasdInput.x = -1;
			else
				wasdInput.x = 0;
		}
		if (Input.GetKeyUp(KeyCode.W))
		{
			if (Input.GetKey(KeyCode.S))
				wasdInput.y = -1;
			else
				wasdInput.y = 0;
		}
		if (Input.GetKeyUp(KeyCode.S))
		{
			if (Input.GetKey(KeyCode.W))
				wasdInput.y = 1;
			else
				wasdInput.y = 0;
		}
        #endregion

        if (wasdInput != oldwasdInput)
		{
			input.value = oldwasdInput = wasdInput;
		}
	}
#endif
}
