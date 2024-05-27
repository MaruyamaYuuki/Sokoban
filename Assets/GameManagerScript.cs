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
        // �ړ��悪�͈͊O�Ȃ�ړ��s��
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        // �ړ����4(��)��������ړ��s��
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall") {
            Debug.Log("�ǂɏՓ�");
            return false; }
        // �ړ����2(��)��������
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        // �v���C���[�E���ւ�炸�ړ�����
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
        // Vector2Int�^�̉ϒ��z��̍쐬
        List<Vector2Int> goals = new List<Vector2Int>();

        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                // �i�[�ꏊ���ۂ����m�F
                if (map[y, x] == 3)
                {
                    // �i�[�ꏊ�̃C���f�b�N�X���T���Ă���
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        // �v�f����goals.const�Ŋl��
        for(int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // ��ł���������������������B��
                return false;
            }
        }
        // �łȂ���Ώ����B��
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        // map�̐���
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
        // �t�B�[���h�T�C�Y����
        field = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];
        // map�ɉ����ĕ`��
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

        // ���Z�b�g�L�[�������ꂽ��
        if (Input.GetKeyDown(KeyCode.R))
        {
            // �v���C���[�A���A�e�L�X�g���A�N�e�B�u�ɂ���
            foreach (GameObject obj in field)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            clearText.SetActive(false);
            // �}�b�v�����Z�b�g���čēx�`�悷��
            Start();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, -1));
            // �����N���A���Ă�����
            if (IsCleard())
            {
                // �Q�[���I�u�W�F�N�g��SetActive���\�b�h���g���L����
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
