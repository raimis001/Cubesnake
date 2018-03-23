using Simulator.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
	public class CameraControl : MonoBehaviour
	{
		[Range(0,100)]
		public float minZoom = 50;
		[Range(0, 100)]
		public float maxZoom = 90;

		public Camera cam;

		private float zoomDelta;
		void Start()
		{

			if (!cam)
			{
				cam = GetComponent<Camera>();
			}
			if (!cam)
			{
				cam = Camera.main;
			}
		}
		void Update()
		{
			Zoom();

		}

		void Zoom()
		{
			float zoom = InputManager.Zoom();
			if (Mathf.Abs(zoom) < Mathf.Epsilon) return;

			zoomDelta += zoom;
			zoomDelta = Mathf.Clamp(zoomDelta, -5f, 5f);

			RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward,Mathf.Infinity,Snake.maskTerrain);
			if (hits.Length < 1)
			{
				Debug.Log("No terrain");
				return;
			}
			float d = 0;
		
			foreach (RaycastHit h in hits)
			{
				if (!h.collider.gameObject.GetComponent<Terrain>()) continue;
				d = h.distance;
				//Log.r("Terran distance:{0:0.00} zoom:{1}",h.distance, zoom);
			}
			if (zoom > 0 && d < minZoom || zoom < 0 && d > maxZoom) return;

			Vector3 move = InputManager.Zoom() * 1 * transform.forward;
			transform.Translate(move, Space.World);
		}

	}
}
