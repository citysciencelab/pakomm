using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Config", order = 1)]
public class Configuration : ScriptableObject
{
    // = GameMode.DEFAULT; //somehow doesnt properly recognize that onChangeEvent... --> Simulation sets it onAwake to default and then to none!
    //Persistant data - hast to be reset?
    private SimulationMode _currentSimulationMode;

    public SimulationMode CurrentSimulationMode 
    {
        get
        {
            return _currentSimulationMode;
        }
        set
        {
            SimulationMode lastSimulationMode = _currentSimulationMode;
            _currentSimulationMode = value;

            if(lastSimulationMode != _currentSimulationMode)
                GlobalEventsManager.InvokeSimulationModeChanged(_currentSimulationMode, lastSimulationMode);

            Debug.Log("_currentSimulationMode:" + _currentSimulationMode);
        }
    }
}
