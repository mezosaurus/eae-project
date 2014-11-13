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
	CREDITS = 1,
	LEVEL_1 = 2,
};

public enum GameState
{
	BEGINLEVEL,
	ENDOFLEVEL_VICTORY,
	ENDOFLEVEL_DEFEAT,
	INLEVEL_DEFAULT,
	INLEVEL_CHASE,
	INLEVEL_EATING,
    INLEVEL_EATING_CINEMATIC,
};

public static class GlobalGameStateManager
{
	public static readonly float originalWidth = 1920;
	public static readonly float originalHeight = 1080;
	public static GameState GameState = GameState.INLEVEL_DEFAULT;
	public static SceneEnum CurrentScene = 0;

	public static Matrix4x4 PrepareMatrix ()
	{
		Vector2 ratio = new Vector2 (Screen.width / originalWidth, Screen.height / originalHeight);
		Matrix4x4 guiMatrix = Matrix4x4.identity;
		guiMatrix.SetTRS (new Vector3 (1, 1, 1), Quaternion.identity, new Vector3 (ratio.x, ratio.y, 1));
		return guiMatrix;
	}
}