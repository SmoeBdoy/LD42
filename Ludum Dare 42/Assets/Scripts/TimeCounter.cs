using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeCounter : MonoBehaviour {

	PopulationMovement populationMovement;

	public float time;
	public Text timeDisplay;

	bool endGameOnce = false;

	public Text finalPopulation;
	public GameObject endGameMenu;

	public AudioSource backgroundMusic;
	public AudioSource[] allOtherSounds;

	void Start ()
	{
		populationMovement = GetComponent<PopulationMovement> ();

		endGameMenu.SetActive (false);
	}

	void Update ()
	{
		time -= Time.deltaTime;
		timeDisplay.text = time.ToString ("F1");

		if (time <= 0 && !endGameOnce) {
			endGameOnce = true;
			EndGame ();
		}

		if (endGameOnce) {
			backgroundMusic.volume -= (Time.deltaTime / 4);
			for (int i = 0; i < allOtherSounds.Length; i++) {
				allOtherSounds [i].volume = 0f;
			}
		}
	}

	void EndGame ()
	{
		endGameMenu.SetActive (true);
		finalPopulation.text = populationMovement.totalPopulation.ToString ();
	}

	public void Quit ()
	{
		Application.Quit ();
	}

	public void Restart ()
	{
		SceneManager.LoadScene ("Main");
	}
}
