using UnityEngine;

[CreateAssetMenu(fileName = "CarScriptableObjContainer", menuName = "Scriptable Objects/CarContainer")]
public class CarContainer : ScriptableObject
{
    public GameObject Car; 
    public bool IsBatchProcessing;
    public int CarsAmounts;

}
