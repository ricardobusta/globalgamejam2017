using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
  void Start() {
  }

  public void Handle() {
    transform.rotation = Quaternion.LookRotation(GameManager.Instance().player.transform.position - transform.position, Vector3.back);
  }
}
