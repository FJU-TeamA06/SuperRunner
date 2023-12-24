# API Documentation

以下是 Super Runner 遊戲伺服器提供的 API 端點列表以及它們的用途：

## 取得所有玩家資料
要獲取所有玩家數據，您可以發送一個 GET 請求到以下URL：
```
GET {ServerConfig.APIServerURL}/players?mode=getalldata
```
*注意：目前Server的URL可以透過 `ServerConfig.APIServerURL` 來取得，如在2024年2月後使用，請自行架設並在 `ServerConfig.cs` 中的 `APIServerURL` 更新成您的Server IP。*

## 查詢單筆玩家資料
要查詢單筆玩家資料，請使用以下格式的 GET 請求：
```
GET {ServerConfig.APIServerURL}/players?mode=getplayerdata&pname=玩家名稱&sname=會話名稱
```

## 寫入玩家資料
要寫入或更新玩家資料，並獲取修改後的玩家分數，使用以下請求：
```
GET {ServerConfig.APIServerURL}/players?mode=setplayerdata&pname=玩家名稱&sname=會話名稱&score=分數
```

## 重設分數
要重設特定遊戲會話中所有玩家的分數，使用：
```
GET {ServerConfig.APIServerURL}/players?mode=resetscore&sname=會話名稱
```

## 刪除會話所有玩家資料
要刪除特定遊戲會話中的所有玩家資料，使用：
```
GET {ServerConfig.APIServerURL}/players?mode=clearsession&sname=會話名稱
```

## 取得該會話所有玩家資料
要獲取特定遊戲會話中的所有玩家資料，使用：
```
GET {ServerConfig.APIServerURL}/players?mode=getsessionplayers&sname=會話名稱
```

## 幫該玩家加分
要為特定玩家增加分數並回傳加分後的玩家分數，使用：
```
GET {ServerConfig.APIServerURL}/players?mode=addplayerscore&sname=遊戲會話名稱&pname=玩家名稱&score=加多少分
```

## 取得該會話的玩家分數
要取得特定遊戲會話的玩家分數並依照分數由高到低排序，使用：
```
GET {ServerConfig.APIServerURL}/players?mode=getorderplayers&sname=會話名稱
```

所有 API 端點均通過 `mode` 參數來指定操作，例如查詢、寫入等。

寫入/更新/刪除資料時的參數說明：

- `pname`: 玩家名稱
- `sname`: 遊戲會話名稱
- `score`: 玩家分數(或加多少分)