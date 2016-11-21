using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFade : MonoBehaviour
{
    public Color spriteColor = Color.white;
    public float fadeInTime = 1.5f;
    public float fadeOutTime = 3f;
    public float delayToFadeOut = 5f;
    public float delayToFadeIn = 5f;
    public bool fadeOut = true;
    public bool fadeOnce = false;

    SpriteRenderer sprite;

    void Start ()
    {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine("FadeCycle");
    }

    IEnumerator FadeCycle ()
    {
        float fade = 0f;
        float startTime;
        while (true)
        {
            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / fadeInTime);
                spriteColor.a = fade;
                sprite.color = spriteColor;
                yield return null;
            }
            //Make sure it's set to exactly 1f
            fade = 1f;
            spriteColor.a = fade;
            sprite.color = spriteColor;
            if (!fadeOut)
            {
                Destroy(this);
                yield break;
            }
            yield return new WaitForSeconds(delayToFadeOut);

            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / fadeOutTime);
                spriteColor.a = fade;
                sprite.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            sprite.color = spriteColor;
            if (fadeOnce)
            {
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(delayToFadeIn);
        }
    }
}