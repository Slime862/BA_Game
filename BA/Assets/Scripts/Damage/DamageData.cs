using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    //���ÿ�������һ�� ���Ը�������/����/Buff/�ȼ� �����Ա仯 һ������һ������˺�ʱҪ��������ݿ鴫�������˺��ĺ�������
    [CreateAssetMenu(fileName = "DamageData", menuName = "DamageSystem/DamageData", order = 1)]
    public class DamageData : ScriptableObject
    {
        public int BaseDamage;//�����˺�

        public float CriticalChange;//������

        public float CriticalMultiplier;//��������


       


    }
}
