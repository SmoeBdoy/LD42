using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour {

	ColorPicker colorPicker;
	GameObject gameController;
	PopulationMovement populationMovement;
	public AudioSource explosion;

	PolygonCollider2D col;
	SpriteRenderer sp;
	bool chosenColor = false;
	int chosenNumber;

	bool nukedOnce = false;
	public bool nuked = false;
	public bool almostNuked = false;
	public float nukedCounter = 10f;

	Color firstPickedColor;
	Color alphaLessColor;

	public Vector2 center;

	public GameObject selectionThing;
	public bool selected = false;

	public GameObject populationItemToSpawn;

	public GameObject canvasChild;
	public Text populationText;
	public int population;

	void Start ()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController");
		colorPicker = gameController.GetComponent<ColorPicker> ();
		populationMovement = gameController.GetComponent<PopulationMovement> ();

		sp = GetComponent<SpriteRenderer> ();
		col = GetComponent<PolygonCollider2D> ();

		population = 100;
		populationMovement.totalPopulation += population;

		selectionThing.SetActive (false);

		float centerX = 0f;
		float centerY = 0f;
		for (int i = 0; i < col.points.Length; i++) {
			float centerXX = col.points [i].x;
			centerX += centerXX;

			float centerYY = col.points [i].y;
			centerY += centerYY;
		}

		float sumX = centerX / col.points.Length;
		float sumY = centerY / col.points.Length;

		center = new Vector2 (sumX, sumY);
		//canvasChild.GetComponent<RectTransform>().position = new Vector3 (center.x, center.y, 0);

		populationText.color = Color.white;
	}

	void Update ()
	{
		if (almostNuked) {
			nukedCounter -= Time.deltaTime;
			if (nukedCounter <= 0) {
				nuked = true;
			}
		}
		if (nuked && !nukedOnce) {
			nukedOnce = true;
			sp.color = Color.black;
			explosion.Play ();
		}

		// Pick color
		if (!chosenColor) {
			chosenNumber = Random.Range (0, colorPicker.blues.Length);
			if (colorPicker.chosenNumbers.Contains (chosenNumber)) {
				chosenColor = true;
				colorPicker.chosenNumbers.Remove (chosenNumber);
				sp.color = colorPicker.blues [chosenNumber];
				firstPickedColor = sp.color;

				alphaLessColor = sp.color;
				alphaLessColor.a = 0.75f;
				sp.color = alphaLessColor;
			} else {
				chosenNumber = Random.Range (0, colorPicker.blues.Length);
			}
		}

		populationText.text = population.ToString ();

		if (Input.GetKeyDown (KeyCode.Escape) && selected && !populationMovement.confirmed) {
			selected = false;
			selectionThing.SetActive (false);
		}

		if (nuked) {
			if (population > 0) {
				populationMovement.totalPopulation -= population;
				population = 0;
			}
		}
	}

	void OnMouseExit ()
	{
		if (!nuked && !almostNuked) {
			sp.color = alphaLessColor;
		}
	}

	void OnMouseEnter ()
	{
		if (!nuked && !almostNuked) {
			sp.color = firstPickedColor;
		}
	}

	void OnMouseDown ()
	{
		if (!nuked) {
			if (!populationMovement.somethingIsSelected && !populationMovement.firstClicked) {
				// this would be the start location
				//populationMovement.somethingIsSelected = true;

				selectionThing.SetActive (true);
				selectionThing.transform.position = center;
				//selected = true;
			} else if (populationMovement.firstClicked) {
				// this would be the end location
				//populationMovement.confirmed = true;
				//selected = true;
				selectionThing.SetActive (false);
			}
		}
	}
}
