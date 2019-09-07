using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Doll))]
public class DollGUI : MonoBehaviour //,IPunOwnershipCallbacks
{
    public void NickName(string name)
    {
        nickName.gameObject.SetActive(name != "");
        nickName.text = name;
    }

    private GameObject guiPrefab;
    private Image cdProgress;
    private Slider hpBar;
    private Text nickName;

    private void Start()
    {
        GetComponent<Doll>().GuiAction = Action;
        guiPrefab = Resources.Load<GameObject>("Prefab/Doll/DollGUI");
        GameObject canvas = Instantiate(guiPrefab, transform);
        canvas.name = "DollGUI";
        cdProgress = canvas.FindObject("Progress").GetComponent<Image>();
        hpBar = canvas.FindObject("HPBar").GetComponent<Slider>();
        nickName = canvas.FindObject("NickName").GetComponent<Text>();
    }

    private void Action(Global.DollComm dc)
    {
        switch (dc.DollCDType)
        {
            case Global.DollCDType.PossessCd:
                PossessCdRun(dc.Data);
                break;
            case Global.DollCDType.SkillCd:
                break;
            case Global.DollCDType.HPBar:
                HPBar(dc.Data);
                break;
        }
    }

    private void PossessCdRun(float amount)
    {
        bool condition = amount > 0;
        hpBar.gameObject.SetActive(false);
        cdProgress.transform.parent.gameObject.SetActive(condition);
        if (condition)
            cdProgress.fillAmount = amount;
    }

    private void HPBar(float data)
    {
        hpBar.gameObject.SetActive(data>0f);
        hpBar.value = data;
    }

    //public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    //{
        
    //}

    //public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    //{
    //    if (photonView.IsSceneView)
    //    {
    //        nickName.gameObject.SetActive(false);
    //    }
    //    else if (photonView.IsOwnerActive)
    //    {
    //        if (!photonView.IsMine)
    //        {
    //            nickName.gameObject.SetActive(true);
    //            print("NickName :" + targetView.Owner.NickName);
    //            nickName.text = targetView.Owner.NickName;
    //        }
    //    }
    //}
}
