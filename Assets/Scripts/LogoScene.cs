using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour {

  AsyncOperation async;
  public string nextScene;

  void Start() {
    StartCoroutine(Load());
  }

  IEnumerator Load() {
    async = SceneManager.LoadSceneAsync(nextScene);
    async.allowSceneActivation = false;
    yield return null;
  }

  public void ChangeScene() {
    async.allowSceneActivation = true;
  }
}
