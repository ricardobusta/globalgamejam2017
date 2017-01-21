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

  public float shootCooldown = 1;
  float currentShootCooldown = 0;

  public Text waveName;

  public WaveSpawner waveSpawner;

  static GameManager _instance;

  public List<BasicEnemy> enemyList = new List<BasicEnemy>();

  public Bullet bulletprefab;
  List<Bullet> playerBullets = new List<Bullet>();

  public static GameManager Instance() {
    if (_instance == null) {
      _instance = FindObjectOfType<GameManager>();
    }
    return _instance;
  }

  void Start() {
    currentShootCooldown = shootCooldown;

    for(int i = 0; i < 10; i++) {
      Bullet b = Instantiate(bulletprefab);
      b.gameObject.SetActive(false);
      playerBullets.Add(b);
      
    }
  }

  void Update() {
    currentShootCooldown -= Time.deltaTime;
    if (Input.GetMouseButton(0)) {
      PlayerClickedControl();
      PlayerShoot();
    }
    player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, playerSpeed * 100 * Time.deltaTime);
    waveSpawner.Handle();
    foreach (BasicEnemy e in enemyList) {
      e.Handle();
    }
    foreach(Bullet b in playerBullets) {
      b.Handle();
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

  public void PlayerShoot() {
    if (currentShootCooldown <= 0 ) {
      Debug.Log("Pew");
      currentShootCooldown = shootCooldown;
      Bullet bullet = null;
      foreach(Bullet o in playerBullets) {
        if (!o.gameObject.activeSelf) {
          bullet = o;
          break;
        }
      }
      if (bullet == null) return;

      bullet.gameObject.SetActive(true);
      bullet.transform.position = player.transform.position;
      bullet.transform.rotation = player.transform.rotation;
    }
  }
}
