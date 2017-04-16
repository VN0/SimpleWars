using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System;

public class SceneLoader : Singleton<SceneLoader>
{
    protected SceneLoader () { }

    public GameObject mask;
    public float fadeDuration = 0.5f;
    public Shader blurShader;

    static GameObject canvas;
    static Image _mask;
    static Color color;
    static AsyncOperation loadingScene;
    static Action _callback;
    static bool loading;
    static bool reverse = false;
    static BlurOptimized blur;
    static Shader _blurShader;
    static float startTime = 0;

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
            AddBlur();
        }
        else
        {
            SceneManager.LoadScene(scene, mode);
            (callback ?? (() => { }))();
        }
        return true;
    }
    public static bool LoadScene (int scene, LoadSceneMode mode = LoadSceneMode.Single, bool fade = true, Action callback = null)
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
            AddBlur();
        }
        else
        {
            SceneManager.LoadScene(scene, mode);
            (callback ?? (() => { }))();
        }
        return true;
    }

    protected override void Initialize ()
    {
        _blurShader = blurShader;
    }

    void Update ()
    {
        if (loading)
        {
            float unscaledTime = Time.unscaledTime;
            if (startTime == 0)
            {
                startTime = unscaledTime;
            }
            float time = unscaledTime - startTime;
            float value = Mathf.SmoothStep(0, 1, time / fadeDuration);
            if (reverse)
            {
                value = 1 - value;
            }
            color.a = value;
            if (blur != null)
            {
                blur.blurSize = value * 10;
            }
            _mask.color = color;
            if (color.a >= 1 && !reverse)
            {
                Destroy(blur);
                loadingScene.allowSceneActivation = true;
                if (loadingScene.isDone == true)
                {
                    reverse = true;
                    _callback();
                    startTime = Time.unscaledTime;
                    //AddBlur();
                }
            }
            else if (color.a <= 0 && reverse)
            {
                startTime = 0;
                reverse = false;
                loading = false;
                Destroy(canvas);
                Destroy(blur);
            }
        }
    }

    static void AddBlur ()
    {
        blur = Camera.main.gameObject.AddComponent<BlurOptimized>();
        blur.blurShader = _blurShader;
        blur.blurType = BlurOptimized.BlurType.SgxGauss;
        blur.blurIterations = 1;
        blur.downsample = 1;
        blur.blurSize = 0;
    }
}
