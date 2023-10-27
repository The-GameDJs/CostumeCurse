using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostTree : MonoBehaviour
{
    [SerializeField] private Renderer[] LeafMaterials;
    private float[] InitialIntensityTimes;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeMaterialEmissivity();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMaterialsEmissivity();
    }

    private void InitializeMaterialEmissivity()
    {
        InitialIntensityTimes = new float[LeafMaterials.Length];
        for (var i = 0; i < InitialIntensityTimes.Length; i++)
        {
            InitialIntensityTimes[i] = Random.Range(0.0f, 3.0f);
            LeafMaterials[i].material.EnableKeyword("_EMISSION");
            LeafMaterials[i].material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            LeafMaterials[i].UpdateGIMaterials();
        }
    }
    
    private void UpdateMaterialsEmissivity()
    {
        for (var i = 0; i < LeafMaterials.Length; i++)
        {
            if (LeafMaterials[i].material.HasProperty("_Emission"))
            {
                var color = LeafMaterials[i].material.GetColor("_EmissionColor");
                var intensity = 1.5f * Mathf.Cos(Time.time + InitialIntensityTimes[i]) + 3.0f;
                LeafMaterials[i].material.SetColor("_EmissionColor", color * intensity);
                DynamicGI.SetEmissive(LeafMaterials[i], color);
                DynamicGI.UpdateEnvironment();
            }
        }
    }
}
