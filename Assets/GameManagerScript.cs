using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    int[] map;

    void PrintArray()
    {
        // �ǉ��B������̐錾�Ə�����
        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            // �ύX�B������Ɍ������Ă���
            debugText += map[i].ToString() + ",";
        }
        // ����������������o��
        Debug.Log(debugText);
    }

    int GetPlayerIndex()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }

    bool MoveNumber(int number, int moveFrom, int moveTo)
    {
        // �ړ��悪�͈͊O�Ȃ�ړ��s��
        if (moveTo < 0 || moveTo >= map.Length) { return false; }
        // �ړ����2(��)��������
        if (map[moveTo] == 2)
        {
            // �ǂ̕����ֈړ����邩���Z�o
            int velocity = moveTo - moveFrom;
            // �v���C���[�̈ړ��悩��A����ɐ��2(��)���ړ�������B
            // ���̈ړ������BMoveNumber���\�b�h����MoveNumber���\�b�h��
            // �ĂсA�������ċA���Ă���B�ړ��s��bool�ŋL�^
            bool success = MoveNumber(2, moveTo, moveTo + velocity);
            // ���������C�h���s������A�v���C���[�̈ړ������s
            if (!success) { return false; }
        }
        // �v���C���[�E���ւ�炸�ړ�����
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {

        map = new int[] { 0, 2, 0, 1, 0, 2, 0, 0, 0, 2, 0, 0, 0, };
        PrintArray();
    }

    // Update is called once per frame
    void Update()
    {
        // �E�ړ�
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int playerIndex = GetPlayerIndex();
            MoveNumber(1, playerIndex, playerIndex + 1);
            PrintArray();
        }
        // ���ړ�
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int playerIndex = GetPlayerIndex();
            MoveNumber(1, playerIndex, playerIndex - 1);
            PrintArray();
        }
    }
}