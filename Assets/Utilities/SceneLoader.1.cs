using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneLoader : Singleton<SceneLoader>
{
    protected SceneLoader () { }

    public GameObject mask;
    public float fadeDuration = 0.5f;

    static GameObject canvas;
    static Image _mask;
    static Color color;
    static AsyncOperation loadingScene;
    static Action _callback;
    static bool loading;
    static short loadState = 1;

    public static bool LoadScene (string scene, LoadSceneMode mode = LoadSceneMode.Single, bool fade = true, Action callback = null)
    {
        if (loading)
        {
            return false;
        }
        else if (fade)
        {
            loading = true;
            _callback = callback ?? (() => { });
            canvas = Instantiate(instance.mask);
            _mask = canvas.GetComponentInChildren<Image>();
            color = _mask.color;
            DontDestroyOnLoad(canvas);
            loadingScene = SceneManager.LoadSceneAsync(scene, mode);
            loadingScene.allowSceneActivation = false;
        }
        else
        {
            SceneManager.LoadScene(scene, mode);
            callback();
        }
        return true;
    }

    void Update ()
    {
        if (loading)
        {
            color.a += 1 / fadeDuration * loadState * Time.deltaTime;
            _mask.color = color;
            if (color.a >= 1)
            {
                loadingScene.allowSceneActivation = true;
                if (loadingScene.isDone == true)
                {
                    loadState = -1;
                    _callback();
                }
            }
            else if (color.a <= 0)
            {
                loadState = 1;
                loading = false;
                Destroy(canvas);
            }
        }
    }
}
