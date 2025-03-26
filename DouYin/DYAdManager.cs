using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace W_Scripts.AdManager
{
    public static class DYAdManager
    {
        private const string RewardParam = "e7ffi9o549d365tp6v";
        private const string InterstitialParam = "";
        private const string BannerParam = "";
        private static TTRewardedVideoAd ttRewardedVideoAd;

        private static TTBannerAd ttBannerAd;
        public static event Action UserFromSidebarEvent;

        static DYAdManager()
        {
            InitReward();
        }

        /// <summary>
        /// 提前创建并绑定错误处理与预加载激励视频
        /// </summary>
        private static void InitReward()
        {
            ttRewardedVideoAd = TT.CreateRewardedVideoAd(RewardParam);
            ttRewardedVideoAd.OnError += (code, Error) =>
            {
                ttRewardedVideoAd.Load();
                Debug.Log($"错误码：{code} , 错误信息：{Error}");
            };
            ttRewardedVideoAd.OnLoad += () => { Debug.Log($"激励广告加载完成_OnLoad已触发"); };
            ttRewardedVideoAd.Load();
        }

        /// <summary>
        /// 封装调用激励广告
        /// </summary>
        /// <param name="closeCallBack">发放奖励方法</param>
        public static void ShowRewardAd(Action closeCallBack)
        {
            //创建临时变量用来存储发放奖励逻辑
            RewardedAdClosedDelegate rewardedAdClosedDelegate = null;
            //临时匿名委托实现具体发放奖励逻辑
            rewardedAdClosedDelegate = (ended, count) =>
            {
                if (ended || count == 1)
                {
                    closeCallBack?.Invoke();
                    Debug.Log("发放奖励");
                    //解除订阅
                }
                else
                {
                    Debug.Log("未看完激励不发放奖励");
                }
                ttRewardedVideoAd.OnClose -= rewardedAdClosedDelegate;
            };
            if (ttRewardedVideoAd != null)
            {
                //先取消上一次的订阅关系
                ttRewardedVideoAd.OnClose -= rewardedAdClosedDelegate;
                //订阅关闭回调
                ttRewardedVideoAd.OnClose += rewardedAdClosedDelegate;
                //展示广告
                ttRewardedVideoAd.Show();
                Debug.Log("展示激励广告");
            }
            else
            {
                Debug.Log("没有激励组件实例");
            }
        }

        /// <summary>
        /// 访问侧边栏功能
        /// </summary>
        public static void GetStarkSideBar()
        {
            //检查当前是否支撑跳转侧边栏
            TT.CheckScene(TTSideBar.SceneEnum.SideBar, (IsSuccess) =>
            {
                Debug.Log($"访问侧边栏是否成功 ： {IsSuccess}");
                if (IsSuccess)
                {
                    JsonData data = new()
                    {
                        ["scene"] = "sidebar"
                    };
                    TT.NavigateToScene(data, () =>
                    {
                        Debug.Log("侧边栏跳转成功");
                        /*跳转成功后开启订阅用户是如何进入游戏的*/
                        CheckIsUserEnterGameFromSidebar();
                    }, () => { Debug.Log("侧边栏跳转完成后调用"); }, (code, Error) => { Debug.Log($"跳转侧边栏报错 ： 错误码{code},错误信息：{Error}"); });
                }
                else
                {
                    Debug.Log("不支持侧边栏跳转");
                }
            }, () => { Debug.Log("检查完成调用"); }, (Code, Error) => { Debug.Log($"检查是否支持侧边栏跳转出错 错误码：{Code},错误信息：{Error}"); });
        }

        public static void InterstitialAd()
        {
            Debug.Log("创建插屏广告");
            TT.CreateInterstitialAd(InterstitialParam, (code, Error) => { Debug.Log($"调用插屏出现错误,错误码：{code},错误信息：{Error}｝"); },
                () => { Debug.Log("关闭插屏"); }, () => { Debug.Log("插屏加载完毕"); });
        }

        public static void BannerAd()
        {
            TTBannerStyle bannerStyle = new TTBannerStyle();
            bannerStyle.top = 10;
            bannerStyle.left = 10;
            bannerStyle.width = 320;
            if (ttBannerAd != null && ttBannerAd.IsInvalid())
            {
                ttBannerAd.Destroy();
                ttBannerAd = null;
            }
            if (ttBannerAd == null)
            {
                var param = new CreateBannerAdParam()
                {
                    BannerAdId = BannerParam,
                    Style = bannerStyle,
                    AdIntervals = 60
                };
                ttBannerAd = TT.CreateBannerAd(param);
                ttBannerAd.OnError += (code, error) => { Debug.Log($"调用Banner广告发生错误，错误码：{code},错误信息：{error}"); };
                ttBannerAd.OnClose += () => { Debug.Log("关闭Banner广告"); };
                ttBannerAd.OnLoad += () => { ttBannerAd?.Show(); };
                ttBannerAd.OnResize += (weight, Height) => { Debug.Log($"广告宽度：{weight},广告高度：{Height}"); };
            }
            ttBannerAd.Show();
        }

        private static void CheckIsUserEnterGameFromSidebar()
        {
            TT.GetAppLifeCycle().OnShow += (DIC) =>
            {
                // foreach (KeyValuePair<string, object> keyValuePair in DIC)
                // {
                //     Debug.Log($"字典中的元素键值对 ： Key:{keyValuePair.Key},Value:{keyValuePair.Value}");
                // }
                if (DIC["launchFrom"].ToString().Equals("homepage"))
                {
                    UserFromSidebarEvent?.Invoke();
                    Debug.Log($"用户从 homepage 进入");
                }
                else if (DIC["location"].ToString().Equals("sidebar_card"))
                {
                    UserFromSidebarEvent?.Invoke();
                    Debug.Log("用户从 sidebar_card 进入");
                }
            };
        }
    }
}