using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;

public class InputController : MonoBehaviour 
{
	private Plane _plane;

    void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.zero);

        Controller.StartGame(new[] { "Mario", "Luigi" })
            .OnValue(game => { return; })
            .OnError(err => { return; });
	}

	void OnMouseDown()
	{
        //Note, this only works if the camera is orthographic and perpendicular to the board
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var x = Mathf.FloorToInt(pos.x);
        var y = Mathf.FloorToInt(pos.y);
        var loc = Location.Create(x, y);

    }
}
