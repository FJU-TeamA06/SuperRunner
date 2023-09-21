using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class PlayerController : NetworkBehaviour
{
    public GameObject MainCameraObject;
    public GameObject SideCameraObject;
    public GameObject FirstCameraObject;
    
    public Camera MainCamera;
    public Camera SideCamera;
    public Camera FirstCamera;
    [Networked] public int isFinished { get; set; }

    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;
    [SerializeField]
    private Bullet bulletPrefab;
    private Vector3 startPoint;
    [SerializeField]
    private float fir = -1, sec = -1, thi = -1, fou = -1, cha;                                    // 排名用
    private int   firS = 0, secS = 0, thiS = 0, fouS = 0;                                      // 分數用
    private string firN, secN, thiN, fouN, chaN;

    private int xxx = 0, yyy = 0;
    [SerializeField]
    private GameObject scoreObject;
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
    private int maxBullet = 5;
    [SerializeField]
    private float maxDist = 100;
    //本地計時器
    private float timer = 0;
    public GameObject timerPrefab;
    public GameObject scorePrefab;
    [Networked(OnChanged = nameof(OnDistChanged))] public float Dist { get; set; }
    //玩家血量
    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }
    //玩家子彈數量
    [Networked]
    public int bulletCount { get; set; }
    //玩家名稱
    [Networked(OnChanged = nameof(OnNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }
    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }

    //不用網路字典了，直接在每個玩家裡面存一個網路變數，Server端可以直接呼叫所有玩家來回傳(用function)
    [Networked]
    [field: SerializeField]
    public float distance { get; private set; }

    [Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<NetworkString<_32>> ScoreLeaderboard { get; } =
    MakeInitializer(new NetworkString<_32>[] { "-1", "-1", "-1", "-1" });   // 排名


    [Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<NetworkString<_32>> FinalScoreBoard { get; } =
    MakeInitializer(new NetworkString<_32>[] { "-1", "-1", "-1", "-1" });       // 分數 

    [Networked]
    [Capacity(4)] // Sets the fixed capacity of the collection
    NetworkArray<int> ScoreBoard { get; } =
    MakeInitializer(new int[] { 0,0 ,0,0 }); 

    [SerializeField]
    private MeshRenderer meshRenderer = null;
    private static readonly float FinishThreshold = 1.0f; // 這個值代表角色距離終點多近時算是已經抵達。
    private GameObject timerInstance;
    public bool isMainCamera=true;

    private void InstantiateHUD_UI()
    {
        if (Object.HasInputAuthority)
        {
            //Runner.Spawn(HUD_UI_Prefab);
            GameObject timerInstance = Instantiate(timerPrefab);
            GameObject scoreInstance = Instantiate(scorePrefab);
        }
    }

    private BasicSpawner basicSpawner;  //引用
    private void Awake()
    {
        basicSpawner = FindObjectOfType<BasicSpawner>(); // 取得 BasicSpawner 的實例
    }
  
    public override void Spawned()
    {
        

        finishObject = GameObject.FindGameObjectWithTag("Finish1");

        if (Object.HasInputAuthority)
        {
            SetPlayerName_RPC(PlayerPrefs.GetString("PlayerName"));
            finishObject = GameObject.FindGameObjectWithTag("Finish1");

            finishCollider = finishObject.GetComponent<Collider>();
            totalDistance = Vector3.Distance(startPoint, finishObject.transform.position);
            InstantiateHUD_UI();
            maxDist = CalculateDistancePercentage();
            //EnablePlayerControl_RPC();
        }
        else
        {

        }
        if (Object.HasStateAuthority)
        {
            bulletCount=maxBullet;
            Hp = maxHp;
        }
        isFinished = 0;
        


    }
    private float CalculateDistancePercentage()
    {
        //Vector3 closestPointOnBounds = finishCollider.boun  ds.ClosestPoint(transform.position);
        float currentDistance = Vector3.Distance(transform.position, finishObject.transform.position);
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
                //發射子彈(子彈數量的檢測)
                if(bulletCount>0)
                {
                    Runner.Spawn(
                    bulletPrefab,
                    transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)),
                    Object.InputAuthority);
                    bulletCount--;
                    print("bulletCount:"+bulletCount);
                }
                
            }
            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveSpeed * moveVector * Runner.DeltaTime);
        }

        timeObject = GameObject.FindGameObjectWithTag("Timer");
        scoreObject = GameObject.FindGameObjectWithTag("Score");

        /*if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f )
        {
            Respawn();
        }*/
        if (ShouldRespawn()==true)
        {
            Respawn();
            a = 0;
        }
        if(frozen == 1)
        {
            StartCoroutine(FreezePlayerForSeconds(5.0f));
            frozen = 0;
        }

        if (HasStateAuthority)
        {
            distance = CalculateDistancePercentage();
        }
       
    }

    int a = 0;
    private bool ShouldRespawn()
    {
       if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)
            return true;
       if (a==1)
       {
            return true;
       }
       else
            return false;
    }

    int frozen = 0; 
    private IEnumerator FreezePlayerForSeconds(float seconds)
    {
        if (frozen == 1)
        {
            moveSpeed = 0f;
            yield return new WaitForSeconds(seconds); // 等待指定的秒數
            moveSpeed = 13f; // 恢復移動速度
        }
        frozen = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 檢查是否已經抵達終點
        if (other.gameObject == finishObject)
        {
            OnReachedFinish();
        }
        if (other.gameObject.CompareTag("trapdead"))
        {
            Debug.Log("Trapdead object collision!");
            // Respawn();
            a = 1;
        }
        if (other.gameObject.CompareTag("Frozen"))
        {
            Debug.Log("Trapdead frozen!");
            frozen = 1;
        }
    }

    private void OnReachedFinish()
    {
        // 在這裡添加您想要在角色抵達終點時執行的程式碼
        timeObject = GameObject.FindGameObjectWithTag("Timer");
        timerUI timerScript = timeObject.GetComponent<timerUI>();
        timerScript.StopTimer();
    }

    /*private void Respawn()
    {
        networkCharacterController.transform.position = Vector3.up * 2;
        Hp = maxHp;
    }*/
    private void Respawn()
    {
        Vector3 spawnPosition = basicSpawner.GetSpawnPosition(basicSpawner.levelIndex, basicSpawner.playerNumber);

        if (spawnPosition != Vector3.zero) // 檢查是否成功獲取重生位置
        {
            networkCharacterController.transform.position = spawnPosition;
        }
        else
        {
            // 如果無法獲取重生位置，可以設定一個默認的重生位置
            networkCharacterController.transform.position = Vector3.up * 2;
        }

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
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
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
    private int currentCameraMode = 0;
    private void Update()
    {
        //切換鏡頭模式
        if (Input.GetKeyDown(KeyCode.C))
        {
            MainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            SideCameraObject = GameObject.FindGameObjectWithTag("SideCamera");
            FirstCameraObject = GameObject.FindGameObjectWithTag("FirstCamera");
            MainCamera = MainCameraObject.GetComponent<Camera>();
            FirstCamera = FirstCameraObject.GetComponent<Camera>();
            SideCamera = SideCameraObject.GetComponent<Camera>();
            // 切换到下一个相机模式
            currentCameraMode = (currentCameraMode + 1) % 3;

            // 根据当前模式启用相应的相机
            MainCamera.enabled = currentCameraMode == 0;
            SideCamera.enabled = currentCameraMode == 1;
            FirstCamera.enabled = currentCameraMode == 2;


            /*if (isMainCamera)
            {
                MainCamera.enabled = false;
                SideCamera.enabled = true;
                isMainCamera=false;
            }
            else
            {
                SideCamera.enabled = false;
                MainCamera.enabled = true;
                isMainCamera=true;
            }*/
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DistRutern_RPC();   
            yyy += 1;            //For Testing! 

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
                StartM_RPC();
               // FinalScoreDisplay_RPC();
                startWallScript.RequestDespawnWall_RPC();
                
            }
            timerUI timerScript = timeObject.GetComponent<timerUI>();
            if (timerScript != null)
            {
                //timerScript.StartTimer();
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

    [Rpc(RpcSources.All, RpcTargets.All)]                                 // 終點＿RPC
    public void Finish_RPC(string a)
    {
        print("Player:"+a+" Is the First Place.");
        DistRutern_RPC();
        yyy += 1;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]                      // 排名顯示＿RPC
    public void ScoreDisplay_RPC()
    {
        timeObject = GameObject.FindGameObjectWithTag("timerText");
        Text timerText = timeObject.GetComponent<Text>();
        print(timerText);
        timerText.text="";
        print("RPC:");
        for (int i = 0; i < ScoreLeaderboard.Length; ++i)
        {
        Debug.Log($"{i}: '{ScoreLeaderboard[i]}''");
        timerText.text=timerText.text+"\n"+i+":"+ScoreLeaderboard[i];
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]                      // 分數顯示＿RPC
    public void FinalScoreDisplay_RPC()
    {
        scoreObject = GameObject.FindGameObjectWithTag("scoreText");
        Text scoreText = scoreObject.GetComponent<Text>();
        print(scoreText);
        scoreText.text="";
        for (int i = 0; i < FinalScoreBoard.Length; ++i)
        {
        Debug.Log($"{i}: '{FinalScoreBoard[i]}''");
        scoreText.text=scoreText.text+"\n"+FinalScoreBoard[i]+"----"+ScoreBoard[i];
        }
        
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void StartM_RPC()
    {
        timeObject = GameObject.FindGameObjectWithTag("timerText");
        Text timerText = timeObject.GetComponent<Text>();
        timerText.text=" Start ! ";
        
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
                    if(xxx == 0){
                        fir=playerController.distance;
                        firN=playerController.PlayerName.ToString();
                    }
                    else if(xxx == 1 && playerController.PlayerName.ToString() != firN){
                        sec=playerController.distance;
                        secN=playerController.PlayerName.ToString();
                        if(sec<=fir){
                            cha=sec;    //2->1
                            chaN=secN;
                            sec=fir;
                            secN=firN;
                            fir=cha;
                            firN=chaN;
                        }
                    }
                    else if(xxx == 2 && playerController.PlayerName.ToString() != secN && playerController.PlayerName.ToString() != firN){
                        thi=playerController.distance;
                        thiN=playerController.PlayerName.ToString();
                        if(thi<=fir){
                            cha=thi;  // 3->1
                            chaN=thiN;
                            thi=fir;
                            thiN=firN;
                            fir=cha;
                            firN=chaN;

                            cha=sec;   // 2->3
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                        else if(thi <= sec){
                            cha=sec;
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                    }
                    else if(xxx == 3 && playerController.PlayerName.ToString() != thiN && playerController.PlayerName.ToString() != secN && playerController.PlayerName.ToString() != firN){
                        fou=playerController.distance;
                        fouN=playerController.PlayerName.ToString();
                        if(fou<=fir){
                            cha=fou;
                            chaN=fouN;
                            fou=fir;
                            fouN=firN;
                            fir=cha;
                            firN=chaN;

                            cha=fou;
                            chaN=fouN;
                            fou=thi;
                            fouN=thiN;
                            thi=cha;
                            thiN=chaN;

                            cha=sec;
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;

                        }
                        else if(fou<=sec){
                            cha=fou;     // 4->3
                            chaN=fouN;
                            fou=thi;
                            fouN=thiN;
                            thi=cha;
                            thiN=chaN;

                            cha=sec;   // 3->2
                            chaN=secN;
                            sec=thi;
                            secN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                        else if(fou<=thi){
                            cha=fou;     // 4->3
                            chaN=fouN;
                            fou=thi;
                            fouN=thiN;
                            thi=cha;
                            thiN=chaN;
                        }
                    }
                        if(yyy == 0 ){
                            FinalScoreBoard.Set(0, firN);
                        }
                        ScoreLeaderboard.Set(0, firN);
                        print(firN+" Is The First Place !! ");
                        if(sec != -1){
                            ScoreLeaderboard.Set(1, secN);
                            if(yyy == 0 ){
                                FinalScoreBoard.Set(1, secN);
                            }
                            print(secN+" Is The Second Place !! ");
                            sec=-1;
                        }
                        if(thi != -1){
                            ScoreLeaderboard.Set(2, thiN);
                            if(yyy == 0 ){
                                FinalScoreBoard.Set(2, thiN);
                            }
                            print(thiN+" Is The Third Place !! ");
                            thi=-1;
                        }
                        if(fou != -1){
                            ScoreLeaderboard.Set(3, fouN);
                            if(yyy == 0 ){
                                FinalScoreBoard.Set(3, fouN);
                            }
                            print(fouN+" Is The Fourth Place !! ");
                            fou=-1;
                        }
                        print("server:");
                        for (int i = 0; i < ScoreLeaderboard.Length; ++i)
                        {
                        Debug.Log($"{i}: '{ScoreLeaderboard[i]}''");
                        }
                        for (int i = 0; i < FinalScoreBoard.Length; ++i)
                        {
                        Debug.Log($"{i}: '{FinalScoreBoard[i]}''");
                        }
                        ScoreDisplay_RPC();
                        FinalScoreDisplay_RPC();

                    xxx=xxx+1;
                }
            }
        }
    }
}
