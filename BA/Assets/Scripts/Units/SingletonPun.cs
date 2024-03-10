using Photon.Pun;
/// <summary>
/// ��ͨ���͵�������
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonPun<T> : MonoBehaviourPun  where T : MonoBehaviourPun
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        instance = this as T;
    }

}
