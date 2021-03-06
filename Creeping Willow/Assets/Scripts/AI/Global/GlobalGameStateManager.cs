﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SceneEnum represents a level for each name.
/// This name must match the name of the scene
/// in the Assets->Scenes folder.
/// </summary>
public enum SceneEnum
{
	QUIT = -1,
	MAINMENU = 0,
	LEVELSELECT = 1,
	OPTIONS = 2,
	HIGHSCORES = 3,
	Evan_Level1 = 10,
	Kevin_Level1 = 11,
	PossessionTestScene = 12,
	TestScene = 13,
	TreeTestScene = 14,
	PrototypeScene = 15,
};

public enum LevelState
{
	BEGINLEVEL,
	INLEVEL,
	ENDOFLEVEL_VICTORY,
	ENDOFLEVEL_DEFEAT,
};

public enum PosessionState
{
	EXORCISABLE,
	NON_EXORCISABLE,
}

public static class GlobalGameStateManager
{
	public static readonly float originalWidth = 1920;
	public static readonly float originalHeight = 1080;
	public static LevelState LevelState = LevelState.BEGINLEVEL;
	public static PosessionState PosessionState = PosessionState.EXORCISABLE;
	public static SceneEnum CurrentScene = 0;
    public static GameObject PanicTree;
    public static bool PlayedJohnny = false;
    public static float AxeManMinigameModifier1 = 1f;
    public static float AxeManMinigameModifier2 = 1f;

	public static int[] highscores = new int[10];
	public static string[] playerNames = new string[10];

    public static IDictionary<NPCSkinType, NPCData> NPCData = PopulateNPCData();

    public static float SoulConsumedTimer = 0f;

	public static Matrix4x4 PrepareMatrix ()
	{
		Vector2 ratio = new Vector2 (Screen.width / originalWidth, Screen.height / originalHeight);
		Matrix4x4 guiMatrix = Matrix4x4.identity;
		guiMatrix.SetTRS (new Vector3 (1, 1, 1), Quaternion.identity, new Vector3 (ratio.x, ratio.y, 1));
		return guiMatrix;
	}

    public static void SendHighscoreToServer()
    {

    }

    private static void SendSecureMessageToServer(byte[] data)
    {

    }

    private static IDictionary<NPCSkinType, NPCData> PopulateNPCData()
    {
        Dictionary<NPCSkinType, NPCData> data = new Dictionary<NPCSkinType, NPCData>();

        AddNPCType(data, NPCSkinType.Bopper, new NPCDataBopper());
        AddNPCType(data, NPCSkinType.Boppina, new NPCDataBoppina());
        AddNPCType(data, NPCSkinType.Hippie, new NPCDataHippie());
        AddNPCType(data, NPCSkinType.MowerMan, new NPCDataMowerMan());
        AddNPCType(data, NPCSkinType.OldMan, new NPCDataOldMan());
        AddNPCType(data, NPCSkinType.Hottie, new NPCDataHottie());
        AddNPCType(data, NPCSkinType.Critter, new NPCDataSquirrel());

        return data;
    }

    private static void AddNPCType(IDictionary<NPCSkinType, NPCData> data, NPCSkinType skinType, NPCData npcData)
    {
        data[skinType] = npcData;

        npcData.SkinType = skinType;
    }

	public static void SavePlayerScores(int[] highscores, string[] names)
	{
		for( int i = 0; i < 5; i++ )
		{
			PlayerPrefs.SetInt(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_score_" + i, highscores[i]);
			PlayerPrefs.SetString(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_name_" + i, names[i]);
		}
	}

	public static int[] LoadScores()
	{
		int[] scores = new int[5];

		for( int i = 0; i < 5; i++ )
		{
			scores[i] = PlayerPrefs.GetInt(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_score_" + i, 0);
		}

		return scores;
	}

	public static string[] LoadNames()
	{
		string[] names = new string[5];

		for( int i = 0; i < 5; i++ )
		{
			names[i] = PlayerPrefs.GetString(Application.loadedLevelName + "_" + LevelLoader.instance.modeName + "_name_" + i, "");
		}

		return names;
	}
}