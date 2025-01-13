using UnityEngine;

/// <summary>
/// currently not used by new Design Choices
/// </summary>
public class ButtonGameModeSwitchFunction : MonoBehaviour
{
    [SerializeField]
    Configuration config = null;

    //used for Menu switching Buttons
    public void InvokeGameModeChangedEvent(int newGameMode)
    {
        config.CurrentSimulationMode = (SimulationMode) newGameMode;
    }
}
