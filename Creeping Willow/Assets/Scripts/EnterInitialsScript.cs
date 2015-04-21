using UnityEngine;
using System.Collections;

/// <summary>
/// Player's current state of putting in the intials
/// </summary>
public enum enterInitialState
{
	FIRST_LETTER,
	SECOND_LETTER,
	THIRD_LETTER,
	SUBMIT
}

public class EnterInitialsScript : MonoBehaviour {

	// public variables (arrows)
	public Texture2D upArrow;
	public Texture2D downArrow;

	// texture positioning and information variables
	bool pressed = false;
	bool submitted = false;

	float width;
	float height;

	float xSize;
	float ySize;
	float arrowYSize;
	float labelXSize;
	float labelYSize;

	float centerStart;
	float leftStart;
	float rightStart;
	float submitXStart;
	float labelXStart;

	float topButtonStart;
	float midLetterStart;
	float bottomButtonStart;
	float submitYStart;
	float labelYStart;

	string labelStr = "enter initials for high score";
	
	int letter1 = 0;
	int letter2 = 0;
	int letter3 = 0;


	// state variables
	int currentState = (int)enterInitialState.FIRST_LETTER;
	bool controllerInUse = false;
	float changeThreshold = .25f;
	float restThreshold = .1f;


	// Use this for initialization
	void Start () {
		width = Screen.width;
		height = Screen.height;

		xSize = width / 15f;
		ySize = xSize * 5f / 3f;
		arrowYSize = ySize * .5f;

		centerStart = width * .5f - xSize * .5f;
		leftStart = width * .5f - xSize * 2f;
		rightStart = width * .5f + xSize * 1f;
		submitXStart = width * .5f + xSize * 3f;

		midLetterStart = height * .5f - ySize * .5f;
		topButtonStart = height * .5f - ySize;
		bottomButtonStart = height * .5f + ySize *.5f ;
		submitYStart = height * .5f - arrowYSize * .5f;

		labelXSize = width * .5f / (float)labelStr.Length;
		labelYSize = labelXSize * 5f / 3f;
		labelXStart = width * .5f - labelXSize * labelStr.Length * .5f;
		labelYStart = height * .1f;

	}
	
	// Update is called once per frame
	void Update () {
		// temporary (will be listening for a message)
		if( Input.GetButtonDown("Start") )
			pressed = true;

		// make sure left thumbstick is close to a resting state
		if( controllerInUse && Input.GetAxis("LSX") < restThreshold && Input.GetAxis("LSX") > -restThreshold && Input.GetAxis("LSY") < restThreshold && Input.GetAxis("LSY") > -restThreshold )
			controllerInUse = false;

		if( pressed && !controllerInUse && !submitted )
		{
			// check which letter the player is on
			if( currentState == (int)enterInitialState.FIRST_LETTER )
			{
				if( Input.GetAxis("LSY") > changeThreshold )
				{
					if( letter1 >= 25 )
						letter1 = 0;
					else
						letter1++;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSY") < -changeThreshold )
				{
					if( letter1 <= 0 )
						letter1 = 25;
					else
						letter1--;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSX") > changeThreshold )
				{
					currentState = (int)enterInitialState.SECOND_LETTER;
					controllerInUse = true;
				}
			}
			else if( currentState == (int)enterInitialState.SECOND_LETTER )
			{
				if( Input.GetAxis("LSY") > changeThreshold )
				{
					if( letter2 >= 25 )
						letter2 = 0;
					else
						letter2++;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSY") < -changeThreshold )
				{
					if( letter2 <= 0 )
						letter2 = 25;
					else
						letter2--;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSX") > changeThreshold )
				{
					currentState = (int)enterInitialState.THIRD_LETTER;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSX") < -changeThreshold )
				{
					currentState = (int)enterInitialState.FIRST_LETTER;
					controllerInUse = true;
				}
			}
			else if( currentState == (int)enterInitialState.THIRD_LETTER )
			{
				if( Input.GetAxis("LSY") > changeThreshold )
				{
					if( letter3 >= 25 )
						letter3 = 0;
					else
						letter3++;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSY") < -changeThreshold )
				{
					if( letter3 <= 0 )
						letter3 = 25;
					else
						letter3--;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSX") > changeThreshold )
				{
					currentState = (int)enterInitialState.SUBMIT;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSX") < -changeThreshold )
				{
					currentState = (int)enterInitialState.SECOND_LETTER;
					controllerInUse = true;
				}
			}
			else if( currentState == (int)enterInitialState.SUBMIT )
			{
				// temporary (will broadcast a message)
				if( Input.GetButtonDown("A") )
				{
					submitted = true;
					controllerInUse = true;
				}
				if( Input.GetAxis("LSX") < -changeThreshold )
				{
					currentState = (int)enterInitialState.THIRD_LETTER;
					controllerInUse = true;
				}
			}
		}
	}

	void OnGUI() {

		if( pressed )
		{
			if( !submitted ) // temporary
				FontConverter.instance.parseStringToTextures(labelXStart,labelYStart,labelXSize,labelYSize,labelStr);

			changeGUIAlpha(.7f);

			// left side
			if( currentState == (int)enterInitialState.FIRST_LETTER )
			{
				changeGUIAlpha(.25f);
				if( Input.GetAxis("LSY") > changeThreshold )
					changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(leftStart, topButtonStart, xSize, arrowYSize), upArrow );
				
				changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(leftStart, midLetterStart, xSize, ySize), getTexture(1) );
				changeGUIAlpha(.25f);

				if( Input.GetAxis("LSY") < -changeThreshold )
					changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(leftStart, bottomButtonStart, xSize, arrowYSize), downArrow );
				changeGUIAlpha(.7f);
			}
			else
				GUI.DrawTexture(new Rect(leftStart, midLetterStart, xSize, ySize), getTexture(1) );

			// center
			if( currentState == (int)enterInitialState.SECOND_LETTER )
			{
				changeGUIAlpha(.25f);
				if( Input.GetAxis("LSY") > changeThreshold )
					changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(centerStart, topButtonStart, xSize, arrowYSize), upArrow );

				changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(centerStart, midLetterStart, xSize, ySize), getTexture(2) );
				changeGUIAlpha(.25f);

				if( Input.GetAxis("LSY") < -changeThreshold )
					changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(centerStart, bottomButtonStart, xSize, arrowYSize), downArrow );
				changeGUIAlpha(.7f);
			}
			else
				GUI.DrawTexture(new Rect(centerStart, midLetterStart, xSize, ySize), getTexture(2) );

			// right side
			if( currentState == (int)enterInitialState.THIRD_LETTER )
			{
				changeGUIAlpha(.25f);
				if( Input.GetAxis("LSY") > changeThreshold )
					changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(rightStart, topButtonStart, xSize, arrowYSize), upArrow );

				changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(rightStart, midLetterStart, xSize, ySize), getTexture(3) );
				changeGUIAlpha(.25f);

				if( Input.GetAxis("LSY") < -changeThreshold )
					changeGUIAlpha(1f);
				GUI.DrawTexture(new Rect(rightStart, bottomButtonStart, xSize, arrowYSize), downArrow );
				changeGUIAlpha(.7f);
			}
			else
				GUI.DrawTexture(new Rect(rightStart, midLetterStart, xSize, ySize), getTexture(3) );

			// submit button
			if( currentState == (int)enterInitialState.SUBMIT && !submitted )
			{
				changeGUIAlpha(1f);
				FontConverter.instance.parseStringToTextures(submitXStart,submitYStart,xSize*1.5f/6f,arrowYSize,"submit");
			}
			else
				FontConverter.instance.parseStringToTextures(submitXStart,submitYStart,xSize*1.5f/6f,arrowYSize,"submit",.7f);
		}
		else // temporary
		{
			FontConverter.instance.parseStringToTextures(width*.5f-xSize*5.5f,height*.5f-ySize*.5f,xSize,ySize,"press start");
		}
	}

	void changeGUIAlpha(float a)
	{
		Color alpha = GUI.color;
		alpha.a = a;
		GUI.color = alpha;
	}

	Texture2D getTexture(int num)
	{
		int letter;

		// get which position to return
		switch( num )
		{
			case 1:
				letter = letter1;
				break;
			case 2:
				letter = letter2;
				break;
			case 3:
				letter = letter3;
				break;
			default:
				letter = letter1;
				break;
		}


		switch(letter)
		{
			case 0:
				return FontConverter.instance.A;
			case 1:
				return FontConverter.instance.B;
			case 2:
				return FontConverter.instance.C;
			case 3:
				return FontConverter.instance.D;
			case 4:
				return FontConverter.instance.E;
			case 5:
				return FontConverter.instance.F;
			case 6:
				return FontConverter.instance.G;
			case 7:
				return FontConverter.instance.H;
			case 8:
				return FontConverter.instance.I;
			case 9:
				return FontConverter.instance.J;
			case 10:
				return FontConverter.instance.K;
			case 11:
				return FontConverter.instance.L;
			case 12:
				return FontConverter.instance.M;
			case 13:
				return FontConverter.instance.N;
			case 14:
				return FontConverter.instance.O;
			case 15:
				return FontConverter.instance.P;
			case 16:
				return FontConverter.instance.Q;
			case 17:
				return FontConverter.instance.R;
			case 18:
				return FontConverter.instance.S;
			case 19:
				return FontConverter.instance.T;
			case 20:
				return FontConverter.instance.U;
			case 21:
				return FontConverter.instance.V;
			case 22:
				return FontConverter.instance.W;
			case 23:
				return FontConverter.instance.X;
			case 24:
				return FontConverter.instance.Y;
			case 25:
				return FontConverter.instance.Z;
			default:
				return FontConverter.instance.A;
		}
	}

	string getLetter(int num)
	{
		switch(num)
		{
			case 0:
				return "A";
			case 1:
				return "B";
			case 2:
				return "C";
			case 3:
				return "D";
			case 4:
				return "E";
			case 5:
				return "F";
			case 6:
				return "G";
			case 7:
				return "H";
			case 8:
				return "I";
			case 9:
				return "J";
			case 10:
				return "K";
			case 11:
				return "L";
			case 12:
				return "M";
			case 13:
				return "N";
			case 14:
				return "O";
			case 15:
				return "P";
			case 16:
				return "Q";
			case 17:
				return "R";
			case 18:
				return "S";
			case 19:
				return "T";
			case 20:
				return "U";
			case 21:
				return "V";
			case 22:
				return "W";
			case 23:
				return "X";
			case 24:
				return "Y";
			case 25:
				return "Z";
			default:
				return "A";
		}
	}

	string getSubmittedInitials()
	{
		return getLetter(letter1) + getLetter(letter2) + getLetter(letter3);
	}
}
