public static class ColorStr
{


    public static int send_Counter = 0;
    public static int receive_Counter = 0;
    private static int m_step = 0;
    public static string LogStr(object color, object content)
    {
        return string.Format("<color={0}><size={1}>{2}</size></color>", color, "12", content);
    }


    /// <summary>
    /// 严格的步骤纪录
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Step(object content)
    {
        return string.Format("<color=white><size=11>{0}</size></color>", "【" + (++m_step).ToString() + "】" + content);
    }
    //private static int m_step = 0;

    //16进制的颜色值
    //https://www.cnblogs.com/summary-2017/p/7504126.html


    public static string StepLog(this object content)
    {
        return string.Format("<color=#FF9900><size=12>{0}</size></color>",/* "【" + (++m_step).ToString() + "】" + */content);
    }


    #region --------------------------------------- 深、重 
    /// <summary>
    /// 春绿色 ,深
    /// </summary>
    public static string FH1_SpringGreen(this object content)
    {
        return string.Format("<color=#00FF7F>{0}</color>", content);
    }

    /// <summary>
    /// 紫罗兰色 ,紫红色
    /// </summary>
    public static string FH2_VioletRed(this object content)
    {
        return string.Format("<color=#D02090>{0}</color>", content);
    }

    //

    /// 番茄色
    /// </summary>
    public static string FH3_Red(this object content)
    {
        return string.Format("<color=#FF0000><b><size=14>{0}</size></b></color>", content);
    }


    /// <summary>
    /// 橘红色
    /// </summary>
    public static string FH4_OrangeRed(this object content)
    {
        return string.Format("<color=#FF4500>{0}</color>", content);
    }


    public static string FH5_Yellow(this object content)
    {
        return string.Format("<color=#FFFF00>{0}</color>", content);
    }
    /// <summary>
    /// 浅绿色
    /// </summary>
    public static string FH6_Aqua(this object content)
    {
        return string.Format("<color=#00FFFF>{0}</color>", content);
    }

    //
    /// <summary>
    /// 皇家蓝 , 宝蓝色
    /// </summary>
    public static string FH7_RoyalBlue(this object content)
    {
        return string.Format("<color=#4169E1>{0}</color>", content);
    }

    #endregion



    #region --------------------------------------- 清 、浅
    /// <summary>
    /// 粉红色
    /// </summary>
    public static string FL1_HotPink(this object content)
    {
        return string.Format("<color=#FF69B4>{0}</color>", content);
    }

    /// <summary>
    /// 苍白的宝石绿
    /// </summary>
    public static string FL2_PaleTurquoise(this object content)
    {
        return string.Format("<color=#AFEEEE>{0}</color>", content);
    }

    /// <summary>
    /// 粉色
    /// </summary>
    public static string FL3_Pink(this object content)
    {
        return string.Format("<color=#FFC0CB>{0}</color>", content);
    }

    /// <summary>
    /// 石兰色
    /// </summary>
    public static string FL4_SlateBlue(this object content)
    {
        return string.Format("<color=#6A5ACD>{0}</color>", content);
    }
    ///<summary>
    /// 番茄色
    /// </summary>
    public static string FL5_Tomato(this object content)
    {
        return string.Format("<color=#FF6347>{0}</color>", content);
    }

    ///<summary>
    /// 卡其色
    /// </summary>
    public static string FL6_Khaki(this object content)
    {
        return string.Format("<color=#F0E68C>{0}</color>", content);
    }

    /// <summary>
    /// 乳白色 , 象牙白
    /// </summary>
    public static string FL7_Ivory(this object content)
    {
        return string.Format("<color=#FFFFF0>{0}</color>", content);
    }
    #endregion

}