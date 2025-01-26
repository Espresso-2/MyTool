/获取时间戳，并缓存一下
        var W_CurrentTime = DateTime.Now;
        //如果当前时间与调用时间的插值大于CD值说明冷却完毕
        if ((W_CurrentTime-W_lastTime).Seconds>W_CD)
        {
            //执行广告，并记录广告调用后的时间节点，相当于重置CD
            // Debug.LogError("展示广告"+((W_CurrentTime-W_lastTime).Seconds));
            //TODO：插屏
            
          W_lastTime = W_CurrentTime;
        }
        else
        {
        }  //  Debug.LogError("不展示广告"+((W_CurrentTime-W_lastTime).Seconds));