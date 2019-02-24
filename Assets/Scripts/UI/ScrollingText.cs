using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class ScrollingText : MonoBehaviour
{
    [SerializeField]
    private TextAsset textFile;

    [SerializeField]
    private float totalTime = 1.0f;

    [SerializeField]
    private int maxLines = 10;

    private Text text;
    private string stringToDisplay;
    private float lineTimer = 0.0f;
    private System.Action onComplete;

    void Awake()
    {
        stringToDisplay = textFile.text;
        text = GetComponent<Text>();

        EventBus.Subscribe<EnterTitleScreenEvent>(e => Initialize());
    }

    private void Initialize()
    {
        lineTimer = 0.0f;
        text.text = "";
    }

    public void StartScroll(System.Action onComplete)
    {
        Initialize();
        this.onComplete = onComplete;
        this.enabled = true;
    }

    void Update()
    {
        lineTimer += Time.deltaTime;
        float lerp = Mathf.Min(lineTimer / totalTime, 1.0f);
        int charactersToDisplay = (int)Math.Round(stringToDisplay.Length * lerp);
        string truncatedString = stringToDisplay.Substring(0, charactersToDisplay);
        List<string> lines = new List<string>(truncatedString.Split('\n'));
        text.text = string.Join("\n", lines.Skip(Math.Max(0, lines.Count - maxLines)).ToArray());

        if (lineTimer >= totalTime)
        {
            // Wait for one more frame so that all the text is displayed
            Utils.OnNextFrame(this, () =>
            {
                if (onComplete != null)
                {
                    onComplete();
                }

                this.enabled = false;
            });
        }
    }
}