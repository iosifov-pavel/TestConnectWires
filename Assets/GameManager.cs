using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform leftPanel=null;
    [SerializeField] Transform rightPanel = null;
    [SerializeField] Configuration configuration=null;
    [SerializeField] BlockLogic playableButtonPrefab = null;
    [SerializeField] TextMeshProUGUI timerUI;
    [SerializeField] TextMeshProUGUI resultUI;
    [SerializeField] TextMeshProUGUI loseScoreUI;
    [SerializeField] Button startButton = null;
    [SerializeField] GameObject gamePanel = null;
    [SerializeField] GameObject losePanel = null;
    [SerializeField] GameObject resultPanel = null;
    [SerializeField] GameObject highscoreContainrer = null;
    [SerializeField] TextMeshProUGUI resultPrefab = null;
    [SerializeField] TMP_InputField playerName = null;
    BlockLogic activeBlock = null;
    int maxPairCount = 0;
    int rightPairCount = 0;
    float timeUntilLose = 0;
    int result = 0;
    int level = 0;
    bool startPlaying = false;
    List<PlayerData> saves = new List<PlayerData>();
    // Update is called once per frame
    void Update()
    {
        if(startPlaying){
            if (timeUntilLose > 0)
            {
                timeUntilLose -= Time.deltaTime;
                timerUI.text = $"{timeUntilLose:N0}";
            }
            else
            {
                Lose();
            }
        }
    }

    public void StartPlay(){
        startPlaying = true;
        startButton.gameObject.SetActive(false);
        gamePanel.SetActive(true);
        GenerateButtons();
    }

    void GenerateButtons(){
        timeUntilLose = configuration.GetTime() - level;
        maxPairCount = configuration.GetCount() + level;
        if(timeUntilLose<configuration.GetMinTime()) timeUntilLose = configuration.GetMinTime();
        if(maxPairCount>configuration.GetMaxElements()) maxPairCount = configuration.GetMaxElements();
        rightPairCount = 0;
        for(int i=0;i<maxPairCount;i++){
            BlockLogic newButton = Instantiate(playableButtonPrefab, leftPanel);
            BlockLogic newButtonTwin = Instantiate(playableButtonPrefab, rightPanel);
            newButton.GenerateColorForBoth(newButtonTwin, this);
        }
        ShuffleBlockOnRightSide();
    }

    private void ShuffleBlockOnRightSide()
    {
        List<Transform> childs = new List<Transform>();
        List<int> childIndex = new List<int>();
        foreach(Transform child in rightPanel.transform){
            Debug.Log(child.gameObject.name);
            childs.Add(child);
            childIndex.Add(child.GetSiblingIndex());
        }
        foreach(int index in childIndex){
            int newIndex = UnityEngine.Random.Range(0,childs.Count);
            childs[newIndex].SetSiblingIndex(index);
            childs.Remove(childs[newIndex]);
        }
    }

    public void SetCurrentBlock(BlockLogic block){
        activeBlock = block;
    }

    public BlockLogic GetCurrentBlock(){
        return activeBlock;
    }

    public void AddRightPairCount(){
        rightPairCount++;
        result++;
        resultUI.text = result.ToString();
        if(rightPairCount>=maxPairCount){
            ResetGameWindowAtNewLevel();
        }
    }

    void ResetGameWindowAtNewLevel(){
        level++;
        DeleteChilds(rightPanel);
        DeleteChilds(leftPanel);
        GenerateButtons();
    }

    public void Restart(){
        level = 0;
        result = 0;
        resultUI.text = result.ToString();
        DeleteChilds(rightPanel);
        DeleteChilds(leftPanel);
        gamePanel.SetActive(true);
        losePanel.SetActive(false);
        resultPanel.SetActive(false);
        GenerateButtons();
    }

    void DeleteChilds(Transform parent){
        foreach(Transform child in parent){
            Destroy(child.gameObject);
        }
    }
    
    void Lose(){
        gamePanel.SetActive(false);
        losePanel.SetActive(true);
        loseScoreUI.text = result.ToString();
    }

    public void Save(){
        string player = playerName.text;
        bool isNewSave = false;
        using (FileStream fs = File.Open(Application.persistentDataPath + ".save", FileMode.OpenOrCreate))
        {
            if(fs.Length==0){
                isNewSave = true;
            }
            else{
                BinaryFormatter formatter = new BinaryFormatter();
                saves = (List<PlayerData>)formatter.Deserialize(fs);
            }
        }
        using(FileStream fs = File.Open(Application.persistentDataPath + ".save", FileMode.OpenOrCreate)){
            BinaryFormatter formatter = new BinaryFormatter();
            PlayerData newData = new PlayerData();
            newData.playerName = player;
            newData.score = result;
            if(isNewSave)
            {
                saves.Add(newData);
                TextMeshProUGUI score = Instantiate(resultPrefab, highscoreContainrer.transform);
                score.text = $"{newData.playerName} : {newData.score}";
            }
            else{
                bool wasWrited = false;
                List<PlayerData> newSave = new List<PlayerData>();
                foreach(PlayerData data in saves){
                    if(result<data.score){
                        newSave.Add(data);
                        continue;
                    }
                    if(result>=data.score && !wasWrited){
                        newSave.Add(newData);
                        wasWrited = true;
                    }
                    newSave.Add(data);
                }
                saves.Clear();
                saves = new List<PlayerData>(newSave);
                newSave.Clear();
                if(!wasWrited){
                    saves.Add(newData);
                }
                if(saves.Count>10){
                    saves.RemoveAt(10);
                }
                DeleteChilds(highscoreContainrer.transform);
                foreach(PlayerData data in saves){  
                    TextMeshProUGUI score = Instantiate(resultPrefab, highscoreContainrer.transform);
                    score.text = $"{data.playerName} : {data.score}";
                    if(data.score == result && data.playerName==player){
                        score.color = Color.red;
                    }
                }
            }
            formatter.Serialize(fs,saves);
        }
        losePanel.SetActive(false);
        resultPanel.SetActive(true);
    }
}
