using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
	[RequireComponent(typeof(Text))]
	public class ShowSliderValue : MonoBehaviour
	{
		public void UpdateLabel (float value)
		{
			Text lbl = GetComponent<Text>();
			if (lbl != null)
				lbl.text = Mathf.RoundToInt (value * 100) + "%";
		}
	}
}
