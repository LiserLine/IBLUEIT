using Ibit.Core.Util;
using NUnit.Framework;
using UnityEngine;

public class FlowMathTest
{
    [Test]
    public void TestEquations()
    {
        var diffpress = 1500f;
        Debug.Log("Volumetric Flow Rate: " + FlowMath.ToLitresPerMinute(diffpress) + " L/min");
    }
}
