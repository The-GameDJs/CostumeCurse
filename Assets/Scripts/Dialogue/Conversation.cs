using UnityEngine;

[System.Serializable]
public struct Line
{
    [TextArea(1, 1)]
    public string Character;

    [TextArea(2, 5)]
    public string text;
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    [SerializeField] private int CandyCornReward;
    [SerializeField] private bool ShowCharacterName;
    [SerializeField] private bool IsRestPoint;
    private bool CandyCornRewardClaimed;
    public Line[] Lines;
    public bool ShouldShowCharacterName => ShowCharacterName;

    // TODO this might break if we ever do more than one OnEnable during the game
    public void OnEnable()
    {
        CandyCornRewardClaimed = false;
    }

    // TODO: Future item hand outs from NPCs?
    public bool IsCandyCornRewardClaimed()
    {
        return CandyCornRewardClaimed;
    }

    public int ClaimReward()
    {
        if (CandyCornRewardClaimed)
        {
            Debug.LogWarning("Already claimed! Check before you call this with IsCandyCornRewardClaimed");
            return 0;
        }
        
        if(!IsRestPoint)
            CandyCornRewardClaimed = true;

        return CandyCornReward;
    }

    public bool HasCandyCornReward()
    {
        return CandyCornReward > 0;
    }
}
