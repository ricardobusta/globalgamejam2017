using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  virtual public void Handle() {
    transform.position += transform.forward * Time.deltaTime;
  }
}
