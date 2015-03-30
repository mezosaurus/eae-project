using UnityEngine;
using System.Collections;

public class FontConverter : MonoBehaviour {

	// public variables storing the textures of the font
	public Texture2D A;
	public Texture2D B;
	public Texture2D C;
	public Texture2D D;
	public Texture2D E;
	public Texture2D F;
	public Texture2D G;
	public Texture2D H;
	public Texture2D I;
	public Texture2D J;
	public Texture2D K;
	public Texture2D L;
	public Texture2D M;
	public Texture2D N;
	public Texture2D O;
	public Texture2D P;
	public Texture2D Q;
	public Texture2D R;
	public Texture2D S;
	public Texture2D T;
	public Texture2D U;
	public Texture2D V;
	public Texture2D W;
	public Texture2D X;
	public Texture2D Y;
	public Texture2D Z;
	public Texture2D ZERO;
	public Texture2D ONE;
	public Texture2D TWO;
	public Texture2D THREE;
	public Texture2D FOUR;
	public Texture2D FIVE;
	public Texture2D SIX;
	public Texture2D SEVEN;
	public Texture2D EIGHT;
	public Texture2D NINE;
	public Texture2D PLUS;
	public Texture2D MINUS;
	public Texture2D SPACE;



	/// <summary>
	/// Return a texture resembling the string passed in as a parameter
	/// </summary>
	/// <param name="startX">Start x</param>
	/// <param name="startY">Start y</param>
	/// <param name="sizeX">Length x</param>
	/// <param name="sizeY">Length y</param>
	/// <param name="text">String to Parse</param>
	public void parseStringToTextures(float startX, float startY, float sizeX, float sizeY, string text)
	{
		float offset = 0;

		for( int i = 0; i < text.Length; i++ )
		{
			GUI.DrawTexture(new Rect (startX + offset, startY, sizeX, sizeY), getTexture( text.Substring(i,1)) );
			offset += sizeX;
		}
	}

	/// <summary>
	/// Return a texture resembling the string passed in as a parameter anchored to the right
	/// </summary>
	/// <param name="startX">Start x</param>
	/// <param name="startY">Start y</param>
	/// <param name="sizeX">Length x</param>
	/// <param name="sizeY">Length y</param>
	/// <param name="text">String to Parse</param>
	public void rightAnchorParseStringToTextures(float startX, float startY, float sizeX, float sizeY, string text)
	{
		float offset = 0;
		
		for( int i = text.Length - 1; i >= 0; i-- )
		{
			GUI.DrawTexture(new Rect (startX + offset, startY, sizeX, sizeY), getTexture( text.Substring(i,1)) );
			offset -= sizeX;
		}
	}


	private Texture2D getTexture(string symbol)
	{
		symbol = symbol.ToUpper(); // make sure letter is not case sensitive

		switch( symbol )
		{
			case "0":
				return ZERO;
			case "1":
				return ONE;
			case "2":
				return TWO;
			case "3":
				return THREE;
			case "4":
				return FOUR;
			case "5":
				return FIVE;
			case "6":
				return SIX;
			case "7":
				return SEVEN;
			case "8":
				return EIGHT;
			case "9":
				return NINE;
			case "A":
				return A;
			case "B":
				return B;
			case "C":
				return C;
			case "D":
				return D;
			case "E":
				return E;
			case "F":
				return F;
			case "G":
				return G;
			case "H":
				return H;
			case "I":
				return I;
			case "J":
				return J;
			case "K":
				return K;
			case "L":
				return L;
			case "M":
				return M;
			case "N":
				return N;
			case "O":
				return O;
			case "P":
				return P;
			case "Q":
				return Q;
			case "R":
				return R;
			case "S":
				return S;
			case "T":
				return T;
			case "U":
				return U;
			case "V":
				return V;
			case "W":
				return W;
			case "X":
				return X;
			case "Y":
				return Y;
			case "Z":
				return Z;
			case "+":
				return PLUS;
			case "-":
				return MINUS;
			default: // return a space
				return SPACE;
		}
	}






	// singleton setup
	private static FontConverter _instance;
	public static FontConverter instance
	{
		get
		{
			if( _instance == null )
				_instance = GameObject.FindObjectOfType<FontConverter>();
			return _instance;
		}
	}

}
