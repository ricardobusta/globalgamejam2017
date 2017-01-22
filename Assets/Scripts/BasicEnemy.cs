using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {
  public float breakRadius;
  public float speed;

  public int pointsWorth = 10;
    [Range(0, 1)]
    public float hPSpawnProbability = 5;
    public HealthPack healthPackPrefab;

    private Transform _tr;
  float interp = 0;
  Vector3 strikePosition;
  Vector3 targetPosition;

  bool deaccelerate = false;

<<<<<<< HEAD
  void Start() {
    collider = GetComponent<Collider>();
    _tr = GetComponent<Transform>();
  }

=======
>>>>>>> 9a1091e1f07624ccf06745986f1618768b95f237
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
      if (Mathf.Abs(transform.position.y) < 0.7f && Mathf.Abs(transform.position.x) < 1.23) {

        // Gets a number between 0 and the probability. Spawns when gets a zero.
        bool spawnHP = (Random.Range(0.0f, 1.0f) <= hPSpawnProbability);
        if (spawnHP)
            Instantiate(healthPackPrefab).transform.position = _tr.position;

        m.explosion.playExplosion(transform.position);
        m.soundManager.playEnemyHit();
        m.enemyList.Remove(this);
        Destroy(gameObject);
        m.AwardPoints(pointsWorth);
      }
    }
  }
}
