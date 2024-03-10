//用来存枚举之类的

using UnityEngine;
/// <summary>
/// 相同buff被施加时执行的操作
/// </summary>
public enum BuffUpdateTimeEnum
{
    Add,
    Replace,
    Keep
}
/// <summary>
/// buff被删除时，层数变化
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