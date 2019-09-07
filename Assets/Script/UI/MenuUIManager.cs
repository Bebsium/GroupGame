using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    public GameObject canvas;
    GameObject startBtn;
    GameObject characterMenu;
    GameObject settingMenu;
    GameObject startBG;

    bool onSetting;
    bool onCharacter;
    public GameObject doll;
    List<GameObject> dollList;
    public List<AudioClip> voiceList;
    public Toggle window_toggle;
    public Toggle SE_toggle;
    public Toggle voice_toggle;
    public Toggle BGM_toggle;
    private AudioSource _BGM;
    private AudioSource _SE;
    private AudioSource _voice;
    
    // Start is called before the first frame update
    void Start()
    {
        dollList = new List<GameObject>();
        for(int i=0; i<doll.transform.childCount; i++)
        {
            dollList.Add(doll.transform.GetChild(i).gameObject);
            dollList[i].SetActive(false);
        }

        startBtn = canvas.transform.Find("StartBut").gameObject;
        settingMenu= canvas.transform.Find("SettingMenu").gameObject;
        characterMenu= canvas.transform.Find("CharacterMenu").gameObject;
        startBG = canvas.transform.Find("StartBG").gameObject;
        settingMenu.SetActive(false);
        characterMenu.SetActive(false);
        _BGM=gameObject.GetComponent<AudioSource>();
        _voice=doll.GetComponent<AudioSource>();
    }

    public void Play(){
        SceneManager.LoadScene("Default");
    } 

    public void Setting(){
        onSetting=!onSetting;
        startBtn.SetActive(!onSetting);
        settingMenu.SetActive(onSetting);
        characterMenu.SetActive(false);
    }

    int index = 0;
    int windowSetting;
    public void Character(){
        onCharacter=!onCharacter;
        startBtn.SetActive(!onCharacter);
        settingMenu.SetActive(false);
        characterMenu.SetActive(onCharacter);
        dollList[index].SetActive(true);
    }
    
    public void CharacterChangeRight()
    {
        index++;
        _voice.Stop();
        if (index >= doll.transform.childCount) {
            index = 0; 
            dollList[doll.transform.childCount-1].SetActive(false);
            dollList[index].SetActive(true);
        }else{
            dollList[index-1].SetActive(false);
            dollList[index].SetActive(true);
        }
        
    }

    public void CharacterChangeLeft()
    {
        index--;
        _voice.Stop();
        if (index < 0) {
            index = doll.transform.childCount-1;
            
            dollList[0].SetActive(false);
            dollList[index].SetActive(true);
        }else{
            dollList[index + 1].SetActive(false);
            dollList[index].SetActive(true);
        }
    }
    
    public void CharacterVoice(){
        GameObject temp = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(temp.name=="HatBtn"){
            _voice.clip=voiceList[0];
            _voice.Play();
        }else if(temp.name=="CatBtn"){
            _voice.clip=voiceList[1];
            _voice.Play();
        }else if(temp.name=="QueenBtn"){
            _voice.clip=voiceList[2];
            _voice.Play();
        }else{
            _voice.clip=voiceList[3];
            _voice.Play();
        }
    }

    public void PageChange(){
        GameObject temp = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(temp.name=="DescriptionBtn"){
            index=1;
            dollList[0].SetActive(false);
            dollList[index].SetActive(true);
        }else if(temp.name=="HatBtn"){
            index=2;
            dollList[0].SetActive(false);
            dollList[index].SetActive(true);
        }else if(temp.name=="CatBtn"){
            index=3;
            dollList[0].SetActive(false);
            dollList[index].SetActive(true);
        }else if(temp.name=="QueenBtn"){
            index=4;
            dollList[0].SetActive(false);
            dollList[index].SetActive(true);
        }else{
            index=5;
            dollList[0].SetActive(false);
            dollList[index].SetActive(true);
        }
        
    }

    public void StartGame()
    {
        Destroy(startBG.gameObject);
        startBtn.SetActive(true);
    }

    public void Window()
    {
        if (window_toggle.isOn == true)
        {
            Screen.fullScreen = Screen.fullScreen;
            PlayerPrefs.SetInt("windowSetting", 1);
        }
        else
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public void SE()
    {
        if (SE_toggle.isOn == true)
        {
            PlayerPrefs.SetInt("SEVolume",1);
        }else
        {
            PlayerPrefs.SetInt("SEVolume",0);
        }
    }

    public void Voice()
    {
        if (voice_toggle.isOn == true)
        {
            PlayerPrefs.SetInt("voiceVolume",1);
        }else
        {
            PlayerPrefs.SetInt("voiceVolume",0);
        }
        _voice.volume=PlayerPrefs.GetInt("voiceVolume");
    }

    public void BGM()
    {
        if (BGM_toggle.isOn == true)
        {
            PlayerPrefs.SetInt("BGMVolume",1);
        }else
        {
            PlayerPrefs.SetInt("BGMVolume",0);
        }
        _BGM.volume=PlayerPrefs.GetInt("BGMVolume");
    }



}
