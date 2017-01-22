using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

  public Camera camera;
  public GameObject background;
  Quaternion targetRotation;

  public GameObject player;
  public Transform shootPoint;
  public float playerSpeed = 1;

  public MeshRenderer shieldRenderer;

  public int playerMaxHealth = 10;
  int playerHealth;
  public TextMesh playerHealthText;

  public float shootCooldown = 1;
  float currentShootCooldown = 0;

  public int score = 0;
  public Text scoreText;

  public Text waveName;

  public WaveSpawner waveSpawner;

  static GameManager _instance;

  public List<BasicEnemy> enemyList = new List<BasicEnemy>();

  public Bullet bulletprefab;
  List<Bullet> playerBullets = new List<Bullet>();

  bool gameOver;

  public static GameManager Instance() {
    if (_instance == null) {
      _instance = FindObjectOfType<GameManager>();
    }
    return _instance;
  }

  void Start() {
    currentShootCooldown = shootCooldown;
    playerHealth = playerMaxHealth;
    for (int i = 0; i < 30; i++) {
      Bullet b = Instantiate(bulletprefab);
      b.gameObject.SetActive(false);
      playerBullets.Add(b);
    }
    gameOver = true;
    StartCoroutine(StartSequence());
  }

  void Update() {
    if (gameOver) {
      return;
    }
    currentShootCooldown -= Time.deltaTime;
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    if (Input.GetMouseButton(0)) {
      PlayerClickedControl();
      PlayerShoot();
    }else if(h!=0 || v != 0) {
      PlayerDirectionControl(new Vector3(h, v, 0).normalized);
      PlayerShoot();
    }
    player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, playerSpeed * 100 * Time.deltaTime);

    if (enemyList.Count == 0) {
      waveSpawner.releaseWave = true;
    }
    waveSpawner.Handle();
    foreach (BasicEnemy e in enemyList) {
      e.Handle();
    }
    foreach (Bullet b in playerBullets) {
      b.Handle();
    }
  }

  IEnumerator StartSequence() {
    float i = 0;
    Vector3 bgsize = new Vector3(2.8f, 1.4f, 1);
    while (i < 1) {
      i += Time.deltaTime;
      float l = Mathf.Lerp(6 , 1, Mathf.Sin(i * Mathf.PI / 2));
      camera.orthographicSize = 0.7f * l;
      background.transform.localScale = bgsize * l;
      yield return new WaitForEndOfFrame();
    }
    camera.orthographicSize = 0.7f;
    gameOver = false;
  }

  float AngleSign(Vector2 v1, Vector2 v2) {
    return Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
  }

  public void Damage(int dmg) {
    if (playerHealth > 0) {
      playerHealth -= dmg;
      playerHealthText.text = playerHealth.ToString();
      shieldRenderer.material.SetFloat("_CrackedIntensity", 1 - ((float)playerHealth / (float)playerMaxHealth));
    } else {
      gameOver = true;
      StartCoroutine(ShowGameOver());
    }
  }

  public void AwardPoints(int points) {
    score += points;
    scoreText.text = score.ToString("000000");
  }

  public void PlayerClickedControl() {
    Vector3 dir = (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2)).normalized;
    targetRotation = Quaternion.LookRotation(dir, Vector3.back);
  }

  public void PlayerDirectionControl(Vector3 dir) {
    targetRotation = Quaternion.LookRotation(dir, Vector3.back);
  }

  public void PlayerShoot() {
    if (currentShootCooldown <= 0) {
      currentShootCooldown = shootCooldown;
      Bullet bullet = null;
      foreach (Bullet o in playerBullets) {
        if (!o.gameObject.activeSelf) {
          bullet = o;
          break;
        }
      }
      if (bullet == null) return;

      bullet.gameObject.SetActive(true);
      bullet.SetLife();
      bullet.transform.position = shootPoint.position;
      bullet.transform.rotation = player.transform.rotation;
    }
  }

  public static Vector3 QuadInterp(Vector3 a, Vector3 b, Vector3 c, float t) {
    return (1 - t) * ((t * b) + (1 - t) * a) + (t) * ((t * c) + (1 - t) * b);
  }

  public IEnumerator ShowWaveName(int currentWave) {
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

  IEnumerator ShowGameOver() {
    Text waveName = GameManager.Instance().waveName;
    waveName.gameObject.SetActive(true);
    waveName.text = "GAME OVER";
    Color c = waveName.color;
    c.a = 0;
    waveName.color = c;

    for (int i = 0; i < 30; i++) {
      c.a = i / 29.0f;
      waveName.color = c;
      yield return new WaitForSeconds(0.05f);
    }
  }
}
