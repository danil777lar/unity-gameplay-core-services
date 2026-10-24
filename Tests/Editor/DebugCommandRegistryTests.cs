using System.Collections.Generic;
using Larje.Core.Debugging;
using NUnit.Framework;
using UnityEngine;

namespace Larje.Core.Tests
{
    public sealed class DebugCommandRegistryTests
    {
        [Test]
        public void RegistersAttributedMethods()
        {
            DebugCommandRegistry registry = new DebugCommandRegistry();
            registry.Register(new TestProvider());

            Assert.AreEqual(3, registry.Commands.Count);
            Assert.IsNotNull(registry.Find("add"));
        }

        [Test]
        public void ExecutesWithParsedArguments()
        {
            DebugCommandRegistry registry = new DebugCommandRegistry();
            registry.Register(new TestProvider());

            DebugCommandResult result = registry.Execute("Add", new Dictionary<string, string> { { "a", "2" }, { "b", "3" } });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(5, result.ReturnValue);
        }

        [Test]
        public void UsesDefaultValues()
        {
            DebugCommandRegistry registry = new DebugCommandRegistry();
            registry.Register(new TestProvider());

            DebugCommandResult result = registry.Execute("Add", new Dictionary<string, string> { { "a", "2" } });

            Assert.AreEqual(12, result.ReturnValue);
        }

        [Test]
        public void ParsesEnumsAndVectors()
        {
            DebugCommandRegistry registry = new DebugCommandRegistry();
            registry.Register(new TestProvider());

            DebugCommandResult result = registry.Execute("Describe", new Dictionary<string, string> { { "mode", "second" }, { "point", "(1, 2.5, 3)" } });

            Assert.AreEqual("Second (1.00, 2.50, 3.00)", result.Message);
        }

        [Test]
        public void ReportsMissingAndInvalidArguments()
        {
            DebugCommandRegistry registry = new DebugCommandRegistry();
            registry.Register(new TestProvider());

            Assert.IsFalse(registry.Execute("Add", new Dictionary<string, string>()).Success);
            Assert.IsFalse(registry.Execute("Add", new Dictionary<string, string> { { "a", "x" } }).Success);
            Assert.IsFalse(registry.Execute("Missing", null).Success);
        }

        [Test]
        public void UnregisterRemovesCommands()
        {
            DebugCommandRegistry registry = new DebugCommandRegistry();
            TestProvider provider = new TestProvider();
            registry.Register(provider);
            registry.Unregister(provider);

            Assert.AreEqual(0, registry.Commands.Count);
        }

        private sealed class TestProvider : IDebugCommandProvider
        {
            [DebugCommand("Test")]
            public int Add(int a, int b = 10)
            {
                return a + b;
            }

            [DebugCommand("Test")]
            public string Describe(Mode mode, Vector3 point)
            {
                return mode + " " + point.ToString("F2");
            }

            [DebugCommand("Test")]
            public void Throw()
            {
                throw new System.InvalidOperationException("boom");
            }
        }

        private enum Mode
        {
            First,
            Second
        }
    }
}
