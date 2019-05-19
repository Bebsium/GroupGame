using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Doll))]
public class DollGUI : MonoBehaviour
{
    private GameObject guiPrefab;
    private Image cdProgress;
    private Slider hpBar;

    private void Start()
    {
        GetComponent<Doll>().GuiAction = Action;
        guiPrefab = Resources.Load<GameObject>("Prefab/Doll/DollGUI");
        GameObject canvas = Instantiate(guiPrefab, transform);
        cdProgress = canvas.FindObject("Progress").GetComponent<Image>();
        hpBar = canvas.FindObject("HPBar").GetComponent<Slider>();
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
}
