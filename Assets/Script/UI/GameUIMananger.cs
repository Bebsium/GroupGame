using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIMananger : MonoBehaviour
{
    public GameObject Canvas;
    private List<Sprite> _loadingList=new List<Sprite>();
    private Image _background;
    private Image _loading;
    float time=2;
    float alpha;
    // Start is called before the first frame update
    void Start()
    {
        _background=Canvas.transform.Find("Background").GetComponent<Image>();
        _loading=Canvas.transform.Find("Loading").GetComponent<Image>();
        for(int i=1;i<6;i++){
            _loadingList.Add(Resources.Load<Sprite>("UIImage/Loading/loading_"+i));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;
        if(Mathf.Round(time)==2){
            _loading.sprite=_loadingList[1];
        }else if(Mathf.Round(time)==3){
            _loading.sprite=_loadingList[2];
        }else if(Mathf.Round(time)==4){
            _loading.sprite=_loadingList[3];
        }else if(Mathf.Round(time)>=5){
            alpha+=Time.deltaTime*0.5f;
            _loading.sprite=_loadingList[4];
            _background.color=Color.Lerp(Color.black,Color.clear,alpha);
        }
        
    }
    IEnumerator ImageChange(int index){
        index++;
        yield return new WaitForSeconds(1);
        _loading.sprite=_loadingList[index];
    }
}
