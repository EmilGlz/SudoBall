using UnityEngine;

// Generate a screenshot and save to disk with the name SomeLevel.png.

public class ScreenShoter : MonoBehaviour
{
    public int screenCount = 0;

    const string savedScreenCount = "";

    private void Start()
    {
        screenCount = PlayerPrefs.GetInt(savedScreenCount);
    }

    public void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("SudoScreen" + screenCount + ".png");
        screenCount++;
        PlayerPrefs.SetInt(savedScreenCount, screenCount);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    print("space key was pressed");
        //}
    }
}
