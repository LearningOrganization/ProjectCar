// using UnityEngine;
// using UnityEngine.AI;



// public class WheelControl : MonoBehaviour
// {
//     public GameObject WheelPrefab; // сюда в инспекторе перетаскиваешь готовую модельку или префаб

//     [HideInInspector] public WheelCollider WheelCollider;

//     private Transform WheelInstance;

//     public bool Steerable;
//     public bool Motorized;
//     public bool IsHandBrakeable;

//     [HideInInspector]
//     public float CurrentSteerAngle = 0f;

//     private void Start()
//     {
//         WheelCollider = GetComponent<WheelCollider>();

//         if (WheelPrefab != null)
//         {
//             WheelInstance = Instantiate(WheelPrefab, transform).transform;
//         }
//     }

//     void Update()
//     {
//         if (WheelInstance == null) return;

//         WheelCollider.GetWorldPose(out Vector3 colliderPos, out Quaternion colliderRot);

//         WheelInstance.position = colliderPos;
//         WheelInstance.rotation = colliderRot;
//     }
// }
