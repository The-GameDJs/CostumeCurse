using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBubble : MonoBehaviour
{
    public TMP_Text Text;
    private string CurrentText;

    const string kAlphaCode = "<color=#00000000>";
    const float kMaxTextTime = 0.1f;
    public static int TextSpeed = 2;

    CanvasGroup Group;
    
    void Start()
    {
        Group = GetComponent<CanvasGroup>();
        Group.alpha = 0;
    }

    public void Show(string text)
    {
        Group.alpha = 1;
        CurrentText = text;
        StartCoroutine(DisplayText());
    }

    public void Close()
    {
        StopAllCoroutines();
        Group.alpha = 0;
    }


    private IEnumerator DisplayText()
    {
        if (Text == null)
        {
            Debug.LogError("Text is not linked in TextBubble: " + gameObject.name);
            yield return null;
        }

        Text.text = "";
 
        string originalText = CurrentText;
        string displayedText = "";
        int alphaIndex = 0;
 
        foreach(char c in CurrentText.ToCharArray())
        {
            alphaIndex++;
            Text.text = originalText;
            displayedText = Text.text.Insert(alphaIndex, kAlphaCode);
            Text.text = displayedText;
 
            yield return new WaitForSecondsRealtime(kMaxTextTime / TextSpeed);
        }
 
        yield return null;
    }
}
