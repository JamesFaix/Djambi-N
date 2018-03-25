using Djambi.Engine;
using Djambi.Model;
using UnityEngine;

public class BoardClickDetector : MonoBehaviour 
{
	private Plane _plane;

    private GameUIController _uiController;

    void Start()
    {
        _uiController = GameObject.Find("Canvas").GetComponent<GameUIController>();    
    }

    void OnMouseDown()
	{
        //Note, this only works if the camera is orthographic and perpendicular to the board
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var x = Mathf.FloorToInt(pos.x);
        var y = Mathf.FloorToInt(pos.y);
        var loc = Location.Create(x, y);
        if (loc.X >= 1
         && loc.Y >= 1 
         && loc.X <= Constants.BoardSize 
         && loc.Y <= Constants.BoardSize)
        {
            _uiController.OnCellClick(loc);
        }
    }
}
