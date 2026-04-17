using UnityEngine;

// temporary solution
[CreateAssetMenu(fileName = "CarsDatabase", menuName = "Scriptable Objects/CarsDatabase")]
public class CarsDatabase : ScriptableObject
{
    public GameObject[] Cars;
}
