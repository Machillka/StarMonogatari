using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.SceneTransition
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public string startSceneName = string.Empty;

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void OnTransitionEvent(string sceneToLoadName, Vector3 targetPosition)
        {
            StartCoroutine(TransitionScene(sceneToLoadName, targetPosition));
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }

        private IEnumerator UnloadActiveScene()
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        private IEnumerator TransitionScene(string targetSceneName, Vector3 targetPosition)
        {
            yield return UnloadActiveScene();

            yield return LoadSceneSetActive(targetSceneName);
        }
    }
}
