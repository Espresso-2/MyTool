using System;
using System.Collections.Generic;
using StarkSDKSpace;
using UnityEngine;

namespace W_Scripts.AdManager
{
    /// <summary>
    /// 抖音广告管理
    /// </summary>
    public static class DouyinAdManager
    {
        private const string Reward = "";
        private const string Interstitial = "";
        private const string Banner = "";
        private static int MaxRecordTime = 0;
        public static bool isFormSliderbar;

        #region 实例

        private static StarkGameRecorder currentRecorder;
        private static List<StarkGameRecorder.TimeRange> clipRanges = new List<StarkGameRecorder.TimeRange>();
        /// <summary>
        /// Banner实例
        /// </summary>
        private static StarkAdManager.BannerAd BannerAd;
        private static StarkAdManager.InterstitialAd InterstitialAd;

        #endregion

        /// <summary>
        /// 展示Banner
        /// </summary>
        public static void ShowBanner()
        {
            if (BannerAd is null)
            {
                BannerAd = StarkSDK.API.GetStarkAdManager().CreateBannerAd(Banner, new StarkAdManager.BannerStyle(), 60,
                    (errCode, errMsg) => { Debug.LogError("Banner错误" + errCode + "错误信息：" + errMsg); }, () =>
                    {
                        if (BannerAd != null)
                            BannerAd.Show();
                    }, (Width, Height) => { Debug.Log("新的宽" + Width + "   " + "新的高" + Height); },
                    () => { Debug.Log("Banner关闭"); }
                );
            }
            else if (BannerAd.IsInvalid())
            {
                BannerAd.Destroy();
                BannerAd = StarkSDK.API.GetStarkAdManager().CreateBannerAd(Banner, new StarkAdManager.BannerStyle(), 60,
                    (errCode, errMsg) => { Debug.LogError("Banner错误" + errCode + "错误信息：" + errMsg); }, () =>
                    {
                        if (BannerAd != null)
                            BannerAd.Show();
                    }, (Width, Height) => { Debug.Log("新的宽" + Width + "   " + "新的高" + Height); },
                    () => { Debug.Log("Banner关闭"); }
                );
            }
            else
            {
                BannerAd.Show();
            }
        }

        /// <summary>
        /// 展示插屏
        /// </summary>
        public static void ShowInterstitialAd()
        {
            if (InterstitialAd is null)
            {
                StarkSDK.API.GetStarkAdManager().CreateInterstitialAd(Interstitial,
                    (errCode, ErrMsg) => { Debug.LogError("插屏错误" + errCode + "错误信息：" + ErrMsg); },
                    () =>
                    {
                        Debug.Log("关闭插屏广告,插屏广告实例已销毁");
                        InterstitialAd = null;
                    }, () =>
                    {
                        if (InterstitialAd == null) return;
                        InterstitialAd.Load();
                        InterstitialAd.Show();
                    });
            }
            else if (InterstitialAd != null)
            {
                InterstitialAd.Destroy();
                InterstitialAd = null;
                StarkSDK.API.GetStarkAdManager().CreateInterstitialAd(Interstitial,
                    (errCode, ErrMsg) => { Debug.LogError("插屏错误" + errCode + "错误信息：" + ErrMsg); },
                    () =>
                    {
                        Debug.Log("关闭插屏广告,插屏广告实例已销毁");
                        InterstitialAd = null;
                    }, () =>
                    {
                        if (InterstitialAd == null) return;
                        InterstitialAd.Load();
                        InterstitialAd.Show();
                    });
            }
        }

        /// <summary>
        /// 非再得奖励展示激励视频
        /// </summary>
        /// <param name="CallBack">观看结束回调</param>
        public static void ShowReward(Action CallBack)
        {
            StarkSDK.API.GetStarkAdManager().ShowVideoAdWithId(Reward, (IsDone) =>
            {
                if (IsDone)
                {
                    CallBack.Invoke();
                }
                else
                {
                    Debug.Log("没有看完奖励");
                }
            }, (errCode, ErrMsg) => { Debug.LogError("激励错误" + errCode + "错误信息：" + ErrMsg); });
        }

        /// <summary>
        /// 开始录屏
        /// </summary>
        public static void OnStartRecord()
        {
            if (currentRecorder is null)
            {
                currentRecorder = StarkSDK.API.GetStarkGameRecorder();
                if (currentRecorder.GetVideoRecordState() != StarkGameRecorder.VideoRecordState.RECORD_STARTED)
                {
                    currentRecorder.StartRecord(true, MaxRecordTime, () => { clipRanges.Clear(); },
                        (errCode, ErrMsg) => { Debug.LogError("录屏错误" + errCode + "错误信息" + ErrMsg); }, (path) =>
                        {
                            Debug.Log("超时" + "地址Path:" + path);
                            MaxRecordTime = 0;
                        });
                }
            }
        }

        /// <summary>
        /// 停止录屏
        /// </summary>
        public static void OnStopRecord()
        {
            currentRecorder.StopRecord((path) =>
            {
                Debug.Log("停止录屏" + "  地址为" + path);
                MaxRecordTime = 0;
            });
        }

        /// <summary>
        /// 侧边栏 WEBGL
        /// </summary>
        public static void GetStarkSideBar()
        {
            StarkSDK.API.GetStarkSideBarManager().CheckScene(StarkSideBar.SceneEnum.SideBar,
                b => { Debug.Log("是否成功" + b); }, () => { Debug.Log("检查跳转已完成"); },
                (errCode, ErrMsg) => { Debug.Log($"错误代码:{errCode}" + "错误信息" + ErrMsg); });
        }

        /// <summary>
        /// 侧边栏 Android
        /// </summary>
        public static void GetStarkSideBarAndroid()
        {
            StarkSDK.API.GetStarkSideBarManager().NavigateToScene(StarkSideBar.SceneEnum.SideBar,
                () => { Debug.Log("成功"); }, () => { Debug.Log("检查完成"); },
                (errCode, errMsg) => { Debug.Log($"错误代码:{errCode}" + "错误信息" + errMsg); });
        }

        /// <summary>
        /// 感知用户是否从侧边栏场景进入
        /// </summary>
        public static void OnShowWithDict()
        {
            StarkSDK.API.GetStarkAppLifeCycle().OnShowWithDict += _ => { isFormSliderbar = true; };
        }
    }
}