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

    private Button _confirmButton;
    private Button _cancelButton;
    private Button _quitButton;
    private Text _errorText;
    private Text _confirmButtonText;
    private Text _cancelButtonText;
    private LogDisplayController _logDisplay;

    private GameObject _pieceSprite;
    private GameObject _assassinSprite;
    private GameObject _chiefSprite;
    private GameObject _diplomatSprite;
    private GameObject _journalistSprite;
    private GameObject _thugSprite;
    private GameObject _undertakerSprite;
    private GameObject _corpseSprite;

    private Dictionary<PlayerColor, Color> _playerColors;
    private Color _disabledButtonTextColor;
    private Color _enabledButtonTextColor;
    
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
        
        _pieceSprite = Resources.Load("Prefabs/PieceSprite") as GameObject;
        _assassinSprite = Resources.Load("Prefabs/AssassinSprite") as GameObject;
        _chiefSprite = Resources.Load("Prefabs/ChiefSprite") as GameObject;
        _diplomatSprite = Resources.Load("Prefabs/DiplomatSprite") as GameObject;
        _journalistSprite = Resources.Load("Prefabs/JournalistSprite") as GameObject;
        _thugSprite = Resources.Load("Prefabs/ThugSprite") as GameObject;
        _undertakerSprite = Resources.Load("Prefabs/UndertakerSprite") as GameObject;
        _corpseSprite = Resources.Load("Prefabs/CorpseSprite") as GameObject;

        _playerColors = new Dictionary<PlayerColor, Color>
        {
            [PlayerColor.Blue] = new Color32(0, 0, 204, 255),
            [PlayerColor.Green] = new Color32(0, 102, 0, 255),
            [PlayerColor.Purple] = new Color32(102, 0, 102, 255),
            [PlayerColor.Red] = new Color32(204, 0, 0, 255),
            [PlayerColor.Dead] = new Color32(102, 102, 102, 255)
        };
        _disabledButtonTextColor = new Color32(180, 180, 180, 150);
        _enabledButtonTextColor = new Color32(50, 50, 50, 255);

        var confirmButtonObject = GameObject.Find("ConfirmButton");
        var cancelButtonObject = GameObject.Find("CancelButton");
        _confirmButton = confirmButtonObject.GetComponent<Button>();
        _cancelButton = cancelButtonObject.GetComponent<Button>();
        _confirmButtonText = confirmButtonObject.GetComponentInChildren<Text>();
        _cancelButtonText = cancelButtonObject.GetComponentInChildren<Text>();
        _quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        _errorText = GameObject.Find("ErrorText").GetComponent<Text>();
        _logDisplay = GameObject.Find("LogDisplay").GetComponent<LogDisplayController>();

        _confirmButton.onClick.AddListener(ConfirmButtonClick);
        _cancelButton.onClick.AddListener(CancelButtonClick);
        _quitButton.onClick.AddListener(QuitButtonClick);

        var game = Controller.GameState;
        UpdateGameState(game);
        AddEntriesToLog(game.Log);
        EnableOrDisableConfirmButtons();
    }

    private void UpdateGameState(GameState game)
    {
        RedrawPieces(game);
        RedrawPlayersDisplay(game);
        RedrawTurnCycleDisplay(game);
        Controller.GetValidSelections()
            .OnValue(ShowSelectionsOptions)
            .OnError(error => ShowError(error.Message));
    }

    private void RedrawPieces(GameState game)
    {
        var pieceSprites = GameObject.FindGameObjectsWithTag("Piece");
        foreach (var sprite in pieceSprites)
        {
            GameObject.Destroy(sprite);
        }

        foreach (var piece in game.Pieces)
        {
            var background = GameObject.Instantiate(_pieceSprite);
            var backgroundColor = GetPieceColor(piece, game);
            background.GetComponent<SpriteRenderer>().color = backgroundColor;

            var foreground = GameObject.Instantiate(GetPieceForegroundSprite(piece));

            background.transform.Translate(piece.Location.X + 0.5f, piece.Location.Y + 0.5f, 0);
            foreground.transform.Translate(piece.Location.X + 0.5f, piece.Location.Y + 0.5f, 0);
        }
    }
    
    private GameObject GetPieceForegroundSprite(Piece piece)
    {
        if (!piece.IsAlive)
        {
            return _corpseSprite;
        }

        switch (piece.Type)
        {
            case PieceType.Assassin: return _assassinSprite;
            case PieceType.Chief: return _chiefSprite;
            case PieceType.Diplomat: return _diplomatSprite;
            case PieceType.Journalist: return _journalistSprite;
            case PieceType.Thug: return _thugSprite;
            case PieceType.Undertaker: return _undertakerSprite;

            default:
                throw new Exception($"Invalid {nameof(PieceType)} value ({piece.Type})");
        }
    }

    private Color GetPieceColor(Piece piece, GameState game)
    {
        if (!piece.IsAlive)
        {
            return _playerColors[PlayerColor.Dead];
        }

        var owner = game.Players.Single(p => p.Id == piece.PlayerId);
        return _playerColors[owner.Color];
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
            trans.localScale = Vector3.one;
            trans.localPosition = new Vector3(0, offset, 0);
            trans.GetChild(0).GetComponent<Image>().color = _playerColors[p.Color];
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
            trans.GetChild(0).GetComponent<Image>().color = _playerColors[t.Color];
            trans.GetChild(1).GetComponent<Text>().text = t.Index.ToString();
            trans.GetChild(2).GetComponent<Text>().text = t.Name;
            offset += _rowHeight;
        }
    }

    private void AddEntriesToLog(IEnumerable<string> messages)
    {
        _logDisplay.Append(messages);
        _logDisplay.ScrollToEnd();
    }

    private void ShowSelectionsOptions(IEnumerable<Selection> selections)
    {
        ShowError("");

        foreach (var sel in selections)
        {
            var pos = new Vector3Int(sel.Location.X, sel.Location.Y, 0);
            _selectionTilemap.SetTile(pos, _selectionOptionTile);
        }
    }

    private void ShowSelections(IEnumerable<Selection> selections)
    {
        foreach (var sel in selections)
        {
            var pos = new Vector3Int(sel.Location.X, sel.Location.Y, 0);
            _selectionTilemap.SetTile(pos, _selectionTile);
        }
    }

    private void ClearSelections()
    {
        _selectionTilemap.ClearAllTiles();
    }

    public void OnCellClick(Location location)
    {
        var state = Controller.TurnState.Status;
        switch (state)
        {
            case TurnStatus.AwaitingSelection:
                Controller.MakeSelection(location)
                    .OnValue(_ => OnSelectionMade())
                    .OnError(error => ShowError(error.Message));
                break;

            case TurnStatus.AwaitingConfirmation:
            case TurnStatus.Paused:
            case TurnStatus.Error:
                break; //Do nothing; you can't click cells or pieces in these states

            default:
                throw new Exception($"Invalid {nameof(TurnStatus)} ({state})");
        }
    }

    private void OnSelectionMade()
    {
        ClearSelections();
        ShowSelections(Controller.TurnState.Selections);
        Controller.GetValidSelections()
            .OnValue(ShowSelectionsOptions)
            .OnError(error => ShowError(error.Message));
        EnableOrDisableConfirmButtons();
    }

    private void ShowError(string message)
    {
        _errorText.text = message;
    }

    private void EnableOrDisableConfirmButtons()
    {
        if (Controller.TurnState.Status == TurnStatus.AwaitingConfirmation)
        {
            _confirmButton.interactable = true;
            _confirmButtonText.color = _enabledButtonTextColor;
        }
        else
        {
            _confirmButton.interactable = false;
            _confirmButtonText.color = _disabledButtonTextColor;
        }

        if (Controller.TurnState.Selections.Any())
        {
            _cancelButton.interactable = true;
            _cancelButtonText.color = _enabledButtonTextColor;
        }
        else
        {
            _cancelButton.interactable = false;
            _cancelButtonText.color = _disabledButtonTextColor;
        }
    }

    private void ConfirmButtonClick()
    {
        var previousLogCount = Controller.GameState.Log.Count;

        Controller.ConfirmTurn()
            .OnValue(game =>
            {
                ClearSelections();
                EnableOrDisableConfirmButtons();
                UpdateGameState(game);
                AddEntriesToLog(game.Log.Skip(previousLogCount));
            })
            .OnError(error => ShowError(error.Message));
    }

    private void CancelButtonClick()
    {
        Controller.CancelTurn()
            .OnValue(_ =>
            {
                ClearSelections();
                EnableOrDisableConfirmButtons();
                Controller.GetValidSelections()
                    .OnValue(ShowSelections)
                    .OnError(error => ShowError(error.Message));
            })
            .OnError(error => ShowError(error.Message));
    }

    private void QuitButtonClick()
    {
        //Prompt user to confirm
        //Return to main menu
    }
}
