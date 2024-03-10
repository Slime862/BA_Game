using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BuffData",menuName ="BuffSystem/BuffData",order =1)]
public class BuffData : ScriptableObject
{
    //基本信息
    public int id;
    public string buffName;
    public string description;
    public Sprite icon;
    public int priority;
    public int maxStack;
    public string[] tags;

    //时间信息
    public bool isForever;
    public float duration;
    public float tickTime; 

    //回调点
    //基础回调点
    public BaseBuffModel OnCreate;
    public BaseBuffModel OnRemove;
    public BaseBuffModel OnTick;
    //伤害回调点
    public BaseBuffModel OnHit;
    public BaseBuffModel OnBeHurt;
    public BaseBuffModel OnKill;
    public BaseBuffModel OnBekill;





    //更新方式
    public BuffUpdateTimeEnum buffUpdateTime;
    public BuffRemoveStackUpdateEnum buffRemoveStackUpdate;

   
}
