using UnityEngine;

[System.Serializable]
public class Chemical
{
    public string chemicalName;
    public Color color = Color.white;
    public float density = 1.0f;
    public bool isLiquid = true;
    public bool isGas = false;
    public bool isSolid = false;
    public float pH = 7.0f;
    public string formula;
    
    // Properties that will be useful for reactions
    public bool isAcid = false;
    public bool isBase = false;
    public bool isFlammable = false;
    public bool isReactive = false;
}