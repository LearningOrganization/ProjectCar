
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class CarUI : MonoBehaviour
{
    [SerializeField] private Slider RPMSlider;
    [SerializeField] private TMP_Text CurrentRPM;
    [SerializeField] private TMP_Text CurrentGear;
    [SerializeField] private TMP_Text CurrentSpeed;

    [HideInInspector]
    public CarPhysicSystem _carPhysicSystem;

    void Start()
    {
        _carPhysicSystem.CarTelemetry += UpdateCarTelemetry;
        RPMSlider.minValue = 0;
        RPMSlider.maxValue = _carPhysicSystem.CarConfigData.EngineConfig.MaxRPM;
    }

    private void UpdateCarTelemetry(CarTelemetry carTelemetry)
    {

        RPMSlider.value = carTelemetry.RPM;

        CurrentRPM.text = $"{carTelemetry.RPM:F0}";

        string gearName = carTelemetry.Gear switch
        {
            -1 => "R",
            0 => "N",
            _ => carTelemetry.Gear.ToString()
        };

        CurrentGear.text = gearName;

        CurrentSpeed.text = carTelemetry.Speed.ToString("F0");
    }

    private void OnDestroy()
    {
        if (_carPhysicSystem != null)
            _carPhysicSystem.CarTelemetry -= UpdateCarTelemetry;
    }
}