using UnityEngine;
using System.Collections;

public class Eatable : MonoBehaviour
{

	public int points = 1;

	public GameObject mesh;

	[Header("Effect")]
	public GameObject effect;
	[Range(0,20)]
	public float effectDelay = 5;
	private float effectTime;
	private float effectRandom;

	public GameObject endEffect;


	private bool isDed;
	void Start()
	{
		effectRandom = effectDelay + Random.value * (effectDelay / 3f);
	}

	void Update()
	{
		if (isDed) return;
		effectTime += Time.deltaTime;
		if (effectTime >= effectRandom)
		{
			effectTime = 0;
			effectRandom = effectDelay + Random.value * (effectDelay / 3f);
			if (effect) effect.SetActive(true);
		}
	}


	public void Delete()
	{
		if (isDed) return;
		if (effect) effect.SetActive(false);
		isDed = true;
		if (endEffect) endEffect.SetActive(true);
		mesh.SetActive(false);
		Destroy(gameObject,1.5f);
	}
}
