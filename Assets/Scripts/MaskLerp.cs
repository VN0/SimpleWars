using UnityEngine;
using UnityEngine.UI;

public class MaskLerp : MonoBehaviour
{
    float alpha = 1;
    float lastTime;
    float currentTime;

    void Awake()
    {
        lastTime = Time.realtimeSinceStartup;
    }

    public void Update()
    {
        currentTime = Time.realtimeSinceStartup;
        if (alpha > 0)
        {
            alpha -= (currentTime - lastTime);
            GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else if (alpha <= 0)
        {
            Destroy(this);
        }
        lastTime = currentTime;
    }
}
