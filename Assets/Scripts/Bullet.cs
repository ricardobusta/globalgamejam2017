using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  public float life = 3;

  public void SetLife() {
    life = 3;
  }

  virtual public void Handle() {
    transform.position += transform.forward * Time.deltaTime;
    life -= Time.deltaTime;
    if (life <= 0) {
      gameObject.SetActive(false);
    }
  }
}
