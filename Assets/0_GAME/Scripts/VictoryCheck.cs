using UnityEngine;
using System.Collections;
namespace Assets.Scripts
{
	public class VictoryCheck : MonoBehaviour
	{
		public int checkScore = 45;
		public GameObject actors;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Snake.Points >= checkScore && !actors.activeSelf) actors.SetActive(true);
		}
	}
}
