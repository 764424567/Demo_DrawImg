## 一、前言
因一个任务要完成如何在Unity上面实现热力图的效果，所以百度了很久，发现资料很少，现在就把我总结的如何在Unity上面基于Canvas实现热力图效果的实现过程分享出来， 此前转载了一篇主要讲的是如何根据数据值，在Canvas上重新绘制RGBA的值，完成热力图的绘制，不过用的是H5写的，我修改了一下，用C#重写的

**效果图：**
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190830104335917.gif)

## 二、参考资料
1.基于Canvas的热力图绘制方法【http://www.blueidea.com/tech/web/2010/7933.asp 】
2.Unity（OpenGL）实现“阴阳师画符”、划线功能【https://blog.csdn.net/yuanhandsome/article/details/78366250】

## 三、正文

实现过程：
1.首先从文档中解析数据，获取需要的数据（当然，这一步也可以在网上获取数据，然后再解析）
2.将获取的数据进行分析
3.保存贴图到Texture，将保存的贴图赋值给Image的Sprite（保存的贴图也可以赋值给物理对象的贴图）
4.清除数据


### 第一步、解析数据

就以下面这一组数据为例
![在这里插入图片描述](https://img-blog.csdnimg.cn/20181229172324566.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3E3NjQ0MjQ1Njc=,size_16,color_FFFFFF,t_70)
这个文件已经上传到云盘了，https://pan.baidu.com/s/177OhwSwlZHNqjzCB7g1ubQ

第一行数据，代表是的模拟网格 160*160
第二行数据，模拟半径 -80km至80km范围;东西方向（x轴）区间范围，西→东
第三行数据，剂量值（z轴），即160*160网格范围内的最小值和最大值
第6-164，表示各个网格点的浓度值

就是说有160*160个数据，可以用二维数组去接收数据
![在这里插入图片描述](https://img-blog.csdnimg.cn/20181229172742928.png)
然后用一个List数组去接收解析完的数据
![在这里插入图片描述](https://img-blog.csdnimg.cn/20181229172806161.png)


#### 完整代码

```csharp
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draw : MonoBehaviour
{
    //文件路径名
    private string fileName;
    //读取数据
    private string str;
    //保存数据
    private List<int> numberList = new List<int>();
    string tempData_Space;
    string[] tempData_Array;
    int[] tempData_IntArray;
    //保存解析完的数据
    private int[,] m_Sum = new int[159, 159];

    void Start()
    {
        ReadFile();
    }

    //读指定目录下的文件
    public void ReadFile()
    {
        //指定目录下的文件
        fileName = "X:/2015_m01_d01_0000(utc+0800)_l00_csi_1hr_conc.grd";
        StreamReader sr = File.OpenText(fileName);
        while (true)
        {
            //按行读取，str保存的是一行的数据
            str = sr.ReadLine();
            int lineLength = 0;
            if (str != null)
            {
                //得到获取到的这行数据的长度
                lineLength = str.Length;
                //过滤掉前几行和后几行的数据
                if (lineLength >= 1000)
                {
                    //SPlit函数只能分割单个空格，所以两个空格替换成一个空格
                    tempData_Space = str.Replace("  ", " ");
                    //分割字符串
                    tempData_Array = tempData_Space.Split(' ');
                    //保存获取到的数据
                    tempData_IntArray = new int[tempData_Array.Length];
                    //取后两位数据
                    for (int i = 0; i < tempData_Array.Length; i++)
                    {
                        tempData_IntArray[i] = int.Parse(tempData_Array[i].Substring(8, 2));
                    }
                    //添加到List数组中
                    for (int i = 0; i < tempData_IntArray.Length; i++)
                    {
                        numberList.Add(tempData_IntArray[i]);
                    }
                }
            }
            if (str == null)
            {
                break;
            }
        }
        //将List数组中保存的数据赋值给二维数组
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                int i = y + x * m_Width;
                m_Sum[x, y] = numberList[i];
            }
        }
        //关闭数据流
        sr.Close();
    }
```

### 第二步、分析数据

数据保存到m_Sum二维数组中，剩下的就是从二维数组中读取数据，然后进行分析
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190103095200192.png)
读取到的数据是最后两位的数据，然后将数据转成int类型进行对比

实现代码

```csharp
//对比数据
    public void DataCompare()
    {
        //因为二维数组的数量是159*159所以就在这个范围内读取
        for (int i = 0; i < 159; i++)
        {
            for (int j = 0; j < 159; j++)
            {
                //数据对比
                int temp = m_Sum[i, j];
                if (temp <= 6)
                {
                    Debug.Log("透明");
                }
                else if (temp > 6 && temp <= 7)
                {
                    Debug.Log("红色");
                }
                else if (temp > 7 && temp <= 8)
                {
                    Debug.Log("橘色");
                }
                else if (temp > 8 && temp <= 9)
                {
                    Debug.Log("黄色");
                }
                else if (temp > 9 && temp <= 10)
                {
                    Debug.Log("绿色");
                }
                else if (temp > 10 && temp <= 11)
                {
                    Debug.Log("蓝色");
                }
                else if (temp > 11 && temp <= 12)
                {
                    Debug.Log("靛蓝色");
                }
                else if (temp > 12)
                {
                    Debug.Log("紫色");
                }
            }
        }
    }
```

### 第三步、保存贴图

最重要的一步，如何将分析过的数据生成贴图，并且赋值给Image呢

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190103100243249.png)
主要就是这两行的代码

#### 完成代码

```csharp
//保存贴图变量
    private Texture2D texture;
    private Image targetMaterial;
    
//创建设置贴图文件
    void CreatTexture()
    {
        texture = new Texture2D(m_Width, m_Height);
        int targetWidth = m_Width;
        int targetHeight = m_Height;
        for (int i = 0; i < targetWidth; i++)
        {
            for (int j = 0; j < targetHeight; j++)
            {
                int temp = m_Sum[i, j];
                if (temp <= 6)
                {
                    texture.SetPixel(i, j, m_ColorTransparency);
                }
                else if (temp > 6 && temp <= 7)
                {
                    texture.SetPixel(i, j, m_ColorRed);
                }
                else if (temp > 7 && temp <= 8)
                {
                    texture.SetPixel(i, j, m_ColorOrange);
                }
                else if (temp > 8 && temp <= 9)
                {
                    texture.SetPixel(i, j, m_ColorYellow);
                }
                else if (temp > 9 && temp <= 10)
                {
                    texture.SetPixel(i, j, m_ColorGreen);
                }
                else if (temp > 10 && temp <= 11)
                {
                    texture.SetPixel(i, j, m_ColorBlue);
                }
                else if (temp > 11 && temp <= 12)
                {
                    texture.SetPixel(i, j, m_ColorIndigo);
                }
                else if (temp > 12)
                {
                    texture.SetPixel(i, j, m_ColorPurple);
                }
            }
        }
        //应用贴图
        texture.Apply();
        //将贴图数据赋值给Image的sprite
        targetMaterial.sprite = Sprite.Create(texture, new Rect(0, 0, m_Width, m_Height), Vector2.zero); ;
    }
```


### 第四步、清除数据
清除数组中的数据，供下次读取数据

```csharp
//清理数组中的数据
    public void ClearList()
    {
        numberList.Clear();
    }
```


### 整体代码

```csharp
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draw : MonoBehaviour
{
    //颜色设置
    public Color m_ColorTransparency;
    public Color m_ColorRed;
    public Color m_ColorOrange;
    public Color m_ColorYellow;
    public Color m_ColorGreen;
    public Color m_ColorBlue;
    public Color m_ColorIndigo;
    public Color m_ColorPurple;
    //保存贴图
    private Texture2D texture;
    private Image targetMaterial;
    //文件路径名
    private string fileName;
    //读取数据
    private string str;
    //保存数据
    private List<int> numberList = new List<int>();
    string tempData_Space;
    string[] tempData_Array;
    int[] tempData_IntArray;
    //保存解析完的数据
    private int[,] m_Sum = new int[159, 159];
    //长宽设置
    private int m_Width = 159;
    private int m_Height = 159;

    void Start()
    {
        targetMaterial = gameObject.GetComponent<Image>();
        targetMaterial.color = Color.white;
        ReadFile();
        CreatTexture();
        ClearList();
    }

    //清理数组中的数据
    public void ClearList()
    {
        numberList.Clear();
    }

    //读指定目录下的文件
    public void ReadFile()
    {
        //指定目录下的文件
        fileName = "X:/2015_m01_d01_0000(utc+0800)_l00_csi_1hr_conc.grd";
        StreamReader sr = File.OpenText(fileName);
        while (true)
        {
            //按行读取，str保存的是一行的数据
            str = sr.ReadLine();
            int lineLength = 0;
            if (str != null)
            {
                //得到获取到的这行数据的长度
                lineLength = str.Length;
                //过滤掉前几行和后几行的数据
                if (lineLength >= 1000)
                {
                    //SPlit函数只能分割单个空格，所以两个空格替换成一个空格
                    tempData_Space = str.Replace("  ", " ");
                    //分割字符串
                    tempData_Array = tempData_Space.Split(' ');
                    //保存获取到的数据
                    tempData_IntArray = new int[tempData_Array.Length];
                    //取后两位数据
                    for (int i = 0; i < tempData_Array.Length; i++)
                    {
                        tempData_IntArray[i] = int.Parse(tempData_Array[i].Substring(8, 2));
                    }
                    //添加到List数组中
                    for (int i = 0; i < tempData_IntArray.Length; i++)
                    {
                        numberList.Add(tempData_IntArray[i]);
                    }
                }
            }
            if (str == null)
            {
                break;
            }
        }
        //将List数组中保存的数据赋值给二维数组
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                int i = y + x * m_Width;
                m_Sum[x, y] = numberList[i];
            }
        }
        //关闭数据流
        sr.Close();
    }

    //创建设置贴图文件
    void CreatTexture()
    {
        texture = new Texture2D(m_Width, m_Height);
        int targetWidth = m_Width;
        int targetHeight = m_Height;
        for (int i = 0; i < targetWidth; i++)
        {
            for (int j = 0; j < targetHeight; j++)
            {
                int temp = m_Sum[i, j];
                if (temp <= 6)
                {
                    texture.SetPixel(i, j, m_ColorTransparency);
                }
                else if (temp > 6 && temp <= 7)
                {
                    texture.SetPixel(i, j, m_ColorRed);
                }
                else if (temp > 7 && temp <= 8)
                {
                    texture.SetPixel(i, j, m_ColorOrange);
                }
                else if (temp > 8 && temp <= 9)
                {
                    texture.SetPixel(i, j, m_ColorYellow);
                }
                else if (temp > 9 && temp <= 10)
                {
                    texture.SetPixel(i, j, m_ColorGreen);
                }
                else if (temp > 10 && temp <= 11)
                {
                    texture.SetPixel(i, j, m_ColorBlue);
                }
                else if (temp > 11 && temp <= 12)
                {
                    texture.SetPixel(i, j, m_ColorIndigo);
                }
                else if (temp > 12)
                {
                    texture.SetPixel(i, j, m_ColorPurple);
                }
            }
        }
        //应用贴图
        texture.Apply();
        //将贴图数据赋值给Image的sprite
        targetMaterial.sprite = Sprite.Create(texture, new Rect(0, 0, m_Width, m_Height), Vector2.zero); ;
    }
}
```

## 四、工程文件
链接：https://pan.baidu.com/s/1akdyDVrwHjQJm9RvD4Rh8w 
提取码：y4be

Github地址：
https://github.com/764424567/Demo_DrawImg

