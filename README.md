# XiaoCaoWindow--Unity编辑器扩展

用于Unity快速搭建编辑器窗口 

本项目基于[NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) 开发
 
# 演示 

## 一个简单窗口
```
public class XiaoCaoEexample_0 : XiaoCaoWindow
{
    [MenuItem("Tools/XiaoCao/Eexample_0")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_0>("窗口名字");
    }
    public string str;

    [Button("执行事件")]
    private void DoEvent()
    {
        Debug.Log($"yns do Event");
    }
}
 ```


<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win0.png" width= "350"/>



# 其他演示

## 使用起来和Mono类的监视器界面差不多, 定义public字段就显示出来

### 1. 样式示例 [XiaoCaoEexample_1.cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XiaoCaoEexample_1.cs) 效果如下 :

<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win1.png" width= "450"/>




### 2. Unity收藏夹 [XiaoCaoObjectUsing .cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XiaoCaoObjectUsing.cs) :
定义一个SO类字段, 便可当作收藏夹
 ```
    [Expandable()]
    public ObjectUsing objectUsing;
 ```


<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win2.png" width= "450"/>

### 3. Json可视化编辑 [XiaoCaoJsonWin.cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XiaoCaoJsonWin.cs) :

<img src="https://github.com/smartgrass/XiaoCaoTools/blob/main/GitImages/win4.png" width= "550"/>

### 4. 其他小功能 [XC_ReadMe.cs](https://github.com/smartgrass/XiaoCaoTools/blob/main/Assets/XiaoCaoTools/Main/Editor/XC_ReadMe.cs)

