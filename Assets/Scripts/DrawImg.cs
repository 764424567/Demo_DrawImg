using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawImg : MonoBehaviour
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
    //解析文件名
    string m_StringHour;
    string m_StringDay;
    int m_Day = 1;
    int m_Hour = 0;
    //长宽设置
    private int m_Width = 159;
    private int m_Height = 159;

    void Start()
    {
        targetMaterial = gameObject.GetComponent<Image>();
        targetMaterial.color = Color.white;
        fileName = "./Assets/Resources/2015_m01_d01_0000(utc+0800)_l00_csi_1hr_conc.grd";
        ReadFile();
        CreatTexture();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //根据点击到UI对象，获取UI对象名字
            //如果获取到对象的话   
            if (OnePointColliderObject() != null)
            {
                //给图片名字赋值
                string onClickName = OnePointColliderObject().name;
                //创建对象
                ButtonImageOnClick(onClickName);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ButtonOnClickUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ButtonOnClickDown();
        }
    }

    

    //根据UI名字响应事件
    void ButtonImageOnClick(string name)
    {
        switch (name)
        {
            case "Button_Up":
                ButtonOnClickUp();
                break;
            case "Button_Down":
                ButtonOnClickDown();
                break;
            default:
                break;
        }
    }

    //点击上一张
    public void ButtonOnClickUp()
    {
        m_Hour--;
        if (m_Hour < 0 && m_Day == 1)
        {
            m_Hour = 0;
            m_Day = 1;
            m_StringDay = "01";
            m_StringHour = "0000";
        }
        else
        {
            if (m_Hour < 0)
            {
                m_Day--;
                m_Hour = 23;
                m_StringHour = m_Hour + "00";
            }
            else if (m_Hour >= 10)
            {
                m_StringHour = m_Hour + "00";
            }
            else
            {
                m_StringHour = "0" + m_Hour + "00";
            }
            if (m_Day >= 10)
            {
                m_StringDay = m_Day.ToString();
            }
            else
            {
                m_StringDay = "0" + m_Day;
            }
        }
        //Debug.Log(m_StringDay + "," + m_StringHour);
        //2015_m01_d01_0000(utc+0800)_l00_csi_1hr_conc
        //2015_m01_d01_01(utc+0800)_l00_csi_1hr_conc
        //2015_m01_d01_01(utc+0800)_l00_csi_1hr_conc.grd
        fileName = "./Assets/Resources/" + "2015_m01_d" + m_StringDay + "_" + m_StringHour + "(utc+0800)_l00_csi_1hr_conc.grd";
        ClearList();
        ReadFile();
        CreatTexture();
    }

    //点击下一张
    public void ButtonOnClickDown()
    {
        m_Hour++;
        if (m_Hour >= 10 && m_Hour < 24)
        {
            m_StringHour = m_Hour + "00";
        }
        else if (m_Hour >= 24)
        {
            m_Hour = 0;
            m_Day++;
            m_StringHour = "0" + m_Hour + "00";
        }
        else
        {
            m_StringHour = "0" + m_Hour + "00";
        }
        if (m_Day >= 10)
        {
            m_StringDay = m_Day.ToString();
        }
        else
        {
            m_StringDay = "0" + m_Day;
        }
        fileName = "./Assets/Resources/" + "2015_m01_d"+ m_StringDay + "_"+ m_StringHour + "(utc+0800)_l00_csi_1hr_conc.grd";
        ClearList();
        ReadFile();
        CreatTexture();
    }

    //清理数组中的数据
    public void ClearList()
    {
        numberList.Clear();
    }

    //点击对象获取到对象的名字
    public GameObject OnePointColliderObject()
    {
        //存有鼠标或者触摸数据的对象
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        //当前指针位置
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //射线命中之后的反馈数据
        List<RaycastResult> results = new List<RaycastResult>();
        //投射一条光线并返回所有碰撞
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //返回点击到的物体
        if (results.Count > 0)
            return results[0].gameObject;
        else
            return null;
    }

    //读指定目录下的文件
    public void ReadFile()
    {
        Debug.Log(fileName);
        StreamReader sr = File.OpenText(fileName);
        while (true)
        {
            str = sr.ReadLine();
            int lineLength = 0;
            if (str != null)
            {
                lineLength = str.Length;
                //过滤掉前几行和后几行的数据
                if (lineLength >= 1000)
                {
                    tempData_Space = str.Replace("  ", " ");
                    tempData_Array = tempData_Space.Split(' ');
                    tempData_IntArray = new int[tempData_Array.Length];
                    for (int i = 0; i < tempData_Array.Length; i++)
                    {
                        tempData_IntArray[i] = int.Parse(tempData_Array[i].Substring(8, 2));
                    }
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
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                int i = y + x * m_Width;
                m_Sum[x, y] = numberList[i];
            }
        }
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
        texture.Apply();
        targetMaterial.sprite = Sprite.Create(texture, new Rect(0, 0, m_Width, m_Height), Vector2.zero); ;
    }
}