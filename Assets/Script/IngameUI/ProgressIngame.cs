using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIngame : MonoBehaviour
{
    float PlayerHP, EnemyHP;
    float[] timer;//6个buff时间和1个游戏时间

    GameObject hourHand, endImage,//时针和结束画面
               icon, iconEnemy,
               skill, skillEnemy,
               HPPlayer, HPEnemy;
               
    GameObject[] buffs, buffBackgrounds;
    Vector3[] buffpositions;

    Sprite spirit, spiritEnemy, 
           buffBackground,
           success, fault;
    Sprite[] playerSprites, skillSprites, buffSprites;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < 3; i++)
            {
                if (!buffs[i])
                {
                    BuffRefresh(i, Random.Range(0, 6), Random.Range(2, 5));
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 3; i < 6; i++)
            {
                if (!buffs[i])
                {
                    BuffRefresh(i, Random.Range(0, 6), Random.Range(2, 5));
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int r = Random.Range(0, 4);
            icon.GetComponent<Image>().sprite = playerSprites[r];
            skill.GetComponent<Image>().sprite = skillSprites[r];
            skill.GetComponent<CanvasGroup>().alpha = 1;
            skill.GetComponent<CanvasGroup>().interactable = true;
            skill.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            int r = Random.Range(0, 4);
            iconEnemy.GetComponent<Image>().sprite = playerSprites[r];
            skillEnemy.GetComponent<Image>().sprite = skillSprites[r];
            skillEnemy.GetComponent<CanvasGroup>().alpha = 1;
            skillEnemy.GetComponent<CanvasGroup>().interactable = true;
            skillEnemy.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerHP -= 5f;
            HPPlayer.GetComponent<Slider>().value = PlayerHP / 100f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EnemyHP -= 5f;
            HPEnemy.GetComponent<Slider>().value = EnemyHP / 100f;
        }
        BuffProgress();
        EndCheck();
    }

    void Init()
    {
        endImage = GameObject.Find("End");
        endImage.GetComponent<CanvasGroup>().alpha = 0;
        endImage.GetComponent<CanvasGroup>().interactable = false;
        endImage.GetComponent<CanvasGroup>().blocksRaycasts = false;

        hourHand = GameObject.Find("HourHandParent");//加载时针
        icon = GameObject.Find("Icon");
        iconEnemy = GameObject.Find("IconEnemy");
        skill = GameObject.Find("Skill");
        skill.GetComponent<CanvasGroup>().alpha = 0;
        skill.GetComponent<CanvasGroup>().interactable = false;
        skill.GetComponent<CanvasGroup>().blocksRaycasts = false;
        skillEnemy = GameObject.Find("SkillEnemy");
        skillEnemy.GetComponent<CanvasGroup>().alpha = 0;
        skillEnemy.GetComponent<CanvasGroup>().interactable = false;
        skillEnemy.GetComponent<CanvasGroup>().blocksRaycasts = false;

        PlayerHP = EnemyHP = 100f;
        HPPlayer = GameObject.Find("HP");
        HPPlayer.GetComponent<Slider>().value = PlayerHP / 100f;
        HPEnemy = GameObject.Find("HPEnemy");
        HPEnemy.GetComponent<Slider>().value = EnemyHP / 100f;

        spirit = Resources.Load<Sprite>("UIImage/IngameUI/spirit");
        icon.GetComponent<Image>().sprite = spirit;
        spiritEnemy = Resources.Load<Sprite>("UIImage/IngameUI/spiritEnemy");
        iconEnemy.GetComponent<Image>().sprite = spiritEnemy;

        playerSprites = new Sprite[4]
        {
            Resources.Load<Sprite>("UIImage/IngameUI/1"),
            Resources.Load<Sprite>("UIImage/IngameUI/2"),
            Resources.Load<Sprite>("UIImage/IngameUI/3"),
            Resources.Load<Sprite>("UIImage/IngameUI/4"),
        };
        skillSprites = new Sprite[4]
        {
            Resources.Load<Sprite>("UIImage/IngameUI/帽子"),
            Resources.Load<Sprite>("UIImage/IngameUI/mao"),
            Resources.Load<Sprite>("UIImage/IngameUI/兔子技能图标"),
            Resources.Load<Sprite>("UIImage/IngameUI/红心"),
        };
        buffBackground = Resources.Load<Sprite>("UIImage/IngameUI/buff");
        buffSprites = new Sprite[6]
        {
            Resources.Load<Sprite>("UIImage/IngameUI/暴走"),
            Resources.Load<Sprite>("UIImage/IngameUI/点燃"),
            Resources.Load<Sprite>("UIImage/IngameUI/禁锢"),
            Resources.Load<Sprite>("UIImage/IngameUI/无敌"),
            Resources.Load<Sprite>("UIImage/IngameUI/沾湿"),
            Resources.Load<Sprite>("UIImage/IngameUI/眩晕、硬直"),
        };

        success = Resources.Load<Sprite>("UIImage/IngameUI/success");
        fault = Resources.Load<Sprite>("UIImage/IngameUI/fault");


        buffpositions = new Vector3[6]
        {
            new Vector3(140, -45, 0),
            new Vector3(95, -45, 0),
            new Vector3(50, -45, 0),

            new Vector3(140, -45, 0),
            new Vector3(95, -45, 0),
            new Vector3(50, -45, 0),
        };
        buffs = new GameObject[6];
        buffBackgrounds = new GameObject[6];

        timer = new float[7] { 4f, 4f, 4f, 4f, 4f, 4f, 60f };
    }
    void BuffRefresh(int i, int j, float t)
    {
        timer[i] = t;
        buffs[i] = Instantiate(Resources.Load("Prefab/imagePrefab") as GameObject);
        buffBackgrounds[i] = Instantiate(Resources.Load("Prefab/imagePrefab") as GameObject);
        buffs[i].GetComponent<Image>().sprite = buffSprites[j];
        buffBackgrounds[i].GetComponent<Image>().sprite = buffBackground;

        if (i < 3)
        {
            buffs[i].transform.SetParent(GameObject.Find("IngameUI/HPBackground").transform);
            buffBackgrounds[i].transform.SetParent(GameObject.Find("IngameUI/HPBackground").transform);

        }
        else
        {
            buffs[i].transform.SetParent(GameObject.Find("IngameUI/HPEnemyBackground").transform);
            buffBackgrounds[i].transform.SetParent(GameObject.Find("IngameUI/HPEnemyBackground").transform);
        }
        buffs[i].transform.localPosition = buffBackgrounds[i].transform.localPosition = buffpositions[i];
        buffs[i].transform.localScale = buffBackgrounds[i].transform.localScale = new Vector3(0.4f, 0.4f, 1);
    }
    void BuffProgress()
    {
        for (int i = 0; i < 6; i++)
        {
            if ((buffs[i] && timer[i] > 0))
            {
                timer[i] -= Time.deltaTime;
            }
            if ((buffs[i] && timer[i] < 0))
            {
                Object.Destroy(buffs[i]);
                Object.Destroy(buffBackgrounds[i]);
            }
        }
        
    }
    void EndCheck()
    {
        if (timer[6] > 1f / 60f)
        {
            timer[6] -= Time.deltaTime;
            hourHand.transform.Rotate(new Vector3(0, 0, -1f / 10f));
        }
        else
        {
            if(PlayerHP>=EnemyHP)endImage.GetComponent<Image>().sprite = success;
            else endImage.GetComponent<Image>().sprite = fault;
            endImage.GetComponent<CanvasGroup>().alpha = 1;
            endImage.GetComponent<CanvasGroup>().interactable = true;
            endImage.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
