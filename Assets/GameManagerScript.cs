using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject wallPrefab;

    public GameObject clearText;
    int[,] map;
    GameObject[,] field;
    Vector3 centeroffset;

    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
           for (int x = 0; x < field.GetLength(1); x++)
           {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x,y);
                }
           }
        }
        return new Vector2Int(-1, -1);
    }

    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        // 移動先が範囲外なら移動不可
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        // 移動先に4(壁)が居たら移動不可
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall") {
            Debug.Log("壁に衝突");
            return false; }
        // 移動先に2(箱)が居たら
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        // プレイヤー・箱関わらず移動処理
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        //field[moveFrom.y, moveFrom.x].transform.position =
            //new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
        Vector3 moveToPosition = new Vector3(
            moveTo.x, map.GetLength(0) - moveTo.y, 0) - centeroffset;
        Move check = field[moveFrom.y, moveFrom.x].GetComponent<Move>();
        if(check == null) {
            Debug.Log("null");
        }
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

        field[moveFrom.y, moveFrom.x] = null;
        return true;
    }

    bool IsCleard()
    {
        // Vector2Int型の可変長配列の作成
        List<Vector2Int> goals = new List<Vector2Int>();

        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                // 格納場所か否かを確認
                if (map[y, x] == 3)
                {
                    // 格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        // 要素数はgoals.constで獲得
        for(int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // 一つでも箱が無かったら条件未達成
                return false;
            }
        }
        // でなければ条件達成
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        // mapの生成
        map = new int[,] {
          {4,4,4,4,4,4,4,4 },
          {4,3,4,3,0,0,0,4 },
          {4,3,4,4,0,4,0,4 },
          {4,0,0,0,0,0,0,4 },
          {4,0,4,4,2,2,0,4 },
          {4,0,0,2,0,4,4,4 },
          {4,0,0,0,0,0,1,4 },
          {4,4,4,4,4,4,4,4 }
        };
        // フィールドサイズ決定
        field = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];
        // mapに応じて描画
        centeroffset = new Vector3(map.GetLength(1) / 2.0f, map.GetLength(0) / 2.0f, 0);
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0.0f) - centeroffset,
                        Quaternion.identity
                    );
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0.0f) - centeroffset,
                        Quaternion.identity
                    );
                }
                if (map[y, x] == 3)
                {
                    GameObject instance = Instantiate(
                        goalPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0.01f) - centeroffset,
                        Quaternion.identity
                    );
                }
                if (map[y, x] == 4)
                {
                    field[y, x] = Instantiate(
                        wallPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0.0f) - centeroffset,
                        Quaternion.identity
                    );
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        // リセットキーが押されたら
        if (Input.GetKeyDown(KeyCode.R))
        {
            // プレイヤー、箱、テキストを非アクティブにする
            foreach (GameObject obj in field)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            clearText.SetActive(false);
            // マップをリセットして再度描画する
            Start();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, -1));
            // もしクリアしていたら
            if (IsCleard())
            {
                // ゲームオブジェクトのSetActiveメソッドを使い有効化
                clearText.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, 1));
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(1, 0));
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(-1, 0));
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }
    }
}
