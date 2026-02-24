using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Zenject;

public class RaceController : MonoBehaviour
{
    // racing data
    public float RaceTime { get; private set; }
    public bool IsRaceActive { get; private set; }
    
    // Events for FSM and another systems
    public event Action OnPreparePhaseStarted;
    public event Action OnCountdownStarted;
    public event Action<int> OnCountdownTick; 
    public event Action OnRaceStarted;
    public event Action OnRaceFinished;
    public event Action<float> OnTimerUpdated;
    public event Action<int, float> OnRaceResultsShown;

    // not released
    public event Action OnFalseStart;
    public event Action OnRacePaused;
    public event Action OnRaceResumed;

    private bool _timerRunning;

    private GameObject _playerCar;
    private CarPhysicSystem _carPhysicSystem;

    private Coroutine _coroutine;

    private StateMachine<IRaceState> _fsm;
    private StateMachine<IGameState> _highLevelFSM;
    private FinishScript _finishScript;

    [Inject]
    public void Construct(StateMachine<IRaceState> fsm, FinishScript finishScript, StateMachine<IGameState> highLevelFSM)
    {
        _fsm = fsm;
        _finishScript = finishScript;
        _finishScript.OnFinishLineCrossed += OnPlayerCrossedFinish;
        _highLevelFSM = highLevelFSM;
    }

    public void SetPlayerCar(GameObject car)
    {
        _playerCar = car;
        _carPhysicSystem = car.GetComponent<CarPhysicSystem>();
    }

    public void EnableCarInput() => _carPhysicSystem.CurrentPermissions = InputPermissions.All;
    public void DisableCarInput() => _carPhysicSystem.CurrentPermissions = InputPermissions.None;

    // race preparation
    public void StartRacePreparing()
    {
        DisableCarInput();
        OnPreparePhaseStarted?.Invoke();
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(PrepareRaceRoutine());
    }

    public void StartRaceCountdown()
    {
        OnCountdownStarted?.Invoke();
        //make logic for DNF if you started in countdown 
        //EnableCarInput();
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CountdownRoutine(_carPhysicSystem));
    }

    public void StartRace()
    {
        EnableCarInput();
        IsRaceActive = true;
        RaceTime = 0f;
        OnRaceStarted?.Invoke();
    }

    public void FinishRace()
    {
        IsRaceActive = false;
        _carPhysicSystem.DisableCarEngineTorque();
        DisableCarInput();
    
        OnRaceFinished?.Invoke();
    }

    public void RequestRaceResults()
    {
        _fsm.Enter<RaceResultState>();
    }

    public void ShowRaceResults(int money)
    {
        OnRaceResultsShown?.Invoke(money, RaceTime);
    }

    public void ToGarage()
    {
        _ = _highLevelFSM.Enter<LoadingState>().LoadAndEnterNonPlayingScene<GarageState>(NonPlayingScene.GarageScene, () => Debug.Log("Welcome to your garage!"));
    }

    // Calls by finish collider
    #region Collider Finish
    public void OnPlayerCrossedFinish()
    {
        if (IsRaceActive)
            _fsm.Enter<RaceFinishState>();
    }
    #endregion

    void Update()
    {
        if(IsRaceActive)
        {
            RaceTime += Time.deltaTime;
            OnTimerUpdated?.Invoke(RaceTime);
        }
    }

    private IEnumerator CountdownRoutine(CarPhysicSystem carPhysicSystem)
    {
        int count = 3;
        while(count > 0)
        {
            OnCountdownTick?.Invoke(count);
            yield return new WaitForSeconds(1f);
            count --;

            // make logic for DNF if you started in countdown 
        }
        _fsm.Enter<RaceInProgressState>();
    }

    private IEnumerator PrepareRaceRoutine()
    {
        yield return new WaitForSeconds(5);
        _fsm.Enter<RaceCountdownState>();
    }

    private void OnDestroy()
    {
        _finishScript.OnFinishLineCrossed -= OnPlayerCrossedFinish;
    }

}
