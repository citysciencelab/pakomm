public static class GlobalEventsManager
{
    public delegate void SimulationModeChanged(SimulationMode newSimulationMode, SimulationMode lastSimulationMode);

    //we wrap those events, so they can be private, we have a better visibility on how they are used
    private static event SimulationModeChanged simulationModeChangedEvent;

    public static void AddListenerToSimulationModeChanged(SimulationModeChanged call)
    {
        simulationModeChangedEvent += call;
    }

    public static void RemoveListenerSimulationModeChanged(SimulationModeChanged call)
    {
        simulationModeChangedEvent += call;
    }

    public static void InvokeSimulationModeChanged(SimulationMode newSimulationMode, SimulationMode lastSimulationMode)
    {
        simulationModeChangedEvent?.Invoke(newSimulationMode, lastSimulationMode);
    }
}