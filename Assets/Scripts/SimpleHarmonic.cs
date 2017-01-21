using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHarmonic : MonoBehaviour {

  public float startingAngleX = 0;
  public float startingAngleY = 0;
  public float speedAngleX = 5;
  public float speedAngleY = 5;
  public float radiusX = 0.1f;
  public float radiusY = 0.1f;

  public Vector3 startingPosition;

  float currentAngle = 0;

  void Start() {
    startingPosition = transform.position;
  }

  void Update() {
    currentAngle += Time.deltaTime;
    transform.position = startingPosition + new Vector3(radiusX * Mathf.Cos(startingAngleX + speedAngleX * currentAngle), radiusY * Mathf.Sin(startingAngleY + speedAngleY * currentAngle));
  }
}
