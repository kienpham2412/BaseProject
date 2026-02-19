using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Loading;
using Pixelplacement;

public class BootstrapLoader : Singleton<BootstrapLoader>
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField, SceneName] List<string> sceneLoadingDestroy;
    
    private void Awake()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return StartCoroutine(LoadSplashSceneRoutine());
        yield return StartCoroutine(LoadLoadingSceneRoutine());
        yield return StartCoroutine(LoadMainSceneRoutine());
        yield return StartCoroutine(DestroyLoadingScrene());
    }
    
    private IEnumerator LoadMainSceneRoutine()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("Home", LoadSceneMode.Additive);
        sceneLoad.allowSceneActivation = false;
        yield return new WaitUntil(() => sceneLoad.isDone || sceneLoad.progress > 0.85f);

        var tween = loadingSlider.DOValue(1f, 0.75f);
        yield return tween.WaitForCompletion();
        DebugLogger.Log("Loading Done");
        sceneLoad.allowSceneActivation = true;
    }

    private IEnumerator LoadSplashSceneRoutine()
    {
        yield return new WaitUntil(() => DataController.Instance.DataLoaded);
        // yield return new WaitUntil(() => GoogleMobileAdInitializer.IsConsentInfomationUpdated);
        // yield return new WaitUntil(() => FirebaseServiceController.Instance.IsFirebaseInited);
        var sceneLoad = SceneManager.LoadSceneAsync("Splash", LoadSceneMode.Additive);

        sceneLoad.allowSceneActivation = false;
        var tween = loadingSlider.DOValue(0.8f, 2f);
        yield return tween.WaitForCompletion();
        sceneLoad.allowSceneActivation = true;
    }
    
    private IEnumerator LoadLoadingSceneRoutine()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        yield return sceneLoad;
    }
    
    private IEnumerator DestroyLoadingScrene()
    {
        Scene[] loadedScenes = new Scene[SceneManager.sceneCount];
        for (int i = 0; i < loadedScenes.Length; i++)
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        
        yield return null;
            
        foreach (Scene scene in loadedScenes)
        {
            // Kiểm tra xem scene có tên là "SceneA" không
            if (IsConstainInList(scene.path))
            {
                // Lấy tất cả các game object trong scene A
                GameObject[] gameObjects = scene.GetRootGameObjects();
    
                // Hủy bỏ tất cả các game object trong scene A
                foreach (GameObject go in gameObjects)
                {
                    Destroy(go);
                }
    
                // Hủy bỏ scene A khỏi danh sách các scene
                AsyncOperation asyncOperationUnload = SceneManager.UnloadSceneAsync(scene);
                yield return new WaitUntil(() => asyncOperationUnload.isDone);
            }
        }
    
        yield return null;
    }
    
    private bool IsConstainInList(string path)
    {
        foreach (var scenePath in sceneLoadingDestroy)
            if (scenePath.Equals(path))
                return true;
        return false;
    }
}
