using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
	[SerializeField] float speed = 1f;

	private Joystick joystick;
	private Transform _transform;


	private void Start()
	{
		joystick = FindObjectOfType<Joystick>();
		_transform = transform;
	}
	private void Update()
	{
		if(joystick.Horizontal != 0)
		{
			var pos = new Vector3(_transform.position.x, 0, -10f);
			pos.x += (joystick.Horizontal * speed * Time.deltaTime);
			_transform.position = pos;

			if (_transform.position.x < -35) _transform.position = new Vector3(-35, 0, -10f);
			if (_transform.position.x > 35) _transform.position = new Vector3(35, 0, -10f);
		}
	}
}
