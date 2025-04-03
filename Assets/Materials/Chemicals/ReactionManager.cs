using System.Collections.Generic;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    [System.Serializable]
    public class Reaction
    {
        public string reactionName;
        public List<ReactionInput> inputs;
        public List<ReactionOutput> outputs;
        public bool needsHeat = false;
        public bool needsCatalyst = false;
        public string catalystName;
        public float reactionTime = 1.0f; // Time in seconds
        public bool producesBubbles = false;
        public bool producesHeat = false;
        public bool producesLight = false;
        public Color lightColor = Color.white;
    }
    
    [System.Serializable]
    public class ReactionInput
    {
        public string chemicalName;
        public float minVolume = 1.0f; // Minimum volume needed in mL
        public float ratioInReaction = 1.0f; // For stoichiometry
    }
    
    [System.Serializable]
    public class ReactionOutput
    {
        public Chemical chemical;
        public float ratioInReaction = 1.0f; // For stoichiometry
    }
    
    public List<Reaction> possibleReactions = new List<Reaction>();
    
    public void CheckForReaction(Container container, bool isHeated, List<string> catalysts)
    {
        foreach (var reaction in possibleReactions)
        {
            if (CanReactionOccur(reaction, container, isHeated, catalysts))
            {
                PerformReaction(reaction, container);
                break; // Only perform one reaction at a time
            }
        }
    }
    
    private bool CanReactionOccur(Reaction reaction, Container container, bool isHeated, List<string> catalysts)
    {
        // Check if all required inputs are present
        foreach (var input in reaction.inputs)
        {
            var mixture = container.contents.Find(m => m.chemical.chemicalName == input.chemicalName);
            if (mixture == null || mixture.volume < input.minVolume)
                return false;
        }
        
        // Check if heat is needed but not provided
        if (reaction.needsHeat && !isHeated)
            return false;
            
        // Check if catalyst is needed but not provided
        if (reaction.needsCatalyst && 
            (catalysts == null || !catalysts.Contains(reaction.catalystName)))
            return false;
            
        return true;
    }
    
    private void PerformReaction(Reaction reaction, Container container)
    {
        // Calculate the limiting reagent scale factor
        float reactionScale = float.MaxValue;
        foreach (var input in reaction.inputs)
        {
            var mixture = container.contents.Find(m => m.chemical.chemicalName == input.chemicalName);
            float possibleReactionAmount = mixture.volume / input.ratioInReaction;
            reactionScale = Mathf.Min(reactionScale, possibleReactionAmount);
        }
        
        // Remove input chemicals
        foreach (var input in reaction.inputs)
        {
            var mixture = container.contents.Find(m => m.chemical.chemicalName == input.chemicalName);
            float amountToRemove = input.ratioInReaction * reactionScale;
            mixture.volume -= amountToRemove;
            container.currentVolume -= amountToRemove;
            
            // Remove if volume is now zero
            if (mixture.volume <= 0.001f)
                container.contents.Remove(mixture);
        }
        
        // Add output chemicals
        foreach (var output in reaction.outputs)
        {
            float amountToAdd = output.ratioInReaction * reactionScale;
            container.AddChemical(output.chemical, amountToAdd);
        }
        
        // Visual effects
        if (reaction.producesBubbles)
            StartBubbleEffect(container);
            
        if (reaction.producesHeat)
            StartHeatEffect(container);
            
        if (reaction.producesLight)
            StartLightEffect(container, reaction.lightColor);
            
        // Update visualization
        container.UpdateLiquidVisualization();
    }
    
    private void StartBubbleEffect(Container container)
    {
        // Instantiate particle system for bubbles
        // Code to create bubble particle effect...
    }
    
    private void StartHeatEffect(Container container)
    {
        // Heat distortion effect or steam particles
        // Code to create heat effect...
    }
    
    private void StartLightEffect(Container container, Color color)
    {
        // Create point light with specified color
        GameObject lightObj = new GameObject("ReactionLight");
        lightObj.transform.position = container.transform.position;
        Light light = lightObj.AddComponent<Light>();
        light.color = color;
        light.intensity = 1.0f;
        light.range = 3.0f;
        
        // Destroy after 2 seconds
        Destroy(lightObj, 2.0f);
    }
}