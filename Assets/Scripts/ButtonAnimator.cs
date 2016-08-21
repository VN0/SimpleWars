using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour
{
    public Animator animator;
    public string parameter;
    public bool state = false;
    public bool toggleAnimation = false;
    public bool autoDetectChanges = true;

    void Awake ()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
    }
    
    void Toggle ()
    {
        if (toggleAnimation)
        {
            state = !state;
            animator.SetBool(parameter, state);
        }
        else
        {
            animator.SetBool(parameter, !state);
        }
    }

    void Update ()
    {
        if (autoDetectChanges)
        {
            state = animator.GetBool(parameter);
        }
    }
}
