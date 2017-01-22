using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public AudioSource buttonSound;
  public AudioSource playerShoot;
  public AudioSource damageReceived;
  public AudioSource[] enemyHit;

  public void playEnemyHit() {
    foreach(AudioSource a in enemyHit) {
      if (!a.isPlaying) {
        a.Play();
        return;
      }
    }
  }
}
