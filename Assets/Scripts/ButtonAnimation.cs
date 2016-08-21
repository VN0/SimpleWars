using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAnimation : MonoBehaviour
{

    public Animation anim;
    public bool useCloseAnimation = false;
    public bool state = false;
    public string animationName;
    public string closeAnimationName;
    Button btn;

    void Awake ()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Toggle);
    }

    void Toggle ()
    {
        if (useCloseAnimation)
        {
            anim.Play(state ? closeAnimationName : animationName);
            state = !state;
        }
        else
        {
            anim.Play(animationName);
        }
    }
}
