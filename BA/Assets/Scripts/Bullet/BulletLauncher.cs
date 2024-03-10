using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����ģ��Ľű����ں����������ݺͷ��䷽�������Ա��̳���д
/// </summary>
public class BulletLauncher : ScriptableObject
{
    ///<summary>
    ///Ҫ������ӵ�
    ///</summary>
    public BulletData model;

    ///<summary>
    ///Ҫ�����ӵ�������˵�gameObject��������Ͻ�ɫ��ӵ��ChaState�ģ�
    ///��Ȼ������null����ģ�����дЧ���߼���ʱ���С��caster��null�����
    ///</summary>
    public GameObject caster;

    /// <summary>
    /// ����ķ���
    /// </summary>
    //*******************************TODO�ĳ���bulletmanager��Ķ��������

    public float bulletSpeed=3;

    public  float shootInterval = 0.3f;

    //���ֻ��Ĭ�ϵ����������������������ķ�ʽ����Ӧ���Լ�д
    public float lastShootTime;

    public virtual void Shoot(ChaState chaState)
    {
        //***TODO  Ҫ��������ļ��

        if (Time.time - lastShootTime < shootInterval) return;

        lastShootTime = Time.time;
        // ����ҵ�ǰλ�������ӵ�??ǹ��
        Vector3 position = chaState.transform.position + chaState.transform.forward * 0.05f+chaState.transform.up*0.05f;


        BulletState bullet = BulletManager.Instance.pool.Get();
        //���ӵ���һЩ��ֵ��ʼ��
        bullet.gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);

        Debug.Log("Shoot!!!!!!");

        //GameObject bullet = Instantiate(BulletManager.Instance.defaultBullet, position, Quaternion.identity);

        // ���ӵ��ķ�������Ϊ������ҵ� Z ����
        Vector3 direction = chaState.transform.forward;
        bullet.transform.forward = direction;
        //BulletState bulletState = bullet.AddComponent<BulletState>();


        if (chaState.gameObject.tag == "Character")//����ҽ�ɫ������ӵ� �������ӵ����ܹ�����ҽ�ɫ
        {
            bullet.Set(model, direction, chaState.gameObject, bulletSpeed, 5, true);

        }
        else if (chaState.gameObject.tag == "Enemy")//�ǵ��˽�ɫ������ӵ� �������ӵ����ܹ������˽�ɫ
        {
            bullet.Set(model, direction, chaState.gameObject, bulletSpeed, 5, false);

        }


        //BulletManager.Instance.bullet.Add(bulletState);
        //TODO:�����Ż�һ�·�manager��
        if (bullet.model.OnCreate != null)
            bullet.model.OnCreate.Apply(bullet);

    }



}
