using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Configuration", menuName = "TestWires/Configuration", order = 0)]
public class Configuration : ScriptableObject {
    [SerializeField] float startTime = 30;
    [SerializeField] int baseValue = 3;
    [SerializeField] int maxElements = 15;
    [SerializeField] float minTime = 10;

    public float GetTime(){
        return startTime;
    }

    public int GetCount(){
        return baseValue;
    }

    public int GetMaxElements(){
        return maxElements;
    }

    public float GetMinTime(){
        return minTime;
    }

}