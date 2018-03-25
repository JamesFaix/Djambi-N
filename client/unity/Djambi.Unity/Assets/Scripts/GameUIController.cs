using System.Collections.Generic;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;
using UnityEngine;

public class GameUIController : MonoBehaviour {

	void Start ()
    {
        PlacePiecesInInitialPositions(Controller.GameState);
        AddEntriesToLog(Controller.GameState.Log);
        Controller.GetValidSelections()
            .OnValue(ShowSelectionsOptions)
            .OnError(error => ShowError(error.Message));
    }
	
	void Update ()
    {
		
	}

    private void PlacePiecesInInitialPositions(GameState game)
    {

    }

    private void AddEntriesToLog(IEnumerable<string> messages)
    {

    }

    private void ShowSelectionsOptions(IEnumerable<Selection> selections)
    {

    }

    private void ShowError(string message)
    {

    }
}
