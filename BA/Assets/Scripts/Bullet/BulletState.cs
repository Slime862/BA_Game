using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletState : MonoBehaviour
{
    ///<summary>
    ///����һ���������ӵ�
    ///</summary>
    public BulletData model;

    ///<summary>
    ///Ҫ�����ӵ�������˵�gameObject��������Ͻ�ɫ��ӵ��ChaState�ģ�
    ///��Ȼ������null����ģ�����дЧ���߼���ʱ���С��caster��null�����
    ///</summary>
    public GameObject caster;

    ///<summary>
    ///�ӵ��ĳ��ٶȣ���λ����/�룬��Tween��ϻ��Tween�õ���ǰ�ƶ��ٶ�
    ///</summary>
    public float speed;

    ///<summary>
    ///�ӵ����������ڣ���λ����
    ///</summary>
    public float duration;

    ///<summary>
    ///�ӵ��Ѿ������˶���ˣ���λ����
    ///�Ͼ�duration�ǿ��Ա�����ģ����羭��һ��aoe���������ڼ�����
    ///</summary>
    public float timeElapsed = 0;
    /// <summary>
    /// �ӵ�������ЧĿ��Ĵ���
    /// </summary>
    public int hitTimer;           
    /// <summary>
    /// �ӵ�Ŀǰ���е��ˣ����û����Ч���У������null
    /// </summary>
    public GameObject currentTouch=null;
    /// <summary>
    /// �����ӵ�ʱ��ɫ�ĳ���
    /// </summary>
    public Vector3 createDirection;

    /// <summary>
    /// ���Թ����������tag����
    /// </summary>
    public bool isPlayerBullet;


    public void Set(BulletData model,Vector3 createDirection,GameObject caster= null, float speed = 2f, float duration = 5f,bool isPlayerBullet= true,int hitTimer=1 )
    {
        this.model=model;
        this.caster = caster;
        this.speed = speed;
        this.duration = duration;
        this.createDirection=createDirection;
        this.isPlayerBullet = isPlayerBullet;
        this.hitTimer = hitTimer;


    }
    

}
