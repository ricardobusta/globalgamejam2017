using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlanet : MonoBehaviour {
  public Vector3 delta = new Vector3(1, 30, 0);

	void Update () {
    transform.Rotate(delta*Time.deltaTime);
	}
}
