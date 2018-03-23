using UnityEngine;
using System.Net;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.game;
using AssemblyCSharp;
using System.Collections.Generic;
using System;
using Assets.Scripts;
using com.shephertz.app42.paas.sdk.csharp.app42Event;

public class ScoreElement
{
	public string name;
	public int score;
	public int rank;
}

public class ScoreBoard : MonoBehaviour
{
	public const string gameName = "3DSnake";
	public const string prefName = "userName";
	public const string prefScore = "userScore";
	public const int maxInt = 1000000;

	public const string defName = "n00bs";

	public static string PlayerName;
	public static int PlayerScore;

	public static List<ScoreElement> scores = new List<ScoreElement>();

	public delegate void ListChanged();
	public static event ListChanged OnListChanged;

	private static ScoreBoard instance;

	private ScoreBoardService scoreBoardService = null;
	private Constant cons = new Constant();
	private ServiceAPI sp = null;

	private ScoreMaxResponse maxResponse = new ScoreMaxResponse();

	private void Awake()
	{
		instance = this;

		PlayerName = PlayerPrefs.GetString(prefName, defName);

#if UNITY_EDITOR
		ServicePointManager.ServerCertificateValidationCallback = ScoreBoardTest.Validator;
#endif
		sp = new ServiceAPI(cons.apiKey, cons.secretKey);

		App42API.EnableAppStateEventTracking(true);

		GetMaxScore();
		GetAllscores();

	}

	

	public static void SendScore(int score)
	{
		if (!instance) return;
		if (PlayerName == defName) return;

		PlayerScore = score;
		instance.sendScore();
	}

	public static void AllScores()
	{
		if (!instance) return;

		instance.GetAllscores();
	}

	protected void sendScore()
	{
		ScoreBoardService service = sp.BuildScoreBoardService(); // Initializing ScoreBoard Service.
		int score = maxInt - PlayerScore;
		service.SaveUserScore(gameName, PlayerName, score, maxResponse);
	}

	protected void GetMaxScore()
	{
		if (PlayerName == defName) return;
		ScoreBoardService service = sp.BuildScoreBoardService();
		service.GetLowestScoreByUser(gameName, PlayerName, maxResponse);
	}

	protected void GetAllscores()
	{
		ScoreBoardService service = sp.BuildScoreBoardService(); // Initializing ScoreBoard Service.
		service.GetTopRankings(cons.gameName, new ScoreAllResponse());
	}

	public static void ShotEvent()
	{
		if (OnListChanged != null) OnListChanged();
	}
	#region LEVELS

	public static void StartLevel(string level)
	{
		if (!instance) return;

		instance.startLevel(level);
	}

	const string noneLevel = "none";
	private string currentLevel = noneLevel;
	const string activityName = "LevelChange";

	protected void startLevel(string level)
	{
		if (currentLevel != noneLevel)
		{
			endLevel(currentLevel);
		}

		currentLevel = level;

		Dictionary<String, object> properties = new Dictionary<string, object>();
		properties.Add("Name", level);
		properties.Add("State", "Started");
		properties.Add("Score", Snake.Points);

		EventService eventService = sp.BuildEventService();
		eventService.StartActivity(activityName, properties, new UnityCallBack());
	}

	protected void endLevel(string level)
	{
		Dictionary<String, object> properties = new Dictionary<string, object>();
		properties.Add("Name", currentLevel);
		properties.Add("State", "End");
		properties.Add("Score", Snake.Points);

		EventService eventService = sp.BuildEventService();
		eventService.EndActivity(activityName, properties, new UnityCallBack());
	}
	#endregion

	public static void LoseGame()
	{
		if (!instance) return;
		instance.loseGame();
	}
	public static void WinGame()
	{
		if (!instance) return;
		instance.winGame();
	}

	protected void loseGame()
	{
		if (currentLevel != noneLevel)
		{
			endLevel(currentLevel);
		}
		currentLevel = noneLevel;

		String eventName = "loseGame";
		Dictionary<String, object> properties = new Dictionary<string, object>();
		properties.Add("Name", "Lose");
		properties.Add("Score", Snake.Points);

		EventService eventService = sp.BuildEventService();
		eventService.TrackEvent(eventName, properties, new UnityCallBack());   

	}
	protected void winGame()
	{
		if (currentLevel != noneLevel)
		{
			endLevel(currentLevel);
		}
		currentLevel = noneLevel;

		String eventName = "winGame";
		Dictionary<String, object> properties = new Dictionary<string, object>();
		properties.Add("Name", "WIN");
		properties.Add("Score", Snake.Points);

		EventService eventService = sp.BuildEventService();
		eventService.TrackEvent(eventName, properties, new UnityCallBack());

	}

	protected void OnDestroy()
	{
		if (currentLevel != noneLevel)
		{
			endLevel(currentLevel);
		}

		//String eventName = "close";
		//Dictionary<String, object> properties = new Dictionary<string, object>();
		//properties.Add("Name", "close");
		//properties.Add("Score", Snake.Points);

		//EventService eventService = sp.BuildEventService();
		//eventService.TrackEvent(eventName, properties, new UnityCallBack());
	}
}

public class ScoreMaxResponse : App42CallBack
{
	public void OnException(Exception ex)
	{

	}

	public void OnSuccess(object response)
	{
		if (!(response is Game)) return;

		Game game = (Game)response;

		if (game.GetScoreList() == null) return;

		IList<Game.Score> scoreList = game.GetScoreList();
		for (int i = 0; i < scoreList.Count; i++)
		{
			ScoreBoard.PlayerScore = ScoreBoard.maxInt - (int)scoreList[i].GetValue();
			//Debug.Log("UserName is  : " + scoreList[i].GetUserName());
			//Debug.Log("CreatedOn is  : " + scoreList[i].GetCreatedOn());
			//Debug.Log("ScoreId is  : " + scoreList[i].GetScoreId());
			//Debug.Log("Value is  : " + scoreList[i].GetValue());
		}
		PlayerPrefs.SetInt(ScoreBoard.prefScore, ScoreBoard.PlayerScore);
		ScoreBoard.AllScores();

	}
}

public class ScoreAllResponse : App42CallBack
{
	public void OnException(Exception ex)
	{

	}

	public void OnSuccess(object response)
	{
		if (!(response is Game)) return;

		Game game = (Game)response;

		if (game.GetScoreList() == null) return;

		ScoreBoard.scores.Clear();

		IList<Game.Score> scoreList = game.GetScoreList();
		for (int i = 0; i < scoreList.Count; i++)
		{
			ScoreElement element = new ScoreElement() { name = scoreList[i].GetUserName(), score = ScoreBoard.maxInt - (int)scoreList[i].GetValue(), rank = i + 1 };

			ScoreBoard.scores.Add(element);

			//Debug.Log("UserName is  : " + scoreList[i].GetUserName());
			//Debug.Log("CreatedOn is  : " + scoreList[i].GetCreatedOn());
			//Debug.Log("ScoreId is  : " + scoreList[i].GetScoreId());
			//Debug.Log("Value is  : " + scoreList[i].GetValue());
			if (i > 10) return;
		}

		ScoreBoard.ShotEvent();
	}
}
public class UnityCallBack : App42CallBack
{
	public void OnSuccess(object response)
	{
		App42Response app42Response = (App42Response)response;
		App42Log.Console("App42Response Is : " + app42Response);
	}
	public void OnException(Exception e)
	{
		App42Log.Console("Exception : " + e);
	}
}


