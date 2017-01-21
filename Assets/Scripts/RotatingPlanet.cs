using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlanet : MonoBehaviour {
	void Update () {
    transform.Rotate(Time.deltaTime, 30*Time.deltaTime, 0);
	}
}
