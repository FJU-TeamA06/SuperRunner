using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class PlayerController : NetworkBehaviour
{
    [Networked] public int isFinished { get; set; }
    
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;
    [SerializeField]
    private Bullet bulletPrefab;
    private Vector3 startPoint;
    [SerializeField]
    private GameObject timeObject;
    public GameObject wallObject;
    private float totalDistance;
    private Collider finishCollider;
    private GameObject finishObject;
    [SerializeField]
    private float moveSpeed = 15f;
    [SerializeField]
    private Text UIname = null;
    [SerializeField]
    private Image hpBar = null;
    [SerializeField]
    private int maxHp = 100;
    [SerializeField]
    private float maxDist = 100;
    //本地計時器
    private float timer = 0;
    public GameObject timerPrefab;
    [Networked(OnChanged=nameof(OnDistChanged))] public float Dist { get; set; }
    //玩家血量(預計最終要移除)
    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }

    //玩家名稱
    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }

    //不用網路字典了，直接在每個玩家裡面存一個網路變數，Server端可以直接呼叫所有玩家來回傳(用function)
    [Networked] 
    [field: SerializeField] 
    public float distance { get; private set; }
    

    [SerializeField]
    private MeshRenderer meshRenderer = null;
    private static readonly float FinishThreshold = 1.0f; // 這個值代表角色距離終點多近時算是已經抵達。
    private GameObject timerInstance;
    private void InstantiateHUD_UI()
    {
        if (Object.HasInputAuthority)
        {
            //Runner.Spawn(HUD_UI_Prefab);
            GameObject timerInstance = Instantiate(timerPrefab);
        }
    }

    public override void Spawned()
    {
        finishObject=GameObject.FindGameObjectWithTag("Finish1");
        if(Object.HasInputAuthority)
        {
            SetPlayerName_RPC(PlayerPrefs.GetString("PlayerName"));
            finishObject = GameObject.FindGameObjectWithTag("Finish1");
            
            finishCollider = finishObject.GetComponent<Collider>();
            totalDistance = Vector3.Distance(startPoint, finishObject.transform.position);
            InstantiateHUD_UI(); 
            maxDist=CalculateDistancePercentage();
            //EnablePlayerControl_RPC();
        }
        else
        {

        }
        if (Object.HasStateAuthority)
            Hp = maxHp;
            isFinished=0;
            
        
    }
    private float CalculateDistancePercentage()
    {
        //Vector3 closestPointOnBounds = finishCollider.boun  ds.ClosestPoint(transform.position);
        float currentDistance = Vector3.Distance(transform.position,finishObject.transform.position);
        //Debug.Log($"Absolute distance to the finish edge: {currentDistance}");
        return currentDistance;
    }
    public void EnablePlayerControl()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;
            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterController.Jump();
            }
            if (pressed.IsSet(InputButtons.FIRE))
            {
                Runner.Spawn(
                    bulletPrefab,
                    transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)),
                    Object.InputAuthority);
            }
            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveSpeed * moveVector * Runner.DeltaTime);
        }
          
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)
        {
            Respawn();
        }
        
        if(HasStateAuthority)
        {
            distance = CalculateDistancePercentage();
        }




    }
    private void OnTriggerEnter(Collider other)
    {
        // 檢查是否已經抵達終點
        if (other.gameObject == finishObject)
        {
            OnReachedFinish();
        }
    }
    private void OnReachedFinish()
    {
        // 在這裡添加您想要在角色抵達終點時執行的程式碼
        
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        timerUI timerScript = timeObject.GetComponent<timerUI>();
        timerScript.StopTimer();
    }
    private void Respawn()
    {
        networkCharacterController.transform.position = Vector3.up * 2;
        Hp = maxHp;
    }
    
    static void OnNameChanged(Changed<PlayerController> changed)
    {
        Debug.Log($"Name changed for player to {changed.Behaviour.PlayerName}");
        changed.Behaviour.UIname.text = changed.Behaviour.PlayerName.ToString();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void SetPlayerName_RPC(string name,RpcInfo info=default)
    {
        Debug.Log($"[RPC] SetName {name}");
        this.PlayerName= name;
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            Hp -= damage;
        }
    }
    private static void OnHpChanged(Changed<PlayerController> changed)
    {
        //changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    }
    private static void OnDistChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Dist / changed.Behaviour.maxDist;
    }
    public void OnFinished()
    {
        if(isFinished==0)
        {
            print("Finally Finished");
            Finish_RPC(this.PlayerName.ToString());
            isFinished=1;
            
        }
        
        
    }

    private void Update()
    {
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            DistRutern_RPC();
            ChangeColor_RPC(Color.red);
        }
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.U))
        {
            print(distance);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeColor_RPC(Color.green);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeColor_RPC(Color.blue);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            timeObject = GameObject.FindGameObjectWithTag("Timer");
            wallObject = GameObject.FindGameObjectWithTag("StartWall");
            
            StartWall startWallScript = wallObject.GetComponent<StartWall>();
            if (startWallScript != null)
            {
                startWallScript.RequestDespawnWall_RPC();
                
            }
            timerUI timerScript = timeObject.GetComponent<timerUI>();
            if (timerScript != null)
            {
                timerScript.StartTimer();
                // 在這裡訪問 timerScript 或執行相關操作
                //之後要實作timer相關的rpc呼叫
            }
            else
            {
                print("錯誤");
            }
            
            
        }
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            //CalculateDistancePercentage();
            timer = 0;
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Finish_RPC(string a)
    {
        print("Player:"+a+" Is the First Place.");
        DistRutern_RPC();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DistRutern_RPC()
    {
        if (HasStateAuthority)
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in allPlayers)
            {
                // 先檢查遊戲物件是否有你想要的 Component（比如你的 PlayerController 或其他）
                PlayerController playerController = player.GetComponent<PlayerController>();
                if(playerController != null)
                {
                    print(playerController.PlayerName+" : "+playerController.distance);
                }
            }
        }
    }
}
