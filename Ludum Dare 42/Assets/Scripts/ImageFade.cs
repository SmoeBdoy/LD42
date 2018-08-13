using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour {

	public bool distanceBased;
	public float movementSpeed = 0.8f;
	public float distanceUntilFade = 1f;
	[Space (10)]
	public bool timeBased;
	public float timeUntilStartFading = 1f;
	float counter = 0;
	bool counterStarted = false;

	[Space (10)]
	public bool child = false;
	public bool imageFade;
	public bool textFade;

	public Color fullAlpha;
	public Color noAlpha;
	float fadeTime = 0;

	Vector3 target;

	void Start ()
	{
		if (distanceBased) {
			target = new Vector3 (transform.localPosition.x, transform.localPosition.y + distanceUntilFade, transform.localPosition.z);
		} else if (timeBased) {
			counter = 0f;
		}
	}

	void Update ()
	{
		if (distanceBased) {
			transform.Translate (Vector3.up * movementSpeed * Time.deltaTime);

			if (transform.localPosition.y >= target.y) {
				fadeTime += Time.deltaTime;
				if (imageFade) {
					GetComponent<Image> ().color = Color.Lerp (fullAlpha, noAlpha, fadeTime);
				} else if (textFade) {
					GetComponent<Text> ().color = Color.Lerp (fullAlpha, noAlpha, fadeTime);
				}
			}
		} else if (timeBased && gameObject.activeInHierarchy) {
			counterStarted = true;
			if (counterStarted) {
				counter += Time.deltaTime;
			}

			if (counter >= timeUntilStartFading) {
				fadeTime += Time.deltaTime;
				if (imageFade) {
					GetComponent<Image> ().color = Color.Lerp (fullAlpha, noAlpha, fadeTime);
				} else if (textFade) {
					GetComponent<Text> ().color = Color.Lerp (fullAlpha, noAlpha, fadeTime);
				}
			}
		}
		if (imageFade) {
			if (GetComponent<Image> ().color == noAlpha) {
				if (!child) {
					gameObject.SetActive (false);
				}
				GetComponent<Image> ().color = fullAlpha;
				fadeTime = 0f;
				if (timeBased) {
					counterStarted = false;
					counter = 0f;
				}
			}
		}
		else if (textFade) {
			if (GetComponent<Text> ().color == noAlpha) {
				if (!child) {
					gameObject.SetActive (false);
				}
				GetComponent<Text> ().color = fullAlpha;
				fadeTime = 0f;
				if (timeBased) {
					counterStarted = false;
					counter = 0f;
				}
			}
		}
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		float lineLength = 0.1f;
		Gizmos.DrawLine (new Vector3 (target.x - lineLength, target.y - lineLength, 0), new Vector3 (target.x + lineLength, target.y + lineLength, 0));
		Gizmos.DrawLine (new Vector3 (target.x - lineLength, target.y + lineLength, 0), new Vector3 (target.x + lineLength, target.y - lineLength, 0));
	}
}
