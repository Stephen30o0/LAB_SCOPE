using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public float maxVolume = 100.0f; // in mL
    public float currentVolume = 0.0f;
    public List<ChemicalMixture> contents = new List<ChemicalMixture>();
    public Renderer liquidRenderer; // To visualize the liquid
    
    [System.Serializable]
    public class ChemicalMixture
    {
        public Chemical chemical;
        public float volume;
    }
    
    public void AddChemical(Chemical chemical, float volume)
    {
        if (currentVolume + volume > maxVolume)
            volume = maxVolume - currentVolume;
            
        var existingMixture = contents.Find(m => m.chemical.chemicalName == chemical.chemicalName);
        
        if (existingMixture != null)
            existingMixture.volume += volume;
        else
            contents.Add(new ChemicalMixture { chemical = chemical, volume = volume });
            
        currentVolume += volume;
        UpdateLiquidVisualization();
    }
    
    public void UpdateLiquidVisualization()
    {
        if (liquidRenderer && contents.Count > 0)
        {
            // Calculate blended color based on chemical volumes
            Color blendedColor = Color.clear;
            foreach (var mixture in contents)
            {
                float ratio = mixture.volume / currentVolume;
                blendedColor += mixture.chemical.color * ratio;
            }
            
            // Update material color
            Material mat = liquidRenderer.material;
            mat.color = blendedColor;
            
            // Update liquid level (assuming Y scale represents height)
            Vector3 scale = liquidRenderer.transform.localScale;
            scale.y = Mathf.Clamp01(currentVolume / maxVolume);
            liquidRenderer.transform.localScale = scale;
        }
    }
}