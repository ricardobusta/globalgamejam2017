using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

  public Camera camera;

  public Transform titlePosition;
  public Transform creditsPosition;
  public Transform scorePosition;
  public Transform transitionPosition;
  public Transform gamePosition;
  public Transform settingsPosition;

  public GameObject titleScreen;
  public GameObject creditsScreen;
  public GameObject scoreScreen;
  public GameObject loadingScreen;
  public GameObject settingsScreen;

  public GameObject titleButton;
  public GameObject creditsButton;
  public GameObject scoreButton;
  public GameObject settingsButton;

  public EventSystem eventSystem;

  public Toggle soundOption;
  public Toggle musicOption;

  public Toggle[] controlType;

  public AudioSource musicSource;
  public AudioSource soundSource;

  GameObject currentScreen;

  public Text score;

  public AudioSource buttonClick;

  private void Start() {
    titleScreen.SetActive(true);
    creditsScreen.SetActive(false);
    scoreScreen.SetActive(false);
    settingsScreen.SetActive(false);
    camera.transform.position = titlePosition.position;
    currentScreen = titleScreen;
    score.text = GameManager.GetScores(10);
    soundOption.isOn = (PlayerPrefs.GetInt("muteSound", 0) == 1);
    musicOption.isOn = (PlayerPrefs.GetInt("muteMusic", 0) == 1);
    soundSource.mute = soundOption.isOn;
    musicSource.mute = musicOption.isOn;
    controlType[PlayerPrefs.GetInt("controlOption", 0)].isOn = true;
  }

  public void StartGame() {
    StartCoroutine(GoToScreen(gamePosition.position, null, null, true));
  }

  public void ExitGame() {
    Application.Quit();
  }

  public void ShowCredits() {
    StartCoroutine(GoToScreen(creditsPosition.position, creditsScreen, creditsButton));
  }

  public void ShowTitleScreen() {
    StartCoroutine(GoToScreen(titlePosition.position, titleScreen, titleButton));
  }

  IEnumerator GoToScreen(Vector3 to, GameObject showMenu, GameObject focusObject, bool startGame = false) {
    currentScreen.SetActive(false);
    buttonClick.Play();
    Vector3 from = camera.transform.position;
    float i = 0;
    while (i < 1) {
      i += Time.deltaTime;
      camera.transform.position = GameManager.QuadInterp(from, transitionPosition.position, to, i);
      yield return new WaitForEndOfFrame();
    }
    camera.transform.position = to;
    currentScreen = showMenu;
    if (!startGame) {
      currentScreen.SetActive(true);
      eventSystem.SetSelectedGameObject(focusObject);
    } else {
      loadingScreen.SetActive(true);
      yield return GameManager.loadSceneAsync("GameScene");
    }
  }

  public void ShowScore() {
    StartCoroutine(GoToScreen(scorePosition.position, scoreScreen, scoreButton));
  }

  public void ShowSettings() {
    StartCoroutine(GoToScreen(settingsPosition.position, settingsScreen, settingsButton));
  }

  public void ToggleSound() {
    PlayerPrefs.SetInt("muteSound", soundOption.isOn ? 1 : 0);
    soundSource.mute = soundOption.isOn;
  }

  public void ToggleMusic() {
    PlayerPrefs.SetInt("muteMusic", musicOption.isOn ? 1 : 0);
    musicSource.mute = musicOption.isOn;
  }

  public void SelectControl(int c) {
    PlayerPrefs.SetInt("controlOption", c);
  }
}
