//������ö��֮���

using UnityEngine;
/// <summary>
/// ��ͬbuff��ʩ��ʱִ�еĲ���
/// </summary>
public enum BuffUpdateTimeEnum
{
    Add,
    Replace,
    Keep
}
/// <summary>
/// buff��ɾ��ʱ�������仯
/// </summary>
public enum BuffRemoveStackUpdateEnum
{
    Clear,
    Reduce
}

public class BuffInfo
{
    public BuffData buffData;
    public GameObject creator;
    public GameObject target;
    public float duraTimer;
    public float tickTimer;
    public int curStack;

}
public class DamageInfo
{
    public GameObject creator;
    public GameObject target;
    public float damage;
}