using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
  public int playerHealth;
  public Text playerHealthText;
  public GameObject debris;

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

  public GameObject gameOverScreen;

  public GameObject playerHealthDisplay;

  public GameObject loadingScreen;

  public EventSystem eventSystem;
  public GameObject gameOverButton;

  public SoundManager soundManager;

  public ExplosionManager explosion;

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
    playerHealthText.text = playerHealth.ToString("00");
    playerHealthDisplay.SetActive(false);
    gameOver = true;
    StartCoroutine(StartSequence());
    debris.SetActive(false);
    shieldRenderer.gameObject.SetActive(true);
    shieldRenderer.material.SetFloat("_CrackedIntensity", 0);
    gameOverScreen.SetActive(false);
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

    if (enemyList.Count == 0 && (!waveSpawner.spawning)) {
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
    playerHealthDisplay.SetActive(true);
  }

  float AngleSign(Vector2 v1, Vector2 v2) {
    return Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
  }

  public void Damage(int dmg) {
    if (playerHealth > 0) {
      playerHealth -= dmg;
      playerHealthText.text = playerHealth.ToString("00");
      shieldRenderer.material.SetFloat("_CrackedIntensity", Mathf.Clamp01(1 - ((float)playerHealth / (float)playerMaxHealth)));
    } else {
      gameOver = true;
      playerHealthDisplay.SetActive(false);
      debris.SetActive(true);
      shieldRenderer.gameObject.SetActive(false);
      StartCoroutine(ShowGameOver());
    }

    if (dmg > 0)
        StartCoroutine(ScreenShake());
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
      soundManager.playerShoot.Play();
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
    //Text waveName = GameManager.Instance().waveName;
    gameOverScreen.SetActive(true);

    Image[] images = gameOverScreen.GetComponentsInChildren<Image>();
    Text[] texts = gameOverScreen.GetComponentsInChildren<Text>();
    foreach(Image i in images) {
      i.color = new Color(1, 1, 1, 0);
    }
    foreach(Text t in texts) {
      t.color = new Color(1, 1, 0, 0);
    }

    eventSystem.SetSelectedGameObject(gameOverButton);
    yield return null;

    for (int j = 0; j < 10; j++) {
      float a = j / 9.0f;
      foreach (Image i in images) {
        i.color = new Color(1, 1, 1, a);
      }
      foreach (Text t in texts) {
        Color c = t.color;
        c.a = a;
        t.color = c;
      }
      yield return new WaitForSeconds(0.05f);
    }
  }

  IEnumerator ScreenShake() {
    Vector3 original = camera.transform.position;
    soundManager.damageReceived.Play();

    float shakeEffect = 1;

    while (shakeEffect > 0) {
      camera.transform.position = (Random.insideUnitSphere * (shakeEffect/10))+(Vector3.forward*original.z);
      shakeEffect -= Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }

    camera.transform.position = original;
  }

  public static IEnumerator loadSceneAsync(string scene) {
    AsyncOperation async = SceneManager.LoadSceneAsync(scene);
    while (!async.isDone) {
      yield return null;
    }
  }

  public void RetryClicked() {
    soundManager.buttonSound.Play();
    loadingScreen.SetActive(true);
    StartCoroutine(loadSceneAsync(SceneManager.GetActiveScene().name));
  }

  public void ExitClicked() {
    soundManager.buttonSound.Play();
    loadingScreen.SetActive(true);
    StartCoroutine(loadSceneAsync("TitleScreen"));
  }
}
