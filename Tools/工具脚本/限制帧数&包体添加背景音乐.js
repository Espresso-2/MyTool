
//限制帧数
qg.setPreferredFramesPerSecond(30);

//包体中添加背景音乐代码
var audio = qg.createInnerAudioContext();
audio.src = "/BGM.mp3";
audio.loop = true;
audio.volume = 0.7;
audio.autoplay = true;

//创建上方Banner
window.bannerAd = qg.createBannerAd({
	posId: Pointer_stringify(posIdStr),
	style: {
		top: 0,
		left: (qg.getSystemInfoSync().screenWidth - 1080) * 0.5
	}
});