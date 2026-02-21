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
    public event Action<int> OnCountdownTick; 
    public event Action OnRaceStarted;
    public event Action OnRaceFinished;
    public event Action<float> OnTimerUpdated;

    public event Action OnFalseStart;
    public event Action OnRacePaused;
    public event Action OnRaceResumed;

    private bool _timerRunning;

    private GameObject _playerCar;
    private CarPhysicSystem _carPhysicSystem;

    private Coroutine _coroutine;

    private StateMachine<IRaceState> _fsm;

    [Inject]
    public void Construct(StateMachine<IRaceState> fsm)
    {
        _fsm = fsm;
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
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(PrepareRaceRoutine());
    }

    public void StartRaceCountdown()
    {
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
        DisableCarInput();
        OnRaceFinished?.Invoke();
        _fsm.Enter<RaceFinishState>();
    }

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

    // Calls by finish collider
    #region Collider Finish
    public void OnPlayerCrossedFinish()
    {
        if (IsRaceActive)
            FinishRace();
    }
    #endregion


}
