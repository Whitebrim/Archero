// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;

    /// <summary>
    /// A GlobalVariable representing a GameState.
    /// </summary>
    [CreateAssetMenu(menuName = "Soda/GlobalVariable/GameState", order = 200)]
    public class GlobalGameState : GlobalVariableBase<GameState>
    {

    }

    /// <summary>
    /// A ScopedVariable representing either a GlobalGameState or a local GameState value.
    /// </summary>
    [System.Serializable]
    public class ScopedGameState : ScopedVariableBase<GameState, GlobalGameState>
    {

    }
}
