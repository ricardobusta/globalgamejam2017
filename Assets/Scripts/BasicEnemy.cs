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

  private void OnTriggerEnter(Collider other) {
    if (other.tag == "Planet") {
      GameManager.Instance().Damage(1);
    } else if(other.tag == "Player Bullets") {
      GameManager.Instance().enemyList.Remove(this);
      Destroy(gameObject);
    }
  }
}
