using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogDisplayController : MonoBehaviour
{
    private Text _textControl;
    private ScrollRect _scrollRect;

    void Start()
    {
        _textControl = GetComponentsInChildren<Text>().Single(comp => comp.name == "LogText");
        _scrollRect = GetComponentInChildren<ScrollRect>();
    }

    public void Append(IEnumerable<string> messages)
    {
        var oldText = _textControl.text;

        var text = "";
        if (oldText != "")
        {
            text = oldText + "\n";
        }

        text += string.Join("\n", messages);

        _textControl.text = text;
    }

    public void ScrollToEnd()
    {
        StartCoroutine(ScrollToEndImpl());
    }	

    private IEnumerator ScrollToEndImpl()
    {
        yield return new WaitForEndOfFrame();
        _scrollRect.gameObject.SetActive(true);
        _scrollRect.normalizedPosition = Vector2.zero;
    }
}
