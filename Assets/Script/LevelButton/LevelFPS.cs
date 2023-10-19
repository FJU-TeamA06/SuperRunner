using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFPS : MonoBehaviour
{
    private int levelNumber = 3; // 关卡编号
    private int playerNum = 1;
    public NameInputHandler nameInputHandler; // 对NameInputHandler的引用
    public GameObject levelSelectionPanel; // 对关卡选择面板的引用

    // Start is called before the first frame update
    void Start()
    {
        // 获取按钮组件并添加点击事件
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnButtonClick()
    {
        // 调用NameInputHandler脚本的StartGameAfterLevelSelection方法并传递关卡编号
        nameInputHandler.StartGameAfterLevelSelection(levelNumber, playerNum);
        Destroy(levelSelectionPanel); // 销毁关卡选择面板
    }
}
