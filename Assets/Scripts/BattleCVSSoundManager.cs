using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCVSSoundManager : MonoBehaviour
{
	[SerializeField] AudioClip pointSound = default;
	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void OnPointSound()
	{
		audioSource.PlayOneShot(pointSound);
	}
}
