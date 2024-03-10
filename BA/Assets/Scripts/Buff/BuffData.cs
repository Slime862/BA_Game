using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BuffData",menuName ="BuffSystem/BuffData",order =1)]
public class BuffData : ScriptableObject
{
    //������Ϣ
    public int id;
    public string buffName;
    public string description;
    public Sprite icon;
    public int priority;
    public int maxStack;
    public string[] tags;

    //ʱ����Ϣ
    public bool isForever;
    public float duration;
    public float tickTime; 

    //�ص���
    //�����ص���
    public BaseBuffModel OnCreate;
    public BaseBuffModel OnRemove;
    public BaseBuffModel OnTick;
    //�˺��ص���
    public BaseBuffModel OnHit;
    public BaseBuffModel OnBeHurt;
    public BaseBuffModel OnKill;
    public BaseBuffModel OnBekill;





    //���·�ʽ
    public BuffUpdateTimeEnum buffUpdateTime;
    public BuffRemoveStackUpdateEnum buffRemoveStackUpdate;

   
}
