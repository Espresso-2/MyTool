using UnityEngine;
using UnityEngine.UI;
using W_Scripts;
using W_Scripts.AdManager;

/// <summary>
/// 侧边栏管理
/// </summary>
public class SliderbarManager : MonoBehaviour
{
    /// <summary>
    /// 获取奖励与打开侧边栏按钮
    /// </summary>
    private Button OpenSliderBar;
    /// <summary>
    /// 文字显示
    /// </summary>
    private Image OpenSliderImage;
    /// <summary>
    /// 替换文字
    /// </summary>
    [SerializeField] private Sprite GerReward;

    /// <summary>
    /// 组件引用
    /// </summary>
    private void Awake()
    {
        OpenSliderBar = GetComponent<Button>();
        OpenSliderImage = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        //首先判断用户是否从侧边栏进入并记录bool
        DouyinAdManager.OnShowWithDict();
        //如果是从侧边栏进入则将按钮点击事件清空并更换显示文本，添加解锁体力事件
        if (DouyinAdManager.isFormSliderbar)
        {
            OpenSliderImage.sprite = GerReward;
            OpenSliderBar.onClick.RemoveAllListeners();
            OpenSliderBar.onClick.AddListener(() => { StaminaSystem.Instance.UnLockStamina(); });
        }
        //如果不是从侧边栏进入游戏则将按钮点击事件清空，根据平台判断注册对应平台打开侧边栏的功能接口
        else
        {
            OpenSliderBar.onClick.RemoveAllListeners();
            OpenSliderBar.onClick.AddListener(() =>
            {
#if PLATFORM_WEBGL
                DouyinAdManager.GetStarkSideBar();
#else
                DouyinAdManager.GetStarkSideBarAndroid();
#endif
            });
        }
    }
}