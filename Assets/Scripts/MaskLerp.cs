using UnityEngine;
using UnityEngine.UI;

public class MaskLerp : MonoBehaviour
{
    float alpha = 1;

    public void Update()
    {
        if (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime;
            GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else if (alpha <= 0)
        {
            Destroy(this);
        }
    }
}
