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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DisplayText()
    {
        if (Text == null)
        {
            Debug.LogError("Text is not linked in DialogueWindow: " + gameObject.name);
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
