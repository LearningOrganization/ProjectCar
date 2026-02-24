using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RaceUIController : MonoBehaviour
{
    [SerializeField] private GameObject _preparePanel;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private GameObject _countdownPanel;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private GameObject _finishPanel;
    [SerializeField] private GameObject _resultPanel;

    [SerializeField] private Button _resultButton;
    [SerializeField] private TMP_Text _raceTime;
    [SerializeField] private TMP_Text _feeText;

    private RaceController _raceController;

    [Inject]
    public void Construct(RaceController raceController)
    {
        _raceController = raceController;

        _raceController.OnPreparePhaseStarted += ShowPrepare;
        _raceController.OnCountdownStarted += ShowCountdown;
        _raceController.OnCountdownTick += UpdateCountdownTick;
        _raceController.OnRaceStarted += ShowHUD;
        _raceController.OnRaceFinished += ShowFinish;
        _raceController.OnTimerUpdated += UpdateTimer;
        _raceController.OnRaceResultsShown += ShowResultPanel;
    }

    void Start()
    {
        _resultButton.onClick.AddListener(() => _raceController.RequestRaceResults());
    }

    private void ShowPrepare()
    {
        _preparePanel.SetActive(true);
        _countdownPanel.SetActive(false);
        _hudPanel.SetActive(false);
        _resultPanel.SetActive(false);
    }

    private void ShowCountdown()
    {
        _preparePanel.SetActive(false);
        _countdownPanel.SetActive(true);
    }

    private void UpdateCountdownTick(int count)
    {
        _countdownText.text = count.ToString();
    }

    private void ShowHUD()
    {
        _countdownPanel.SetActive(false);
        _hudPanel.SetActive(true);
    }

    private void ShowFinish()
    {
        _hudPanel.SetActive(false);
        _finishPanel.SetActive(true);
    }

    private void UpdateTimer(float time)
    {
        _timerText.text = TimeSpan.FromSeconds(time).ToString(@"mm\:ss\.ff");
    }

    private void ShowResultPanel(int fee, float raceTime)
    {
        _finishPanel.SetActive(false);
        _resultPanel.SetActive(true);

        _raceTime.text = "Race Time: " + TimeSpan.FromSeconds(raceTime).ToString(@"mm\:ss\.ff");
        _feeText.text = "Money: " + fee.ToString();
    }

    private void OnDestroy()
    {
        _raceController.OnPreparePhaseStarted -= ShowPrepare;
        _raceController.OnCountdownStarted -= ShowCountdown;
        _raceController.OnCountdownTick -= UpdateCountdownTick;
        _raceController.OnRaceStarted -= ShowHUD;
        _raceController.OnRaceFinished -= ShowFinish;
        _raceController.OnTimerUpdated -= UpdateTimer;
        _raceController.OnRaceResultsShown -= ShowResultPanel;

    }
}
