using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public enum GameState
    {
        placing,
        moving
    }
    public bool isWhiteTurn;
    public GameState gameState;
    public GameData(Game game)
    {
        isWhiteTurn = game.isWhiteTurn;
        gameState = (GameState)game.gameState;
    }
}
