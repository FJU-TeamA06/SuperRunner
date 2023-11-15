1. 取得所有玩家資料

```
GET http://140.136.151.71:5000/players?mode=getalldata
```

2. 查詢單筆玩家資料 

```
GET http://140.136.151.71:5000/players?mode=getplayerdata&pname=玩家名稱&sname=會話名稱
```

3. 寫入玩家資料(回傳修改後玩家分數)

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
7. 幫該玩家加分(回傳加好分後玩家分數)
```
GET http://140.136.151.71:5000/players?mode=addplayerscore&sname=遊戲會話名稱&pname=玩家名稱&score=加多少分
```
8. 取得該會話的玩家分數（已依照分數由大到小排序）

```
GET http://140.136.151.71:5000/players?mode=getorderplayers&sname=會話名稱
```
所有 API 均通過 mode 參數來指定操作,如查詢、寫入等。

寫入/更新/刪除資料時的參數說明:

- pname:玩家名稱
- sname:遊戲會話名稱 
- score:玩家分數(或加多少分)