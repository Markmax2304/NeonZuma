using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

public class Randomizer
{
    #region Fields
    private static Contexts _contexts;
    private static readonly ColorInfo[] colorsInfo;
    private readonly int minLengthSeries;
    private readonly int maxLengthSeries;

    private int currentSeries;
    private ColorBall currentColor;
    private static ColorBall[] existedColors;
    #endregion

    #region Constructors
    static Randomizer()
    {
        _contexts = Contexts.sharedInstance;
        colorsInfo = _contexts.global.levelConfig.value.colors;

        existedColors = new ColorBall[colorsInfo.Length];
    }

    public Randomizer(int minLength, int maxLength)
    {
        minLengthSeries = minLength;
        maxLengthSeries = maxLength;
    }
    #endregion

    #region Public Methods
    public ColorBall GetRandomColorType()
    {
        if(currentSeries == 0)
            RandomNewSeries();

        currentSeries--;
        return currentColor;
    }

    public static ColorBall GetSingleColor()
    {
        // TODO: maybe change to more compicated logic
        var colors = _contexts.global.ballColors.value;
        int count = 0;
        foreach(var color in colors.Keys)
        {
            existedColors[count++] = color;
        }

        if (count > 0)
        {
            return existedColors[Random.Range(0, count)];
        }
        else
        {
            return GetRandomSingleColor();
        }
    }

    public static ColorBall GetRandomSingleColor()
    {
        return colorsInfo[Random.Range(0, colorsInfo.Length)].type;
    }

    public static Color ConvertToColor(ColorBall colorType)
    {
        foreach (var info in colorsInfo)
        {
            if (info.type == colorType)
            {
                return info.color;
            }
        }

#if UNITY_EDITOR
        Debug.Log($"Color type({colorType}) is not found in colors collection");
#endif
        return Color.white;
    }
    #endregion

    #region Private Methods
    private void RandomNewSeries()
    {
        // TODO: In future, change choosing logic to more complicated (based on existing colors and its quantity)
        int newColorIndex = (int)currentColor;
        do
        {
            newColorIndex = Random.Range(0, colorsInfo.Length);
        }
        while (newColorIndex == (int)currentColor);

        currentColor = (ColorBall)newColorIndex;
        currentSeries = Random.Range(minLengthSeries, maxLengthSeries + 1);
    }
    #endregion
}
