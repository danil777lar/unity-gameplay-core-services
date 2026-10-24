using System.IO;
using Larje.Core.Saves;
using NUnit.Framework;
using UnityEngine;

namespace Larje.Core.Tests
{
    public sealed class SaveServiceTests
    {
        private string _root;
        private SaveConfig _config;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "larje_save_tests_" + System.Guid.NewGuid().ToString("N"));
            _config = ScriptableObject.CreateInstance<SaveConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }

            Object.DestroyImmediate(_config);
        }

        [Test]
        public void SectionSurvivesSaveAndLoad()
        {
            SaveService first = CreateService();
            first.Get<TestSection>().Coins = 42;
            first.Get<TestSection>().Position = new Vector3(1f, 2f, 3f);
            first.SaveGame();
            first.Dispose();

            SaveService second = CreateService();
            Assert.AreEqual(42, second.Get<TestSection>().Coins);
            Assert.AreEqual(new Vector3(1f, 2f, 3f), second.Get<TestSection>().Position);
            second.Dispose();
        }

        [Test]
        public void UnknownSectionsArePreserved()
        {
            SaveService first = CreateService();
            Assert.IsTrue(first.ImportSection(SaveScope.Game, "foreign", "{\"a\":1}"));
            first.SaveGame();
            first.Dispose();

            SaveService second = CreateService();
            second.Get<TestSection>().Coins = 5;
            second.SaveGame();
            second.Dispose();

            SaveService third = CreateService();
            StringAssert.Contains("\"a\": 1", third.ExportSection(SaveScope.Game, "foreign"));
            third.Dispose();
        }

        [Test]
        public void SystemScopeIsSeparateFromSlots()
        {
            SaveService service = CreateService();
            service.Get<TestSystemSection>().Volume = 0.25f;
            service.SaveSystem();
            service.NewGame("other");
            Assert.AreEqual(0.25f, service.Get<TestSystemSection>().Volume);
            service.Dispose();
        }

        [Test]
        public void EncryptedSaveRoundTrips()
        {
            SetEncryption(_config);
            SaveService first = CreateService();
            first.Get<TestSection>().Coins = 7;
            first.SaveGame();
            first.Dispose();

            string raw = File.ReadAllText(Path.Combine(_root, "slot_default.json"));
            Assert.IsFalse(raw.StartsWith("{"));

            SaveService second = CreateService();
            Assert.AreEqual(7, second.Get<TestSection>().Coins);
            second.Dispose();
        }

        [Test]
        public void GetSavesReturnsMeta()
        {
            SaveService service = CreateService();
            service.SaveGame("a");
            service.SaveGame("b");
            Assert.AreEqual(2, service.GetSaves().Count);
            service.Dispose();
        }

        private SaveService CreateService()
        {
            SaveService service = new SaveService(_config, new FileSaveStorage(_root));
            service.Initialize();
            return service;
        }

        private void SetEncryption(SaveConfig config)
        {
            UnityEditor.SerializedObject serialized = new UnityEditor.SerializedObject(config);
            serialized.FindProperty("encrypt").boolValue = true;
            serialized.FindProperty("aesKey").stringValue = AesUtility.GenerateKey();
            serialized.FindProperty("aesIv").stringValue = AesUtility.GenerateIv();
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [SaveSection("test")]
        private sealed class TestSection
        {
            public int Coins;
            public Vector3 Position;
        }

        [SaveSection("test_system", SaveScope.System)]
        private sealed class TestSystemSection
        {
            [SerializeField] private float volume = 1f;

            public float Volume
            {
                get => volume;
                set => volume = value;
            }
        }
    }
}
