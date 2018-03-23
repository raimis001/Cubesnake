using UnityEngine;
using Simulator.Scripts;

public class Obsticle : MonoBehaviour
{


	private void OnDrawGizmos()
	{
//		BoxCollider collider = GetComponent<BoxCollider>();
//		if (!collider) return;

//		Vector3 size = Vector3.Scale(collider.size, transform.lossyScale);
//		size = size.RotateAroundPivot(collider.center, transform.eulerAngles);

//		Gizmos.color = Color.cyan;
		//Gizmos.DrawWireCube(transform.position + collider.center, size );
//		Gizmos.DrawWireCube(transform.position + Vector3.Scale(collider.center, transform.lossyScale), size);

		BoxCollider collider = GetComponent<BoxCollider>();
		if (!collider) return;
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);

	}



}
