using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour {

	public Color[] blues;

	public List<int> chosenNumbers = new List<int>();

	void Start ()
	{
		for (int i = 0; i < blues.Length; i++) {
			chosenNumbers.Add (i);
		}
	}
}
