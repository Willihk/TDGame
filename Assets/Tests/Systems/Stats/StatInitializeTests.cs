using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using TDGame.Extensions;
using TDGame.Systems.Stats;

namespace Tests.Systems.Stats
{
    [TestFixture]
    public class StatInitializeTests
    {
        [TestCase]
        public void TestEmptyInitialize()
        {
            var stat = new Stat();

            Assert.AreEqual(0, stat.Cap);
            Assert.AreEqual(0, stat.BaseValue);
            Assert.AreEqual(0, stat.Value);
            Assert.AreEqual(null, stat.Name);
            Assert.AreEqual(new List<StatModifier>().AsReadOnly(), stat.StatModifiers);
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(92)]
        public void TestInitializeWithBaseValue(float value)
        {
            var stat = new Stat(value);

            Assert.AreEqual(0, stat.Cap);
            Assert.AreEqual(value, stat.BaseValue);
            Assert.AreEqual(value, stat.Value);
            Assert.AreEqual(null, stat.Name);
            Assert.AreEqual(new List<StatModifier>().AsReadOnly(), stat.StatModifiers);
        }

        [TestCase(0,1)]
        [TestCase(2, 211)]
        [TestCase(92, 90)]
        public void TestInitializeWithCapAndBaseValue(float baseValue, float cap)
        {
            var stat = new Stat(baseValue, cap);

            Assert.AreEqual(cap, stat.Cap);
            Assert.AreEqual(baseValue, stat.BaseValue);
            Assert.AreEqual(baseValue.Clamp(0, cap), stat.Value);
            Assert.AreEqual(null, stat.Name);
            Assert.AreEqual(new List<StatModifier>().AsReadOnly(), stat.StatModifiers);
        }
    }
}