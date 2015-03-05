using UnityEngine;
using System.Collections;

public class TrainCarScript : MonoBehaviour
{
	public float frequency = 15.0f, amplitude = 0.1f;
	protected float degrees, baseY;
	protected Transform _transform;

	void Awake()
	{
		_transform = transform;
		degrees = 0;
		baseY = transform.position.y;
	}
	
	void Update()
	{
		_transform.position = new Vector3(_transform.position.x, baseY + amplitude * Mathf.Sin(degrees), _transform.position.z);
		degrees += frequency * Time.deltaTime;
	}
}
