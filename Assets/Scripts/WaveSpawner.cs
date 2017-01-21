using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
  public bool releaseWave = false;
  public int currentWave = 0;

  public BasicEnemy[] availableEnemies;

  // Use this for initialization
  void Start() {
    GameManager.Instance().waveName.gameObject.SetActive(false);
  }

  public void Handle() {
    if (releaseWave) {
      currentWave++;
      releaseWave = false;

      SpawnEnemies(currentWave);

      StartCoroutine(ShowWaveName());
    }
  }

  public void SpawnEnemies(int wave) {
    float baseAngle = Random.Range(0.0f, 2 * Mathf.PI);
    float rangeAngle = Mathf.PI / 4;
    float shipDistance = 0.5f;
    float minDistance = 5;
    int enemyCount = 30;
    for (int i = 0; i < enemyCount; i++) {
      BasicEnemy enemy = Instantiate(availableEnemies[0]);
      float angle = baseAngle + rangeAngle * Mathf.Sin(Mathf.Deg2Rad*i*30);
      enemy.transform.position = (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)) * (i * shipDistance + minDistance);
      enemy.transform.rotation = Quaternion.LookRotation(GameManager.Instance().player.transform.position - enemy.transform.position, Vector3.back);
      GameManager.Instance().enemyList.Add(enemy);
    }
  }

  IEnumerator ShowWaveName() {
    Text waveName = GameManager.Instance().waveName;
    waveName.gameObject.SetActive(true);
    waveName.text = "WAVE " + currentWave.ToString();
    Color c = waveName.color;
    c.a = 0;
    waveName.color = c;

    for (int i = 0; i < 30; i++) {
      c.a = i / 29.0f;
      waveName.color = c;
      yield return new WaitForSeconds(0.05f);
    }
    for (int i = 0; i < 30; i++) {
      c.a = 1 - (i / 29.0f);
      waveName.color = c;
      yield return new WaitForSeconds(0.05f);
    }
    waveName.gameObject.SetActive(false);
  }
}
