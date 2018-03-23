using UnityEngine;

namespace Assets.Scripts.Ui
{
	public class ActiveStateToggler : MonoBehaviour {

		public void ToggleActive () {
			gameObject.SetActive (!gameObject.activeSelf);
		}
	}
}
