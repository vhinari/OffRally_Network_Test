using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LapCheck : MonoBehaviour
{
    public List<Transform> LapChecker { get; set; }

    private int m_iPrevChecker;
    private int m_iCurChecker;
    private int m_iMaxCheckerNumber;

    private int m_iLapCount;
    public int g_MaxLapNumber;

    private bool m_bFlag;

    public void Awake()
    {
        m_bFlag = false;

        m_iPrevChecker =
        m_iCurChecker  = 0;
        m_iLapCount    = 0;

        LapChecker     = new List<Transform>();

        int idx = 0;
        //현재 Transform의 child들을 저장
        foreach (Transform child in transform)
        {
            LapChecker.Add(child);
            //Child Transform에 Checker 스크립트를 붙임
            LapChecker[idx].gameObject.AddComponent<Checker>();
            LapChecker[idx].gameObject.GetComponent<Checker>().m_CheckerID = idx;
            LapChecker[idx++].gameObject.SetActive(false);
        }
        LapChecker[m_iCurChecker].gameObject.SetActive(true);

        if (LapChecker != null)
            m_iMaxCheckerNumber = LapChecker.Count;
        else
            throw new System.Exception("LapChecker is null");

        if (g_MaxLapNumber <= 0)
        {
            Debug.LogWarning("MaxLapNumber instance is null");
            g_MaxLapNumber = 1;
        }
    }

    void CheckerCounter(int checkerID)
    {
        m_iPrevChecker = checkerID;
        ++m_iCurChecker;

        if (checkerID + 1 == m_iMaxCheckerNumber)
        {
            m_iCurChecker = 0;
            ++m_iLapCount;
            m_bFlag = true;
        }
    }

    void LapCounter()
    {
        if (m_iLapCount >= g_MaxLapNumber)
        {
            TimeRecorder.Instance.Push(TimeRecorder.TR_Type.Track_Record);

            //모든 랩을 다 돌았으므로 게임이 끝났다는 메시지를 전송.
            //BroadcastMessage("GameMessageHandler", GameOver);
            //LapChecker.Clear();
            HUDManager.Instance.EnableHUD<int>(HUDManager.LOCAL_HUD.RESULT, 0);

        }
        else if (m_bFlag)
        {
            TimeRecorder.Instance.Push(TimeRecorder.TR_Type.Lap_Record);
            m_bFlag = false;
        }
    }

    void CheckerEnabler(int checkerID)
    {
        CheckerCounter(checkerID);
        LapCounter();

        LapChecker[m_iPrevChecker].gameObject.SetActive(false);
        LapChecker[m_iCurChecker].gameObject.SetActive(true);
    }

}
