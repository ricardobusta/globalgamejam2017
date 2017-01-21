using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour {

  AsyncOperation async;
  public string nextScene;

  public SpriteRenderer logo;

  void Start() {
    StartCoroutine(Load());
  }

  IEnumerator Load() {
    async = SceneManager.LoadSceneAsync(nextScene);
    async.allowSceneActivation = false;

    float delay = 0.05f;

    for (float f = 0.0f; f < 1.0f; f += delay) {
      logo.color = new Color(1, 1, 1, f);
      yield return new WaitForSeconds(delay);
      Debug.Log(f);
    }

    yield return new WaitForSeconds(1);

    for (float f = 0.0f; f < 1.0f; f += delay) {
      logo.color = new Color(1, 1, 1, 1 - f);
      yield return new WaitForSeconds(delay);
      Debug.Log(f);
    }

    async.allowSceneActivation = true;
  }
}
