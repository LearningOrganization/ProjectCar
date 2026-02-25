
using Unity.Splines.Examples;
using UnityEngine;

public class RaceResultState : IRaceState
{

    private readonly RaceController _controller;

  
    public RaceResultState(RaceController controller)
    {
        _controller = controller;
    }

    public void Enter()
    {
        var calc = new SimpleTestFeeCalculator();

        _controller.ShowRaceResults(calc.MoneyCalc());
        Debug.Log("Race result state running");
    }

    public void Exit()
    {
        Debug.Log("Race result state was finished");
    }
}

public class SimpleTestFeeCalculator
{
    public int MoneyCalc()
    {
        return 10000;
    }
}