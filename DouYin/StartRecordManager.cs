using System.Collections.Generic;
using StarkSDKSpace;
using UnityEngine;
using UnityEngine.UI;

public class StartRecordManager : MonoBehaviour
{
    [SerializeField, Header("开始录屏按钮的名字"), Space(10)]
    private string StartButtonName = "OpenRecord";
    [SerializeField, Header("停止录屏按钮的名字"), Space(10)]
    private string StopButtonName = "StopRecord";

    private StarkGameRecorder starkGameRecorder;
    private bool m_IsRecordAudio = true;
    private int m_MaxRecordTime = 0;
    private List<StarkGameRecorder.TimeRange> m_ClipRanges = new List<StarkGameRecorder.TimeRange>();

    private void Start()
    {
        starkGameRecorder = StarkSDK.API.GetStarkGameRecorder();
        transform.Find(StartButtonName)?.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartRecord();
            transform.Find(StartButtonName)?.gameObject.SetActive(false);
            transform.Find(StopButtonName)?.gameObject.SetActive(true);
        });
        transform.Find(StopButtonName)?.GetComponent<Button>().onClick.AddListener(() =>
        {
            StopRecorder();
            transform.Find(StartButtonName)?.gameObject.SetActive(true);
            transform.Find(StopButtonName)?.gameObject.SetActive(false);
        });
    }

    #region 开始录屏

    private void StartRecord()
    {
        if (starkGameRecorder.GetVideoRecordState() != StarkGameRecorder.VideoRecordState.RECORD_STARTED)
        {
            starkGameRecorder.StartRecord(m_IsRecordAudio, m_MaxRecordTime, OnRecordStart, OnRecordError,
                OnRecordTimeout);
        }
        else
        {
            Debug.Log("Recorder is started");
        }
    }

    private void OnRecordTimeout(string videopath)
    {
        Debug.Log($"地址：{videopath}");
        m_MaxRecordTime = 0;
    }

    private void OnRecordError(int errcode, string errmsg)
    {
        Debug.Log($"错误代码 :{errcode}" + $"错误信息 :{errmsg}");
    }

    private void OnRecordStart()
    {
        Reset();
    }

    private void Reset()
    {
        m_ClipRanges.Clear();
    }

    #endregion

    #region 停止录屏

    private void StopRecorder()
    {
        starkGameRecorder.StopRecord(OnRecordComplete, OnRecordError);
    }

    private void OnRecordComplete(string videopath)
    {
        Debug.Log($"地址 ：{videopath}");
        m_MaxRecordTime = 0;
    }

    #endregion
}