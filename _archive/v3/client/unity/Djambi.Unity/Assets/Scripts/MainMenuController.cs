using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Djambi.Engine;
using Djambi.Engine.Extensions;

public class MainMenuController : MonoBehaviour
{
    private EventSystem _system;

    private List<Selectable> _selectables;

    private List<InputField> _inputFields;

    private Text _validationTextBox;

    void Start ()
    {
        _system = EventSystem.current;

        var inputFieldObjects = Enumerable.Range(1, 4)
            .Select(n => GameObject.Find($"NameInputField{n}"));

        var startGameButtonObject = GameObject.Find("StartGameButton");

        _inputFields = inputFieldObjects
            .Select(obj => obj.GetComponent<InputField>())
            .ToList();

        _selectables = inputFieldObjects
            .Concat(new []{ startGameButtonObject })
            .Select(obj => obj.GetComponent<Selectable>())
            .ToList();

        var startGameButton = startGameButtonObject.GetComponent<Button>();
        startGameButton.onClick.AddListener(TryStartGame);

        _validationTextBox = GameObject.Find("ValidationText")
            .GetComponent<Text>();
    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var current = _system.currentSelectedGameObject.GetComponent<Selectable>();
            var index = _selectables.IndexOf(current);
            Selectable next;

            if (index == _selectables.Count - 1)
            {
                //Loop to beginning
                next = _selectables[0];
            }
            else
            {
                next = _selectables[index + 1];
            }

            if (next != null)
            {
                //if it's an input field, also set the text caret
                next.GetComponent<InputField>()
                    ?.OnPointerClick(new PointerEventData(_system));

                _system.SetSelectedGameObject(next.gameObject, new BaseEventData(_system));
            }
            else
            {
                Debug.Log("Next UI element not found");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            TryStartGame();
        }
    }

    private void TryStartGame()
    {
        _validationTextBox.text = "";

        var names = GetEnteredPlayerNames();

        Controller.StartGame(names)
           .OnValue(game =>
           {
               SceneManager.LoadScene("GameScene");
           })
           .OnError(error =>
           {
               _validationTextBox.text = error.Message;
           });
    }

    private List<string> GetEnteredPlayerNames() =>
        _inputFields
            .Select(field => field.text)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();
}
