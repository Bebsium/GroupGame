using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Doll))]
public class DollGUI : MonoBehaviour
{
    private Image cdProgress;
    private GameObject guiPrefab;

    private void Start()
    {
        GetComponent<Doll>().GuiAction = Action;
        guiPrefab = Resources.Load<GameObject>("Prefab/Doll/DollGUI");
        cdProgress = Instantiate(guiPrefab, transform).FindObject("Progress").GetComponent<Image>();
    }

    private void Action(Global.DollComm dc)
    {
        switch (dc.DollCDType)
        {
            case Global.DollCDType.PossessCd:
                PossessCdRun(dc.Data);
                break;
            case Global.DollCDType.SkillCd:
                SkillCdRun();
                break;
        }
    }

    private void PossessCdRun(float amount)
    {
        bool condition = amount > 0;
        cdProgress.transform.parent.gameObject.SetActive(condition);
        if (condition)
            cdProgress.fillAmount = amount;
    }

    private void SkillCdRun()
    {
        
    }
}
