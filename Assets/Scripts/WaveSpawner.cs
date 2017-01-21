using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
  public float waitTimer;
  float currentWaitTimer = 0;
  public int currentWave = 1;

  // Use this for initialization
  void Start() {
  }

  public void Handle() {
    currentWaitTimer += Time.deltaTime;
    if (currentWaitTimer > waitTimer) {
      currentWaitTimer = 0;
      SpawnWave();
      currentWave++;
    }
  }

  void SpawnWave() {
    StartCoroutine(ShowWaveName());
  }
 
  IEnumerator ShowWaveName() {
    Text waveName = GameManager.Instance().waveName;
    waveName.text = "WAVE " + currentWave.ToString();
    Color c = waveName.color;
    c.a = 0;
    waveName.color = c;

    for(int i = 0; i < 30; i++) {
      c.a = i / 29.0f;
      waveName.color = c;
      yield return new WaitForSeconds(0.05f);
    }
    for (int i = 0; i < 30; i++) {
      c.a = 1 - (1 / 29.0f);
      waveName.color = c;
      yield return new WaitForSeconds(0.05f);
    }
  }
}
