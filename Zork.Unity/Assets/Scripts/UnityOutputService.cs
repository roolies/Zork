using UnityEngine;
using Zork.Common;
using TMPro;

public class UnityOutputService : MonoBehaviour, IOutputService
{
    [SerializeField]
    private TextMeshProUGUI TextLine;
    public void Write(object obj)
    {
    }

    public void Write(string message)
    {
        TextLine.text = message;
    }

    public void WriteLine(object obj)
    {
        TextLine.text = obj.ToString();
    }

    public void WriteLine(string message)
    {
        TextLine.text = message;
    }
}
