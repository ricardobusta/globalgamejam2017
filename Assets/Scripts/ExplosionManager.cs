using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour {
  public GameObject[] explosion;
  public float scale=1;

  private void Start() {
    foreach(GameObject e in explosion) {
      e.SetActive(false);
    }
  }

  public void playExplosion(Vector3 p) {
    GameObject exp = null;
    foreach(GameObject e in explosion) {
      if (!e.activeSelf) {
        exp = e;
        break;
      }
    }
    if (exp != null) {
      exp.transform.position = p;
      exp.SetActive(true);
      StartCoroutine(playExplosionAnimation(exp));
    }
  }

  IEnumerator playExplosionAnimation(GameObject e) {
    float t = 0;
    while (t<1) {
      t += Time.deltaTime*3;
      Debug.Log(t);
      e.transform.localScale = Vector3.one * t * scale;
      yield return new WaitForEndOfFrame();
    }
    e.SetActive(false);
  }
}
