using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BountyState
{
	BOUNTY_HIDDEN,
	BOUNTY_HIDING,
	BOUNTY_SHOWN,
	BOUNTY_SHOWING
}

public enum ScoreState
{
	NO_SCORING,
	START_SCORING,
	SCORING,
	MOVE_SCORING,
	STOP_SCORING,
	END_SCORING
}

/****************************************
 * 
 * Added by Matt for high score purposes
 * 
 ****************************************/

public enum ScoreGameType
{
    SoulSurvivor = 1,
    Marked,
    Timed
}

public enum ScoreLevel
{
    Tutorial = 1,
    BloodyBeginnings,
    LakesideLullaby,
    OverTroubledWaters,
    HallowedLabyrinth
}

public class ScoreScript : MonoBehaviour {
	
	/// <summary>
	/// Probability that next npc will be the new targeted bounty.
	/// </summary>
	public float frequency;

	// bounty images
	public Texture2D bountyBoxImage;
	public Texture2D bountyBoxTextImage;

	// multiplier bar images
	public Texture2D bar1;
	public Texture2D bar2;
	public Texture2D bar3;
	public Texture2D bar4;
	public Texture2D bar5;
	public Texture2D bar6;
	public Texture2D bar7;
	public Texture2D bar8;
	public Texture2D bar9;
	public Texture2D bar10;
	public Texture2D bar11;
	public Texture2D bar12;
	public Texture2D bar13;
	public Texture2D bar14;
	public Texture2D bar15;
	public Texture2D bar16;
	public Texture2D bar17;
	public Texture2D bar18;
	public Texture2D bar19;
	public Texture2D bar20;
	public Texture2D bar21;
	public Texture2D bar22;

	bool gameStarted = false;
	
	/*
	 * SCORING SYSTEM:
	 * 
	 * (TEMPORARY)
	 * Eating an npc - 50 pts
	 * Avoiding axeman detection - 100 pts
	 * Luring an npc - 25 pts
	 * Grabbing an npc - 25
	 * 
	 */
	public readonly int NPC_EATEN = 50;
	public readonly int AXEMAN_EATEN = 200;
	public readonly int LURED_NPC = 25;
	public readonly int NPC_GRABBED = 25;
	public readonly int BOUNTY_EATEN = 100;
	
	// stationary multipliers
	public readonly int stealthMultiplier = 4;
	public readonly int droppingFliesMultiplier = 4;
	public readonly int lureMultiplier = 2;

	public readonly int axemanEaten3Multiplier = 8;
	public readonly int axemanEaten4Multiplier = 10;
	public readonly int oldEaten3Multiplier = 6;
	public readonly int oldEaten4Multiplier = 8;
	public readonly int childEaten3Multiplier = 6;
	public readonly int childEaten4Multiplier = 8;
	public readonly int hottieEaten3Multiplier = 6;
	public readonly int hottieEaten4Multiplier = 8;
	public readonly int mowerEaten3Multiplier = 6;
	public readonly int mowerEaten4Multiplier = 8;
	public readonly int varietyEaten3Multiplier = 6;

	int streakMultiplier = 1;
	
	// multiplier points
	public readonly int scaredNPCMultiplier = 1;
	
	
	
	int tmpMultiplier = 0;

	// bools relating to multipliers
	bool isStealth = false;
	bool isQuick = false;
	bool isLured = false;
	bool isPattern = false;
	
	
	// output strings
	readonly string npcEatenString = "Soulful Snack";
	readonly string bountyEatenString = "Target Kill";
	readonly string enemyEatenString = "The Axe Man";
	string eatenString;
	string eatenRowString;
	//readonly string npcGrabbedString = "Person grabbed";
	//readonly string npcLuredString = "Person lured";
	
	readonly string stealthString = "Quiet Giant";
	readonly string streakString = "Ghastly Glutton";
	readonly string luredString = "Come To Me";

	readonly string axemanEaten3 = "Lumberjacked!";
	readonly string axemanEaten4 = "Flannel Forager";
	readonly string oldEaten3 = "Geriatricide!";
	readonly string oldEaten4 = "Grey Dawn";
	readonly string childEaten3 = "Infanticide!";
	readonly string childEaten4 = "Bane of the Playground";
	readonly string hottieEaten3 = "Hot Tamale";
	readonly string hottieEaten4 = "Bye Bye Love";
	readonly string mowerEaten3 = "Yard Worked";
	readonly string mowerEaten4 = "The Lost Landscapers";
	readonly string varietyEaten3 = "Variety Platter";

	
	float timeSinceLastKill = -1;
	float lastKillTime = -100;
	readonly float lastKillBonus = 15;
	
	ArrayList alertedNPCs = new ArrayList(); // alerted npcs
	ArrayList luredNPCs = new ArrayList(); // lured npcs
	ArrayList scaredNPCs = new ArrayList(); // scared npcs
	Queue multiplierQueue = new Queue(); // queue of multipliers
	
	ArrayList deleteScaredNPCS = new ArrayList();
	

	ArrayList npcsEatenList = new ArrayList();
	
	
	// saved scores
	int[] highscores = new int[10];
	string[] names = new string[10];
	string initials = "";
	bool win = false;
	bool endLevel = false;
	bool entered = false;
	
	
	

	// GUI Variables
	
	
	// Score GUI variables
	float offsetX = 450;
	float offsetY = 50;
	float sizeX = 20;
	float sizeY = 40;
	
	// popups
	float popupX = 20;
	float popupY = 30;
	float popupIncrement = 0;
	readonly float popupIncrementMax = 50;
	float popupAlpha;
	
	float popupX2 = 100;
	float popupY2 = 30;
	float popupIncrement2 = 0;
	readonly float popupIncrementMax2 = 100;
	float popupAlpha2;

	float popupXEnd = 14;
	float popupYEnd = 21;

	float scoreTextIncrement = 0;
	readonly float scoreTextIncrementMax = 100;
	float scoreTextAlpha;
	
	// sliding animation
	float slideIncrement = 2f;
	readonly float slideMax = 100;
	
	float endFont = 20;
	
	int _score;
	readonly int chainMax = 20;
	bool scoreDisplay;
	
	int displayScore;
	int displayMultiplier;
	int scoreState;
	
	float scoreTimer = 0;
	readonly float scoreIncrement = 1f;
	readonly float scoreIncrementMax = 100f;
	float moveStartTime;
	
	float sideL;
	float sideR;
	float startHeight;
	float endHeight;
	float endX;
	
	
	// Bounty GUI variables
	float bountySizeX;
	float bountyLabelSizeY;
	float bountyRectSizeY;
	
	int bountyState;
	float bountyIncrement = 0;
	float currentBountyImageHeight;
	float currentBountyTextHeight;
	float currentBountyBoxHeight;
	
	GameObject BountyNPC;
	Texture2D BountyNPCImage;
	
	// when bounty is given
	bool bountyRaised = false;
	float bountyRaiseTime = -1;
	bool bountyRaiseSetup = true;
	
	
	// score multiplier variables
	readonly int multIncre = 5;
	float multYOffset = 20;
	float multXOffset = 20;
	float multLength;
	float multHeight;

	int currentMultiplier = 3;
	int popupMultiplier = 0;
	int multiplierPoints = 0;

	// multiplier timers
	float lastMultiplierTime;
	float currentMultiplierTime;
	readonly float multiplierTimeLength = 10;

	// multiplier sliders
	bool multiplierIsChanging = false;
	float prevValue;
	float newValue;
	float multiplierSliderTime = 1;

	bool multiplierIsPositive = true;
	string multiplierSign;
	int multiplierPointImage;

	// game mode marked selected
	bool marked = false;
	bool paused = false;
	
	// Use this for initialization
	void Start () {
		
		if( frequency > 1 || frequency < 0 )
			frequency = .5f;
		
		RegisterListeners ();
		_score = 0;
		streakMultiplier = 1;
		
		scoreDisplay = false;
		displayMultiplier = streakMultiplier;
		displayScore = 0;
		
		scoreState = (int)ScoreState.NO_SCORING;
		bountyState = (int)BountyState.BOUNTY_HIDDEN;
		BountyNPCImage = null;//new Texture2D (1, 1);
		
		// bounty
		bountySizeX = (int)Screen.width * .25f;
		bountyLabelSizeY = (int)Screen.height * .125f;
		bountyRectSizeY = (int)Screen.height * .25f;

		currentBountyBoxHeight = Screen.height-bountyLabelSizeY;
		currentBountyTextHeight = Screen.height-bountyLabelSizeY*3/4;
		currentBountyImageHeight = Screen.height+bountyRectSizeY;
		
		popupIncrement = popupIncrementMax;
		popupIncrement2 = popupIncrementMax2;
		
		sideL = Screen.width / 3;
		sideR = Screen.width * 2 / 3;
		startHeight = Screen.height * 1 / 3;
		endHeight = 100;
		endX = multXOffset;
		
		highscores = GlobalGameStateManager.highscores;
		names = GlobalGameStateManager.playerNames;
		
		// mult variabes
		multLength = Screen.width / 3;
		multHeight = Screen.height / 10;
		lastMultiplierTime = Time.time;
		currentMultiplierTime = Time.time;

		if( LevelLoader.instance.modeName == "Feast" )
		{
			popupMultiplier = 0;
			multiplierPoints = 95;
			currentMultiplier = (int)Mathf.Ceil(multiplierPoints/multIncre) + 1;
			marked = true;
		}

		multiplierSign = "+";
		multiplierPointImage = 0;
	}
	
	
	
	
	// Update is called once per frame
	void Update() 
	{
		if( !gameStarted )
			return;

		// end of level
		if (endLevel) 
		{	
			if (win && !entered) 
			{
				// check input
				if (initials.Length == 3) 
				{
					if (Input.GetButtonDown ("A"))
					{
						updateHighScores (_score, initials);
						GlobalGameStateManager.highscores = highscores;
						GlobalGameStateManager.playerNames = names;
						entered = true;						
						MessageCenter.Instance.Broadcast (new ScoreAddingMessage (false));

                        // Upload the high score to the server
                        GameMode gameType = GameMode.Survival;

                        EndConditions ec = GameObject.FindObjectOfType<EndConditions>();

                        if (ec != null) gameType = ec.gameMode;

                        ServerMessaging.UploadScoreToServer(initials, (System.UInt32)_score, gameType, Application.loadedLevelName);
					}
				}
			}
			return;
		}
		
		timeSinceLastKill = Time.time - lastKillTime;
		
		// used for uncalled showing of target for player
		if (bountyRaiseSetup) 
		{
			float bountyTime = Time.time;
			
			if (bountyRaised && bountyState == (int)BountyState.BOUNTY_SHOWN) {
				bountyRaised = false;
				bountyRaiseTime = Time.time;
				
				if (Input.GetButtonDown ("RB")) {
					bountyState = (int)BountyState.BOUNTY_HIDING;
					bountyRaiseSetup = false;
				}
			}
			
			if (bountyTime > bountyRaiseTime + 3 && bountyRaiseTime > 0) {
				bountyState = (int)BountyState.BOUNTY_HIDING;
				bountyRaiseSetup = false;
			}
		}
		
		
		// ignore inputs
		/*if (GetComponent<TreeController> ().state == Tree.State.Eating) {
			bountyState = (int)BountyState.BOUNTY_HIDDEN;
			bountyRaiseSetup = false;
			return;
		}*/
		
		// change state of bountyDisplay
		if (Input.GetButtonDown ("RB")) {
			if (bountyState == (int)BountyState.BOUNTY_SHOWING || bountyState == (int)BountyState.BOUNTY_HIDING) {
				// Do nothing
			} 
			else {
				if (bountyState == (int)BountyState.BOUNTY_SHOWN) {
					bountyState = (int)BountyState.BOUNTY_HIDING;
				} 
				else if (bountyState == (int)BountyState.BOUNTY_HIDDEN) {
					bountyState = (int)BountyState.BOUNTY_SHOWING;
				}
			}
		}
		
		// multiplier updates
		currentMultiplier = 1 + multiplierPoints / multIncre;
		updateMultiplier ();

		// data structure updates
		updateDataStructures ();
		
		// update bounty texture; NOTE: Framerate issues
		/*if(		 BountyNPC != null && bountyState != (int)BountyState.BOUNTY_HIDDEN )
		{
			// used to convert from sprite sheet to current sprite
			Sprite sprite = BountyNPC.GetComponent<SpriteRenderer> ().sprite;
			Color[] pixels = sprite.texture.GetPixels (
				(int)sprite.textureRect.x, 
				(int)sprite.textureRect.y, 
				(int)sprite.textureRect.width, 
				(int)sprite.textureRect.height
				);
			
			BountyNPCImage = new Texture2D ((int)sprite.rect.width, (int)sprite.rect.height);
			
			BountyNPCImage.SetPixels (pixels);
			BountyNPCImage.Apply ();
		}*/
	}
	
	
	
	
	
	// GUI stuff
	void OnGUI()
	{
		/***** Scoring GUI *****/
		
		GUIStyle myStyle = new GUIStyle ();
		//myStyle.font = myFont;
		myStyle.alignment = TextAnchor.MiddleCenter;
		myStyle.fontSize = 30;
		myStyle.normal.textColor = Color.white;
		
		
		Color guiColor = GUI.color;
		Color savedGuiColor = GUI.color;
		
		
		if( scoreState == (int)ScoreState.NO_SCORING )
		{
			// nothing
		}
		else if( scoreState == (int)ScoreState.SCORING )
		{
			/*** display scores ***/
			int offset = 1;

			// change alpha/transparency of score
			if( scoreTextIncrement <= .4f * scoreTextIncrementMax )
			{
				scoreTextAlpha = GUI.color.a * scoreTextIncrement / (.4f * scoreTextIncrementMax );
				guiColor.a = scoreTextAlpha;
				GUI.color = guiColor;
			}
			else if( scoreTextIncrement >= .6f * scoreTextIncrementMax )
			{
				scoreTextAlpha = GUI.color.a * ( scoreTextIncrementMax - scoreTextIncrement ) / (.4f * scoreTextIncrementMax );
				guiColor.a = scoreTextAlpha;
				GUI.color = guiColor;
			}

			// npc type
			FontConverter.instance.parseStringToTextures (sideL, startHeight, popupX, popupY, eatenString);
			
			// pattern
			if( isPattern )
			{
				FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, eatenRowString);
				offset++;
			}
			// stealth
			if( isStealth )
			{
				FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, stealthString);
				offset++;
			}
			// lured
			if( isLured )
			{
				FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, luredString);
				offset++;
			}
			// quick
			if( isQuick )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, streakString);
				offset++;
			}
			// streak
			FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, "Current Streak");
			FontConverter.instance.rightAnchorParseStringToTextures (sideL + popupX * 20, startHeight + offset*(popupY+2), popupX, popupY, displayMultiplier + "");
			offset++;
			
			// score
			FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, "Eating Score");
			FontConverter.instance.rightAnchorParseStringToTextures (sideL + popupX * 20, startHeight + offset*(popupY+2), popupX, popupY, displayScore + "");
			offset++;
			
			// multiplier
			FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, "Multiplier");
			FontConverter.instance.rightAnchorParseStringToTextures (sideL + popupX * 20, startHeight + offset*(popupY+2), popupX, popupY, currentMultiplier + "");
			offset++;
			
			// total score
			FontConverter.instance.parseStringToTextures (sideL, startHeight + offset*(popupY+2), popupX, popupY, "Total Score");
			FontConverter.instance.rightAnchorParseStringToTextures (sideL + popupX * 20, startHeight + offset*(popupY+2), popupX, popupY, "" + (displayScore * currentMultiplier * displayMultiplier));
			offset++;

			// increment timer
			if( !paused )
				scoreTextIncrement += .5f;
			
			// check statement for pop-up statements
			if( scoreTextIncrement > scoreTextIncrementMax )
			{
				scoreState = (int)ScoreState.MOVE_SCORING;
				scoreTextIncrement = 0;
			}

			GUI.color = savedGuiColor; // revert to previous alpha
		}
		else if( scoreState == (int)ScoreState.MOVE_SCORING )
		{
			scoreState = (int)ScoreState.STOP_SCORING;
		}
		else if( scoreState == (int)ScoreState.STOP_SCORING )
		{
			int offset = 1;

			// change alpha/transparency of score
			if( scoreTextIncrement <= .2f * scoreTextIncrementMax )
			{
				scoreTextAlpha = GUI.color.a * scoreTextIncrement / (.2f * scoreTextIncrementMax );
				guiColor.a = scoreTextAlpha;
				GUI.color = guiColor;
			}
			else if( scoreTextIncrement >= .8f * scoreTextIncrementMax )
			{
				scoreTextAlpha = GUI.color.a * ( scoreTextIncrementMax - scoreTextIncrement ) / (.2f * scoreTextIncrementMax );
				guiColor.a = scoreTextAlpha;
				GUI.color = guiColor;
			}

			// npc type
			FontConverter.instance.parseStringToTextures (endX, endHeight, popupXEnd, popupYEnd, eatenString);

			// pattern
			if( isPattern )
			{
				FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, eatenRowString);
				offset++;
			}
			// stealth
			if( isStealth )
			{
				FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, stealthString);
				offset++;
			}
			// lured
			if( isLured )
			{
				FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, luredString);
				offset++;
			}
			// quick
			if( isQuick )
			{
				FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, streakString);
				offset++;
			}
			// streak
			FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, "Current Streak");
			FontConverter.instance.rightAnchorParseStringToTextures (endX + popupXEnd * 20, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, displayMultiplier + "");
			offset++;
			
			// score
			FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, "Eating Score");
			FontConverter.instance.rightAnchorParseStringToTextures (endX + popupXEnd * 20, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, "" + displayScore);
			offset++;
			
			// multiplier
			FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, "Multiplier");
			FontConverter.instance.rightAnchorParseStringToTextures (endX + popupXEnd * 20, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, currentMultiplier + "");
			offset++;
			
			// total score
			FontConverter.instance.parseStringToTextures (endX, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, "Total Score");
			FontConverter.instance.rightAnchorParseStringToTextures (endX + popupXEnd * 20, endHeight + offset*(popupYEnd+2), popupXEnd, popupYEnd, "" + (displayScore * currentMultiplier * displayMultiplier));
			offset++;

			// increment timer
			if( !paused )
				scoreTextIncrement += .2f;
			
			// check statement for pop-up statements
			if( scoreTextIncrement > scoreTextIncrementMax )
			{
				scoreState = (int)ScoreState.END_SCORING;
				scoreTextIncrement = 0;
			}
			
			GUI.color = savedGuiColor; // revert to previous alpha
		}
		else if( scoreState == (int)ScoreState.START_SCORING )
		{
			popupIncrement = 0;
			slideIncrement = 0;
			scoreTimer = 0;
			scoreState = (int)ScoreState.SCORING;
		}
		else if( scoreState == (int)ScoreState.END_SCORING )
		{
			addMultiplier(tmpMultiplier);
			tmpMultiplier = 0;

			isPattern = false;
			isLured = false;
			isStealth = false;
			isQuick = false;
			
			scoreTimer = 0;
			scoreState = (int)ScoreState.NO_SCORING;
			displayScore = 0;
			displayMultiplier = streakMultiplier;
		}
		
		// display multiplier points
		if( scoreDisplay )
		{
			// change alpha/transparency of score
			if( popupIncrement2 <= .4f * popupIncrementMax2 )
			{
				popupAlpha2 = GUI.color.a * popupIncrement2 / (.4f * popupIncrementMax2 );
				guiColor.a = popupAlpha2;
				GUI.color = guiColor;
			}
			else if( popupIncrement2 >= .6f * popupIncrementMax2 )
			{
				popupAlpha2 = GUI.color.a * ( popupIncrementMax2 - popupIncrement2 ) / (.4f * popupIncrementMax2 );
				guiColor.a = popupAlpha2;
				GUI.color = guiColor;
			}
			
			// multiplier pop-up text
			FontConverter.instance.parseStringToTextures (Screen.width/2 - popupX/2, Screen.height/2 - popupY/2 - popupIncrement2 + 75, popupX, popupY, multiplierSign + multiplierPointImage);


			// increment height
			if( !paused )
				popupIncrement2 += 1.5f;
			
			// check statement for pop-up statements
			if( popupIncrement2 > popupIncrementMax2 )
			{
				scoreDisplay = false;
				popupIncrement2 = 0;
			}
			
			
			GUI.color = savedGuiColor; // revert to previous alpha
		}
		
		GUI.color = savedGuiColor; // revert to previous alpha
		
		// popup added score
		if( popupIncrement < popupIncrementMax )
		{
			myStyle.fontSize = 30;
			myStyle.normal.textColor = Color.white;
			myStyle.alignment = TextAnchor.MiddleRight;
			
			// change alpha/transparency of score
			if( popupIncrement <= .4f * popupIncrementMax )
			{
				popupAlpha = GUI.color.a * popupIncrement / (.4f * popupIncrementMax );
				guiColor.a = popupAlpha;
				GUI.color = guiColor;
			}
			else if( popupIncrement >= .6f * popupIncrementMax )
			{
				popupAlpha = GUI.color.a * ( popupIncrementMax - popupIncrement ) / (.4f * popupIncrementMax );
				guiColor.a = popupAlpha;
				GUI.color = guiColor;
			}
			
			FontConverter.instance.rightAnchorParseStringToTextures (Screen.width - 2*sizeX, Screen.height-offsetY-sizeY-popupIncrement, sizeX, sizeY, "" + (displayScore * currentMultiplier * displayMultiplier));

			if( !paused )
				popupIncrement += .75f;
			
			GUI.color = savedGuiColor; // revert to previous alpha
		}
		
		// score
		FontConverter.instance.parseStringToTextures (Screen.width - offsetX + sizeX * 5, Screen.height-offsetY, sizeX, sizeY, "score");
		FontConverter.instance.rightAnchorParseStringToTextures (Screen.width - 2*sizeX, Screen.height-offsetY, sizeX, sizeY, "" + _score);
		
		// high score
		FontConverter.instance.parseStringToTextures (Screen.width - offsetX, 10, sizeX, sizeY, "high score");
		FontConverter.instance.rightAnchorParseStringToTextures (Screen.width - 2*sizeX, 10, sizeX, sizeY, "" + highscores[0]);
		
		
		
		
		
		/***** Bounty GUI *****/
		
		// probably should use switch-case, but meh
		if( bountyState == (int)BountyState.BOUNTY_HIDDEN )
		{
			GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyRectSizeY+bountyLabelSizeY), bountyBoxImage);
			GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX*3/8,Screen.height-bountyLabelSizeY*3/4,bountySizeX*3/4,bountyLabelSizeY*3/4), bountyBoxTextImage);
			if( BountyNPCImage != null )
				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/8,Screen.height+bountyRectSizeY/4,bountySizeX/4,bountyRectSizeY*3/5), BountyNPCImage);
		}
		else if( bountyState == (int)BountyState.BOUNTY_SHOWN )
		{
			GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyRectSizeY,bountySizeX,bountyRectSizeY+bountyLabelSizeY), bountyBoxImage);
			GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX*3/8,Screen.height-bountyLabelSizeY*3/4-bountyRectSizeY,bountySizeX*3/4,bountyLabelSizeY*3/4), bountyBoxTextImage);
			if( BountyNPCImage != null )
				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/8,Screen.height-bountyRectSizeY*3/4,bountySizeX/4,bountyRectSizeY*3/5), BountyNPCImage);
		}
		else if( bountyState == (int)BountyState.BOUNTY_SHOWING )
		{
			if( bountyIncrement >= bountyRectSizeY )
			{
				currentBountyBoxHeight = Screen.height-bountyLabelSizeY-bountyIncrement;
				currentBountyTextHeight = Screen.height-bountyLabelSizeY*3/4-bountyIncrement;
				currentBountyImageHeight = Screen.height-bountyIncrement+bountyRectSizeY/4;

				bountyState = (int)BountyState.BOUNTY_SHOWN;
				bountyIncrement = bountyRectSizeY;
			}
			else
			{
				if( !paused )
					bountyIncrement += 2f;

				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyIncrement,bountySizeX,bountyRectSizeY+bountyLabelSizeY), bountyBoxImage);
				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX*3/8,Screen.height-bountyLabelSizeY*3/4-bountyIncrement,bountySizeX*3/4,bountyLabelSizeY*3/4), bountyBoxTextImage);
				if( BountyNPCImage != null )
					GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/8,Screen.height-bountyIncrement+bountyRectSizeY/4,bountySizeX/4,bountyRectSizeY*3/5), BountyNPCImage);
			}
			
			
		}
		else if( bountyState == (int)BountyState.BOUNTY_HIDING )
		{
			if( bountyIncrement <= 0 )
			{
				currentBountyBoxHeight = Screen.height-bountyLabelSizeY-bountyIncrement;
				currentBountyTextHeight = Screen.height-bountyLabelSizeY*3/4-bountyIncrement;
				currentBountyImageHeight = Screen.height-bountyIncrement+bountyRectSizeY/4;

				bountyState = (int)BountyState.BOUNTY_HIDDEN;
				bountyIncrement = 0;
			}
			else
			{
				if( !paused )
					bountyIncrement -= 2f;

				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyIncrement,bountySizeX,bountyRectSizeY+bountyLabelSizeY), bountyBoxImage);
				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX*3/8,Screen.height-bountyLabelSizeY*3/4-bountyIncrement,bountySizeX*3/4,bountyLabelSizeY*3/4), bountyBoxTextImage);
				if( BountyNPCImage != null )
					GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/8,Screen.height-bountyIncrement+bountyRectSizeY/4,bountySizeX/4,bountyRectSizeY*3/5), BountyNPCImage);
			}
			
		}
		else // default - BOUNTY_HIDDEN
		{

			GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyRectSizeY+bountyLabelSizeY), bountyBoxImage);
			GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX*3/8,Screen.height-bountyLabelSizeY*3/4,bountySizeX*3/4,bountyLabelSizeY*3/4), bountyBoxTextImage);
			if( BountyNPCImage != null )
				GUI.DrawTexture (new Rect(Screen.width/2-bountySizeX/8,Screen.height+bountyRectSizeY/4,bountySizeX/4,bountyRectSizeY*3/5), BountyNPCImage);
		}
		
		
		
		
		/***** High Score GUI *****/
		
		myStyle.alignment = TextAnchor.MiddleCenter;
		myStyle.fontSize = 50;
		myStyle.normal.textColor = Color.white;
		
		// end of level
		if( endLevel )
		{
			// not a high score
			if( _score <= highscores[9] )
				entered = true;
			
			if( win && !entered )
			{
				GUI.skin.textField.alignment = TextAnchor.UpperCenter;
				GUI.skin.textField.fontSize = 30;
				GUI.skin.textField.normal.textColor = Color.white;
				myStyle.fontSize = 30;
				GUI.Label(new Rect(Screen.width/2,Screen.height/2+5,100,100), "Top 10 High Score", myStyle);
				GUI.Label(new Rect(Screen.width/2-200,Screen.height/2+50,100,100), "Enter Your Initials ", myStyle);
				GUI.SetNextControlName("MyTextField");
				initials = GUI.TextField(new Rect(Screen.width/2,Screen.height/2+70,100,50), initials, 3);
				GUI.FocusControl("MyTextField");
				
				if( initials.Length == 3 )
				{
					GUI.Label(new Rect(Screen.width/2+200,Screen.height/2+50,100,100), "Press A To Save", myStyle);
				}
			}
		}
		
		
		/***** Score Multiplier GUI *****/

		FontConverter.instance.rightAnchorParseStringToTextures (multXOffset + multLength + 50, multYOffset, 40, multHeight, currentMultiplier + "x");

		// multiplier bar
		if( multiplierIsChanging )
		{
			// change(slide) data
			if( Time.time - lastMultiplierTime <= multiplierSliderTime )
			{
				float timeRatio = (Time.time - lastMultiplierTime) / multiplierSliderTime;
				float tempRatio = (newValue - prevValue) * timeRatio;

				GUI.DrawTexture (new Rect (multXOffset, multYOffset, multLength, multHeight), getMultiplierBarImage(((prevValue + tempRatio) % (float)multIncre) / (float)multIncre * (multLength - 4),multLength));
			}
			else
			{
				multiplierIsChanging = false;
				GUI.DrawTexture (new Rect (multXOffset, multYOffset, multLength, multHeight), getMultiplierBarImage((multiplierPoints % multIncre) / (float)multIncre * (multLength - 4),multLength));
			}
		}
		else
		{
			GUI.DrawTexture (new Rect (multXOffset, multYOffset, multLength, multHeight), getMultiplierBarImage((multiplierPoints % multIncre) / (float)multIncre * (multLength - 4),multLength));
		}
	}
	
	
	
	
	
	
	
	// update multiplier
	void addMultiplier(int num)
	{
		if( marked )
			return;

		if( multiplierIsChanging )
			prevValue = prevValue + (newValue - prevValue) * ( (Time.time - lastMultiplierTime) / multiplierSliderTime );
		else
			prevValue = multiplierPoints;
		newValue = multiplierPoints + num;

		multiplierPoints += num;
		scoreDisplay = true;
		popupMultiplier = num;
		multiplierQueue.Enqueue (num);

		lastMultiplierTime = Time.time;
		multiplierIsChanging = true;
		multiplierIsPositive = true;
		multiplierSign = "+";
		multiplierPointImage = num;
	}

	// subtract a portion of multiplier
	void subMultiplier(int num)
	{
		if( multiplierIsChanging )
			prevValue = prevValue + (newValue - prevValue) * ( (Time.time - lastMultiplierTime) / multiplierSliderTime );
		else
			prevValue = multiplierPoints;
		newValue = multiplierPoints - num;

		multiplierPoints -= num;
		scoreDisplay = true;
		popupMultiplier = -num;
		multiplierQueue.Enqueue (num);

		lastMultiplierTime = Time.time;
		multiplierIsChanging = true;
		multiplierIsPositive = false;
		multiplierSign = "-";
		multiplierPointImage = num;
	}
	
	// reset multiplier
	void resetMultiplier()
	{
		//subMultiplier (multiplierPoints);
		multiplierPoints = 0;
	}

	// reset multiplier
	void halfMultiplier()
	{
		//subMultiplier (multiplierPoints);
		multiplierPoints /= 2;
	}

	// decrement multiplierPoints
	void updateMultiplier()
	{
		if( multiplierPoints < 0 )
			multiplierPoints = 0;

		if( multiplierPoints <= 0 )
			return;

		currentMultiplierTime = Time.time;

		// subtract from multiplier if no recent updates to the multiplier
		if( currentMultiplierTime - lastMultiplierTime > multiplierTimeLength )
		{
			subMultiplier(1);
			lastMultiplierTime = Time.time;
		}
	}

	// get corresponding image from multiplier points
	Texture2D getMultiplierBarImage(float num, float total)
	{
		switch((int)Mathf.Round((num/total)*21))
		{
		case 0:
			return bar22;
		case 1:
			return bar21;
		case 2:
			return bar20;
		case 3:
			return bar19;
		case 4:
			return bar18;
		case 5:
			return bar17;
		case 6:
			return bar16;
		case 7:
			return bar15;
		case 8:
			return bar14;
		case 9:
			return bar13;
		case 10:
			return bar12;
		case 11:
			return bar11;
		case 12:
			return bar10;
		case 13:
			return bar9;
		case 14:
			return bar8;
		case 15:
			return bar7;
		case 16:
			return bar6;
		case 17:
			return bar5;
		case 18:
			return bar4;
		case 19:
			return bar3;
		case 20:
			return bar2;
		case 21:
			return bar1;
		default:
			return bar22;
		}
		
	}


	// add a score
	void addScore(int score)
	{
		_score += score;
	}


	
	
	// save new high score
	void updateHighScores(int score, string playerName)
	{
		int spot = 0;
		bool newScore = false;
		
		// loop through scores
		for( int i = 0; i < 10; i++ )
		{
			if( score > highscores[i] )
			{
				spot = i;
				newScore = true;
				break;
			}
		}
		
		// sorts them
		if( newScore )
		{
			// move down lower scores
			for( int i = 9; i > spot; i-- )
			{
				highscores[i] = highscores[i-1];
				names[i] = names[i-1];
			}
			highscores[spot] = score;
			names[spot] = playerName;
		}
	}


	// update arraylists and other data strutures
	void updateDataStructures()
	{
		foreach( GameObject go in scaredNPCs )
		{
			if( go == null )
				continue;
			else if( go.GetComponent<AIController>() != null )
			{
				AIController ai = go.GetComponent<AIController>();

				if( ai.scared == false )
					deleteScaredNPCS.Add(go);
			}
		}

		foreach( GameObject go in deleteScaredNPCS )
		{
			if( scaredNPCs.Contains(go) )
			{
				scaredNPCs.Remove(go);
			}
		}

		deleteScaredNPCS = new ArrayList ();
	}
	
	
	void invokeAudio()
	{
		
	}
	
	
	
	
	
	
	
	
	void OnDestroy()
	{
		UnregisterListeners ();
	}
	
	void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener (MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.RegisterListener (MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.RegisterListener (MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.RegisterListener (MessageType.LureReleased, HandleLureReleased);
		MessageCenter.Instance.RegisterListener (MessageType.NPCEaten, HandleNPCEaten);
		MessageCenter.Instance.RegisterListener (MessageType.NPCCreated, HandleNPCCreated);
		MessageCenter.Instance.RegisterListener (MessageType.NPCDestroyed, HandleNPCDestroyed);
		MessageCenter.Instance.RegisterListener (MessageType.LevelFinished, HandleLevelFinished);
		MessageCenter.Instance.RegisterListener (MessageType.LevelStart, HandleLevelStart);
		MessageCenter.Instance.RegisterListener (MessageType.PauseChanged, HandlePauseChanged);
	}
	
	void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener(MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.UnregisterListener(MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.UnregisterListener(MessageType.LureReleased, HandleLureReleased);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCEaten, HandleNPCEaten);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCCreated, HandleNPCCreated);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCDestroyed, HandleNPCDestroyed);
		MessageCenter.Instance.UnregisterListener(MessageType.LevelFinished, HandleLevelFinished);
		MessageCenter.Instance.UnregisterListener(MessageType.PauseChanged, HandlePauseChanged);
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	/*
	 * Handler Methods
	 */
	
	void HandleLureEntered(Message message)
	{
		LureEnteredMessage mess = message as LureEnteredMessage;
		
		GameObject NPC = mess.NPC;
		
		if( NPC.GetComponent<AIController>() as AIController == null )
			return;
		
		if( NPC.GetComponent<AIController>().getLastLure() != null && NPC.GetComponent<AIController>().getLastLure().Equals(mess.Lure) )
		{
			if( NPC.GetComponent<AIController>().getLastLure().Equals(mess.Lure) )
				return;
		}
		
		luredNPCs.Add (NPC);
	}
	
	
	
	
	
	void HandleGrabbedNPCs(Message message)
	{
		bountyState = (int)BountyState.BOUNTY_HIDDEN;
		lastMultiplierTime = Time.time;
	}
	
	
	
	
	
	void HandleNPCAlertLevel(Message message)
	{
		NPCAlertLevelMessage mess = message as NPCAlertLevelMessage;
		
		if(mess.alertLevelType == AlertLevelType.Alert)
		{
			streakMultiplier = 1;
			if( !alertedNPCs.Contains(mess.NPC) )
				alertedNPCs.Add(mess.NPC);
			/*if( streakMultiplier > 2 )
			{
				streakMultiplier -= 2;
			}
			else
			{
				streakMultiplier = 1;
			}*/
		}
		else if( mess.alertLevelType == AlertLevelType.Panic )
		{
			streakMultiplier = 1;

			if( marked )
				halfMultiplier();
			else
				resetMultiplier();

			if( !alertedNPCs.Contains(mess.NPC) )
				alertedNPCs.Add(mess.NPC);
		}
		else if( mess.alertLevelType == AlertLevelType.Scared )
		{
			// add lured/scared multiplier
			if( !scaredNPCs.Contains(mess.NPC) )
			{
				scaredNPCs.Add(mess.NPC);
				addMultiplier(scaredNPCMultiplier);
			}
		}
	}
	
	
	
	
	
	void HandleLureReleased(Message message)
	{
		LureReleasedMessage mess = message as LureReleasedMessage;
		
		GameObject NPC = mess.NPC;
		
		if( luredNPCs.Contains(NPC) )
			luredNPCs.Remove (NPC);
		
	}
	
	
	
	
	
	void HandleNPCDestroyed(Message message)
	{
		NPCDestroyedMessage mess = message as NPCDestroyedMessage;
		
		if( alertedNPCs.Contains(mess.NPC) )
			alertedNPCs.Remove(mess.NPC);

		if( BountyNPC != null && BountyNPC.Equals(mess.NPC) )
		{
			BountyNPC = null;
			BountyNPCImage = null;//new Texture2D(1,1);
		}
	}
	
	
	
	
	
	void HandleNPCEaten(Message message)
	{
		NPCEatenMessage mess = message as NPCEatenMessage;

		if( mess.NPC == null )
			return;

		if( mess.NPC.GetComponent<CritterController>() != null )
			return;
		
		// initial multiplier values
		int lure = 1;
		int quick = 1;
		int stealth = 1;
		int npcScore = NPC_EATEN;
		eatenString = npcEatenString;
		
		tmpMultiplier = 0;
		lastMultiplierTime = Time.time;

		AIController controller = mess.NPC.GetComponent<AIController> ();
		string skin = controller.SkinType.ToString();
		npcsEatenList.Add (skin);

		int eatenCount = npcsEatenList.Count;
		eatenRowString = "";
		bool match = true;
		bool passed = true;
		string[] eatenArray = (string[])npcsEatenList.ToArray(typeof(string));
		
		// check for specific ai skins and patterns
		if( eatenCount >= 4 )
		{
			passed = false;

			if( skin.Equals("Bopper") )
			{
				// eat 4+ children in a row
				for( int i = eatenCount - 4; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("Bopper") )
					{
						continue;
					}
					else
					{
						match = false;
						passed = true;
						break;
					}
				}

				if( match )
				{
					tmpMultiplier += childEaten4Multiplier;
					isPattern = true;
					eatenRowString = childEaten4;
				}
			}
			else if( skin == "Hottie" )
			{
				// eat 4+ hotties in a row
				for( int i = eatenCount - 4; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("Hottie") )
					{
						continue;
					}
					else
					{
						match = false;
						passed = true;
						break;
					}
				}

				if( match )
				{
					tmpMultiplier += hottieEaten4Multiplier;
					isPattern = true;
					eatenRowString = hottieEaten4;
				}
			}
			else if( skin.Equals("MowerMan") )
			{
				// eat 4+ mowermen in a row
				for( int i = eatenCount - 4; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("MowerMan") )
					{
						continue;
					}
					else
					{
						match = false;
						passed = true;
						break;
					}
				}

				if( match )
				{
					tmpMultiplier += mowerEaten4Multiplier;
					isPattern = true;
					eatenRowString = mowerEaten4;
				}
			}
			else if( skin.Equals("OldMan") )
			{
				// eat 4+ oldmen in a row
				for( int i = eatenCount - 4; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("OldMan") )
					{
						continue;
					}
					else
					{
						match = false;
						passed = true;
						break;
					}
				}

				if( match )
				{
					tmpMultiplier += oldEaten4Multiplier;
					isPattern = true;
					eatenRowString = oldEaten4;
				}
			}
			else if( skin.Equals("AxeMan") )
			{
				// eat 4+ axemen in a row
				for( int i = eatenCount - 4; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("AxeMan") )
					{
						continue;
					}
					else
					{
						match = false;
						passed = true;
						break;
					}
				}

				if( match )
				{
					tmpMultiplier += axemanEaten4Multiplier;
					isPattern = true;
					eatenRowString = axemanEaten4;
				}
			}
		}

		if( eatenCount >= 3 && ( match == false || passed == true ) )
		{
			match = true;

			if( skin.Equals("Bopper") )
			{
				// eat 3 children in a row
				for( int i = eatenCount - 3; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("Bopper") )
					{
						continue;
					}
					else
					{
						match = false;
						break;
					}
				}
				
				if( match )
				{
					tmpMultiplier += childEaten3Multiplier;
					isPattern = true;
					eatenRowString = childEaten3;
				}
			}
			else if( skin.Equals("Hottie") )
			{
				// eat 3 hotties in a row
				for( int i = eatenCount - 3; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("Hottie") )
					{
						continue;
					}
					else
					{
						match = false;
						break;
					}
				}
				
				if( match )
				{
					tmpMultiplier += hottieEaten3Multiplier;
					isPattern = true;
					eatenRowString = hottieEaten3;
				}
			}
			else if( skin.Equals("MowerMan") )
			{
				// eat 3 mowermen in a row
				for( int i = eatenCount - 3; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("MowerMan") )
					{
						continue;
					}
					else
					{
						match = false;
						break;
					}
				}
				
				if( match )
				{
					tmpMultiplier += mowerEaten3Multiplier;
					isPattern = true;
					eatenRowString = mowerEaten3;
				}
			}
			else if( skin.Equals("OldMan") )
			{
				// eat 3 oldmen in a row
				for( int i = eatenCount - 3; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("OldMan") )
					{
						continue;
					}
					else
					{
						match = false;
						break;
					}
				}
				
				if( match )
				{
					tmpMultiplier += oldEaten3Multiplier;
					isPattern = true;
					eatenRowString = oldEaten3;
				}
			}
			else if( skin.Equals("AxeMan") )
			{
				// eat 3 axemen in a row
				for( int i = eatenCount - 3; i < eatenCount; i++ )
				{
					if( eatenArray[i].Equals("AxeMan") )
					{
						continue;
					}
					else
					{
						match = false;
						break;
					}
				}
				
				if( match )
				{
					tmpMultiplier += axemanEaten3Multiplier;
					isPattern = true;
					eatenRowString = axemanEaten3;
				}
			}

			// 3 different npcs in a row
			if( match == false )
			{
				match = true;

				string npc1 = eatenArray[eatenCount-3];
				string npc2 = eatenArray[eatenCount-2];
				string npc3 = eatenArray[eatenCount-1];

				if( npc1.Equals(npc2) || npc1.Equals(npc3) || npc2.Equals(npc3) )
				{
					match = false;
				}

				if( match )
				{
					tmpMultiplier += varietyEaten3Multiplier;
					isPattern = true;
					eatenRowString = varietyEaten3;
				}
			}
		}

		
		// update score multipliers and base scores
		if( timeSinceLastKill < lastKillBonus )
		{
			quick = droppingFliesMultiplier;
			isQuick = true;
			tmpMultiplier += droppingFliesMultiplier;
		}
		if( BountyNPC != null && BountyNPC.Equals(mess.NPC) )
		{
			npcScore = BOUNTY_EATEN;
			eatenString = bountyEatenString;
			BountyNPC = null;
			BountyNPCImage = null;//new Texture2D(1,1);
		}
		if( mess.NPC.GetComponent<EnemyAIController>() != null )
		{
			npcScore = AXEMAN_EATEN;
			eatenString = enemyEatenString;
		}
		if( luredNPCs.Contains(mess.NPC) )
		{
			luredNPCs.Remove(mess.NPC);
			lure = lureMultiplier;
			isLured = true;
			tmpMultiplier += lureMultiplier;
		}
		if( alertedNPCs.Contains(mess.NPC) )
		{
			alertedNPCs.Remove(mess.NPC);
			isStealth = false;
		}
		else
		{
			stealth = stealthMultiplier;
			isStealth = true;
			tmpMultiplier += stealthMultiplier;
		}
		
		displayMultiplier = streakMultiplier; // get multiplier
		displayScore = npcScore * stealth * quick * lure * streakMultiplier;

		currentMultiplier = 1 + multiplierPoints / multIncre;
		addScore(displayScore*currentMultiplier*displayMultiplier);
		
		scoreState = (int)ScoreState.START_SCORING;
		
		// update global variables
		lastKillTime = Time.time;
		streakMultiplier++;
	}
	
	
	
	
	// sets up bounty
	void HandleNPCCreated(Message message)
	{
		NPCCreatedMessage mess = message as NPCCreatedMessage;
		
		// bounty filled or tree is eating
		if( BountyNPC != null ) //|| GetComponent<TreeController>().state == Tree.State.Eating)
			return;
		
		// bounty can't be an axeman/enemy
		if( mess.NPC.GetComponent<EnemyAIController>() != null )
			return;

		// bounty can't be a critter
		if( mess.NPC.GetComponent<CritterController>() != null )
			return;
		
		// bounty/target
		if( Random.value > frequency )
			return;

		// if player hasn't started
		if( !gameStarted )
			return;
		
		BountyNPC = mess.NPC;

		ParticleSystem ps = BountyNPC.GetComponent<ParticleSystem> ();
		ps.Play ();
		
		
		// used to convert from sprite sheet to current sprite
		Sprite sprite = BountyNPC.GetComponent<SpriteRenderer> ().sprite;
		if (sprite == null)
		{
			Debug.Log ("Something Broke here: " + this.GetType ().Name);
			return;
		}
		Color[] pixels = sprite.texture.GetPixels (
			(int)sprite.textureRect.x, 
			(int)sprite.textureRect.y, 
			(int)sprite.textureRect.width, 
			(int)sprite.textureRect.height
			);
		
		BountyNPCImage = new Texture2D ((int)sprite.rect.width, (int)sprite.rect.height);
		
		BountyNPCImage.SetPixels (pixels);
		BountyNPCImage.Apply ();

		bountyState = (int)BountyState.BOUNTY_SHOWING;
		bountyRaised = true;
	}
	
	
	void HandleLevelFinished(Message message)
	{
		LevelFinishedMessage mess = message as LevelFinishedMessage;
		
		endLevel = true;
		
		if( mess.Type == (int)LevelFinishedType.Win )
		{
			win = true;
			if( _score + displayScore > highscores[9] ) // last score not added, needs displayScore
			{
				MessageCenter.Instance.Broadcast(new ScoreAddingMessage(true));
			}
		}
	}

	void HandleLevelStart(Message message)
	{
		LevelStartMessage msg = message as LevelStartMessage;
		
		gameStarted = true;
		lastMultiplierTime = Time.time;
		/*
		if( BountyNPC == null )
			return;

		ParticleSystem ps = BountyNPC.GetComponent<ParticleSystem> ();
		ps.Play ();
		
		
		// used to convert from sprite sheet to current sprite
		Sprite sprite = BountyNPC.GetComponent<SpriteRenderer> ().sprite;
		Color[] pixels = sprite.texture.GetPixels (
			(int)sprite.textureRect.x, 
			(int)sprite.textureRect.y, 
			(int)sprite.textureRect.width, 
			(int)sprite.textureRect.height
			);
		
		BountyNPCImage = new Texture2D ((int)sprite.rect.width, (int)sprite.rect.height);
		
		BountyNPCImage.SetPixels (pixels);
		BountyNPCImage.Apply ();
		
		//BountyNPCImage = mess.NPC.GetComponent<SpriteRenderer> ().sprite.texture;
		
		bountyState = (int)BountyState.BOUNTY_SHOWING;
		bountyRaised = true;*/
	}



	protected void HandlePauseChanged(Message message)
	{
		PauseChangedMessage mess = message as PauseChangedMessage;
		
		paused = mess.isPaused;

		if( paused )
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}
}
