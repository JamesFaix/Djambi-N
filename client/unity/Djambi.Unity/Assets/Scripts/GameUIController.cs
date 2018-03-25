using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    private GameObject _turnCycleDisplay;
    private GameObject _playersDisplay;
    private GameObject _turnRowPrefab;
    private GameObject _playerRowPrefab;
    private Tile _selectionOptionTile;
    private Tile _selectionTile;

    private Tilemap _selectionTilemap;

    private const int _rowHeight = -38;
    private const int _initialRowOffset = 90;

    void Start()
    {
        Controller.StartGame(new[] { "Mario", "Luigi" });



        _turnCycleDisplay = GameObject.Find("TurnCycleDisplay");
        _playersDisplay = GameObject.Find("PlayersDisplay");
        _selectionTilemap = GameObject.Find("SelectionTilemap").GetComponent<Tilemap>();

        _turnRowPrefab = Resources.Load("Prefabs/TurnRow") as GameObject;
        _playerRowPrefab = Resources.Load("Prefabs/PlayerRow") as GameObject;
        
        _selectionOptionTile = Resources.Load("Tiles/yellowHighlightTile") as Tile;
        _selectionTile = Resources.Load("Tiles/greenHighlightTile") as Tile;

        SetupInitialGameState(Controller.GameState);
        AddEntriesToLog(Controller.GameState.Log);
        Controller.GetValidSelections()
            .OnValue(ShowSelectionsOptions)
            .OnError(error => ShowError(error.Message));
    }

    private void SetupInitialGameState(GameState game)
    {
        RedrawPlayersDisplay(game);
        RedrawTurnCycleDisplay(game);
    }

    private void RedrawPlayersDisplay(GameState game)
    {
        var oldRows = _playersDisplay.transform
            .OfType<Transform>()
            .Select(tr => tr.gameObject)
            .Where(tr => tr.name == "PlayerRow");

        foreach (var row in oldRows)
        {
            GameObject.Destroy(row);
        }

        var offset = _rowHeight + _initialRowOffset;

        foreach (var p in game.Players)
        {
            var row = GameObject.Instantiate(_playerRowPrefab);
            var trans = row.transform;
            trans.parent = _playersDisplay.transform;
            trans.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            trans.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            trans.localScale = Vector3.one;
            trans.localPosition = new Vector3(0, offset, 0);
            trans.GetChild(0).GetComponent<Image>().color = GetPlayerColor(p.Color);
            trans.GetChild(1).GetComponent<Text>().text = p.Name;
            offset += _rowHeight;
        }
    }

    private void RedrawTurnCycleDisplay(GameState game)
    {
        var oldRows = _turnCycleDisplay.transform
            .OfType<Transform>()
            .Select(tr => tr.gameObject)
            .Where(tr => tr.name == "TurnRow");

        foreach (var row in oldRows)
        {
            GameObject.Destroy(row);
        }

        var turns = game.TurnCycle
            .Join(game.Players,
                turnPlayerId => turnPlayerId,
                player => player.Id,
                (t, p) => new
                {
                    Name = p.Name,
                    Color = p.Color
                })
            .Select((t, n) => new
            {
                Name = t.Name,
                Color = t.Color,
                Index = n + 1
            })
            .ToList();

        var offset = _rowHeight + _initialRowOffset;

        foreach (var t in turns)
        {
            var row = GameObject.Instantiate(_turnRowPrefab);
            var trans = row.transform;
            trans.parent = _turnCycleDisplay.transform;
            trans.localScale = Vector3.one;
            trans.localPosition = new Vector3(0, offset, 0);
            //trans.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            //trans.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            trans.GetChild(0).GetComponent<Image>().color = GetPlayerColor(t.Color);
            trans.GetChild(1).GetComponent<Text>().text = t.Index.ToString();
            trans.GetChild(2).GetComponent<Text>().text = t.Name;
            offset += _rowHeight;
        }
    }

    private Color GetPlayerColor(PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Blue: return Color.blue;
            case PlayerColor.Green: return Color.green;
            case PlayerColor.Purple: return Color.magenta;
            case PlayerColor.Red: return Color.red;
            case PlayerColor.Dead: return Color.gray;

            default:
                throw new Exception($"Invalid {nameof(PlayerColor)} value ({color}).");
        }
    }

    private void AddEntriesToLog(IEnumerable<string> messages)
    {

    }

    private void ShowSelectionsOptions(IEnumerable<Selection> selections)
    {
        foreach (var sel in selections)
        {
            var pos = new Vector3Int(sel.Location.X, sel.Location.Y, 0);
            _selectionTilemap.SetTile(pos, _selectionOptionTile);
        }
    }

    private void ClearSelections()
    {
        _selectionTilemap.ClearAllTiles();
    }

    private void ShowError(string message)
    {

    }
}
