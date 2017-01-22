using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
  public float breakRadius;
  public float speed;

  public int pointsWorth = 10;

  float interp = 0;
  Vector3 strikePosition;
  Vector3 targetPosition;

  bool deaccelerate = false;

  float outInCubic(float t) {
    if (t < (0.5f)) {
      return 0.5f * (Mathf.Pow(t * 2 - 1, 3) + 1);
    }
    return 0.5f * Mathf.Pow(t * 2, 3) + 1 / 2;
    //return t * t * ((s + 1) * t - s);
  }

  virtual public void Handle() {
    if (deaccelerate) {
      transform.position = Vector3.Lerp(strikePosition, targetPosition, outInCubic(interp));
      interp += 0.2f * Time.deltaTime;
    } else {
      if (Vector3.Distance(transform.position, GameManager.Instance().shieldRenderer.gameObject.transform.position) < breakRadius) {
        deaccelerate = true;
        strikePosition = transform.position;
        targetPosition = GameManager.Instance().shieldRenderer.gameObject.transform.position;
      } else {
        transform.position += speed * transform.forward * Time.deltaTime;
      }
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.tag == "Planet") {
      GameManager m = GameManager.Instance();
      m.explosion.playExplosion(transform.position);
      m.Damage(1);
      m.enemyList.Remove(this);
      Destroy(gameObject);
    } else if (other.tag == "Player Bullets") {
      GameManager m = GameManager.Instance();
      if (Mathf.Abs(transform.position.y) < 0.7f && Mathf.Abs(transform.position.x) < 1.23f) {
        m.explosion.playExplosion(transform.position);
        m.soundManager.playEnemyHit();
        m.enemyList.Remove(this);
        Destroy(gameObject);
        m.AwardPoints(pointsWorth);
      }
    }
  }
}
