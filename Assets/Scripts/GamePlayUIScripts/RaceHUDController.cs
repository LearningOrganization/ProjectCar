using System;
using TMPro;
using UnityEngine;
using Zenject;

public class RaceHUDController : MonoBehaviour
{
    [SerializeField] private GameObject _preparePanel;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private GameObject _countdownPanel;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private GameObject _finishPanel;

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
    }

    private void ShowPrepare()
    {
        _preparePanel.SetActive(true);
        _countdownPanel.SetActive(false);
        _hudPanel.SetActive(false);
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

    private void OnDestroy()
    {
        _raceController.OnPreparePhaseStarted -= ShowPrepare;
        _raceController.OnCountdownStarted -= ShowCountdown;
        _raceController.OnCountdownTick -= UpdateCountdownTick;
        _raceController.OnRaceStarted -= ShowHUD;
        _raceController.OnRaceFinished -= ShowFinish;
        _raceController.OnTimerUpdated -= UpdateTimer;
    }
}
