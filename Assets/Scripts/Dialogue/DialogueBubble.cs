using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class DialogueBubble : MonoBehaviour
{
    private const float FastForwardTextSpeed = 500f;
    private List<SpecialCommand> SpecialCommands;

    private TMP_Text Text;
    private string CurrentText;

    const string KAlphaCode = "<color=#00000000>";
    const float KMaxTextTime = 0.1f;
    public static float TextSpeed = 2.6f;
    private float OriginalTextSpeed;

    CanvasGroup Group;
    Animator ArrowAnim;
    GameObject Arrow;
    RectTransform RectTrans;
    private const float WidthOffsetScaler = 2.85f;

    private float AngleMultiplier = 1.0f;
    private float CurveScale = 5.0f;
    private bool HasTextChanged;

    const string ShakyCommandName = "shaky";

    // Contains animation data
    private struct VertexAnim
    {
        public float AngleRange;
        public float Angle;
        public float Speed;
    }

    void Awake()
    {
        RectTrans = GetComponent<RectTransform>();
        Arrow = transform.parent.gameObject.transform.GetChild(1).gameObject;
        ArrowAnim = transform.parent.GetComponentInChildren<Animator>();
        Text = GetComponentInChildren<TMP_Text>();
        // Store original speed to restore after acceleration
        OriginalTextSpeed = TextSpeed;
        Group = GetComponent<CanvasGroup>();
        Group.alpha = 0;
    }

    void Update()
    {
        // Simple hacky way to accelerate the text speed.
        if (Input.GetButtonDown("Fast Forward"))
        {
            TextSpeed = FastForwardTextSpeed;
        }

        // Update arrow position
        Arrow.transform.position = new Vector3(transform.position.x + RectTrans.rect.width / WidthOffsetScaler, transform.position.y, 0);
    }

    public void Display(string text)
    {
        ArrowAnim.SetBool("Open", false);
        Group.alpha = 1;
        CurrentText = text;
        StopAllCoroutines();
        StartCoroutine(DisplayText());
        TextSpeed = OriginalTextSpeed;
    }

    public void Close()
    {
        ArrowAnim.SetBool("Open", false);
        Group.alpha = 0;
        StopAllCoroutines();
    }

    private IEnumerator DisplayText()
    {
        if (Text == null)
        {
            Debug.LogError("Text is not linked in TextBubble: " + gameObject.name);
            yield return null;
        }

        Text.text = "";

        SpecialCommands = BuildSpecialCommandList(CurrentText);

        string cleanedText = StripAllTags(CurrentText);
        string originalText = cleanedText;
        string displayedText = "";
        int alphaIndex = 0;
        int characterIndex = 0;

        foreach (char c in cleanedText.ToCharArray())
        {
            if (SpecialCommands.Count > 0)
            {
                CheckForCommands(characterIndex);
            }
            alphaIndex++;
            Text.text = originalText;
            displayedText = Text.text.Insert(alphaIndex, KAlphaCode);
            Text.text = displayedText;
            characterIndex++;

            yield return new WaitForSecondsRealtime(KMaxTextTime / TextSpeed);
        }

        ArrowAnim.SetBool("Open", true);

        yield return null;
    }

    private List<SpecialCommand> BuildSpecialCommandList(string text)
    {
        List<SpecialCommand> listCommand = new List<SpecialCommand>();

        string command = "";
        char[] tags = { '<', '>' };

        // Go through the dialogue line, get all our special commands
        for (int i = 0; i < text.Length; i++)
        {
            string currentChar = text[i].ToString();

            //If true, we are getting a command.
            if (currentChar == "<")
            {
                // Go ahead and get the command.
                while (currentChar != ">" && i < text.Length)
                {
                    currentChar = text[i].ToString();
                    command += currentChar;
                    text = text.Remove(i, 1);  // Remove current character. We want to get the next character in the command
                }

                // Done getting the command
                if (currentChar == ">")
                {
                    command = command.Trim(tags);

                    if (command.Contains("/"))
                    {
                        if (listCommand.Count > 0)
                        {
                            listCommand[listCommand.Count - 1].EndIndex = i;
                        }
                    }
                    else
                    {
                        SpecialCommand newCommand = new SpecialCommand(command, i);
                        listCommand.Add(newCommand);
                    }

                    command = "";

                    // Take a step back otherwise a character will be skipped
                    i--;
                }
                else
                {
                    Debug.Log("Command in dialogue line not closed.");
                }
            }
        }

        return listCommand;
    }

    private void ExecuteCommand(SpecialCommand command)
    {
        if (command == null)
        {
            return;
        }

        if (command.Name == ShakyCommandName)
        {
            StartCoroutine(ShakeTextAt(command.StartIndex, command.EndIndex));
        }
    }

    // Check all commands in a given index. 
    // It's possible to have two commands next to each other in the dialogue line.
    // This means both will share the same index.
    private void CheckForCommands(int index)
    {
        for (int i = 0; i < SpecialCommands.Count; i++)
        {
            if (SpecialCommands[i].StartIndex == index)
            {
                ExecuteCommand(SpecialCommands[i]);

                // Remove it
                SpecialCommands.RemoveAt(i);

                //Take a step back since we removed one command from the list. Otherwise, the script will skip one command.
                i--;
            }
        }
    }

    // We use regex to strip all <tags> from our current dialogue line
    // We have two strings: one with tags and the one printing on screen
    // We keep track of both in order to know when there's a tag to execute, if any
    private string StripAllTags(string text)
    {
        // Regex Pattern. Remove all "<tag>" from our dialogue line
        string pattern = "<[^>]+>";
        return Regex.Replace(text, pattern, "");
    }

    // Shaking example taken from the TextMeshPro demo.
    private IEnumerator ShakeTextAt(int startIndex, int endIndex)
    {

        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        Text.ForceMeshUpdate();

        TMP_TextInfo textInfo = Text.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;
        HasTextChanged = true;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].AngleRange = Random.Range(10f, 25f);
            vertexAnim[i].Speed = Random.Range(1f, 3f);
        }

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while (true)
        {

            // Get new copy of vertex data if the text has changed.
            if (HasTextChanged)
            {
                // Update the copy of the vertex data for the text object.
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                HasTextChanged = false;
            }

            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = startIndex; i < endIndex; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible)
                    continue;

                // Retrieve the pre-computed animation data for the given character.
                VertexAnim vertAnim = vertexAnim[i];

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.Angle = Mathf.SmoothStep(-vertAnim.AngleRange, vertAnim.AngleRange, Mathf.PingPong(loopCount / 25f * vertAnim.Speed, 1f));
                Vector3 jitterOffset = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                vertexAnim[i] = vertAnim;
            }

            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                Text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount += 1;

            yield return new WaitForSeconds(0.1f);
        }
    }
}

//Basic Class for special commands
class SpecialCommand
{
    public string Name;

    public int StartIndex;
    public int EndIndex;

    public SpecialCommand(string name, int i)
    {
        Name = name;
        StartIndex = i;
        EndIndex = i;
    }
}

