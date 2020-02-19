using UnityEngine;
using ThirteenPixels.Soda;

public class MovementSystem : MonoBehaviour
{
	[SerializeField] private GlobalVector2 input;
	private Vector3 direction;
	Entity thisBody;
	Rigidbody rigidbody;

	private void OnEnable()
	{
		input.onChange.AddResponse(Move);
	}

	private void OnDisable()
	{
		input.onChange.RemoveResponse(Move);
	}

	private void Awake()
	{
		if(rigidbody==null)
			rigidbody = GetComponent<Rigidbody>();
		if(thisBody==null)
			thisBody = GetComponent<Entity>();
	}

	/// <summary>
	/// Called when Input Vector2 is changed
	/// </summary>
	/// <param name="input"></param>
	private void Move(Vector2 input)
	{
		float temp = Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y));
		input.Normalize();
		input *= temp;
		direction = new Vector3(input.x, 0, input.y);
	}

	private void FixedUpdate()
	{
		transform.LookAt(transform.position + direction);
#if UNITY_EDITOR
		Debug.DrawRay(transform.position, direction * 3, Color.red);
#endif
		rigidbody.velocity = direction * thisBody.Speed;
		if (direction == Vector3.zero)
			rigidbody.angularVelocity = direction;
	}
}
