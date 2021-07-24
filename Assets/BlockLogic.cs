using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class BlockLogic : MonoBehaviour
{
    Image image;
    Button button;
    GameManager manager=null;
    LineRenderer lineRenderer;
    bool isActive = false;
    bool isDone = false;
    // Start is called before the first frame update
    private void Awake() {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive){
            Vector2 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(1,endPoint);
        }
    }

    public void GenerateColorForBoth(BlockLogic twin, GameManager manager){
        float red = Random.Range(0,1f);
        float green = Random.Range(0, 1f); 
        float blue = Random.Range(0, 1f);
        this.image.color = new Color(red,green,blue,1);
        this.SetManager(manager);
        twin.SetManager(manager);
        twin.GetImage().color = new Color(red, green, blue, 1);
    }

    public Color GetColor(){
        return image.color;
    }

    public void SeekForPair(){
        BlockLogic currentBlock = manager.GetCurrentBlock();
        if(currentBlock ==null){
            if(IsDone()) return;
            manager.SetCurrentBlock(this);
            isActive = true;
            SetLine();
            return;
        }
        else{
            if(currentBlock.IsDone() ||
            this.Equals(currentBlock) ||
            OnOneSide(currentBlock.transform.parent)){
                return;
            }
            else{
                if(GetColor()==currentBlock.GetColor()){
                    currentBlock.OnRightChoice(this);
                }
                else{
                    Debug.Log("Wrong");
                    currentBlock.ResetBlock();
                }
            }
            manager.SetCurrentBlock(null);
        }
    }

    public void ResetBlock()
    {
        isActive = false;
        lineRenderer.positionCount = 0;
    }

    public void OnRightChoice(BlockLogic newPair)
    {
        Vector2 newPairPosition = newPair.transform.position;
        lineRenderer.SetPosition(1, newPairPosition);
        isActive = false;
        SetInActive();
        newPair.SetInActive();
        manager.AddRightPairCount();
    }

    private void SetLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.material.color = GetColor();
        lineRenderer.startColor = GetColor();
        lineRenderer.endColor = GetColor();
        lineRenderer.material.SetColor("_EMISSION", GetColor());
        Vector2 linePosition = transform.position;
        lineRenderer.SetPosition(0, linePosition);
    }

    public bool OnOneSide(Transform toCheck){
        return transform.parent.Equals(toCheck);
    }

    public Image GetImage(){
        return image;
    }

    public void SetInActive(){
        isDone = true;
    }

    public bool IsDone(){
        return isDone;
    }

    public void SetManager(GameManager manager){
        this.manager = manager;
    }
}
