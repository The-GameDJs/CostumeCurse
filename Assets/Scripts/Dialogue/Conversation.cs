using System.Collections;
using System.Collections.Generic;
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
    private bool CandyCornRewardClaimed;
    public Line[] Lines;

    // TODO this might break if we ever do more than one OnEnable during the game
    public void OnEnable()
    {
        CandyCornRewardClaimed = false;
    }

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

        CandyCornRewardClaimed = true;

        return CandyCornReward;
    }

}
