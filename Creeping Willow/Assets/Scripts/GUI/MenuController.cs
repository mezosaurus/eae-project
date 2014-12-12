using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
	public GameObject[] buttons;
	public AudioClip sound;

	private bool axisBusy;
	private int selected;
	private int fullColumns;
	private AudioSource mapAudio;

	bool resting;
	bool canUse;
	float curTime;
	float endTime;

	void Awake()
	{
		canUse = true;
		MessageCenter.Instance.RegisterListener (MessageType.ScoreAdding, HandleScoreAdding);
		selected = 0;
		axisBusy = false;
		
		fullColumns = Mathf.CeilToInt( Mathf.Sqrt( buttons.Length ) );
		
		mapAudio = gameObject.AddComponent<AudioSource>();
		mapAudio.clip = sound;

		Select( buttons[ 0 ] );
	}

	void Start()
	{
		resting = true;
		curTime = 0;
		endTime = 1;
	}

	void OnDestroy()
	{
		MessageCenter.Instance.UnregisterListener (MessageType.ScoreAdding, HandleScoreAdding);
	}

	void HandleScoreAdding(Message message)
	{
		ScoreAddingMessage mess = message as ScoreAddingMessage;
		canUse = !mess.adding;
	}

	// Update is called once per frame
	void Update()
	{
		// wait for player to save high score
		if( !canUse )
			return;

		// ignore quick inputs at start of menu
		if( resting )
		{
			if( curTime < endTime )
				curTime += .04f;
			else
				resting = false;
			return;
		}

			if (Input.GetMouseButtonDown (0)) {
					axisBusy = false;
			} else if (Input.GetAxis ("LSX") != 0) {
					if (!axisBusy) {
							if (selected >= 0) {
									if (Input.GetAxisRaw ("LSX") < 0) {
											if (selected % fullColumns == 0) {
													selected += (fullColumns - 1);
											} else {
													selected--;
											}

											while (selected >= buttons.Length) {
													selected--;
											}
									} else {
											selected++;

											if (selected % fullColumns == 0) {
													selected -= fullColumns;
											}

											if (selected > buttons.Length - 1) {
													while (selected % fullColumns != 0) {
															selected++;
													}
													selected -= fullColumns;
											}
									}
							} else {
									selected = 0;
							}

							Screen.showCursor = false;
							Screen.lockCursor = true;
							axisBusy = true;
							Select (buttons [selected]);
					}
			} else if (Input.GetAxis ("LSY") != 0) {
					if (!axisBusy) {
							if (selected >= 0) {
									if (Input.GetAxisRaw ("LSY") < 0) {
											selected -= fullColumns;

											if (selected < 0) {
													selected += fullColumns * fullColumns;
											}

											if (selected >= buttons.Length) {
													selected -= fullColumns;
											}
									} else {
											selected += fullColumns;

											if (selected >= buttons.Length) {
													selected = selected % fullColumns;
											}
									}
							} else {
									selected = 0;
							}

							Screen.showCursor = false;
							Screen.lockCursor = true;
							axisBusy = true;
							Select (buttons [selected]);
					}
			} else {
					axisBusy = false;
			}

		if ((Input.GetAxisRaw ("Start") != 0 || Input.GetAxisRaw( "A" ) != 0) && selected >= 0) {
					Screen.showCursor = true;
					Screen.lockCursor = false;
					buttons [selected].GetComponent<GUIButton> ().changeScenes ();
			}
		else if ((Input.GetAxisRaw ("Back") != 0 || Input.GetAxisRaw( "A" ) != 0) & selected >= 0) {
					// this all depends on the build order in project settings
					if (Application.loadedLevel == 0) {
							buttons[3].GetComponent<GUIButton>().changeScenes();
					}
					else if (Application.loadedLevel == 1) {
							buttons[0].GetComponent<GUIButton>().changeScenes();
					}
			}
	}

	private void Select (GameObject button)
	{
			UnselectAll ();

			button.GetComponent<GUIButton> ().defaultImage = button.GetComponent<GUIButton> ().hoverImage;
			mapAudio.PlayOneShot (sound);
	}

	private void UnselectAll ()
	{
			foreach (GameObject button in buttons) {
					button.GetComponent<GUIButton> ().defaultImage = button.GetComponent<GUIButton> ().downClickImage;
			}
	}
}
