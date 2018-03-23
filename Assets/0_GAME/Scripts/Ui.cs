using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Simulator.Scripts;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ui {

	public class Ui : MonoBehaviour
	{
		private static Ui instance;

		public Text scoreText;
		public Text movesText;
		public Text levelText;
		public Text bestMovesText;

		public GameObject victoryObject;
		public GameObject looseObject;

		public GameObject hideObject;
		public GameObject CloudButton;

		public PanelManager manager;
		public Animator menuAnimator;

		public AudioMixer masterVolume;
		public AudioMixer sfxVolume;

		public InputField nameEdit;

		private uiUserLine[] userLines;

		public void Awake()
		{
			instance = this;
			userLines = GetComponentsInChildren<uiUserLine>();
			ScoreBoard.OnListChanged += OnScoreList;
		}

		public void Start()
		{
			nameEdit.text = ScoreBoard.PlayerName;

		}

		public void Update()
		{
			if (scoreText) scoreText.text = Snake.Points.ToString();
			if (movesText) movesText.text = Snake.Moves.ToString();
			if (levelText) levelText.text = Snake.CurrentLevel;

			if (Snake.isDead) return;

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Log.r("Main menu:{0}", Snake.isPaused);
				if (!Snake.isPaused)
				{
					CloudButton.SetActive(false);
					Snake.isPaused = true;
					ShowDeadScreen();
				}
				else {
					Snake.isPaused = false;
					manager.CloseCurrent();
				}
			}
			
		}

		public void CloseWindows()
		{
			Snake.isPaused = false;
			manager.CloseCurrent();
		}

		private void OnScoreList()
		{
			for (int i = 0; i < userLines.Length;i++)
			{
				if (ScoreBoard.scores.Count < i+1)
				{
					userLines[i].nameText.text = "- - -";
					userLines[i].scoreText.text = "-";
				} else
				{
					userLines[i].nameText.text = ScoreBoard.scores[i].name;
					userLines[i].scoreText.text = ScoreBoard.scores[i].score.ToString();
				}
			}
		}

		public void MasterVolume(float value)
		{
			masterVolume.SetFloat("Music", value);
		}
		public void SfxVolume(float value)
		{
			sfxVolume.SetFloat("Sfx", value);
		}
		public void RestartGame()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		public void QuitGame()
		{
			Application.Quit();
		}
		public static void DeadScreen()
		{
			instance.hideObject.SetActive(false);
			instance.looseObject.SetActive(true);
			instance.CloudButton.SetActive(false);
			instance.ShowDeadScreen();
		}
		public void ShowDeadScreen()
		{
			bestMovesText.text = "Your best score: " + ScoreBoard.PlayerScore.ToString();
			ScoreBoard.AllScores();
			manager.OpenPanel(menuAnimator);
		}

		public static void VictoryScreen()
		{
			instance.hideObject.SetActive(false);
			instance.victoryObject.SetActive(true);

			instance.CloudButton.SetActive(true);
			instance.ShowDeadScreen();
		}

		public void StartEditName()
		{
			nameEdit.gameObject.SetActive(true);
			nameEdit.Select();
		}

		public void EndeditName()
		{
			PlayerPrefs.SetString("userName", nameEdit.text);

			nameEdit.gameObject.SetActive(false);
			if (Snake.isWin)
			{
				CloudButton.SetActive(true);
			}
		}

		public void SendScore()
		{
			CloudButton.SetActive(false);
			ScoreBoard.SendScore(Snake.Moves);
		}
	}
}
