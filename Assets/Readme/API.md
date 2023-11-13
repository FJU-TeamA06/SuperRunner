1. 取得所有玩家資料

```
GET http://140.136.151.71:5000/players?mode=getalldata
```

2. 查詢單筆玩家資料 

```
GET http://140.136.151.71:5000/players?mode=getplayerdata&pname=玩家名稱&sname=會話名稱
```

3. 寫入玩家資料

```
GET http://140.136.151.71:5000/players?mode=setplayerdata&pname=玩家名稱&sname=會話名稱&score=分數
```

4. 重設分數

```
GET http://140.136.151.71:5000/players?mode=resetscore&sname=會話名稱 
```

5. 刪除會話所有玩家資料

```
GET http://140.136.151.71:5000/players?mode=clearsession&sname=會話名稱
```
6. 取得該會話所有玩家資料

```
GET http://140.136.151.71:5000/players?mode=getsessionplayers&sname=會話名稱
```
所有 API 均通過 mode 參數來指定操作,如查詢、寫入等。

寫入/更新/刪除資料時的參數說明:

- pname:玩家名稱
- sname:遊戲會話名稱 
- score:玩家分數