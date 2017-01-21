using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
  Collider collider;

  void Start() {
    collider = GetComponent<Collider>();
  }

  virtual public void Handle() {
    transform.position += transform.forward * Time.deltaTime;
  }

  private void OnCollisionEnter(Collision collision) {
    Debug.Log("Collided");
  }
}
