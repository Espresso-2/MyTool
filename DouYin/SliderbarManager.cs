using _scripts;
using UnityEngine;
using UnityEngine.UI;
using W_Scripts.AdManager;

/// <summary>
/// 侧边栏管理
/// </summary>
public class SliderbarManager : MonoBehaviour
{
    /// <summary>
    /// 获取奖励与打开侧边栏按钮
    /// </summary>
    [SerializeField] private Button OpenSliderBar;
    /// <summary>
    /// 文字显示
    /// </summary>
    [SerializeField] private Image OpenSliderImage;
    /// <summary>
    /// 替换文字
    /// </summary>
    [SerializeField] private Sprite GerReward;

    [SerializeField] private Button OpenSideBarWindowButton;

    private bool IsFirst = true;

    private void OnEnable()
    {
        DYAdManager.UserFromSidebarEvent += SidebarEventCallBack;
    }

    private void Start()
    {
        /*初始订阅进入侧边栏后的事件响应器*/
        /*默认按钮分配点击事件为打开侧边栏*/
        OpenSliderBar.onClick.AddListener(DYAdManager.GetStarkSideBar);
    }

    private void OnDisable()
    {
        DYAdManager.UserFromSidebarEvent -= SidebarEventCallBack;
    }

    /*响应用户从侧边栏进入时的事件处理器*/
    private void SidebarEventCallBack()
    {
        if (IsFirst)
        {
            IsFirst = false;
            OpenSliderImage.overrideSprite = GerReward;
            OpenSliderImage.SetNativeSize();
            OpenSliderBar.onClick.RemoveAllListeners();
            OpenSliderBar.onClick.AddListener(() =>
            {
                RunUIManager.instance.AddCoins(1000);
                OpenSideBarWindowButton.gameObject.SetActive(false);
            });
        }
    }
}