using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public CanvasGroup pauseMenuGroup;

    public Text restartText1;
    public Text restartText2;

    public Text resumeText1;
    public Text resumeText2;

    public Text quitText1;
    public Text quitText2;

    public float fadeDuration = 0.5f;

    public int defaultFontSize = 50;
    public int focusedFontSize = 60;

    private void Awake()
    {
        pauseMenuGroup.alpha = 0f;

        EventBus.Subscribe<PauseMenuEngagedEvent>(e => pauseMenuGroup.alpha = 1f);
        EventBus.Subscribe<PauseMenuDisengagedEvent>(e => pauseMenuGroup.alpha = 0f);
        EventBus.Subscribe<TitleScreenLoadedEvent>(e => pauseMenuGroup.alpha = 0f);

        EventBus.Subscribe<SwitchFocusToRestartOptionEvent>(OnSwitchFocusToRestart);
        EventBus.Subscribe<SwitchFocusToResumeOptionEvent>(OnSwitchFocusToResume);
        EventBus.Subscribe<SwitchFocusToQuitOptionEvent>(OnSwitchFocusToQuit);
    }

    public void OnSwitchFocusToRestart(SwitchFocusToRestartOptionEvent e)
    {
        FocusOnText(restartText1, restartText2);
        UnfocusText(resumeText1, resumeText2);
        UnfocusText(quitText1, quitText2);
    }

    public void OnSwitchFocusToResume(SwitchFocusToResumeOptionEvent e)
    {
        UnfocusText(restartText1, restartText2);
        FocusOnText(resumeText1, resumeText2);
        UnfocusText(quitText1, quitText2);
    }

    public void OnSwitchFocusToQuit(SwitchFocusToQuitOptionEvent e)
    {
        UnfocusText(restartText1, restartText2);
        UnfocusText(resumeText1, resumeText2);
        FocusOnText(quitText1, quitText2);
    }

    private void FocusOnText(Text textElement1, Text textElement2)
    {
        if (textElement1) textElement1.fontSize = focusedFontSize;
        if (textElement2) textElement2.fontSize = focusedFontSize;
    }

    private void UnfocusText(Text textElement1, Text textElement2)
    {
        if (textElement1) textElement1.fontSize = defaultFontSize;
        if (textElement2) textElement2.fontSize = defaultFontSize;
    }
}
