using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostTree : MonoBehaviour
{
    [SerializeField] private Renderer[] LeafMaterials;
    [SerializeField] float ColorRangeOffset;
    private float[] InitialIntensityTimes;
    private Color[] OriginalColors;
    
    // VERY COMPUTATIONALLY EXPENSIVE. DEACTIVATE COMPONENT ON INSPECTOR FOR BETTER PERFORMANCE.
    
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
        OriginalColors = new Color[LeafMaterials.Length];
        for (var i = 0; i < InitialIntensityTimes.Length; i++)
        {
            InitialIntensityTimes[i] = Random.Range(0.0f, ColorRangeOffset);
            OriginalColors[i] = LeafMaterials[i].material.GetColor("_EmissionColor");
            LeafMaterials[i].material.EnableKeyword("_EMISSION");
            LeafMaterials[i].material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            LeafMaterials[i].UpdateGIMaterials();
        }
    }
    
    private void UpdateMaterialsEmissivity()
    {
        for (var i = 0; i < LeafMaterials.Length; i++)
        {
            if (LeafMaterials[i].material.HasProperty("_EmissionColor"))
            {
                var intensity = (ColorRangeOffset / 2.0f) * Mathf.Cos(Time.time + InitialIntensityTimes[i]) + ColorRangeOffset;
                LeafMaterials[i].material.SetColor("_EmissionColor", OriginalColors[i] * intensity);
                DynamicGI.SetEmissive(LeafMaterials[i], OriginalColors[i] * intensity);
                DynamicGI.UpdateEnvironment();
            }
        }
    }
}
