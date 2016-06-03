public interface IPanel
{
    void Init();        //初始化回调。获取对象引用，在没销毁Panel对象的情况下只会执行一次
    void InitEvent();   //初始化事件回。UI事件的注册，全局事件的注册，在没销毁Panel对象的情况下只会执行一次
    void OnShow();      //Panel打开时的回调
    void OnHide();      //Panel隐藏时的回调
    void OnBack();      //在当前Panel点击后退按钮的行为
    void OnDestroy();   //Panel销毁时的回调
    void Update();      //Update的回调
}
