using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAnimation : MonoBehaviour {

    public Animation anim;
    public string animationName;
    Button btn;
    
	void Awake () {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            anim.Play(animationName);
        });
	}
}
