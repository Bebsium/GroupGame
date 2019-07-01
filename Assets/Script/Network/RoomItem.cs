using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public void Init(string name, string type, int playerNum, int maxPlayer, bool isopen)
    {
        _roomName = name;
        if (playerNum == maxPlayer || !isopen)
        {
            _canEnter = false;
            gameObject.GetComponentInChildren<Button>().interactable = false;
        }
        nameText.text = _roomName;
        typeText.text = type;
        numberText.text = playerNum + " / " + maxPlayer;
    }

    public void JoinRoom()
    {
        if(_canEnter)
            SignInManager.instance.JoinRoom(_roomName);
    }
    
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text typeText;
    [SerializeField]
    private Text numberText;

    private bool _canEnter;
    private string _roomName;

    private void Start()
    {
        _canEnter = true;
    }
}
