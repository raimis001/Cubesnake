using UnityEngine;

namespace Assets.Scripts
{
	public static class InputManager
	{
		public static SnakeDirection Direction()
		{
			if (Input.GetAxis("Vertical") > 0.1f) return SnakeDirection.Forward;
			if (Input.GetAxis("Vertical") < -0.1f) return SnakeDirection.Back;
			if (Input.GetAxis("Horizontal") > 0.1f) return SnakeDirection.Right;
			if (Input.GetAxis("Horizontal") < -0.1f) return SnakeDirection.Left;

			return SnakeDirection.None;
		}

		public static float Zoom()
		{
			return Input.mouseScrollDelta.y;
		}
	}
}
