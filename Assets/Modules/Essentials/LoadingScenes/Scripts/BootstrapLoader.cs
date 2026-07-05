using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Pixelplacement;

public class BootstrapLoader : Singleton<BootstrapLoader>
{
    [SerializeField] private Slider loadingSlider;
    private List<string> sceneLoadingDestroy = new List<string>() {"Bootstrap", "Splash", "Update", "Loading"};
    
    private void Awake()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        // yield return new WaitUntil(() => GoogleMobileAdInitializer.IsConsentInfomationUpdated);
        yield return new WaitUntil(() => FirebaseServiceController.Instance.IsFirebaseInited);
        yield return new WaitUntil(() => DataController.Instance.DataLoaded);
        
        yield return StartCoroutine(LoadSceneRoutine("Splash"));
        yield return StartCoroutine(LoadSceneRoutine("Update"));
        yield return new WaitUntil(() => UpdateChecking.UpdateCheckingComplete);
        
        yield return StartCoroutine(ChangeLoadingProgressValue(0.2f, 0.4f));
        yield return StartCoroutine(LoadSceneRoutine("Loading"));
        yield return StartCoroutine(ChangeLoadingProgressValue(0.4f, 0.4f));
        yield return StartCoroutine(LoadMainSceneRoutine());
        yield return StartCoroutine(DestroyLoadingScrene());
    }

    private IEnumerator LoadMainSceneRoutine()
    {
        yield return null;
        yield return LoadSceneRoutine("Gameplay", 1f, 0.75f);
    }
    
    private IEnumerator LoadSceneRoutine(string sceneName, float loadingProgress, float duration)
    {
        var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        sceneLoad.allowSceneActivation = false;
        yield return new WaitUntil(() => sceneLoad.isDone || sceneLoad.progress > 0.85f);
        yield return StartCoroutine(ChangeLoadingProgressValue(loadingProgress, duration));
        DebugLogger.Log("Loading Done");
        sceneLoad.allowSceneActivation = true;
    }
    
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        var sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return sceneLoad;
    }

    private IEnumerator ChangeLoadingProgressValue(float loadingProgress, float duration)
    {
        var tween = loadingSlider.DOValue(loadingProgress, duration);
        yield return tween.WaitForCompletion();
    }
    
    private IEnumerator DestroyLoadingScrene()
    {
        yield return null;
        foreach (string sceneName in sceneLoadingDestroy)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            yield return StartCoroutine(DestroyScene(scene));
        }
        yield return null;
    }

    private IEnumerator DestroyScene(Scene scene)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();
    
        // Hủy bỏ tất cả các game object trong scene A
        foreach (GameObject go in gameObjects)
            Destroy(go);
    
        // Hủy bỏ scene A khỏi danh sách các scene
        AsyncOperation asyncOperationUnload = SceneManager.UnloadSceneAsync(scene);
        yield return new WaitUntil(() => asyncOperationUnload.isDone);
    }
}
