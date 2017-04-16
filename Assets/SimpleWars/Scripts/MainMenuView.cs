using UnityEngine;
using MarkLight;
using MarkLight.Views;
using MarkLight.Views.UI;
using SimpleWars;

public sealed class MainMenuView : UIView
{
    public ViewAnimation b0Hover, b1Hover, b2Hover;
    public Button b0, b1, b2;
    bool l0, l1, l2;
    UISoundPlayer sound;

    private void Awake ()
    {
        print(ResourceDictionary.Language);
        ResourceDictionary.NotifyObservers();
        sound = UISoundPlayer.instance;
    }

    private void Update ()
    {
        if (b0.IsMouseOver != l0)
        {
            l0 = b0.IsMouseOver;
            if (l0) b0Hover.StartAnimation();
            else b0Hover.ReverseAnimation();
        }
        if (b1.IsMouseOver != l1)
        {
            l1 = b1.IsMouseOver;
            if (l1) b1Hover.StartAnimation();
            else b1Hover.ReverseAnimation();
        }
        if (b2.IsMouseOver != l2)
        {
            l2 = b2.IsMouseOver;
            if (l2) b2Hover.StartAnimation();
            else b2Hover.ReverseAnimation();
        }
    }

    public void LoadBuilder ()
    {
        SceneLoader.LoadScene("Builder");
    }

    public void Quit ()
    {
        Application.Quit();
    }

    public void MEnter ()
    {
        sound.PlayMouseEnter(0.25f);
    }
    public void MExit ()
    {
        sound.PlayMouseExit();
    }
    public void MClick ()
    {
        sound.PlayMouseClick();
    }
}