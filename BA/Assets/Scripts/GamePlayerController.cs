using Ba;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayerController : MonoBehaviourPun, IPunObservable, IDamagable
{
    public static GameInputActions GameInputActions;
    public  ChaState chaState;
    public Transform pickPoint;
    //声明一个移动速度属性初始值可以自行安排，我这里给个2
    private float moveSpeed;
    //将读取到的Move返回值赋值给moveVector2 
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
        //实例化我们刚刚生成的rpgInputActions脚本
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
        //使用前需要将该rpgInputActions开启
        GameInputActions.PC.Enable();
    }
    void OnDisable()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }
        //使用完需要将该rpgInputActions关闭
        GameInputActions.PC.Disable();
    }
    //Update生命周期函数

    //将获取Move输入方法写在Update方法中逐帧调用
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
    //获取Move输入方法
    private void getMoveInput()
    {
        
        //判断是否有按下对应的Move按键
        if (moveVector2 != Vector2.zero)
        {
            //将获取到的返回值打印出来
            //Debug.Log(moveVector2);
            //使用获取到的Vector2.x和Vector2.y返回值作为角色移动的参数
            playerMove(moveVector2.x, moveVector2.y);
        }
    }
    //使角色真正移动的方法
    private void playerMove(float horizontal, float vertical)
    {
        //****TODO:在对输入进行处理之前，进行buff的计算
        if(chaState.canMove)
        transform.Translate(new Vector3(horizontal, 0, vertical).normalized * Time.deltaTime * moveSpeed, Space.World);
        
        //角色移动后调用

    } 


    //获取Shoot输入方法
    private void getShootInput()
    {
        //将读取到的Jump返回值赋值给isJump 
        bool isShoot = GameInputActions.PC.Shoot.IsPressed();
        //判断是否有按下对应的Jump按键
        if (isShoot)
        {
            //将获取到的返回值打印出来
            //  Debug.Log(isShoot);
            //****TODO 发射的方法，包含射击间隔，发射子弹要用到对象池/ECS架构的系统去管理，发射子弹时还要经过当前buff对子弹进行处理
            chaState.bulletLauncher.Shoot(chaState);
            
            // 将子弹数据传递给其他玩家
            photonView.RPC("SyncBulletData", RpcTarget.Others,BaGameSetting.PLAYER_CHARACTER+"(Clone)");

        }
    }
   
    //控制人物的方向向鼠标落点的方向
    private void getShootDirectionInput()
    {
        // 将读取到的 CameraControl 返回值赋值给 cameraOffset
        Vector2 cameraOffset = GameInputActions.PC.ShootDirection.ReadValue<Vector2>();
        
        // 判断是否有鼠标产生偏移
        if (cameraOffset != Vector2.zero)
        {
            
            // 发射一条从鼠标位置的射线
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // 检测射线是否与标记为 "ground" 的物体相交
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
               // Debug.Log("123");
                // 获取射线与地面的交点位置
                Vector3 targetPosition = hit.point;

                // 计算人物应该朝向的方向
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0f; // 将 y 轴置为 0，保持在水平面上

                // 将人物的方向设置为计算得到的方向
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
            //如果已经有东西了，就使用这个东西的方法，如果是老师，那么方法内容就是把老师丢出去
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
            //先做Hp的显示
            if (oldHp != chaState.resource.hp)
            {
                GetComponent<HealthBarUI>().UpdateHealthBar(chaState.resource.hp, chaState.property.hp);
            }
        }
    }

    public void TakeDamage(DamageData damageData)
    {
        //如果角色处于无敌时间就不受伤害
        if (chaState.immuneTime > 0) return;


        float damage = damageData.BaseDamage;
        if (Random.Range(0, 1) < damageData.CriticalChange)//暴击了
        {
            damage *= (1 + damageData.CriticalMultiplier);
        }
        chaState.resource.hp = Mathf.Max(0, chaState.resource.hp - (int)damage);
        GetComponent<HealthBarUI>().UpdateHealthBar(chaState.resource.hp, chaState.property.hp);

        //TODO 被攻击的效果(以朝向的反向被击飞一小段距离，期间也会收到伤害）
        HurtFly(damage*1.5f);
        AudioManager.Instance.PlayBeHurted();


    }

    public void HurtFly(float flyForce)
    {
        chaState.canMove = false;
        // 计算击飞方向
        Vector3 knockbackDirection = -transform.forward;
        knockbackDirection.y = transform.up.y*2;
        // 施加击飞力
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
