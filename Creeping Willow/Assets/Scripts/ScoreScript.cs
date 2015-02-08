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

public class ScoreScript : MonoBehaviour {
	
	/// <summary>
	/// Probability that next npc will be the new targeted bounty.
	/// </summary>
	public float frequency;

	// scoring texts
	public Texture2D plusSign;
	public Texture2D minusSign;
	public Texture2D numZero;
	public Texture2D numOne;
	public Texture2D numTwo;
	public Texture2D numThree;
	public Texture2D numFour;
	public Texture2D numFive;
	public Texture2D numSix;
	public Texture2D numSeven;
	public Texture2D numEight;
	public Texture2D numNine;
	public Font myFont;

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
	public readonly int stealthMultiplier = 6;
	public readonly int droppingFliesMultiplier = 3;
	public readonly int lureMultiplier = 2;
	int streakMultiplier = 1;
	
	// multiplier points
	public readonly int scaredNPCMultiplier = 1;
	
	
	
	
	// bools relating to multipliers
	bool isStealth = false;
	bool isQuick = false;
	bool isLured = false;
	
	
	// output strings
	readonly string npcEatenString = "Person Kill";
	readonly string bountyEatenString = "Target Kill";
	readonly string enemyEatenString = "Axeman Kill";
	string eatenString;
	//readonly string npcGrabbedString = "Person grabbed";
	//readonly string npcLuredString = "Person lured";
	
	readonly string stealthString = "Stealth Kill";
	readonly string streakString = "Quick Kill";
	readonly string luredString = "Lured Kill";
	
	float timeSinceLastKill = -1;
	float lastKillTime = -100;
	readonly float lastKillBonus = 15;
	
	ArrayList alertedNPCs = new ArrayList(); // alerted npcs
	ArrayList luredNPCs = new ArrayList(); // lured npcs
	Queue multiplierQueue = new Queue(); // queue of multipliers
	
	
	
	
	
	
	// saved scores
	int[] highscores = new int[10];
	string[] names = new string[10];
	string initials = "";
	bool win = false;
	bool endLevel = false;
	bool entered = false;
	
	
	
	
	
	
	// Score GUI variables
	float offsetX = 200;
	float offsetY = 50;
	float sizeX = 150;
	float sizeY = 50;
	
	// popups
	float popupX = 100;
	float popupY = 30;
	float popupIncrement = 0;
	readonly float popupIncrementMax = 50;
	float popupAlpha;
	
	float popupX2 = 100;
	float popupY2 = 30;
	float popupIncrement2 = 0;
	readonly float popupIncrementMax2 = 100;
	float popupAlpha2;
	
	// sliding animation
	float slideIncrement = 2f;
	readonly float slideMax = 100;
	
	int _score;
	readonly int chainMax = 20;
	bool scoreDisplay;
	
	int displayScore;
	int displayMultiplier;
	int scoreState;
	
	float scoreTimer = 0;
	readonly float scoreIncrement = 1f;
	readonly float scoreIncrementMax = 150f;
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
	float multHeight = 30;

	int currentMultiplier = 1;
	int multiplierPoints = 15;
	int popupMultiplier = 0;

	// multiplier timers
	float lastMultiplierTime;
	float currentMultiplierTime;
	readonly float multiplierTimeLength = 5;

	// multiplier sliders
	bool multiplierIsChanging = false;
	float prevValue;
	float newValue;
	float multiplierSliderTime = 1;

	bool multiplierIsPositive = true;
	Texture2D currentMultiplierImage;
	Texture2D multiplierSign;
	Texture2D multiplierPointImage;
	
	
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
		BountyNPCImage = new Texture2D (1, 1);
		
		// bounty
		bountySizeX = (int)Screen.width * .2f;
		bountyLabelSizeY = 30;
		bountyRectSizeY = (int)Screen.height * .2f;
		
		popupIncrement = popupIncrementMax;
		popupIncrement2 = popupIncrementMax2;
		
		sideL = Screen.width / 3;
		sideR = Screen.width * 2 / 3;
		startHeight = Screen.height * 2 / 3;
		endHeight = 50;
		endX = multXOffset;
		
		highscores = GlobalGameStateManager.highscores;
		names = GlobalGameStateManager.playerNames;
		
		// mult variabes
		multLength = Screen.width / 5;
		lastMultiplierTime = Time.time;
		currentMultiplierTime = Time.time;

		currentMultiplierImage = getNumberImage (currentMultiplier);
		multiplierSign = plusSign;
		multiplierPointImage = numZero;
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
		currentMultiplierImage = getNumberImage (currentMultiplier);
		updateMultiplier ();
		
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
			if( slideIncrement <= .4f * slideMax )
			{
				popupAlpha = GUI.color.a * popupIncrement / (.4f * popupIncrementMax );
				guiColor.a = popupAlpha;
				GUI.color = guiColor;
			}
			
			// update sliding increment
			if( slideIncrement < slideMax )
				slideIncrement += 2f;
			
			// npc type
			if( eatenString == npcEatenString )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(sideL, startHeight, popupX, popupY), eatenString, myStyle);
				//myStyle.alignment = TextAnchor.MiddleRight;
				//GUI.Label(new Rect(sideR, startHeight, popupX, popupY), "" + NPC_EATEN, myStyle);
			}
			else if( eatenString == bountyEatenString )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(sideL, startHeight, popupX, popupY), eatenString, myStyle);
				//myStyle.alignment = TextAnchor.MiddleRight;
				//GUI.Label(new Rect(sideR, startHeight, popupX, popupY), "" + BOUNTY_EATEN, myStyle);
			}
			else // axeman
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(sideL, startHeight, popupX, popupY), eatenString, myStyle);
				//myStyle.alignment = TextAnchor.MiddleRight;
				//GUI.Label(new Rect(sideR, startHeight, popupX, popupY), "" + AXEMAN_EATEN, myStyle);
			}
			// stealth
			if( isStealth )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), stealthString, myStyle);
				//myStyle.alignment = TextAnchor.MiddleRight;
				//GUI.Label(new Rect(sideR, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), stealthMultiplier + "x", myStyle);
				offset++;
			}
			// lured
			if( isLured )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), luredString, myStyle);
				//myStyle.alignment = TextAnchor.MiddleRight;
				//GUI.Label(new Rect(sideR, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), lureMultiplier + "x", myStyle);
				offset++;
			}
			// quick
			if( isQuick )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), streakString, myStyle);
				//myStyle.alignment = TextAnchor.MiddleRight;
				//GUI.Label(new Rect(sideR, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), droppingFliesMultiplier + "x", myStyle);
				offset++;
			}
			// streak
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Current Streak " + displayMultiplier + "x", myStyle);
			//myStyle.alignment = TextAnchor.MiddleRight;
			//GUI.Label(new Rect(sideR, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), displayMultiplier + "x", myStyle);
			offset++;
			
			// score
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Eating Score: " + displayScore, myStyle);
			offset++;
			
			// multiplier
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Multiplier: " + currentMultiplier, myStyle);
			offset++;
			
			// total score
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(sideL, startHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Total Score: " + (displayScore * currentMultiplier), myStyle);
			offset++;
			
			// update time left
			scoreTimer += scoreIncrement;
			
			if( scoreTimer > scoreIncrementMax )
			{
				scoreState = (int)ScoreState.MOVE_SCORING;
				moveStartTime = Time.time;
			}
		}
		else if( scoreState == (int)ScoreState.MOVE_SCORING )
		{
			float currentFontSize;

			scoreTimer = 0;
			
			float fracTime = (Time.time - moveStartTime) / 2f;
			float xPos = Mathf.Lerp(sideL, endX, fracTime);
			float yPos = Mathf.Lerp(startHeight, endHeight, fracTime);
			
			int offset = 1;
			
			// npc type
			if( eatenString == npcEatenString )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(xPos, yPos, popupX, popupY), eatenString, myStyle);
			}
			else if( eatenString == bountyEatenString )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(xPos, yPos, popupX, popupY), eatenString, myStyle);
			}
			else // axeman
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(xPos, yPos, popupX, popupY), eatenString, myStyle);
			}
			// stealth
			if( isStealth )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), stealthString, myStyle);
				offset++;
			}
			// lured
			if( isLured )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), luredString, myStyle);
				offset++;
			}
			// quick
			if( isQuick )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), streakString, myStyle);
				offset++;
			}
			// streak
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), "Current Streak " + displayMultiplier + "x", myStyle);
			offset++;
			
			// score
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), "Eating Score: " + displayScore, myStyle);
			offset++;
			
			// multiplier
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), "Multiplier: " + currentMultiplier, myStyle);
			offset++;
			
			// total score
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(xPos, yPos + offset*(myStyle.fontSize+2), popupX, popupY), "Total Score: " + (displayScore * currentMultiplier), myStyle);
			offset++;
			
			if( xPos < endX + .1f )
				scoreState = (int)ScoreState.STOP_SCORING;
		}
		else if( scoreState == (int)ScoreState.STOP_SCORING )
		{
			int offset = 1;
			
			// npc type
			if( eatenString == npcEatenString )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(endX, endHeight, popupX, popupY), eatenString, myStyle);
			}
			else if( eatenString == bountyEatenString )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(endX, endHeight, popupX, popupY), eatenString, myStyle);
			}
			else // axeman
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(endX, endHeight, popupX, popupY), eatenString, myStyle);
			}
			// stealth
			if( isStealth )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), stealthString, myStyle);
				offset++;
			}
			// lured
			if( isLured )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), luredString, myStyle);
				offset++;
			}
			// quick
			if( isQuick )
			{
				myStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), streakString, myStyle);
				offset++;
			}
			// streak
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Current Streak " + displayMultiplier + "x", myStyle);
			offset++;
			
			// score
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Eating Score: " + displayScore, myStyle);
			offset++;
			
			// multiplier
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Multiplier: " + currentMultiplier, myStyle);
			offset++;
			
			// total score
			myStyle.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(endX, endHeight + offset*(myStyle.fontSize+2), popupX, popupY), "Total Score: " + (displayScore * currentMultiplier), myStyle);
			offset++;
			
			// update time left
			scoreTimer += scoreIncrement;
			
			if( scoreTimer > scoreIncrementMax )
				scoreState = (int)ScoreState.END_SCORING;
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
			isLured = false;
			isStealth = false;
			isQuick = false;
			
			scoreTimer = 0;
			scoreState = (int)ScoreState.NO_SCORING;
			displayScore = 0;
			displayMultiplier = 0;
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
			/*
			// score pop-up text
			myStyle.normal.textColor = Color.black;
			GUI.Label(new Rect(Screen.width/2 - popupX/2, Screen.height/2 - popupY/2 - popupIncrement2, popupX, popupY), "" + displayScore, myStyle);*/
			
			// multiplier pop-up text
			myStyle.normal.textColor = Color.red;
			myStyle.fontSize = 100;
			//GUI.Label(new Rect(Screen.width/2 - popupX/2 + 25, Screen.height/2 - popupY/2 - popupIncrement2 + 75, popupX, popupY), "" + popupMultiplier, myStyle);	
			GUI.DrawTexture(new Rect(Screen.width/2 - popupX/2, Screen.height/2 - popupY/2 - popupIncrement2 + 75, popupX, popupY), multiplierSign, ScaleMode.ScaleToFit);
			GUI.DrawTexture(new Rect(Screen.width/2 - popupX/2 + 25, Screen.height/2 - popupY/2 - popupIncrement2 + 75, popupX, popupY), multiplierPointImage, ScaleMode.ScaleToFit);
			
			// increment height
			popupIncrement2 += 1.5f;
			
			// check statement for pop-up statements
			if( popupIncrement2 > popupIncrementMax2 )
			{
				scoreDisplay = false;
				popupIncrement2 = 0;
			}
			
			
			GUI.color = savedGuiColor; // revert to previous alpha
			/*
			// score reason string
			myStyle.fontSize = 30;
			myStyle.normal.textColor = Color.white;
			GUI.Label(new Rect(Screen.width/2, Screen.height/2 - popupY/2 + 150, popupX, popupY), "" + displayScoreString, myStyle);*/
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
			
			GUI.Box (new Rect (Screen.width-offsetX, Screen.height-offsetY-myStyle.fontSize-10-popupIncrement, sizeX, sizeY), "" + (displayScore * currentMultiplier), myStyle);
			
			popupIncrement += 1f;
			
			GUI.color = savedGuiColor; // revert to previous alpha
		}
		
		// score
		myStyle.fontSize = 30;
		myStyle.normal.textColor = Color.white;
		myStyle.alignment = TextAnchor.MiddleRight;
		GUI.Box (new Rect (Screen.width-offsetX, Screen.height-offsetY, sizeX, sizeY), "SCORE: " + _score, myStyle);
		
		// high score
		myStyle.normal.textColor = Color.yellow;
		GUI.Box (new Rect (Screen.width-offsetX, 10, sizeX, sizeY), "HIGH SCORE: " + highscores[0], myStyle);
		
		
		
		
		
		/***** Bounty GUI *****/
		
		// probably should use switch-case, but meh
		if( bountyState == (int)BountyState.BOUNTY_HIDDEN )
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height,bountySizeX,bountyRectSizeY), BountyNPCImage);
		}
		else if( bountyState == (int)BountyState.BOUNTY_SHOWN )
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyRectSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyRectSizeY,bountySizeX,bountyRectSizeY), BountyNPCImage);
		}
		else if( bountyState == (int)BountyState.BOUNTY_SHOWING )
		{
			if( bountyIncrement >= bountyRectSizeY )
			{
				bountyState = (int)BountyState.BOUNTY_SHOWN;
				bountyIncrement = bountyRectSizeY;
			}
			else
			{
				bountyIncrement += 2f;
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyIncrement,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyIncrement,bountySizeX,bountyRectSizeY), BountyNPCImage);
			}
			
			
		}
		else if( bountyState == (int)BountyState.BOUNTY_HIDING )
		{
			if( bountyIncrement <= 0 )
			{
				bountyState = (int)BountyState.BOUNTY_HIDDEN;
				bountyIncrement = 0;
			}
			else
			{
				bountyIncrement -= 2f;
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyIncrement,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyIncrement,bountySizeX,bountyRectSizeY), BountyNPCImage);
			}
			
		}
		else // default - BOUNTY_HIDDEN
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height,bountySizeX,bountyRectSizeY), BountyNPCImage);
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
		
		myStyle.fontSize = 50;

		GUI.Box (new Rect (multXOffset, multYOffset, multLength, multHeight), "");
		//GUI.Label(new Rect (multXOffset + multLength + 10, multYOffset, 50, multHeight), currentMultiplier + "x", myStyle);
		GUI.DrawTexture (new Rect (multXOffset + multLength + 10, multYOffset, 50, multHeight), currentMultiplierImage, ScaleMode.ScaleToFit);

		if( multiplierIsChanging )
		{
			// change(slide) data
			if( Time.time - lastMultiplierTime <= multiplierSliderTime )
			{
				float timeRatio = (Time.time - lastMultiplierTime) / multiplierSliderTime;
				float tempRatio = (newValue - prevValue) * timeRatio;

				GUI.Box (new Rect (multXOffset, multYOffset+2, ((prevValue + tempRatio) % (float)multIncre) / (float)multIncre * (multLength - 4), multHeight-4), "");
			}
			else
			{
				multiplierIsChanging = false;

				if( multiplierPoints % multIncre != 0 )
					GUI.Box (new Rect (multXOffset, multYOffset+2, (multiplierPoints % multIncre) / (float)multIncre * (multLength - 4), multHeight-4), "");
			}
		}
		else
		{
			if( multiplierPoints % multIncre != 0 )
				GUI.Box (new Rect (multXOffset, multYOffset+2, (multiplierPoints % multIncre) / (float)multIncre * (multLength - 4), multHeight-4), "");
		}
	}
	
	
	
	
	
	
	
	// update multiplier
	void addMultiplier(int num)
	{
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
		multiplierSign = plusSign;
		multiplierPointImage = getNumberImage (num);
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
		multiplierSign = minusSign;
		multiplierPointImage = getNumberImage (num);
	}
	
	// reset multiplier
	void resetMultiplier()
	{
		//subMultiplier (multiplierPoints);
		multiplierPoints = 0;
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

	// get corresponding image from number
	Texture2D getNumberImage(int num)
	{
		//Debug.Log ("num image: " + num);

		switch(num)
		{
		case 0:
			return numZero;
		case 1:
			return numOne;
		case 2:
			return numTwo;
		case 3:
			return numThree;
		case 4:
			return numFour;
		case 5:
			return numFive;
		case 6:
			return numSix;
		case 7:
			return numSeven;
		case 8:
			return numEight;
		case 9:
			return numNine;
		default:
			return numZero;
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
		MessageCenter.Instance.UnregisterListener (MessageType.LevelFinished, HandleLevelFinished);
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
		
		if( mess.Lure.lurePower >= NPC.GetComponent<AIController>().lurePower )
		{
			//luredNPCs.Add (NPC);
		}
		
		luredNPCs.Add (NPC);
	}
	
	
	
	
	
	void HandleGrabbedNPCs(Message message)
	{
		bountyState = (int)BountyState.BOUNTY_HIDDEN;
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
			resetMultiplier();
			if( !alertedNPCs.Contains(mess.NPC) )
				alertedNPCs.Add(mess.NPC);
		}
		else if( mess.alertLevelType == AlertLevelType.Scared )
		{
			// add lured/scared multiplier
			addMultiplier(scaredNPCMultiplier);
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
			BountyNPCImage = new Texture2D(1,1);
		}
	}
	
	
	
	
	
	void HandleNPCEaten(Message message)
	{
		NPCEatenMessage mess = message as NPCEatenMessage;
		
		if( mess.NPC == null )
			return;
		
		// initial multiplier values
		int lure = 1;
		int quick = 1;
		int stealth = 1;
		int npcScore = NPC_EATEN;
		eatenString = npcEatenString;
		
		int tmpMultiplier = 0;
		
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
			BountyNPCImage = new Texture2D(1,1);
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
		
		
		if( tmpMultiplier != 0 )
			addMultiplier (tmpMultiplier);
		currentMultiplier = 1 + multiplierPoints / multIncre;
		addScore(displayScore*currentMultiplier);
		
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

}
