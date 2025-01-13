using UnityEngine;

/// <summary>
/// currently not used by new Design Choices
/// </summary>
public class Simulation : MonoBehaviour
{
    [SerializeField]
    Configuration config = null;

    private void Awake()
    {
        config.CurrentSimulationMode = SimulationMode.DEFAULT;
        config.CurrentSimulationMode = SimulationMode.NONE;
    }
}
