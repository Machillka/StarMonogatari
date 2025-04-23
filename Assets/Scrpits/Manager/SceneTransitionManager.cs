using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.SceneTransition
{
    public class SceneTransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup _fadeCanvasGroup;

        private bool _isFade;                   //TODO: 考虑使用 dotween 实现

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
            if (!_isFade)
                StartCoroutine(TransitionScene(sceneToLoadName, targetPosition));
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
            _fadeCanvasGroup = FindAnyObjectByType<CanvasGroup>();
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
            yield return Fade(1f); // Fade to black
            EventHandler.CallBeforeSceneLoadedEvent();

            yield return UnloadActiveScene();

            yield return LoadSceneSetActive(targetSceneName);
            yield return new WaitForSeconds(1f);
            EventHandler.CallMoveToPosition(targetPosition);

            EventHandler.CallAfterSceneLoadedEvent();

            yield return Fade(0f); // Fade to clear

        }
        //TODO: 考虑加载和动画时间的结合，如果加载时间小于动画时间，则动画时间持续到预设为止；如果加载时间非常长，就一直显示动画
        /// <summary>
        ///
        /// </summary>
        /// <param name="targetAlpha">1表明黑色，0表示透明</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            _isFade = true;

            _fadeCanvasGroup.blocksRaycasts = true;

            float fadeSpeed = Mathf.Abs(_fadeCanvasGroup.alpha - targetAlpha) / Settings.sceneFadeDuration;

            while (Mathf.Approximately(_fadeCanvasGroup.alpha, targetAlpha) == false)
            {
                _fadeCanvasGroup.alpha = Mathf.MoveTowards(_fadeCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

                yield return null;
            }

            _fadeCanvasGroup.blocksRaycasts = false;
            _isFade = false;
        }
    }
}
