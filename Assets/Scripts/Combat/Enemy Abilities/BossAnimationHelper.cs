using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationHelper : MonoBehaviour
{
    public void ReturnToOriginalRotation()
    {
        transform.Rotate(90f, 0f, 0f);
    }
}
