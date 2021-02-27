using System.Collections;
using NUnit.Framework;
using TDGame.Extensions;
using UnityEditor.VersionControl;
using UnityEngine.TestTools;

namespace Tests.Extensions
{
    public class ExtensionsTest
    {
        // A Test behaves as an ordinary method
        [TestCase(10, 0, 100, 10)]
        [TestCase(10, 11, 100, 11)]
        [TestCase(10, 0, 9, 9)]
        public void ClampTest(float value, float min, float max, float expected)
        {
            Assert.AreEqual(expected, value.Clamp(min, max));
        }
    }
}