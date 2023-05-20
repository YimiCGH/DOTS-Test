using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [FormerlySerializedAs("AverageFpsLable")] public TextMeshProUGUI AverageFpsLabel;
    [FormerlySerializedAs("HighestFpsLable")] public TextMeshProUGUI HighestFpsLabel;
    [FormerlySerializedAs("LowestFpsLable")] public TextMeshProUGUI LowestFpsLabel;
    
    [System.Serializable]
    public class FPSColor
    {
        public Color color;
        public int minimumFPS;
    }
    
    public int frameRange = 60;
    
    //帧率颜色从高到低进行配置
    public FPSColor[] FPSColors = new FPSColor[5];
    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }
    public int AverageFPS { get; private set; }

    private int[] fpsBuffer;
    private int fpsBufferIndex;

    // private static string[] NumberString =
    // {
    //     "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
    //     "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
    //     "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
    //     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
    //     "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
    //     "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
    //     "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
    //     "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
    //     "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
    //     "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
    // };
    private static string[] NumberString;
    private void Start()
    {
        Application.targetFrameRate = 500;
        
        NumberString = new string[500];
        for (int i = 0; i < NumberString.Length; i++)
        {
            NumberString[i] = i.ToString();
        }
    }

    void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }

        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
        {
            InitializeBuffer();
        }

        UpdateBuffer();
        CalculateFPS();

        DisplayFPS(AverageFpsLabel,AverageFPS);
        DisplayFPS(HighestFpsLabel,HighestFPS);
        DisplayFPS(LowestFpsLabel,LowestFPS);
    }

    void DisplayFPS(TextMeshProUGUI label,int fps)
    {
        label.text = NumberString[Mathf.Clamp(fps, 0, 300)];
        //label.text = Mathf.Clamp(fps, 0, 1000).ToString();
        for (int i = 0; i < FPSColors.Length; i++)
        {
            if (fps >= FPSColors[i].minimumFPS)
            {
                label.color = FPSColors[i].color;
                break;
            }
        }
    }


    void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }

    void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        for (int i = 0; i < frameRange; i++)
        {
            int fps = fpsBuffer[i]; 
            sum += fps;

            if (fps > highest)
            {
                highest = fps;
            }

            if (fps < lowest)
            {
                lowest = fps;
            }
        }

        AverageFPS = sum / frameRange;
        HighestFPS = highest;
        LowestFPS = lowest;
    }

}
