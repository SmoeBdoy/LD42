using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualPopulationMovements : MonoBehaviour {

	PopulationMovement populationMovement;

	public float speed;
	public int seatingSpace;
	public int numberOfPeopleOnBoard = 0;
	public Text numberOfPeopleOnBoardText;

	Vector3 zAngles;
	Vector3 lastPos;
	Vector3 newPos;
	bool runOnce = false;

	public Vector3 locationToMoveTo;
	float angle;

	public GameObject startPoint;
	public GameObject endPoint;
	public GameObject line;

	public Location endLocation;

	void Start ()
	{
		populationMovement = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PopulationMovement> ();

		lastPos = transform.position;

		// adjust rotation
		if (populationMovement.rotationObject.transform.rotation.eulerAngles.y <= 90.1f) {
			angle = (populationMovement.rotationObject.transform.rotation.eulerAngles.x - 90f) * -1f;
		} else {
			angle = populationMovement.rotationObject.transform.rotation.eulerAngles.x - 90f;
		}
		zAngles = transform.rotation.eulerAngles;
		zAngles.z = angle + 90;
		transform.rotation = Quaternion.Euler (zAngles);
	}

	void Update ()
	{
		transform.position = Vector3.MoveTowards (transform.position, locationToMoveTo, speed * Time.deltaTime);

		float distance = Mathf.Sqrt (Mathf.Pow (transform.position.x - locationToMoveTo.x, 2) +
		                 Mathf.Pow (transform.position.y - locationToMoveTo.y, 2));

		if (distance <= 0.1f) {
			CompletedMovement ();
		}

		numberOfPeopleOnBoardText.text = numberOfPeopleOnBoard.ToString ();

		if (!runOnce) {
			runOnce = true;
			newPos = transform.position;
			// moving right
			if (lastPos.x < newPos.x) {
				zAngles.z = angle - 90;
				transform.rotation = Quaternion.Euler (zAngles);
				transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
			}
		}
	}

	void CompletedMovement ()
	{
		endLocation.population += numberOfPeopleOnBoard;

		Destroy (startPoint);
		Destroy (endPoint);
		Destroy (line);
		Destroy (gameObject);
	}
}
