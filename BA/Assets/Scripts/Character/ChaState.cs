using Ba;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChaState : MonoBehaviour
{

    public ChaProperty property = new ChaProperty();
    ///<summary>
    ///��ɫ���е���Դ������hp֮���
    ///</summary>

    public ChaResource resource = new ChaResource(1);

    public BulletLauncher bulletLauncher;

    public DamageData damageData;
    ///<summary>
    ///��ɫ�Ƿ��Ѿ����ˣ��ⲻ�������ϵͳ�жϣ�����ϵͳӦ�ø�����
    ///</summary>
    public bool dead = false;
    /// <summary>
    /// ��ɫ�����ܷ��ƶ�
    /// </summary>
    public bool canMove = true;
    ///<summary>
    ///��ɫ���޵�״̬����ʱ�䣬������޵�״̬�У��ӵ�������ײ��DamageInfo������Ч��
    ///��λ����
    ///</summary>
    public float immuneTime
    {
        get
        {
            return _immuneTime;
        }
        set
        {
            _immuneTime = Mathf.Max(_immuneTime, value);
        }
    }
    private float _immuneTime = 0.00f;

   





}
