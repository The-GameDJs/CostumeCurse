using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBattleScript : MonoBehaviour
{
    void OnDestroy()
    {
        SceneManager.LoadScene("Game_Victory");
    }
}
