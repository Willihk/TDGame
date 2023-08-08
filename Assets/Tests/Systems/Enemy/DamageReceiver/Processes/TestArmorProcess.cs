using NUnit.Framework;
// using TDGame.Systems.Old_Enemy.DamageReceiver.Processes;
// using TDGame.Systems.Stats;

// namespace Tests.Systems.Enemy.DamageReceiver.Processes
// {
//     [TestFixture]
//     public class TestArmorProcess
//     {
//         [TestCase(10F, 1F, 9F)]
//         [TestCase(10F, 11F, 0F)]
//         [TestCase(99F, 11F, 88F)]
//         public void TestCalculateDamageTaken(float damage, float armor, float expected)
//         {
//             var stat = new Stat(armor);
//
//             float result = ArmorProcess.CalculateDamageTaken(damage, stat);
//             
//             Assert.That(expected, Is.EqualTo(result));
//         }
//     }
// }