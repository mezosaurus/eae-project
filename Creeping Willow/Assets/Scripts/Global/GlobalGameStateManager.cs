using UnityEngine;
using System.Collections;

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

public enum PlayerState
{
	DEFAULT,
	EATING_MINIGAME,
	EATING_CINEMATIC,
	POSSESION_CASTING,
	POSSESION_POSSESED,
}

public static class GlobalGameStateManager
{
	public static readonly float originalWidth = 1920;
	public static readonly float originalHeight = 1080;
	public static LevelState LevelState = LevelState.BEGINLEVEL;
	public static PlayerState PlayerState = PlayerState.DEFAULT;
	public static SceneEnum CurrentScene = 0;

	public static int[] highscores = new int[10];
	public static string[] playerNames = new string[10];

	public static Matrix4x4 PrepareMatrix ()
	{
		Vector2 ratio = new Vector2 (Screen.width / originalWidth, Screen.height / originalHeight);
		Matrix4x4 guiMatrix = Matrix4x4.identity;
		guiMatrix.SetTRS (new Vector3 (1, 1, 1), Quaternion.identity, new Vector3 (ratio.x, ratio.y, 1));
		return guiMatrix;
	}
}