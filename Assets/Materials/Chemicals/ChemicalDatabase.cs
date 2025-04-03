using UnityEngine;
using System.Collections.Generic;

public class ChemicalDatabase : MonoBehaviour
{
    public static ChemicalDatabase Instance { get; private set; }
    
    private Dictionary<string, Chemical> chemicals = new Dictionary<string, Chemical>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeChemicals();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeChemicals()
    {
        // Water
        AddChemical(new Chemical
        {
            chemicalName = "Water",
            formula = "H2O",
            color = new Color(0.7f, 0.85f, 1.0f, 0.8f),
            density = 1.0f,
            pH = 7.0f
        });
        
        // Hydrochloric Acid
        AddChemical(new Chemical
        {
            chemicalName = "Hydrochloric Acid",
            formula = "HCl",
            color = new Color(1.0f, 1.0f, 0.7f, 0.8f),
            density = 1.2f,
            pH = 1.0f,
            isAcid = true
        });
        
        // Sodium Hydroxide
        AddChemical(new Chemical
        {
            chemicalName = "Sodium Hydroxide",
            formula = "NaOH",
            color = new Color(0.9f, 0.9f, 0.9f, 0.8f),
            density = 1.1f,
            pH = 14.0f,
            isBase = true
        });
        
        // Phenolphthalein (indicator)
        AddChemical(new Chemical
        {
            chemicalName = "Phenolphthalein",
            formula = "C20H14O4",
            color = new Color(1.0f, 1.0f, 1.0f, 0.8f),
            density = 1.0f,
            pH = 7.0f
        });
        
        // Copper Sulfate
        AddChemical(new Chemical
        {
            chemicalName = "Copper Sulfate",
            formula = "CuSO4",
            color = new Color(0.0f, 0.5f, 1.0f, 0.8f),
            density = 1.1f,
            pH = 4.0f
        });
        
        // Add more as needed...
    }
    
    private void AddChemical(Chemical chemical)
    {
        chemicals.Add(chemical.chemicalName, chemical);
    }
    
    public Chemical GetChemical(string name)
    {
        if (chemicals.ContainsKey(name))
            return chemicals[name];
        
        Debug.LogWarning("Chemical not found: " + name);
        return null;
    }
}