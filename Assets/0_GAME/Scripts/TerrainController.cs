using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour
{

	public Vector3 startDelta;

	void Start()
	{
		transform.position = startDelta;
	}


}
