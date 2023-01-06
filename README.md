# XiaoCaoWindow编辑器扩展

用于Unity快速搭建编辑器窗口 

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

下图为以上代码的效果:

<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win0.png" width= "350"/>



# 其他演示

## 使用起来和Mono类的监视器界面差不多, 定义public字段就显示出来

### 1. [XiaoCaoEexample_1.cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XiaoCaoEexample_1.cs) 效果如下 :

<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win1.png" width= "450"/>




### 2. [XiaoCaoObjectUsing .cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XiaoCaoObjectUsing.cs) :
可以定义一个SO类字段, 便可以收藏夹使用
 ```
    [Expandable()]
    public ObjectUsing objectUsing;
 ```


<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win2.png" width= "450"/>

### 3. 其他详细案例 [XC_ReadMe.cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XC_ReadMe.cs)

