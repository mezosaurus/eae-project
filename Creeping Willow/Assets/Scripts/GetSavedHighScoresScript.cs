using UnityEngine;
using System.Collections;

public static class GetSavedHighScoresScript {

	public static string[] LoadStalkSurvivorNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Evan_Level1_Survival_name_" + i, "");
		}

		return names;
	}

	public static int[] LoadStalkSurvivorScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Evan_Level1_Survival_score_" + i, 0);
		}
		
		return scores;
	}

	public static string[] LoadStalkFeastNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Evan_Level1_Feast_name_" + i, "");
		}
		
		return names;
	}

	public static int[] LoadStalkFeastScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Evan_Level1_Feast_score_" + i, 0);
		}
		
		return scores;
	}

	public static string[] LoadStalkMarkedNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Evan_Level1_Marked_name_" + i, "");
		}
		
		return names;
	}

	public static int[] LoadStalkMarkedScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Evan_Level1_Marked_score_" + i, 0);
		}
		
		return scores;
	}





	public static string[] LoadQuadrantsSurvivorNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Quadrants_Survival_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadQuadrantsSurvivorScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Quadrants_Survival_score_" + i, 0);
		}
		
		return scores;
	}
	
	public static string[] LoadQuadrantsFeastNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Quadrants_Feast_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadQuadrantsFeastScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Quadrants_Feast_score_" + i, 0);
		}
		
		return scores;
	}
	
	public static string[] LoadQuadrantsMarkedNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Quadrants_Marked_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadQuadrantsMarkedScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Quadrants_Marked_score_" + i, 0);
		}
		
		return scores;
	}





	public static string[] LoadBridgeSurvivorNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Bridge_Level_Survival_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadBridgeSurvivorScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Bridge_Level_Survival_score_" + i, 0);
		}
		
		return scores;
	}
	
	public static string[] LoadBridgeFeastNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Bridge_Level_Feast_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadBridgeFeastScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Bridge_Level_Feast_score_" + i, 0);
		}
		
		return scores;
	}
	
	public static string[] LoadBridgeMarkedNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Bridge_Level_Marked_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadBridgeMarkedScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Bridge_Level_Marked_score_" + i, 0);
		}
		
		return scores;
	}





	public static string[] LoadMazeSurvivorNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Maze_Level_Survival_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadMazeSurvivorScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Maze_Level_Survival_score_" + i, 0);
		}
		
		return scores;
	}
	
	public static string[] LoadMazeFeastNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Maze_Level_Feast_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadMazeFeastScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Maze_Level_Feast_score_" + i, 0);
		}
		
		return scores;
	}
	
	public static string[] LoadMazeMarkedNames()
	{
		string[] names = new string[5];
		
		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString("Maze_Level_Marked_name_" + i, "");
		}
		
		return names;
	}
	
	public static int[] LoadMazeMarkedScores()
	{
		int[] scores = new int[5];
		
		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt("Maze_Level_Marked_score_" + i, 0);
		}
		
		return scores;
	}

}
