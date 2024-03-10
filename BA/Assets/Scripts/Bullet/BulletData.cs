using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �洢�ӵ��Ĺ̶�����Ϣ
/// </summary>
[CreateAssetMenu(fileName ="BulletData",menuName ="BulletSystem/BulletData",order =1)]
public class BulletData : ScriptableObject
{

    public string id;

    ///<summary>
    ///�ӵ���Ҫ�õ�prefab��Ĭ����Resources/Prefabs/Bullet/�µģ��������string��Ҫʡ��ǰ�벿��
    ///������BlueRocket0���ͻᴴ����Resources/Prefabs/Bullet/BlueRocket0���prefab
    ///</summary>
    public GameObject prefab;

    ///<summary>
    ///�ӵ�����ײ�뾶����λ���ס������Ϸ���ӵ����߼����綼��Բ�εģ���Ȼ�������Ϸ�趨��ˣ�ʵ�ʲ߻�������δ��ֻ����Բ�Ρ�
    ///</summary>
    public float radius;

    ///<summary>
    ///�ӵ����������Ĵ�����ÿ����������Ŀ��-1����0��ʱ���ӵ��ͽ����ˡ�
    ///</summary>
    public int hitTimes;

    ///<summary>
    ///�ӵ�����ͬһ��Ŀ����ӳ٣���λ���룬��Сֵ��Time.fixedDeltaTime��ÿ֡����һ�Σ�
    ///</summary>
    public float sameTargetDelay;

    public BaseBulletModel OnCreate;

    public BaseBulletModel OnHit;

    public BaseBulletModel OnRemove;





}
