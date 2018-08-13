using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NukeController : MonoBehaviour {

	GameObject[] locations;

	public AudioSource nukeIncomingSound;

	GameObject oldLocationWithHighestPopulation;
	public GameObject locationWithHighestPopulation;
	public float timeBeforeNuking;
	float nukeCounter;
	public float minimumTimeBetweenNukes;
	float counterDown;

	void Start ()
	{
		locations = GameObject.FindGameObjectsWithTag ("Location");
		counterDown = timeBeforeNuking;
	}

	void Update ()
	{
		counterDown -= Time.deltaTime;

		if (counterDown <= 0) {
			DetectHighestNumber ();
			if (!locationWithHighestPopulation.GetComponent<Location> ().almostNuked) {
				locationWithHighestPopulation.GetComponent<Location> ().almostNuked = true;
				locationWithHighestPopulation.GetComponent<SpriteRenderer> ().color = Color.red;
				nukeIncomingSound.Play ();
				if (timeBeforeNuking <= minimumTimeBetweenNukes) {
					timeBeforeNuking = minimumTimeBetweenNukes;
				} else {
					timeBeforeNuking--;
				}
				counterDown = timeBeforeNuking;
			} else {
				DetectHighestNumber ();
			}
		}
	}

	public void DetectHighestNumber ()
	{
		int max = 0;
		for (int i = 0; i < locations.Length; i++) {
			if (locations [i].GetComponent<Location> ().population > max) {
				oldLocationWithHighestPopulation = locationWithHighestPopulation;
				max = locations [i].GetComponent<Location> ().population;
				locationWithHighestPopulation = locations [i];
				if (oldLocationWithHighestPopulation == locationWithHighestPopulation) {
					locationWithHighestPopulation = locations [i + 1];
				}
			}
		}
	}


}
