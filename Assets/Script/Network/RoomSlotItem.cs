using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomSlotItem : MonoBehaviour
{
    public void Init(string nickName, int point)
    {
        _nickNameText.text = nickName;
        _ready.SetActive(false);
        GetComponent<Button>().onClick.AddListener(() => RoomController.instance.SetCameraTarget(point));
    }

    public void Ready()
    {
        _ready.SetActive(true);
    }

    [SerializeField]
    private Text _nickNameText;
    [SerializeField]
    private GameObject _ready;
    private int _point;
}
