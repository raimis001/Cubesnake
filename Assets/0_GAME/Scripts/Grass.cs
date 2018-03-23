using UnityEngine;
using System.Collections;

public class Grass : MonoBehaviour
{

	public bool randomX;
	public bool randomY = true;
	public bool randomZ;

	// Use this for initialization
	void Start()
	{
		float val = Random.Range(0, 360);
		Vector3 rot = transform.localEulerAngles;

		transform.localEulerAngles = new Vector3(randomX ? val :rot.x,randomY ? val : rot.y, randomZ ? val : rot.z);
	}

}
