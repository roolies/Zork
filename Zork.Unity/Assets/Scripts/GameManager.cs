using Newtonsoft.Json;
using UnityEngine;
using Zork.Common;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LocationText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI MovesText;

    [SerializeField]
    public UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService;

    private void Awake()
    {
        TextAsset gameJson = Resources.Load<TextAsset>("GameJson");
        _game = JsonConvert.DeserializeObject<Game>(gameJson.text);
        _game.Player.LocationChanged += Player_LocationChanged;
        _game.Run(InputService, OutputService);
        LocationText.text = _game.Player.CurrentRoom.Name;
    }

    private void Player_LocationChanged(object sender, Room location)
    {
        LocationText.text = location.Name;
    }

    private void Start()
    {
        InputService.SetFocus();
        LocationText.text = _game.Player.CurrentRoom.Name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InputService.ProcessInput();
            InputService.SetFocus();
        }

        if (_game.IsRunning == false)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Applicaton.Quit()
#endif
        }
    }

    private Game _game;
}
