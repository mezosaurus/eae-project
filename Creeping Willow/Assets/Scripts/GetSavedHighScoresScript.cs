using UnityEngine;
using System.Collections;

public static class GetSavedHighScoresScript {

	public static string[] LoadStalkSurvivorNames()
	{
		string[] names = new string[10];
		
		for( int i = 0; i < 10; i++ )
		{
			names[i] = PlayerPrefs.GetString(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_name_" + i, "");
		}

		return names;
	}

	public static int[] LoadStalkSurvivorScores()
	{
		int[] scores = new int[10];
		
		for( int i = 0; i < 10; i++ )
		{
			scores[i] = PlayerPrefs.GetInt(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_score_" + i, 0);
		}
		
		return scores;
	}

	public static string[] LoadStalkFeastNames()
	{
		string[] names = new string[10];
		
		for( int i = 0; i < 10; i++ )
		{
			names[i] = PlayerPrefs.GetString(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_name_" + i, "");
		}
		
		return names;
	}

	public static int[] LoadStalkFeastScores()
	{
		int[] scores = new int[10];
		
		for( int i = 0; i < 10; i++ )
		{
			scores[i] = PlayerPrefs.GetInt(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_score_" + i, 0);
		}
		
		return scores;
	}

	public static string[] LoadStalkMarkedNames()
	{
		string[] names = new string[10];
		
		for( int i = 0; i < 10; i++ )
		{
			names[i] = PlayerPrefs.GetString(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_name_" + i, "");
		}
		
		return names;
	}

	public static int[] LoadStalkMarkedScores()
	{
		int[] scores = new int[10];
		
		for( int i = 0; i < 10; i++ )
		{
			scores[i] = PlayerPrefs.GetInt(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_score_" + i, 0);
		}
		
		return scores;
	}
}
