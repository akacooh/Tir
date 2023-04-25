using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateChangeResponder
{
    void OnStateChanged(GameState state);
}
