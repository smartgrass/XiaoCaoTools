# XiaoCaoTools
# 主要功能：XiaoCaoWindow

作为窗口基类，用于快速搭建编辑器窗口

基于开源项目[NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) 改写 
 


如定义简单窗口 带一个按钮
```
  public class XiaoCaoEexample_1 : XiaoCaoWindow
  {
      [MenuItem("Tools/XiaoCao/Eexample_1")]
      static void Open()
      {
          OpenWindow<XiaoCaoEexample_1>("窗口名字");
      }

      [Button("执行事件")]
      private void DoUnityEvent()
      {
          Debug.Log($"yns do Event");
          unityEvent?.Invoke();
      }  
  }
 ```


<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win0.png" width= "350"/>


其他窗口

详细使用可见 Assets/XiaoCaoTools/Main/Editor/XC_ReadMe.cs

<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win1.png" width= "500"/>
<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win2.png" width= "500"/>

