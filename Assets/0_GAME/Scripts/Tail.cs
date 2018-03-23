using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class Tail : MonoBehaviour
	{
		public Transform headTransform;
		public Tail tail;

		public Position position;

		private Position oldPos;

		private void Start()
		{
			//transform.localPosition = new Vector3(0, 0, 0.25f * position.z);
		}


		public void MoveTo(Position pos, bool imediatly = false)
		{
			oldPos = position;

			if (!imediatly && tail) tail.MoveTo(position);

			position = pos;

			if (imediatly)
			{
				transform.localPosition = position.Pos();
				return;
			}
			StartCoroutine(Move());
		}

		IEnumerator Move()
		{
			float time = 0;

			Vector3 dest = position.Pos();
			Vector3 pos = transform.localPosition;

			Vector3 rot = headTransform.eulerAngles;
			float rotDest = position.DirectionAngle(position.GetDir(oldPos));

			while (time < Snake.MoveTime)
			{
				time += Time.deltaTime;
				float p = time / Snake.MoveTime;

				float a = Mathf.LerpAngle(rot.y, rotDest, p);
				Vector3 pp = Vector3.Lerp(pos, dest, p);


				transform.localPosition = pp;
				headTransform.localEulerAngles = new Vector3(pos.x, a, pos.z);

				yield return null;
			}

			transform.localPosition = dest;
			headTransform.localEulerAngles = new Vector3(pos.x, rotDest, pos.z);

		}

		public void ShowTail(bool show)
		{
			headTransform.gameObject.SetActive(show);
		}
	}
}
