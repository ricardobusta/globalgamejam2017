using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  Quaternion targetRotation;

  public GameObject player;
  public float playerSpeed = 1;

  public int playerHealth = 10;
  public TextMesh playerHealthText;

  public Text waveName;

  public WaveSpawner waveSpawner;

  static GameManager _instance;

  public List<BasicEnemy> enemyList = new List<BasicEnemy>();

  public static GameManager Instance() {
    if (_instance == null) {
      _instance = FindObjectOfType<GameManager>();
    }
    return _instance;
  }

  void Start() {
  }

  void Update() {
    if (Input.GetMouseButton(0)) {
      PlayerClickedControl();
    }
    player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, playerSpeed * 100 * Time.deltaTime);
    waveSpawner.Handle();
    foreach (BasicEnemy e in enemyList) {
      e.Handle();
    }
  }

  float AngleSign(Vector2 v1, Vector2 v2) {
    return Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
  }

  public void Damage(int dmg) {
    playerHealth -= dmg;
    playerHealthText.text = playerHealth.ToString();
  }

  public void PlayerClickedControl() {
    Vector3 dir = (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2)).normalized;
    targetRotation = Quaternion.LookRotation(dir, Vector3.back);
  }
}
