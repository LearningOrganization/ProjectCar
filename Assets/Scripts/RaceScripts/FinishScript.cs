using System;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    public event Action OnFinishLineCrossed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayableCar"))
        {
            OnFinishLineCrossed?.Invoke();
        }
    }
}
