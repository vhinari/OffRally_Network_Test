using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



/*
 *  존나 시발 망했어ㅋㅋㅋㅋㅋㅋㅋㅋ 
 */

public class TimeRecorder : MonoBehaviour
{
    private static TimeRecorder _instance;
    public static TimeRecorder Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(TimeRecorder)) as TimeRecorder;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "TimeRecorder";
                    _instance = container.AddComponent(typeof(TimeRecorder)) as TimeRecorder;
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    //타임 레코드 템플릿
    public struct TR_Template
    {
        public double fTime;
        public string szID;
    };

    //저장할 기록의 Type
    public enum TR_Type { /*Checker_Record,*/ Lap_Record, Track_Record };

    private const string TR_FileName           = "TimeRecorder.txt";
    private const string TR_LapRecordSector    = "[Lap Records]";
    private const string TR_TrackRecordSector  = "[Track Records]";
    private string filePath                    = Application.persistentDataPath + TR_FileName;

    private List<TR_Template> TR_NewLapRecords;//Lap의 new record - 게임이 끝난 후 표시될 현재 Lap Result
    private TR_Template TR_NewTrackRecords;//Track의 new record - 게임이 끝난 후 표시될 현재 Track Result

    //저장된 기록을 읽어들일 변수들 - 어쩌면 DontDestroy를 써야 할지도?
    private List<TR_Template> TR_AllTimeLapRecords;
    private List<TR_Template> TR_AllTimeTrackRecords;

    private string TR_RecordBackup;//전체 기록을 string으로 변환하여 저장할 변수

    void Awake()
    {
        LapCheck tmp = GameObject.Find("[ Lap Checker ]").GetComponent<LapCheck>();

        TR_NewLapRecords        = new List<TR_Template>();

        TR_AllTimeLapRecords    = new List<TR_Template>();
        TR_AllTimeTrackRecords  = new List<TR_Template>();

        ReadAllTimeRecords();
    }

    //이전 기록을 읽어들이는 함수
    void ReadAllTimeRecords()
    {
        FileStream   fs = null;
        StreamWriter sw = null;
        StreamReader sr = null;

        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath).Close();
            sw = new StreamWriter(filePath);

            sw.WriteLine(TR_LapRecordSector);
            sw.WriteLine(TR_TrackRecordSector);

            sw.Flush();
            sw.Close();
        }

        fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        sr = new StreamReader(fs, Encoding.GetEncoding("euc-kr"));

        //기록을 읽어들여 백업 string에 복사
        TR_RecordBackup = sr.ReadToEnd();
        //파일 포인터 초기화
        sr.BaseStream.Seek(0, SeekOrigin.Begin);

        //line을 [Lap Records]로 초기화
        string line = sr.ReadLine();
        float temp  = 0;

        bool trackSector = false;
        do
        {
            line = sr.ReadLine();

            if (line != null)
            {
                //현재 Line이 [Track Records]에 도달했다면
                if (line == TR_TrackRecordSector)
                    trackSector = true;

                //ID와 Record를 분리
                if (line.Contains(","))
                {
                    int spliter = line.IndexOf(' ');
                    //Record를 string에서 float로 변환
                    float.TryParse(line.Substring(spliter), out temp);
                    //ID는 별도로 저장
                    string id = line.Substring(0, spliter - 1);

                    TR_Template TR_temp = new TR_Template();
                    TR_temp.fTime = temp;
                    TR_temp.szID = id;

                    //ReadLine을 하다 [Track Records]라는 string이 읽혀지면 TrackRecord에 저장
                    if ((!trackSector) && (temp > 0))
                        TR_AllTimeLapRecords.Add(TR_temp);
                    
                    else if ((trackSector) && (temp > 0))
                        TR_AllTimeTrackRecords.Add(TR_temp);
                }
            }
        } while (line != null);//파일을 모두 읽어들일 때까지

        sr.Close();
        fs.Close();
    }


    //각 Lap, Track을 돌 때마다 외부에서 호출되는 함수
    public void Push(TR_Type type)
    {

        TR_Template tr_Data;
        tr_Data.szID = "Player";

        switch (type)
        {
            case TR_Type.Lap_Record:
                {
                    tr_Data.fTime = 112;//LapSW.Elapsed.TotalSeconds;

                    //한 바퀴를 돈 다음 저장
                    TR_NewLapRecords.Add(tr_Data);//현재 게임에서의 Lap Result 출력을 위해 저장
                    TR_AllTimeLapRecords.Add(tr_Data);

                    HUDManager.Instance.LapSW.Stop();

                } break;

            case TR_Type.Track_Record:
                {
                    tr_Data.fTime = 112;//TrackSW.Elapsed.TotalSeconds;

                    TR_NewTrackRecords = tr_Data;

                    TR_NewLapRecords.Add(tr_Data);//현재 게임에서의 Lap Result 출력을 위해 저장
                    TR_AllTimeTrackRecords.Add(tr_Data);

                    HUDManager.Instance.TrackSW.Stop();

                    SaveNewRecords();//새로운 기록 저장
                } break;
        }
    }

    //새로운 기록을 저장하는 함수
    void SaveNewRecords()
    {
        //새로운 기록을 저장하기 전에 기록을 정렬시킴
        BubbleSort(ref TR_AllTimeLapRecords, TR_AllTimeLapRecords.Count);
        BubbleSort(ref TR_AllTimeTrackRecords, TR_AllTimeTrackRecords.Count);

        StreamWriter sw = new StreamWriter(filePath);

        string line;
        //1~10까지의 순위만 기록하기 위한 카운트 변수
        int Ranking = 0;

        //파일 포인터를 한 칸 아래로 내리기 위한 꼼수-_-
        sw.WriteLine(TR_LapRecordSector);

        do
        {
            //line = sr.ReadLine();
            line = TR_AllTimeLapRecords[Ranking].szID.ToString() + ", " + TR_AllTimeLapRecords[Ranking].fTime.ToString();
            if (line != null)
                sw.WriteLine(line);

            ++Ranking;

        } while (Ranking < 10);//Track Record 구간 전까지//while (line != TR_TrackRecordSector);

        //마찬가지로 [Track Records]를 쓴 다음 파일 포인터를 한 칸 아래로 내리기 위한 꼼수-_-ㅋ
        sw.WriteLine(TR_TrackRecordSector);
        Ranking = 0;

        do
        {
            //line = sr.ReadLine();
            line = TR_AllTimeTrackRecords[Ranking].szID.ToString() + ", " + TR_AllTimeTrackRecords[Ranking].fTime.ToString();
            if (line != null)
                sw.WriteLine(line);
            ++Ranking;

        } while (Ranking < 10);//while(line != null);

        //sr.Close();
        sw.Flush();
        sw.Close();
        //fs.Close();
    }

    //기록들을 정렬하는 함수
    void BubbleSort(ref List<TR_Template> mergeList, int size)
    {
        int Row, Col;
        TR_Template temp;
        for(Row = 0; Row < size - 1; Row++)
        {
            for(Col = 0; Col < size - 1; Col++)
            {
                if(mergeList[Col].fTime > mergeList[Col + 1].fTime)
                {
                    temp = mergeList[Col + 1];
                    mergeList[Col + 1] = mergeList[Col];
                    mergeList[Col] = temp;
                }
            }
        }
        
    }

    ~TimeRecorder()
    {
        System.GC.Collect();
    }
}
