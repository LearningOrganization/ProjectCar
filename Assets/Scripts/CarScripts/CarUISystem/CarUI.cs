
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

    private RaceController _raceController;

    [Inject]
    public void Construct(RaceController raceController)
    {
        _raceController = raceController;
    }

    void Awake()
    {
        _raceController.OnTelemetryUpdated += UpdateCarTelemetry;
    }

    private void UpdateCarTelemetry(CarTelemetry carTelemetry)
    {
        // not the best solution
        RPMSlider.maxValue = carTelemetry.MaxRPM;

        RPMSlider.value = carTelemetry.RPM;

        CurrentRPM.text = $"{carTelemetry.RPM:F0}";

        string gearName = carTelemetry.Gear switch
        {
            Gear.Reverse => "R",
            Gear.Neutral => "N",
            _ => ((int)carTelemetry.Gear).ToString()
        };

        CurrentGear.text = gearName;

        CurrentSpeed.text = carTelemetry.Speed.ToString("F0");
    }

    private void OnDestroy()
    {
        if (_raceController != null)
            _raceController.OnTelemetryUpdated -= UpdateCarTelemetry;
    }
}