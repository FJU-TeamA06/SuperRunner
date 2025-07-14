# SUPER RUNNER
<img src="SuperRunner_logo.jpg" alt="Description" style="width: 200px; height: 200px;">

## Introduction

#### SUPER RUNNER 是一款多人連線競技遊戲，提供三種不同的遊戲場景及兩種玩法，由最快到達終點的玩家獲勝。
#### 在想方設法阻礙對手到達終點的同時，也不要栽在前方的陷阱和障礙面前。

## [Demo影片](https://drive.google.com/file/d/11sq8hAEq4ZrXCpnE2kzbwya-zXdb_lTG/view?usp=sharing)
## [專題報告](https://drive.google.com/file/d/1ZM0ePBOpYo9pZvM3bVFNBkA6SBSPCmpj/view?usp=sharing)
## Program Structure
[**程式架構**](程式架構.md)



### Client
**GamePlay**
- 遊戲的主要場景
- 介紹整體遊戲流程和玩家互動

**Main Code**
- BasicSpawner : 負責角色生成、加入房間、操作轉送等
- PlayerController : 控制遊戲流程，包括移動、視角、分數、陷阱等
- MyControls : 處理實體和虛擬搖桿輸入
- InputHandler：在玩家輸入好名字和房間名稱後，存檔供之後流程使用
- Camera : 處理玩家視角計算
- Tool : 處理遊戲中的道具
- CountdownTimer：本地運行的計時器

### [Server](https://github.com/FJU-TeamA06/SuperRunner_Server)

**以Flask為架構的Python程式**
- 透過HTTP API去存取SQL資料庫
- GET /players + Query String
- Get /leaderboard + Query String
- 定期重置排行榜

## Tech skills
- Unity 
- Photon Fusion (Host Mode)
- GitHub
- Blender
- MySQL
- Flask

## Support
- PC 
- MacOS
- IOS
- Android (包含有搖桿的Android裝置,如裝了[Switchroot Android 11](https://wiki.switchroot.org/wiki/android/11-r-setup-guide)的Nintendo Switch)

## Game Features
- 鏡頭切換: 以 2.5D 視角為主，第三人稱視角為輔
- 虛擬搖桿: 行動裝置遊玩的輔助

## Demo

- Level 1
<img src="SEA.jpg" alt="Description" style="width: 400px; height: 250px;">

- Level 2
<img src="Desert.jpg" alt="Description" style="width: 400px; height: 250px;">

- 尋物關卡
<img src="FPS_level.jpg" alt="Description" style="width: 400px; height: 250px;">
