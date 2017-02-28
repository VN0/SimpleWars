using System.IO;
using UnityEngine;
using MarkLight;
using MarkLight.Views;
using MarkLight.Views.UI;
using MarkLight.Animation;
using SimpleWars;

public sealed class BuilderView : UIView
{
    public ViewFieldAnimator ExpandPanel;
    public Panel SavesPanel;
    public List SavesList;
    public ObservableList<FileInfo> files = new ObservableList<FileInfo>();
    public _int screenHeight;
    public FileInfo selectedSave;

    bool sPanelActive = false;

    public void Awake ()
    {
        screenHeight.Value = Screen.height;
        SavesPanel.Offset.Value = new ElementMargin(0, -screenHeight.Value);

        ExpandPanel = new ViewFieldAnimator();
        ExpandPanel.EasingFunction = EasingFunctionType.ExponentialEaseOut;
        ExpandPanel.Field = "Offset";
        ExpandPanel.From = new ElementMargin(0, -screenHeight.Value);
        ExpandPanel.To = new ElementMargin(0, 0);
        ExpandPanel.Duration = 0.5f;
        ExpandPanel.TargetView = SavesPanel;
    }

    public void Update ()
    {
        ExpandPanel.Update();
    }

    public void LoadPlanet ()
    {
        SceneLoader.LoadScene("TestPlanet");
    }

    public void SaveSelected ()
    {
        var f = SavesList.SelectedItem.Value as FileInfo;
        if (f == selectedSave)
        {
            print("Loading save " + f.FullName);
        }
        selectedSave = f;
    }

    public void ShowSaves ()
    {
        if (sPanelActive == true)
        {
            ExpandPanel.ReverseAnimation();
            sPanelActive = false;
            return;
        }
        sPanelActive = true;
        files.Replace(Manager.GetSaves());
        files.ItemsModified();
        ExpandPanel.StartAnimation();
    }
}
