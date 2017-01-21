using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

  public Camera camera;

  public Vector3 titlePosition;
  public Vector3 creditsPosition;
  public Vector3 scorePosition;
  public Vector3 transitionPosition;

  public GameObject titleScreen;
  public GameObject creditsScreen;
  public GameObject scoreScreen;

  GameObject currentScreen;

  private void Start() {
    titleScreen.SetActive(true);
    creditsScreen.SetActive(false);
    scoreScreen.SetActive(false);
    camera.transform.position = titlePosition;
    currentScreen = titleScreen;
  }

  public void StartGame() {
    loadSceneAsync("");
  }

  IEnumerator loadSceneAsync(string scene) {
    AsyncOperation async = SceneManager.LoadSceneAsync(scene);
    while (!async.isDone) {
      yield return null;
    }
  }


  public void ExitGame() {
    Application.Quit();
  }

  public void ShowCredits() {
    StartCoroutine(GoToScreen(creditsPosition, creditsScreen));
  }

  public void ShowTitleScreen() {
    StartCoroutine(GoToScreen(titlePosition, titleScreen));
  }

  IEnumerator GoToScreen(Vector3 to, GameObject showMenu) {
    currentScreen.SetActive(false);
    Vector3 from = camera.transform.position;
    float i = 0;
    while (i < 1) {
      i += Time.deltaTime;
      Vector3 p = GameManager.QuadInterp(from,transitionPosition,to, i);
      camera.transform.position = p;
      yield return new WaitForEndOfFrame();
    }
    currentScreen = showMenu;
    currentScreen.SetActive(true);
  }

  public void ShowScore() {
    StartCoroutine(GoToScreen(scorePosition, scoreScreen));
  }
}
