using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public string sceneName;
    public GameObject loadingScreen; 
    public bool fakeLoading;
    public Slider progressBar;
    [SerializeField] private ScreenFader fader;


    private float loadingProgress;
    private int currentSceneIndex;
    private void Awake()
    {
        LoadSceneAsync(sceneName);
    }
    
    private void LoadSceneAsync(string sceneNameString)
    {
        var loadDuration = fakeLoading ? 1f : 0f;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (fakeLoading)
        {
            DOTween.To(() => loadingProgress, x => loadingProgress = x, 1, loadDuration).SetEase(Ease.Flash).OnUpdate((() =>
            {
                progressBar.value = Mathf.Clamp01(loadingProgress);
            }));
        }
        Timer.Register(loadDuration, () =>
        {
            fader.FadeToBlack(1, () =>
            {
                SceneManager.LoadScene(sceneName);
            });
           // StartCoroutine(LoadSceneAsyncCoroutine(sceneNameString));
        });
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneNameString)
    {
        AsyncOperation o = SceneManager.LoadSceneAsync(sceneNameString);
        while (!o.isDone)
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        loadingScreen.SetActive(false);
    }
}