using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class CoefficientGenerator
{
    public enum RiskLevel
    {
        Low,
        Medium,
        High
    }
    
    private static readonly Dictionary<RiskLevel, double[]> BaseMultipliers = new Dictionary<RiskLevel, double[]>
    {
        { RiskLevel.Low, new double[] { 0.5, 0.9, 1.0, 1.4, 1.7, 3.5, 7.0, 18.0 } },
        { RiskLevel.Medium, new double[] { 0.4, 0.6, 0.9, 1.5, 2.6, 4.5, 15.0, 35.0 } },
        { RiskLevel.High, new double[] { 0.2, 0.3, 0.6, 1.7, 3.0, 5.0, 25.0, 550.0 } }
    };

    public static List<double> GenerateMultipliers(int lines, RiskLevel risk)
    {
        int multiplierCount = lines + 1;
        double[] baseMultipliers = BaseMultipliers[risk];
        List<double> multipliers = new List<double>(new double[multiplierCount]);

       
        double[] scaledBaseMultipliers = ScaleMultipliers(baseMultipliers, lines);

        // Определяем центр
        bool isOdd = lines % 2 != 0;
        double centerPoint = (multiplierCount - 1) / 2.0;

        for (int i = 0; i < multiplierCount; i++)
        {
            double distanceFromCenter = Math.Abs(i - centerPoint);

            if (isOdd && Math.Abs(distanceFromCenter - 0.5) < 0.01)
            {
                distanceFromCenter = 0;
            }

            double t = distanceFromCenter / (multiplierCount / 2.0);
            multipliers[i] = Interpolate(scaledBaseMultipliers, t);
        }

        return multipliers.Select(x => Math.Round(x, 1)).ToList();
    }

    private static double[] ScaleMultipliers(double[] baseMultipliers, int lines)
    {
        double scaleFactor = lines / 16.0;
        double[] scaled = new double[baseMultipliers.Length];

        for (int i = 0; i < baseMultipliers.Length; i++)
        {
            if (i == baseMultipliers.Length - 1)
            {
                scaled[i] = baseMultipliers[i] * Math.Pow(scaleFactor, 3.2);
            }
            else
            {
                scaled[i] = baseMultipliers[i];
            }
        }
        
        if (scaled.Last() > 1000)
            scaled[scaled.Length - 1] = 1000;

        return scaled;
    }

    private static double Interpolate(double[] baseMultipliers, double t)
    {
        t = Math.Max(0, Math.Min(1, t));
        int baseIndex = (int)Math.Floor(t * (baseMultipliers.Length - 1));
        double frac = (t * (baseMultipliers.Length - 1)) - baseIndex;

        if (baseIndex >= baseMultipliers.Length - 1)
            return baseMultipliers.Last();

        return baseMultipliers[baseIndex] * (1 - frac) + baseMultipliers[baseIndex + 1] * frac;
    }
}