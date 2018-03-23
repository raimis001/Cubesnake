using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Simulator.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
	public enum SnakeDirection
	{
		None, Forward, Back, Left, Right, Down, Up
	}

	[Serializable]
	public struct Position
	{
		public int x;
		public int y;
		public int z;

		public Vector3 Pos()
		{
			return new Vector3(x, y, z) * Snake.cubeSize + new Vector3(-0.125f, 0.00f, -0.125f);
		}
		public Vector3 Pos(Vector3 delta)
		{
			return Pos() + delta;
		}

		private Vector3 Neibor(SnakeDirection direction)
		{
			switch (direction)
			{
				case SnakeDirection.None:
					return Pos();
				case SnakeDirection.Forward:
					return new Vector3(x, y, z + 1) * Snake.cubeSize;
				case SnakeDirection.Back:
					return new Vector3(x, y, z - 1) * Snake.cubeSize;
				case SnakeDirection.Left:
					return new Vector3(x - 1, y, z) * Snake.cubeSize;
				case SnakeDirection.Right:
					return new Vector3(x + 1, y, z) * Snake.cubeSize;
				case SnakeDirection.Down:
					return new Vector3(x, y - 1, z) * Snake.cubeSize;
				case SnakeDirection.Up:
					return new Vector3(x, y + 1, z) * Snake.cubeSize;
			}

			return Pos();
		}
		public Vector3 Direction(SnakeDirection direction)
		{
			switch (direction)
			{
				case SnakeDirection.None:
					return Vector3.zero;
				case SnakeDirection.Forward:
					return Vector3.forward;
				case SnakeDirection.Back:
					return Vector3.back;
				case SnakeDirection.Left:
					return Vector3.left;
				case SnakeDirection.Right:
					return Vector3.right;
				case SnakeDirection.Down:
					return Vector3.down;
				case SnakeDirection.Up:
					return Vector3.up;
			}

			return Vector3.zero;
		}

		public bool IsObstacle(SnakeDirection direction)
		{
			RaycastHit hit;
			if (Physics.Raycast(Pos(Snake.RayDelta), Direction(direction), out hit, Snake.cubeSize, Snake.maskObstacles))
			{
				Log.r("Direction {0} is obstacle{1}", direction, hit.collider.name);
			}

			return Physics.Raycast(Pos(Snake.RayDelta), Direction(direction), Snake.cubeSize, Snake.maskObstacles);
		}
		public bool IsTerrain(SnakeDirection direction)
		{
			return Physics.Raycast(Pos(Snake.RayDelta), Direction(direction), Snake.cubeSize, Snake.maskTerrain);
		}
		public bool IsTerrain(SnakeDirection neibor, SnakeDirection direction)
		{
			return Physics.Raycast(Neibor(neibor), Direction(direction), Snake.cubeSize, Snake.maskTerrain);
		}

		public bool IsVictory(SnakeDirection direction)
		{
			return Physics.Raycast(Pos(Snake.RayDelta), Direction(direction), Snake.cubeSize, Snake.maskVictory);
		}

		public void AddDirection(SnakeDirection direction)
		{
			switch (direction)
			{
				case SnakeDirection.None:
					return;
				case SnakeDirection.Forward:
					z++;
					break;
				case SnakeDirection.Back:
					z--;
					break;
				case SnakeDirection.Left:
					x--;
					break;
				case SnakeDirection.Right:
					x++;
					break;
				case SnakeDirection.Down:
					y--;
					break;
				case SnakeDirection.Up:
					y++;
					break;
			}
		}

		public SnakeDirection GetDir(Position pos)
		{
			if (pos.z < z) return SnakeDirection.Forward;
			if (pos.z > z) return SnakeDirection.Back;
			if (pos.x < x) return SnakeDirection.Right;
			if (pos.x > x) return SnakeDirection.Left;


			return SnakeDirection.None;
		}

		public float DirectionAngle(SnakeDirection dir)
		{
			switch (dir)
			{
				case SnakeDirection.None:
					return 0;
				case SnakeDirection.Forward:
					return 0;
				case SnakeDirection.Back:
					return 180f;
				case SnakeDirection.Left:
					return 270f;
				case SnakeDirection.Right:
					return 90f;
				case SnakeDirection.Down:
					return 0;
				case SnakeDirection.Up:
					return 0;
			}

			return 0;
		}

		public void DrawRay(SnakeDirection direction)
		{
			Debug.DrawRay(Pos(Snake.RayDelta), Direction(direction), Color.blue, Snake.MoveTime);
		}
	}

	public class Snake : MonoBehaviour
	{
		public static Vector3 RayDelta = new Vector3(0, 0.00f, 0);

		public static bool isPaused;
		public static bool isDead;
		public static bool isWin;

		private static Snake Instance;

		public static int Points = 0;
		public static int Moves = 0;

		public static float MoveTime
		{
			get { return Instance ? Instance.moveTime : 1; }
		}

		public static string CurrentLevel
		{
			get
			{
				if (Points < 10) return "1";
				if (Points < 20) return "2";
				if (Points < 30) return "3";

				return "4";
			}
		}


		internal const float cubeSize = 0.25f;
		internal static int maskTerrain;
		internal static int maskObstacles;
		internal static int maskVictory;

		[Range(0, 2)]
		public float moveTime = 1;

		public Transform headTransform;
		public TextMeshPro pointText;

		public GameObject tailPrefab;
		//public Tail tail;
		public int length = 0;
		private List<Tail> tails = new List<Tail>();

		[Header("Audio")]
		public AudioSource eatSound;
		public AudioSource goSource;
		public AudioSource deadSource;
		public AudioSource victorySource;

		[Header("Setup")]
		public bool stopMove;

		[Header("Effects")]
		public GameObject deadEffect;
		public GameObject victoryEffect;

		internal Position position;

		private SnakeDirection direction = SnakeDirection.None;
		private bool moving;

		void Awake()
		{
			Instance = this;
		}

		void Start()
		{
			isPaused = false;
			isDead = false;
			isWin = false;
			Points = 0;
			Moves = 0;

			maskTerrain = LayerMask.GetMask("Terrain");
			maskObstacles = LayerMask.GetMask("Obstacles");
			maskVictory = LayerMask.GetMask("Victory");

			GameObject parent = transform.parent.gameObject;
			tails = parent.GetComponentsInChildren<Tail>().ToList();

			position.x = (int)(parent.transform.position.x / 0.25f);
			position.z = (int)(parent.transform.position.z / 0.25f);

			parent.transform.position = Vector3.zero;
			transform.localPosition = position.Pos();

			ShowSnake();
			ScoreBoard.StartLevel(CurrentLevel);
		}

		void Update()
		{

			if (pointText) pointText.text = Points.ToString();

			if (isPaused || isDead) return;

			if (moving) return;
			SnakeDirection d = InputManager.Direction();
			if (stopMove && d == SnakeDirection.None)
			{
				return;
			}

			if (d != SnakeDirection.None)
			{
				direction = d;
			}

			if (position.IsObstacle(direction))
			{
				direction = SnakeDirection.None;
				StartCoroutine(Dead());
				return;
			}

			if (position.IsVictory(direction))
			{
				direction = SnakeDirection.None;
				StartCoroutine(Victory());
				return;
			}

			if (direction != SnakeDirection.None)
			{
				if (tails.Count > 0) tails[0].MoveTo(position);
			}

			if (position.IsTerrain(direction))
			{
				position.AddDirection(SnakeDirection.Up);
			}
			else if (position.IsTerrain(SnakeDirection.Down))
			{
				position.AddDirection(direction);
			}
			else if (!position.IsTerrain(direction, SnakeDirection.Down))
			{
				position.AddDirection(SnakeDirection.Down);
			}
			else
			{
				position.AddDirection(direction);
			}

			StartCoroutine(Move());
		}

		private void ShowSnake()
		{
			for (int i = 0; i < tails.Count; i++)
			{
				tails[i].ShowTail(i < length);
			}
		}

		IEnumerator Move()
		{
			while (moving)
			{
				yield return null;
			}

			if (goSource && UnityEngine.Random.Range(0, 5) == 1) goSource.Play();

			moving = true;
			float time = 0;

			Vector3 dest = position.Pos();
			Vector3 pos = transform.localPosition;
			if (Vector3.Distance(pos, dest) > 0.1f) Moves++;

			Vector3 rot = headTransform.eulerAngles;
			float rotDest = position.DirectionAngle(direction);
			position.DrawRay(direction);

			while (time < moveTime)
			{
				time += Time.deltaTime;
				float p = time / moveTime;

				float a = Mathf.LerpAngle(rot.y, rotDest, p);
				Vector3 pp = Vector3.Lerp(pos, dest, p);


				transform.localPosition = pp;
				headTransform.localEulerAngles = new Vector3(rot.x, a, rot.z);

				yield return null;
			}

			transform.localPosition = dest;
			headTransform.localEulerAngles = new Vector3(rot.x, rotDest, rot.z);

			moving = false;
		}


		public void Eat(Eatable eat)
		{
			if (eatSound) eatSound.Play();

			string oldLevel = CurrentLevel;
			Points += eat.points;

			if (CurrentLevel != oldLevel)
			{
				ScoreBoard.StartLevel(CurrentLevel);
			}

			length++;
			eat.Delete();

			Tail tail = Instantiate(tailPrefab).GetComponent<Tail>();
			tail.transform.SetParent(transform.parent);
			tail.position = position;

			moveTime -= 0.003f;

			Position p = position;
			if (tails.Count > 0)
			{
				tails[tails.Count - 1].tail = tail;
				p = tails[tails.Count - 1].position;
			}

			tail.MoveTo(p, true);

			switch (direction)
			{
				case SnakeDirection.Forward:
					tail.position.z = tail.position.z - (tails.Count + 1);
					break;
				case SnakeDirection.Back:
					tail.position.z = tail.position.z + (tails.Count + 1);
					break;
				case SnakeDirection.Left:
					tail.position.x = tail.position.x + (tails.Count + 1);
					break;
			}


			tails.Add(tail);

			//ShowSnake();
		}

		private void OnTriggerEnter(Collider other)
		{
			Eatable eat = other.GetComponent<Eatable>();
			if (!eat) return;

			Eat(eat);

		}

		IEnumerator Dead()
		{
			//yield break;
			isPaused = true;
			isDead = true;

			if (deadEffect) deadEffect.SetActive(true);
			if (deadSource) deadSource.Play();

			ScoreBoard.LoseGame();
			yield return new WaitForSeconds(2);
			Ui.Ui.DeadScreen();
		}
		IEnumerator Victory()
		{
			isPaused = true;
			isDead = true;
			isWin = true;

			if (victorySource) victorySource.Play();
			if (victoryEffect) victoryEffect.SetActive(true);

			ScoreBoard.WinGame();
			yield return new WaitForSeconds(2);
			Ui.Ui.VictoryScreen();
		}

	}
}
