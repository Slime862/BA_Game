using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    public interface  IDamagable//���˽ӿ� �����ǵ��˺����� ���ƻ�����ƷҲ���� �����ӵ�����ȥ��ֻҪ��ȡ����ӿڵ���TakeDamage
    {
        public abstract void TakeDamage(DamageData damageData);

    }
}
