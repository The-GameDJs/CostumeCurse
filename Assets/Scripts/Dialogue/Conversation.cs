using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] public int CandyCornReward;
    [SerializeField] private bool ShowCharacterName;
    [SerializeField] public bool ShouldGrantAbility;
    
    [FormerlySerializedAs("IsRestPoint")] [SerializeField] private bool RestPoint;
    private bool CandyCornRewardClaimed;
    public Line[] Lines;
    public bool ShouldShowCharacterName => ShowCharacterName;
    public bool IsRestPoint => RestPoint;

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
        
        if(!RestPoint)
            CandyCornRewardClaimed = true;

        return CandyCornReward;
    }

    public bool HasCandyCornReward()
    {
        return CandyCornReward > 0;
    }

    public void ResetClaimedReward()
    {
        CandyCornRewardClaimed = false;
    }
}
