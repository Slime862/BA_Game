using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ͻ����ǹ�����ص��ӣ�С����
/// </summary>
[CreateAssetMenu(fileName = "AssaultRifle",menuName ="BulletSystem/AssaultRifle",order =2)]
public class AssaultRifle : BulletLauncher
{
    

    public override void Shoot(ChaState chaState)
    {
        //***TODO  Ҫ��������ļ��

        if (Time.time - lastShootTime < shootInterval) return;

        lastShootTime = Time.time;
        // ����ҵ�ǰλ�������ӵ�??ǹ��
        Vector3 position = chaState.transform.position + chaState.transform.forward * 0.25f + chaState.transform.up * 0.3f;


        BulletState bullet = BulletManager.Instance.pool.Get();
        //���ӵ���һЩ��ֵ��ʼ��
        bullet.gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);

        Debug.Log("Shoot!!!!!!");

        //GameObject bullet = Instantiate(BulletManager.Instance.defaultBullet, position, Quaternion.identity);

        // ���ӵ��ķ�������Ϊ������ҵ� Z ����
        Vector3 direction = chaState.transform.forward;
        bullet.transform.forward = GetRandomDirectionOffset(direction);
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
    // ��ȡ��ҵ�ǰ��������ô˷����������ƫ�뼸��
    public static Vector3 GetRandomDirectionOffset(Vector3 currentDirection)
    {
        

        // ����һ�������ƫ�ƽǶȣ��������ƫ�ƽǶ�Ϊ3�ȣ�
        float randomAngle = Random.Range(-15f, 15f);

        // ����ǰ�����������ƫת����Ƕ�
        Quaternion newRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Vector3 newDirection = newRotation * currentDirection;

        return newDirection;
    }



}
