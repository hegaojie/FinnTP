using System;
using FinnTorget;
using NSubstitute;
using NUnit.Framework;

namespace MyNotifyIconUnitTests
{
    [TestFixture]
    public class FinnConfigManagerTests
    {
        //protected FinnConfigManager _manager;

        [SetUp]
        public void Arrange()
        {
            //_manager = Substitute.For<TestFinnConfigManager>();
            //_manager = new TestFinnConfigManager("");
        }

        [TestFixture]
        public class Given_no_config_file : FinnConfigManagerTests
        {
            protected TestFinnConfigManagerWithoutFile _manager;

            [SetUp]
            public void Arrange()
            {
                _manager = new TestFinnConfigManagerWithoutFile("", FinnConfigManager.DEFAULT_INTERVAL);
            }

            [TestFixture]
            public class When_load_settings : Given_no_config_file
            {
                private FinnConfig _config;

                [SetUp]
                public void Act()
                {
                    _config = _manager.LoadConfiguration();
                }

                [Test]
                public void error_message_box_pops_up()
                {
                    Assert.AreEqual(_manager.ErrorMessage, FinnConfigManager.LOAD_ERROR_MESSAGE);
                }

                [Test]
                public void interval_equal_to_30_seconds_by_default()
                {
                    Assert.AreEqual(_config.Interval, FinnConfigManager.DEFAULT_INTERVAL);
                }

                [Test]
                public void start_time_equal_to_now()
                {
                    Assert.AreEqual(DateTime.Now.ToLongTimeString(), _config.StartTime.ToLongTimeString());
                }
            }

            [TestFixture]
            public class When_save_settings : Given_no_config_file
            {
                [SetUp]
                public void Act()
                {
                    _manager.SaveConfiguration(new FinnConfig());
                }

                [Test]
                public void error_message_pops_up()
                {
                    Assert.AreEqual(_manager.ErrorMessage, FinnConfigManager.SAVE_ERROR_MESSAGE);
                }
            }
        }

        [TestFixture]
        public class Given_config_file : FinnConfigManagerTests
        {
            protected TestFinnConfigManagerWithFile _manager;

            [SetUp]
            public void Arrange()
            {
                _manager = new TestFinnConfigManagerWithFile(FinnConfigManager.DEFAULT_CONFIG_FILE, FinnConfigManager.DEFAULT_INTERVAL);
            }

            [TestFixture]
            public class When_load_settings : Given_config_file
            {
                protected FinnConfig _config;

                [SetUp]
                public void Act()
                {
                    _config = _manager.LoadConfiguration();
                }

                [Test]
                public void no_error_message_pops_up()
                {
                    Assert.IsTrue(String.IsNullOrEmpty(_manager.ErrorMessage));
                }

                [Test]
                public void start_time_is_loaded()
                {
                    Assert.AreEqual(new DateTime(), _config.StartTime);
                }

                [Test]
                public void interval_is_loaded()
                {
                    Assert.AreEqual(0, _config.Interval);
                }
            }

            [TestFixture]
            public class When_save_settings : Given_config_file
            {
                [SetUp]
                public void Act()
                {
                    _manager.SaveConfiguration(new FinnConfig());
                }

                [Test]
                public void no_error_message_pops_up()
                {
                    Assert.IsTrue(String.IsNullOrEmpty(_manager.ErrorMessage));
                }
            }
        }
    }

    public class TestFinnConfigManagerWithoutFile : FinnConfigManager
    {
        public TestFinnConfigManagerWithoutFile(string file, double interval) : base(file, interval)
        {
        }

        public string ErrorMessage { get; set; }

        public override void ShowMessageBox(string message)
        {
            ErrorMessage = message;
        }

        protected override IStreamReader CreateReader(string file)
        {
            throw new Exception();
        }

        protected override IStreamWriter CreateWriter(string file)
        {
            throw new Exception();
        }
    }

    public class TestFinnConfigManagerWithFile : FinnConfigManager
    {
        public TestFinnConfigManagerWithFile(string file, double interval) : base(file, interval)
        {
        }

        public string ErrorMessage { get; set; }

        public override void ShowMessageBox(string message)
        {
            ErrorMessage = message;
        }

        protected override IStreamReader CreateReader(string file)
        {
            return new FakeStreamReader();
        }

        protected override IStreamWriter CreateWriter(string file)
        {
            return new FakeStreamWriter();
        }
    }

    public class FakeStreamWriter : IStreamWriter
    {
        public void Dispose()
        {
        }

        public void WriteLine(string line)
        {
        }
    }

    public class FakeStreamReader : IStreamReader
    {
        public void Dispose()
        {
        }

        public string ReadLine()
        {
            return "";
        }
    }
}
