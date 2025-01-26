using System;
using UnityEngine;
using WeChatWASM;
using Random = UnityEngine.Random;

public static class AdManager
{
    /*public WXBannerAd BannerAd;
    public WXCustomAd CustomAd;
    public WXRewardedVideoAd RewardedVideoAd;
    public WXInterstitialAd InterstitialAd;*/

    private static readonly string[] CustomAdID = new[]
        { "adunit-1831baf1d0312e30", "adunit-06a3585d520b7e7e", "adunit-184f8b6677f5c42e", "adunit-a705e33d3f4de5fe" };
    private static readonly string[] BannerAdID = new[] { "adunit-a705e33d3f4de5fe", };
    private static readonly string[] RewardAdID = new[]
        { "adunit-b6eafa7472347e66", "adunit-afc876b9b8345600", "adunit-653324cbfb5ae705", "adunit-5a91c96ee2618f34" };
    private static readonly string[] InterstitialAdID = new[] { "adunit-b9fceb620dcc0005", "adunit-08543c0d96417e8a", "adunit-da8e89fc0fafc086" };

    public static void WXRewardAd(Action CallBack)
    {
        var Reward = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
        {
            adUnitId = RewardAdID[Random.Range(0, RewardAdID.Length)]
        });
        Reward.OnLoad(_ => { Debug.Log("加载成功"); });
        Reward.Show();
        Reward.OnError((res) => { Debug.Log("错误信息" + res); });
        Reward.OnClose((res) =>
        {
            if (res.isEnded)
            {
                CallBack.Invoke();
            }
            else
            {
                Debug.Log("没有看完广告");
            }
        });
    }

    public static void BannerAd()
    {
        var bannerAd = WX.CreateBannerAd(new WXCreateBannerAdParam()
        {
            adUnitId = BannerAdID[Random.Range(0, RewardAdID.Length)],
            adIntervals = 30,
            style = new Style()
            {
                left = 0,
                top = 0,
                width = 600,
                height = 200
            }
        });
        bannerAd.OnLoad(_ => { bannerAd.Show(); });
        bannerAd.OnError((WXADErrorResponse res) => { Debug.Log(res.errCode); });
    }

    public static void InterstitialAd()
    {
#if UNITY_EDITOR
        Debug.Log("插屏");
#else
         var interstitialAd = WX.CreateInterstitialAd(new() { adUnitId = InterstitialAdID[Random.Range(0, InterstitialAdID.Length)] });
        interstitialAd.Load(_ =>
        {
        }, _ =>
        {
            interstitialAd.Load();
        });
       interstitialAd.OnLoad((_) =>
       {
           interstitialAd.Show();
       });
        interstitialAd.OnError((res) =>
        {
            Debug.Log("插屏播放错误"+res.errCode);
            
        });
#endif
    }

    public static void CustomAd()
    {
#if UNITY_EDITOR
        Debug.Log("原生");
#else
        var customAd = WX.CreateCustomAd(new()
        {
            adUnitId = CustomAdID[Random.Range(0, CustomAdID.Length)],
            adIntervals = 30,
            style =
            {
                left = 0,
                top = 100
            }
        });
        customAd.OnLoad((_) => { customAd.Show(); });
        customAd.OnError((res) => { Debug.Log("原生" + res.errCode); });
#endif
    }
    /*
    private void Start()
    {
        WXBase.InitSDK(code =>
        {
            CreateBannerAd();
            CreateRewardedVideoAd();
            CreateCustomAd();
        });
    }

    private void OnDestroy()
    {
        DestroyBannerAd();
        DestroyRewardedVideoAd();
        DestroyCustomAd();
        CreateInterstitialAd();
    }

    private void CreateInterstitialAd()
    {
        InterstitialAd = WXBase.CreateInterstitialAd(new WXCreateInterstitialAdParam
        {
            adUnitId = InterstitialAdID
        });
    }

    public void ShowInterstitialAd()
    {
        InterstitialAd.Show();
    }


    private void CreateBannerAd()
    {
        BannerAd = WXBase.CreateFixedBottomMiddleBannerAd(BannerAdID, 30, 200);
    }

    public void ShowBannerAd()
    {
        BannerAd.Show();
    }

    public void HideBannerAd()
    {
        BannerAd.Hide();
    }

    public void DestroyBannerAd()
    {
        BannerAd.Destroy();
    }

    private void CreateRewardedVideoAd()
    {
        RewardedVideoAd = WXBase.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam
        {
            adUnitId = RewardAdID
        });
        RewardedVideoAd.OnLoad(res =>
        {
            Debug.Log("RewardedVideoAd.OnLoad:" + JsonUtility.ToJson(res));
            WXRewardedVideoAdReportShareBehaviorResponse reportShareBehaviorRes = RewardedVideoAd.ReportShareBehavior(new RequestAdReportShareBehaviorParam
            {
                operation = 1,
                currentShow = 1,
                strategy = 0,
                shareValue = res.shareValue,
                rewardValue = res.rewardValue,
                depositAmount = 100
            });
            Debug.Log("ReportShareBehavior.Res:" + JsonUtility.ToJson(reportShareBehaviorRes));
        });
        RewardedVideoAd.OnError(err => { Debug.Log("RewardedVideoAd.OnError:" + JsonUtility.ToJson(err)); });
        RewardedVideoAd.OnClose(res => { Debug.Log("RewardedVideoAd.OnClose:" + JsonUtility.ToJson(res)); });
        RewardedVideoAd.Load();
    }

    public void ShowRewardedVideoAd()
    {
        RewardedVideoAd.Show();
    }

    public void DestroyRewardedVideoAd()
    {
        RewardedVideoAd.Destroy();
    }

    private void CreateCustomAd()
    {
        CustomAd = WXBase.CreateCustomAd(new WXCreateCustomAdParam
        {
            adUnitId = CustomAdID,
            adIntervals = 30,
            style =
            {
                left = 0,
                top = 100
            }
        });
        CustomAd.OnLoad(res => { Debug.Log("CustomAd.OnLoad:" + JsonUtility.ToJson(res)); });
        CustomAd.OnError(res => { Debug.Log("CustomAd.onError:" + JsonUtility.ToJson(res)); });
        CustomAd.OnHide(() => { Debug.Log("CustomAd.onHide:"); });
        CustomAd.OnClose(() => { Debug.Log("CustomAd.onClose"); });
    }

    public void ShowCustomAd()
    {
        CustomAd.Show();
    }

    public void HideCustomAd()
    {
        CustomAd.Hide();
    }

    public void DestroyCustomAd()
    {
        CustomAd.Destroy();
    }
    */
}