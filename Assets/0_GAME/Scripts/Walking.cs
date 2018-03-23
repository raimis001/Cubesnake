using UnityEngine;
using System.Collections;

public class Walking : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		BoxCollider collider = GetComponent<BoxCollider>();
		if (!collider) return;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
	}




}
