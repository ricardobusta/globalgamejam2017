using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public AudioSource music;
  public AudioSource buttonSound;
  public AudioSource playerShoot;
  public AudioSource damageReceived;
  public AudioSource[] enemyHit;
  public AudioSource powerup;

  public void playEnemyHit() {
    foreach (AudioSource a in enemyHit) {
      if (!a.isPlaying) {
        a.Play();
        return;
      }
    }
  }

  void Start() {
    music.mute = PlayerPrefs.GetInt("muteMusic", 0) == 1;
    bool muteSound = PlayerPrefs.GetInt("muteSound", 0) == 1;
    Debug.Log("music" + music.mute);
    Debug.Log("sound" + muteSound);

    buttonSound.mute = muteSound;
    playerShoot.mute = muteSound;
    damageReceived.mute = muteSound;
    powerup.mute = muteSound;
    foreach(AudioSource a in enemyHit) {
      a.mute = muteSound;
    }
  }
}
