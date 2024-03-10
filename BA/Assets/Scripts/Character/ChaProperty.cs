using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    ground,fly
}

///<summary>
///��ɫ����ֵ���Բ��֣��������hp���������ȵȶ�������
///�����һ���ṹ����Ϊ����ֻ�н�ɫ����Щ���ԣ�����װ����buff��aoe��damageInfo�ȶ�������
///
///</summary>
[System.Serializable]
public struct ChaProperty
{
    ///<summary>
    ///������������������У����½�ɫֻ��1��װ��������0
    ///</summary>
    public int hp;

    ///<summary>
    ///������
    ///</summary>
   // public int attack;

    ///<summary>
    ///�ƶ��ٶȣ���������/����Ϊ��λ�ģ�����һ��������������ֵ��
    ///����ת��Ϊ��/�룬����Ҫһ������ģ������ǲ߻��ű� int SpeedToMoveSpeed(int speed)������
    ///</summary>
    public float moveSpeed;

    ///<summary>
    ///�ж��ٶȣ����ƶ��ٶȲ�ͬ���������ӽ�ɫ�ж��ٶȣ�Ҳ���Ǳ仯timeline�Ͷ������ŵ�scale�ģ�����wow���濪��Ѫ���Ǽ��ж��ٶ�
    ///�������Ҳ����һ��0.2f���������Ϸ�й����趨�����Ϊ�����ٶȵ�20%�������Ϸ���Լ�������5.0f���������Ϸ�趨�������������ٶ�20%�������ƶ��ٶ�һ����Ҫ�ű��ӿڷ��ز߻���ʽ
    ///</summary>
  //  public int actionSpeed;

    ///<summary>
    ///���֣���ʵ�൱��mp�ˣ�ֻ�����������Ϸ���������Ҫ��Ƥ��
    ///��Ҳ�����⣬����ͨmp���޵������ǽ�ɫ���ֵ����һ�㶼��0����������װ����
    ///</summary>
    public int ammo;

    ///<summary>
    ///����Բ�ΰ뾶�������ƶ���ײ�ģ���λ����
    ///����������˶��죬������ʵ���淨�м��������ܾ�Ӫ����ֻ��buff���ܻ�ı�һ�£�����ֱ������Ϸ���õ����ݾ����ˣ�����Ҫת����
    ///</summary>
    public float bodyRadius;

    ///<summary>
    ///����Բ�ΰ뾶��ͬ����Բ�Σ�ֻ����;��ͬ�������ж��ӵ��Ƿ����е�ʱ��
    ///</summary>
    public float hitRadius;

    ///<summary>
    ///��ɫ�ƶ�����
    ///</summary>
    public MoveType moveType;
    /// <summary>
    /// ��ս�����뾶
    /// </summary>
    public float attackRadius;

    public ChaProperty(
        int moveSpeed, int hp = 0, int ammo = 0, 
        float bodyRadius = 0.25f, float hitRadius = 0.25f, MoveType moveType = MoveType.ground,float attackRadius=0.3f
    )
    {
        this.moveSpeed = moveSpeed;
        this.hp = hp;
        this.ammo = ammo;
        //this.attack = attack;
        //this.actionSpeed = actionSpeed;
        this.bodyRadius = bodyRadius;
        this.hitRadius = hitRadius;
        this.moveType = moveType;
        this.attackRadius = attackRadius;
    }


    public static ChaProperty zero = new ChaProperty(0, 0, 0, 0, 0, 0,0);

    ///<summary>
    ///������ֵ��0
    ///<param name="moveType">�ƶ���������Ϊ</param>
    ///</summary>
    public void Zero(MoveType moveType = MoveType.ground)
    {
        this.hp = 0;
        this.moveSpeed = 0;
        this.ammo = 0;
       // this.attack = 0;
        //this.actionSpeed = 0;
        this.bodyRadius = 0;
        this.hitRadius = 0;
        this.moveType = moveType;
    }

    //����ӷ��ͳ˷����÷�����ʵ���Ӧ���߽ű��������أ��׸��ű��������ChaProperty���ɽű������������ǵ������ϵ�������ؽ��
    /*
    public static ChaProperty operator +(ChaProperty a, ChaProperty b)
    {
        return new ChaProperty(
            (int)(a.moveSpeed + b.moveSpeed),
            a.hp + b.hp,
            a.ammo + b.ammo,
            a.attack + b.attack,
            a.actionSpeed + b.actionSpeed,
            a.bodyRadius + b.bodyRadius,
            a.hitRadius + b.hitRadius,
            a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground
        );
    }
    public static ChaProperty operator *(ChaProperty a, ChaProperty b)
    {
        return new ChaProperty(
            Mathf.RoundToInt(a.moveSpeed * (1.0000f + Mathf.Max(b.moveSpeed, -0.9999f))),
            Mathf.RoundToInt(a.hp * (1.0000f + Mathf.Max(b.hp, -0.9999f))),
            Mathf.RoundToInt(a.ammo * (1.0000f + Mathf.Max(b.ammo, -0.9999f))),
            Mathf.RoundToInt(a.attack * (1.0000f + Mathf.Max(b.attack, -0.9999f))),
            Mathf.RoundToInt(a.actionSpeed * (1.0000f + Mathf.Max(b.actionSpeed, -0.9999f))),
            a.bodyRadius * (1.0000f + Mathf.Max(b.bodyRadius, -0.9999f)),
            a.hitRadius * (1.0000f + Mathf.Max(b.hitRadius, -0.9999f)),
            a.moveType == MoveType.fly || b.moveType == MoveType.fly ? MoveType.fly : MoveType.ground
        );
    }
    public static ChaProperty operator *(ChaProperty a, float b)
    {
        return new ChaProperty(
            Mathf.RoundToInt(a.moveSpeed * b),
            Mathf.RoundToInt(a.hp * b),
            Mathf.RoundToInt(a.ammo * b),
            Mathf.RoundToInt(a.attack * b),
            Mathf.RoundToInt(a.actionSpeed * b),
            a.bodyRadius * b,
            a.hitRadius * b,
            a.moveType
        );
    }
    */
}


///<summary>
///��ɫ����Դ�����ԣ�����hp��mp�ȶ��������
///</summary>
[System.Serializable]
public class ChaResource
{
    ///<summary>
    ///��ǰ����
    ///</summary>
    public int hp;

    ///<summary>
    ///��ǰ��ҩ����������Ϸ����൱��mp��
    ///</summary>
    public int ammo;

    ///<summary>
    ///��ǰ������������һ���ٷֱ����ģ�ʵʱ�ָ��ĸ���������ް��������100�ˣ����������ж���
    ///</summary>
    public int stamina;

    public ChaResource(int hp, int ammo = 0, int stamina = 0)
    {
        this.hp = hp;
        this.ammo = ammo;
        this.stamina = stamina;
    }

    ///<summary>
    ///�Ƿ��㹻
    ///</summary>
    public bool Enough(ChaResource requirement)
    {
        return (
            this.hp >= requirement.hp &&
            this.ammo >= requirement.ammo &&
            this.stamina >= requirement.stamina
        );
    }

    public static ChaResource operator +(ChaResource a, ChaResource b)
    {
        return new ChaResource(
            a.hp + b.hp,
            a.ammo + b.ammo,
            a.stamina + b.stamina
        );
    }
    public static ChaResource operator *(ChaResource a, float b)
    {
        return new ChaResource(
            Mathf.FloorToInt(a.hp * b),
            Mathf.FloorToInt(a.ammo * b),
            Mathf.FloorToInt(a.stamina * b)
        );
    }
    public static ChaResource operator *(float a, ChaResource b)
    {
        return new ChaResource(
            Mathf.FloorToInt(b.hp * a),
            Mathf.FloorToInt(b.ammo * a),
            Mathf.FloorToInt(b.stamina * a)
        );
    }
    public static ChaResource operator -(ChaResource a, ChaResource b)
    {
        return a + b * (-1);
    }

    public static ChaResource Null = new ChaResource(0);
}