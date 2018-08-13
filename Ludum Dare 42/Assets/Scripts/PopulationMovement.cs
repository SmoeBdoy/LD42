using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationMovement : MonoBehaviour {

	public GameObject scrollRect;
	public GameObject dottedLineSpawnObject; // view port
	public RectTransform dottedLineBox;
	public GameObject rotationObject;
	float angle;

	public AudioSource clickLocation;

	public int totalPopulation;
	public Text totalPopulationText;

	public GameObject dot;
	public GameObject populationItemToSpawn;
	public GameObject[] vehicles;
	public VisualPopulationMovements[] vpm;
	public GameObject[] selectionIndicators;
	int vehicleNumberIndicator = 0;

//	public GameObject peopleToMoveTextBox;
//	public InputField peopleToMoveInput;
	public int peopleToMove;
//	public bool numberEnteringActive = false;

	public GameObject enteredTooManyPeople;
	public GameObject notEnoughSeating;


	public GameObject[] locations;
	public List<Location> locationScripts = new List<Location> ();
	public Location location1;
	public Location location2;

	public GameObject[] totalDots;
	public List<RectTransform> startPointsActive = new List<RectTransform>();
	GameObject startPoint;
	GameObject endPoint;
	GameObject line;
//	public List<RectTransform> endPointsActive = new List<RectTransform>();

	public bool somethingIsSelected = false;
	public bool confirmed;

	public bool firstClicked = false;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < vpm.Length; i++) {
			vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
		}

//		peopleToMoveTextBox.SetActive (false);

		ClickWalk ();

		enteredTooManyPeople.SetActive (false);
		notEnoughSeating.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (confirmed) {
			somethingIsSelected = false;
			confirmed = false;
		}
		totalPopulationText.text = totalPopulation.ToString ();

		// CHOOSE VEHICLE
		ChooseVehicle ();

		if (Input.GetMouseButtonDown (0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 mousePos2D = new Vector2 (mousePos.x, mousePos.y);

			RaycastHit2D hit = Physics2D.Raycast (mousePos2D, Vector2.zero);

			if (hit.collider != null) {
				// clicked on something

				if (hit.collider.tag == "Location" && !firstClicked) {
					location1 = hit.collider.GetComponent<Location> ();
					if (!location1.nuked) {
						clickLocation.pitch = 1.0f;
						clickLocation.Play ();
						firstClicked = true;
						location1.selected = true;
						startPoint = Instantiate (dot, location1.center, Quaternion.identity, location1.canvasChild.transform);
						startPointsActive.Add (startPoint.GetComponent<RectTransform> ());
						for (int i = 0; i < vpm.Length; i++) {
							vpm [i].startPoint = startPoint;
						}

						rotationObject.transform.position = location1.center;
					}

				} else if (hit.collider.tag == "Location" && firstClicked) {
					location2 = hit.collider.GetComponent<Location> ();
					if (!location2.nuked) {
						clickLocation.pitch = 1.5f;
						clickLocation.Play ();
						firstClicked = false;
//						numberEnteringActive = true;
						endPoint = Instantiate (dot, location2.center, Quaternion.identity, location2.canvasChild.transform);
//					endPointsActive.Add (endPoint.GetComponent<RectTransform> ());

						for (int i = 0; i < vpm.Length; i++) {
							vpm [i].locationToMoveTo = location2.center;
							vpm [i].endPoint = endPoint;
						}

//						peopleToMoveInput.text = "";
//						peopleToMove = 0;
//						peopleToMoveTextBox.SetActive (true);

						Vector2 pos = new Vector2 ((startPoint.transform.position.x + endPoint.transform.position.x) / 2,
							(endPoint.transform.position.y + startPoint.transform.position.y) / 2);
						
						rotationObject.transform.LookAt (new Vector3 (endPoint.transform.position.x, endPoint.transform.position.y, 0));
						if (rotationObject.transform.rotation.eulerAngles.y <= 90.1f) {
							angle = (rotationObject.transform.rotation.eulerAngles.x - 90f) * -1f;
						} else {
							angle = rotationObject.transform.rotation.eulerAngles.x - 90f;
						}
						Vector3 zAngles = dottedLineBox.rotation.eulerAngles;
						zAngles.z = angle;
						dottedLineBox.rotation = Quaternion.Euler (zAngles);
						
						line = Instantiate (dottedLineSpawnObject, pos, dottedLineBox.transform.rotation, scrollRect.transform);
						for (int i = 0; i < vpm.Length; i++) {
							vpm [i].line = line;
						}
						// change scaling for dotted line box
						Vector2 scalingParameters = line.GetComponent<RectTransform> ().sizeDelta;
						
						scalingParameters.y = Mathf.Sqrt (Mathf.Pow (startPoint.transform.position.x - endPoint.transform.position.x, 2) +
							Mathf.Pow (startPoint.transform.position.y - endPoint.transform.position.y, 2));
						scalingParameters.y *= 100;
						
						line.GetComponent<RectTransform> ().sizeDelta = scalingParameters;


						// after this all used to be under "Return"
						if (peopleToMove <= location1.population && peopleToMove <= vpm [vehicleNumberIndicator].seatingSpace) {
							//				numberEnteringActive = false;
							//				peopleToMoveTextBox.SetActive (false);

							for (int i = 0; i < vpm.Length; i++) {
								vpm [i].numberOfPeopleOnBoard = peopleToMove;
								vpm [i].endLocation = location2;
							}


							Instantiate (populationItemToSpawn, startPointsActive [0].transform.position, Quaternion.identity, location2.canvasChild.transform);

							location1.population -= peopleToMove;

							startPointsActive.Clear ();
						}



					}
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) { // NOT CONFIRMED
//			numberEnteringActive = false;
			firstClicked = false;
//			peopleToMoveTextBox.SetActive (false);
			for (int i = 0; i < vpm.Length; i++) {
				vpm [i].startPoint = null;
				vpm [i].endPoint = null;
			}

			Destroy (startPoint);
			Destroy (endPoint);
			startPointsActive.Clear ();
			startPoint = null;
			endPoint = null;
		}

		if (Input.GetKeyDown (KeyCode.Return)) { // CONFIRMED

			if (peopleToMove <= location1.population && peopleToMove <= vpm [vehicleNumberIndicator].seatingSpace) {
//				numberEnteringActive = false;
//				peopleToMoveTextBox.SetActive (false);

				for (int i = 0; i < vpm.Length; i++) {
					vpm [i].numberOfPeopleOnBoard = peopleToMove;
					vpm [i].endLocation = location2;
				}


				Instantiate (populationItemToSpawn, startPointsActive [0].transform.position, Quaternion.identity, location2.canvasChild.transform);

				location1.population -= peopleToMove;

				startPointsActive.Clear ();
			}
		}
		if (Input.GetKeyDown (KeyCode.Return) && peopleToMove > location1.population) {
			enteredTooManyPeople.SetActive (!enteredTooManyPeople.activeSelf);
		}
		if (Input.GetKeyDown (KeyCode.Return) && peopleToMove > vpm [vehicleNumberIndicator].seatingSpace) {
			notEnoughSeating.SetActive (!notEnoughSeating.activeSelf);
		}

//		for (int i = 0; i < locationScripts.Count; i++) {
//			if (locationScripts [i].selected) {
//				for (int x = 0; x < totalDots.Length; x++) {
//					totalDots [x].SetActive (false);
//				}
//				totalDots [i].SetActive (true);
//			}
//		}

		//endLocationForMovement.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//
		// sets the correct position between start location and mouse
		if (firstClicked) {
			dottedLineBox.gameObject.SetActive (true);
			dottedLineBox.transform.position = new Vector2 ((startPoint.transform.position.x + Camera.main.ScreenToWorldPoint (Input.mousePosition).x) / 2,
				(Camera.main.ScreenToWorldPoint (Input.mousePosition).y + startPoint.transform.position.y) / 2);

			rotationObject.transform.LookAt (new Vector3 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x,
				Camera.main.ScreenToWorldPoint (Input.mousePosition).y, 0));
			if (rotationObject.transform.rotation.eulerAngles.y <= 90.1f) {
				angle = (rotationObject.transform.rotation.eulerAngles.x - 90f) * -1f;
			} else {
				angle = rotationObject.transform.rotation.eulerAngles.x - 90f;
			}
			Vector3 zAngles = dottedLineBox.rotation.eulerAngles;
			zAngles.z = angle;
			dottedLineBox.rotation = Quaternion.Euler (zAngles);

			// change scaling for dotted line box
			Vector2 scalingParameters = dottedLineBox.sizeDelta;

			scalingParameters.y = Mathf.Sqrt (Mathf.Pow (startPoint.transform.position.x - Camera.main.ScreenToWorldPoint (Input.mousePosition).x, 2) +
			Mathf.Pow (startPoint.transform.position.y - Camera.main.ScreenToWorldPoint (Input.mousePosition).y, 2));
			scalingParameters.y *= 100;

			dottedLineBox.sizeDelta = scalingParameters;
		} else {
			dottedLineBox.gameObject.SetActive (false);
		}
	}

	void ChooseVehicle ()
	{
		if (Input.GetKeyDown (KeyCode.Z)) {
			populationItemToSpawn = vehicles [0]; // plane
			peopleToMove = 10;
			for (int i = 0; i < vpm.Length; i++) {
				vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
			}
			for (int i = 0; i < selectionIndicators.Length; i++) {
				selectionIndicators [i].SetActive (false);
			}
			selectionIndicators [0].SetActive (true);
			vehicleNumberIndicator = 0;
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			populationItemToSpawn = vehicles [1]; // boat
			peopleToMove = 15;
			for (int i = 0; i < vpm.Length; i++) {
				vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
			}
			for (int i = 0; i < selectionIndicators.Length; i++) {
				selectionIndicators [i].SetActive (false);
			}
			selectionIndicators [1].SetActive (true);
			vehicleNumberIndicator = 1;
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			populationItemToSpawn = vehicles [2]; // car
			peopleToMove = 20;
			for (int i = 0; i < vpm.Length; i++) {
				vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
			}
			for (int i = 0; i < selectionIndicators.Length; i++) {
				selectionIndicators [i].SetActive (false);
			}
			selectionIndicators [2].SetActive (true);
			vehicleNumberIndicator = 2;
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			populationItemToSpawn = vehicles [3]; // walking
			peopleToMove = 30;
			for (int i = 0; i < vpm.Length; i++) {
				vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
			}
			for (int i = 0; i < selectionIndicators.Length; i++) {
				selectionIndicators [i].SetActive (false);
			}
			selectionIndicators [3].SetActive (true);
			vehicleNumberIndicator = 3;
		}
	}

	public void ClickWalk ()
	{
		populationItemToSpawn = vehicles [3]; // walking
		peopleToMove = 30;
		for (int i = 0; i < vpm.Length; i++) {
			vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
		}
		for (int i = 0; i < selectionIndicators.Length; i++) {
			selectionIndicators [i].SetActive (false);
		}
		selectionIndicators [3].SetActive (true);
		vehicleNumberIndicator = 3;
	}
	public void ClickCar ()
	{
		populationItemToSpawn = vehicles [2]; // car
		peopleToMove = 20;
		for (int i = 0; i < vpm.Length; i++) {
			vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
		}
		for (int i = 0; i < selectionIndicators.Length; i++) {
			selectionIndicators [i].SetActive (false);
		}
		selectionIndicators [2].SetActive (true);
		vehicleNumberIndicator = 2;
	}
	public void ClickBoat ()
	{
		populationItemToSpawn = vehicles [1]; // boat
		peopleToMove = 15;
		for (int i = 0; i < vpm.Length; i++) {
			vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
		}
		for (int i = 0; i < selectionIndicators.Length; i++) {
			selectionIndicators [i].SetActive (false);
		}
		selectionIndicators [1].SetActive (true);
		vehicleNumberIndicator = 1;
	}
	public void ClickPlane ()
	{
		populationItemToSpawn = vehicles [0]; // plane
		peopleToMove = 10;
		for (int i = 0; i < vpm.Length; i++) {
			vpm [i] = populationItemToSpawn.GetComponent<VisualPopulationMovements> ();
		}
		for (int i = 0; i < selectionIndicators.Length; i++) {
			selectionIndicators [i].SetActive (false);
		}
		selectionIndicators [0].SetActive (true);
		vehicleNumberIndicator = 0;
	}
}
