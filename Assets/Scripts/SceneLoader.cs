using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour {

    public string scene;
    public Animator mask;

    void Start () {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            mask.SetBool("open", false);
            StartCoroutine(LoadSceneAnim(mask, delegate
            {
                SceneManager.LoadScene(scene);
            }));
        });
    }

    public static IEnumerator LoadSceneAnim (Animator anim, System.Action func)
    {
        yield return new WaitWhile(delegate
        {
            return anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1;
        });
        func();
    }
}
