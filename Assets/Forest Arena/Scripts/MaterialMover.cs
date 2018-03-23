using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMover : MonoBehaviour
{
	[Range(0,2)]
	public float scrollSpeed = 0.5F;

	[Range(0, 1)]
	public float pong = 0.3f;

	[Range(0, 1)]
	public float pongSpeed = 0.3f;

	private Renderer rend;

	public int materialIndex;
	void Start()
	{
		rend = GetComponent<Renderer>();
	}
	void Update()
	{
		float offset = Time.time * scrollSpeed;
		float offsetX = Mathf.PingPong(Time.time * pongSpeed, pong);
		rend.materials[materialIndex].SetTextureOffset("_MainTex", new Vector2(offsetX, offset));

	}
}