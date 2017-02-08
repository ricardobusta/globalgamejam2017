using System;
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

  public GameObject scoresScreen;
  public Text scores;

  public Text story;

  bool userPressedSkip = false;

  public Toggle pauseButton;

  int controlType;

  float sideSpeed;
  float sideSpeedTouch;

  public GameObject pauseScreen;

  public GameObject directionHelper;
  public GameObject virtualAnalogBase;
  public GameObject virtualAnalogStick;

  Vector3 virtualAnalogPos;

  public RectTransform canvasTransform;

  public static GameManager Instance() {
    if (_instance == null) {
      _instance = FindObjectOfType<GameManager>();
    }
    return _instance;
  }

  void Start() {
    controlType = PlayerPrefs.GetInt("controlOption", 0);
    pauseButton.gameObject.SetActive(false);
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
    scoresScreen.SetActive(false);
    pauseScreen.SetActive(false);

    directionHelper.SetActive(controlType == 2);
    virtualAnalogBase.SetActive(false);
  }

  void Update() {
    if (pauseButton.isOn) {
      return;
    }
    if (gameOver) {
      if (Input.GetMouseButton(0) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("Fire") != 0) {
        userPressedSkip = true;
      }
      return;
    }
    currentShootCooldown -= Time.deltaTime;
    float maxSpeed = playerSpeed * 100 * Time.deltaTime;

    /* Joystick Control */
    float joyH = Input.GetAxis("Horizontal Analog");
    float joyV = Input.GetAxis("Vertical Analog");
    if (joyH != 0 || joyV != 0) {
      PlayerDirectionControl(new Vector3(joyH, joyV, 0).normalized);
      player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, maxSpeed);
      goto FinishedControlManagement;
    }
    /* Keyboard Control */
    float h = Input.GetAxis("Horizontal");
    //float v = Input.GetAxis("Vertical");
    if (h != 0) {
      sideSpeed += Time.deltaTime * h * 20;
      sideSpeed = Mathf.Clamp(sideSpeed, -maxSpeed, maxSpeed);
      player.transform.localRotation = player.transform.localRotation * Quaternion.Euler(0, sideSpeed, 0);
      goto FinishedControlManagement;
    } else {
      sideSpeed = 0;
    }
    /* Mouse Control */
    switch (controlType) {
      case 0:
        /* Point Control Mode */
        if (Input.GetMouseButton(0)) {
          PlayerClickedControl();
          player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, maxSpeed);
          PlayerShoot();
          goto FinishedControlManagement;
        }
        break;
      case 1:
        /* Virtual Analog Control Mode */
        if (Input.GetMouseButtonDown(0)) {
          virtualAnalogBase.SetActive(true);
          virtualAnalogPos = Input.mousePosition;
          virtualAnalogBase.transform.localPosition = new Vector3(canvasTransform.sizeDelta.x * (Input.mousePosition.x / Screen.width - 0.5f), canvasTransform.sizeDelta.y * (Input.mousePosition.y / Screen.height - 0.5f), 0);
        } else if (Input.GetMouseButtonUp(0)) {
          virtualAnalogBase.SetActive(false);
        }
        if (virtualAnalogBase.activeSelf && Input.GetMouseButton(0)) {
          Vector3 dir = (Input.mousePosition - virtualAnalogPos).normalized;
          virtualAnalogStick.transform.localPosition = 30 * dir;
          PlayerDirectionControl(dir);
          player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, maxSpeed);
          PlayerShoot();
          goto FinishedControlManagement;
        }
        break;
      case 2:
        /* Direction Pads Control Mode */
        if (Input.GetMouseButton(0)) {
          float mouseH = 2 * (Input.mousePosition.x / Screen.width) - 1;
          float dir = (mouseH > 0.2) ? 1 : ((mouseH < -0.2) ? -1 : 0);
          Debug.Log(dir);
          if (dir != 0) {
            sideSpeedTouch += Time.deltaTime * dir * 20;
            sideSpeedTouch = Mathf.Clamp(sideSpeedTouch, -maxSpeed, maxSpeed);
            player.transform.localRotation = player.transform.localRotation * Quaternion.Euler(0, sideSpeedTouch, 0);
            goto FinishedControlManagement;
          } else {
            sideSpeedTouch = 0;
          }
        }
        break;
    }

    FinishedControlManagement:

    /* Fire if fire is pressed */
    if (Input.GetAxis("Fire") != 0) {
      PlayerShoot();
    }

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
    story.gameObject.SetActive(true);
    float i = 0;
    Vector3 bgsize = new Vector3(2.8f, 1.4f, 1);
    camera.orthographicSize = 0.7f * 6;
    background.transform.localScale = bgsize * 6;
    while (i < 1) {
      story.color = new Color(1, 1, 1, i);
      i += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    i = 0;
    while (i < 2) {
      if (userPressedSkip) {
        break;
      }
      yield return new WaitForEndOfFrame();
    }
    i = 0;
    while (i < 1) {
      story.color = new Color(1, 1, 1, 1 - i);
      i += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    story.gameObject.SetActive(false);
    i = 0;
    while (i < 1) {
      float l = Mathf.Lerp(6, 1, Mathf.Sin(i * Mathf.PI / 2));
      camera.orthographicSize = 0.7f * l;
      background.transform.localScale = bgsize * l;
      yield return new WaitForEndOfFrame();
      i += Time.deltaTime;
    }
    camera.orthographicSize = 0.7f;
    background.transform.localScale = bgsize;
    gameOver = false;
    playerHealthDisplay.SetActive(true);
    pauseButton.gameObject.SetActive(true);
  }

  float AngleSign(Vector2 v1, Vector2 v2) {
    return Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
  }

  public void Damage(int dmg) {
    playerHealth -= dmg;

    if (playerHealth > 0) {
      playerHealthText.text = playerHealth.ToString("00");
      shieldRenderer.material.SetFloat("_CrackedIntensity", Mathf.Clamp01(1 - ((float)(playerHealth - 1) / (float)(playerMaxHealth - 1))));
    } else {
      playerHealth = 0;
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
    directionHelper.SetActive(false);
    virtualAnalogBase.SetActive(false);
    //Text waveName = GameManager.Instance().waveName;
    gameOverScreen.SetActive(true);

    Image[] images = gameOverScreen.GetComponentsInChildren<Image>();
    Text[] texts = gameOverScreen.GetComponentsInChildren<Text>();
    foreach (Image i in images) {
      i.color = new Color(1, 1, 1, 0);
    }
    foreach (Text t in texts) {
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
    scoresScreen.SetActive(true);
    scores.text = UpdateScores();
  }

  public string UpdateScores() {
    int[] s = new int[10];
    for (int i = 0; i < 10; i++) {
      s[i] = PlayerPrefs.GetInt("PlayerScore" + i, 0);
    }
    if (score > s[0]) {
      s[0] = score;
      Array.Sort(s);
    }
    for (int i = 0; i < 10; i++) {
      PlayerPrefs.SetInt("PlayerScore" + i, s[i]);
    }
    PlayerPrefs.Save();
    return GetScores(5);
  }

  static public string GetScores(int max) {
    string result = "";
    for (int i = 0; i < max; i++) {
      result += PlayerPrefs.GetInt("PlayerScore" + (9 - i), 0).ToString("000000") + '\n';
    }
    return result;
  }

  IEnumerator ScreenShake() {
    Vector3 original = camera.transform.position;
    soundManager.damageReceived.Play();

    float shakeEffect = 1;

    while (shakeEffect > 0) {
      camera.transform.position = (UnityEngine.Random.insideUnitSphere * (shakeEffect / 10)) + (Vector3.forward * original.z);
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

  public void TogglePause() {
    pauseScreen.SetActive(pauseButton.isOn);
  }

  public void ExitClicked() {
    soundManager.buttonSound.Play();
    loadingScreen.SetActive(true);
    StartCoroutine(loadSceneAsync("TitleScreen"));
  }
}
