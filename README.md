# XiaoCaoTools
# 主要功能：XiaoCaoWindow

作为窗口基类，用于快速搭建编辑器窗口

基于开源项目NaughtyAttributes改写 https://github.com/dbrizov/NaughtyAttributes 
 

<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/Window.png" width= "500"/>


如定义简单窗口 带一个按钮

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



详细使用可见Assets/XiaoCaoTools/Main/Editor/XC_ReadMe.cs
