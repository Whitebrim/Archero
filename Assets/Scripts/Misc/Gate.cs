using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda;
public class Gate : MonoBehaviour
{
    [SerializeField] GameEvent levelPassed;
    [SerializeField] List<GameObject> gateDoor;
    private void OnEnable()
    {
        levelPassed.onRaise.AddResponse(OpenTheGate);
    }
    private void OnDisable()
    {
        levelPassed.onRaise.RemoveResponse(OpenTheGate);
    }

    /// <summary>
    /// Opens the gate
    /// </summary>
    private void OpenTheGate()
    {
        foreach (var block in gateDoor)
        {
            block.SetActive(false);
        }
    }
}
