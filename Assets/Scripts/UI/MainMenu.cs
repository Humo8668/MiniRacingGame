using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // PLAY

    // SETTINGS
    public GameObject settingsWindowObj;
    protected Window settingsWindow;

    // EXIT

    private void Awake()
    {
        if (settingsWindowObj == null || !(settingsWindowObj is GameObject) || !settingsWindowObj.TryGetComponent<Window>(out settingsWindow))
        {
            throw new System.Exception("Window for 'Settings'-menu is null or invalid.");
        }
    }

    public void OnBtnPlayClick()
    {
    }

    public void OnBtnSettingsClick()
    {
        settingsWindow.Show();
    }

    public void OnBtnExitClick()
    {
        Application.Quit();
    }


}
