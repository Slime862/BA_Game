using Ba;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayerController : MonoBehaviourPun, IPunObservable, IDamagable
{
    public static GameInputActions GameInputActions;
    public  ChaState chaState;
    public Transform pickPoint;
    //����һ���ƶ��ٶ����Գ�ʼֵ�������а��ţ����������2
    private float moveSpeed;
    //����ȡ����Move����ֵ��ֵ��moveVector2 
    public GameObject throwPoint;
    private Rigidbody rb;

    private IItem item=null;
    private IItem itemTouching=null;
    Vector2 moveVector2 => GameInputActions.PC.Move.ReadValue<Vector2>();

    private void Awake()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }
        //ʵ�������Ǹո����ɵ�rpgInputActions�ű�
        GameInputActions = new GameInputActions();
        moveSpeed = chaState.property.moveSpeed;
        rb = GetComponent<Rigidbody>();
        AudioManager.Instance.SetCurrentAudios(BaGameSetting.PLAYER_CHARACTER);
    }
    void OnEnable()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }
        //ʹ��ǰ��Ҫ����rpgInputActions����
        GameInputActions.PC.Enable();
    }
    void OnDisable()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }
        //ʹ������Ҫ����rpgInputActions�ر�
        GameInputActions.PC.Disable();
    }
    //Update�������ں���

    //����ȡMove���뷽��д��Update��������֡����
    private void Update()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }
        getMoveInput();
        getShootDirectionInput();
        getShootInput();
        getCarryOrUseInput();
    }
    //��ȡMove���뷽��
    private void getMoveInput()
    {
        
        //�ж��Ƿ��а��¶�Ӧ��Move����
        if (moveVector2 != Vector2.zero)
        {
            //����ȡ���ķ���ֵ��ӡ����
            //Debug.Log(moveVector2);
            //ʹ�û�ȡ����Vector2.x��Vector2.y����ֵ��Ϊ��ɫ�ƶ��Ĳ���
            playerMove(moveVector2.x, moveVector2.y);
        }
    }
    //ʹ��ɫ�����ƶ��ķ���
    private void playerMove(float horizontal, float vertical)
    {
        //****TODO:�ڶ�������д���֮ǰ������buff�ļ���
        if(chaState.canMove)
        transform.Translate(new Vector3(horizontal, 0, vertical).normalized * Time.deltaTime * moveSpeed, Space.World);
        
        //��ɫ�ƶ������

    } 


    //��ȡShoot���뷽��
    private void getShootInput()
    {
        //����ȡ����Jump����ֵ��ֵ��isJump 
        bool isShoot = GameInputActions.PC.Shoot.IsPressed();
        //�ж��Ƿ��а��¶�Ӧ��Jump����
        if (isShoot)
        {
            //����ȡ���ķ���ֵ��ӡ����
            //  Debug.Log(isShoot);
            //****TODO ����ķ����������������������ӵ�Ҫ�õ������/ECS�ܹ���ϵͳȥ���������ӵ�ʱ��Ҫ������ǰbuff���ӵ����д���
            chaState.bulletLauncher.Shoot(chaState);
            
            // ���ӵ����ݴ��ݸ��������
            photonView.RPC("SyncBulletData", RpcTarget.Others,BaGameSetting.PLAYER_CHARACTER+"(Clone)");

        }
    }
   
    //��������ķ�����������ķ���
    private void getShootDirectionInput()
    {
        // ����ȡ���� CameraControl ����ֵ��ֵ�� cameraOffset
        Vector2 cameraOffset = GameInputActions.PC.ShootDirection.ReadValue<Vector2>();
        
        // �ж��Ƿ���������ƫ��
        if (cameraOffset != Vector2.zero)
        {
            
            // ����һ�������λ�õ�����
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // ��������Ƿ�����Ϊ "ground" �������ཻ
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
               // Debug.Log("123");
                // ��ȡ���������Ľ���λ��
                Vector3 targetPosition = hit.point;

                // ��������Ӧ�ó���ķ���
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0f; // �� y ����Ϊ 0��������ˮƽ����

                // ������ķ�������Ϊ����õ��ķ���
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    public void getCarryOrUseInput()
    {
        bool isCarryOrUse = GameInputActions.PC.CarryOrUse.WasPerformedThisFrame();
        if (item == null&&isCarryOrUse&&itemTouching!=null)
        {
            if (itemTouching.Picked(gameObject)) item = itemTouching;
        }else if(item != null&&isCarryOrUse)
        {
            //����Ѿ��ж����ˣ���ʹ����������ķ������������ʦ����ô�������ݾ��ǰ���ʦ����ȥ
            item.Used(gameObject);
            item = null;
        }



    }
    [PunRPC]
    private void SyncBulletData(string name)
    {
        ChaState otherChaState = GameObject.Find(name).GetComponent<ChaState>();
        otherChaState.bulletLauncher.Shoot(chaState);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(chaState.resource.hp);
            stream.SendNext(chaState.resource.ammo);
            stream.SendNext(chaState.resource.stamina);

        }
        else
        {

            int oldHp = chaState.resource.hp;
            chaState.resource.hp = (int)stream.ReceiveNext();
            chaState.resource.ammo = (int)stream.ReceiveNext();
            chaState.resource.stamina = (int)stream.ReceiveNext();
            //����Hp����ʾ
            if (oldHp != chaState.resource.hp)
            {
                GetComponent<HealthBarUI>().UpdateHealthBar(chaState.resource.hp, chaState.property.hp);
            }
        }
    }

    public void TakeDamage(DamageData damageData)
    {
        //�����ɫ�����޵�ʱ��Ͳ����˺�
        if (chaState.immuneTime > 0) return;


        float damage = damageData.BaseDamage;
        if (Random.Range(0, 1) < damageData.CriticalChange)//������
        {
            damage *= (1 + damageData.CriticalMultiplier);
        }
        chaState.resource.hp = Mathf.Max(0, chaState.resource.hp - (int)damage);
        GetComponent<HealthBarUI>().UpdateHealthBar(chaState.resource.hp, chaState.property.hp);

        //TODO ��������Ч��(�Գ���ķ��򱻻���һС�ξ��룬�ڼ�Ҳ���յ��˺���
        HurtFly(damage*1.5f);
        AudioManager.Instance.PlayBeHurted();


    }

    public void HurtFly(float flyForce)
    {
        chaState.canMove = false;
        // ������ɷ���
        Vector3 knockbackDirection = -transform.forward;
        knockbackDirection.y = transform.up.y*2;
        // ʩ�ӻ�����
        GetComponent<Rigidbody>().AddForce(knockbackDirection * flyForce, ForceMode.Impulse);


    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            chaState.canMove = true;
        }
        if (collision.gameObject.CompareTag("ItemCanPick"))
        {
            itemTouching = collision.gameObject.GetComponent<IItem>();
            
            


        }
    }
    

}
