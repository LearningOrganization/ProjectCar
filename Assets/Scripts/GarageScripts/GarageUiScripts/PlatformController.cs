using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlatformController : MonoBehaviour
{ 
    [SerializeField] private GameObject CarPlatform;
    [SerializeField] private int PlatformDistance;
    [SerializeField] private float CarPlatformMovementTime = 2.5f;
    [SerializeField] private GameObject CarPlatformsParent;

    private GarageController _garageController;
    private List<GameObject> _carPlatforms;
    private List<GameObject> _carInstances;
    private CarsDatabase _carDatabase;
    private bool _isMoving = false;
    private Vector3 _targetPos;

    [Inject]
    private void Construct(
        [Inject(Id = "CarDatabase")] CarsDatabase carDatabase,
        GarageController garageController
    )
    {
        _carDatabase = carDatabase;
        _garageController = garageController;

        _carPlatforms = new List<GameObject>();
        _carInstances = new List<GameObject>();

        _garageController.ChangeCarPlatform += MovePlatforms;
    }

    private void Start()
    {
        InstanceCarPlatforms();
        InstanceCars();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
    }

    private void InstanceCarPlatforms()
    {
        for(int i = 0; i < _carDatabase.Cars.Length; i++)
        {
            _carPlatforms.Add(Instantiate(CarPlatform, Vector3.zero + new Vector3(i * PlatformDistance, 0, 0), Quaternion.identity, CarPlatformsParent.transform));
        }
    }

    private void InstanceCars()
    {
        for (int i = 0; i < _carDatabase.Cars.Length; i++)
        {
            _carInstances.Add(Instantiate(_carDatabase.Cars[i], FindChildByTag(_carPlatforms[i].transform, "CarSpawnPoint").position, Quaternion.Euler(0f, 150f, 0f), _carPlatforms[i].transform));

            _carInstances[i].GetComponent<CarPhysicSystem>().enabled = false;
        }
    }

    private static Transform FindChildByTag(Transform parent, string tag)
    {
        foreach(Transform child in parent.GetComponentInChildren<Transform>())
        {
            if(child.CompareTag(tag))
            {
                return child;
            }
        }
        return null;
    }

    private void MovePlatforms(int currentCarId)
    {
        if (_isMoving) return;

        _targetPos = new Vector3(
            -currentCarId * PlatformDistance,
            0,
            0
        );

        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        _isMoving = true;

        Vector3 startPos = CarPlatformsParent.transform.position;

        float timer = 0f;

        while (timer < CarPlatformMovementTime)
        {
            timer += Time.deltaTime;

            float t = timer / CarPlatformMovementTime;

            CarPlatformsParent.transform.position = Vector3.Lerp(
                startPos,
                _targetPos,
                t
            );

            yield return null;
        }

        CarPlatformsParent.transform.position = _targetPos;

        _isMoving = false;
    }

    private void OnDestroy()
    {
        _garageController.ChangeCarPlatform -= MovePlatforms;
    }
}
