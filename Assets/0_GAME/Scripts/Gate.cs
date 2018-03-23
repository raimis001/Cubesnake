using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class Gate : MonoBehaviour
	{
		public int openCount;
		[Header("Setup")]
		public Transform gateTransform;
		public float targetY = 0.5f;
		public float openTime = 1f;

		private bool isOpen;
		private bool needOpen;
		private int score;
		public bool testOpen;

		private void Update()
		{
			needOpen = Snake.Points >= openCount;
			score = Snake.Points;
			if (isOpen) return;

			if (testOpen)
			{
				isOpen = true;
				StartCoroutine(OpenGate());
				return;
			}

			if (Snake.Points < openCount) return;

			isOpen = true;
			StartCoroutine(OpenGate());
		}

		IEnumerator OpenGate()
		{
			Vector3 start = gateTransform.localPosition;
			Vector3 target = gateTransform.localPosition;

			float time = 0;

			while (time < openTime)
			{
				target.y = Mathf.Lerp(0, targetY, time / openTime);

				gateTransform.localPosition = target;
				time += Time.deltaTime;
				yield return null;
			}

			gateTransform.localPosition = new Vector3(target.x, targetY, target.z);
		}

	}
}

