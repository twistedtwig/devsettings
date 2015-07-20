using System;
using System.Configuration;
using ConfigurationSetup;
using FluentAssertions;
using NUnit.Framework;

namespace ConfigurationSetupTests
{
    [TestFixture]
    public class ConfigurationLoaderTests
    {

        [Test]
        public void TestWhenNoSectionGivenWillFindAnyInConfig()
        {
            var config = new ConfigurationLoader<DevSettings>();
            Assert.IsNotNull(config);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIncorrectGlobalSectionNameErrors()
        {
            var config = new ConfigurationLoader<DevSettings>("myglobalsettingsThatIsntThere");
        }

        //test each override type individually: when the override is found and when the override is not found

        [Test]
        public void TestEnvironmentalVariableNotFound()
        {
            const string envVariablesName = "myenvVar123xxfdsafdsa";
            var environmentVariable = Environment.GetEnvironmentVariable(envVariablesName);

            Assert.IsNullOrEmpty(environmentVariable);

            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.EnvironmentalVariable);
            var devSettings = config.Create(envVariablesName);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(5);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }

        [Test]
        public void TestEnvironmentalVariableFound()
        {
            const string envVariablesName = "myenvVar123xxfdsafdsaABBVDA";
            Environment.SetEnvironmentVariable(envVariablesName, "myenvvar");


            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.EnvironmentalVariable);
            var devSettings = config.Create(envVariablesName);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(6);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");

        }

        [Test]
        public void TestAppSettingNotFound()
        {
            const string appsettingKey = "someRandomKey";
            var appSetting = ConfigurationManager.AppSettings[appsettingKey];
            appSetting.Should().BeNull();

            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.AppSettingKey);
            var devSettings = config.Create(appsettingKey);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(5);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }

        [Test]
        public void TestAppSettingFound()
        {
            const string appsettingKey = "myoverridekey";
            var appSetting = ConfigurationManager.AppSettings[appsettingKey];
            appSetting.Should().NotBeNull();

            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.AppSettingKey);
            var devSettings = config.Create(appsettingKey);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(7);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }

        [Test]
        public void TestEmptyAppSettingsDoesNothing()
        {
            const string appsettingKey = "";

            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.AppSettingKey);
            var devSettings = config.Create(appsettingKey);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(5);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }



        //not sure how to test the machine name
        [Test]
        public void TestMachineNameNotFound()
        {
            var machineNameFinder = new FakeMachineNameFinder("bob");
            var appsettings = new FakeAppsettingsLoader("myoverridekey", "jonDevAppSettingTest");

            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.MachineName, machineNameFinder: machineNameFinder, appSettings: appsettings);
            var devSettings = config.Create();

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(5);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }

        [Test]
        public void TestMachineNameFound()
        {
            var machineNameFinder = new FakeMachineNameFinder("jonDevAppSettingTest");
            var appsettings = new FakeAppsettingsLoader("myoverridekey", "jonDevAppSettingTest");

            var config = new ConfigurationLoader<DevSettings>(overrideType: OverrideType.MachineName, machineNameFinder: machineNameFinder, appSettings: appsettings);
            var devSettings = config.Create();

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(7);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }
        
       


        //test chaining: when each setting is found in turn, and when none found.. 4 or so tests.


        [Test]
        public void TestChain_EnvironmentVariableFound()
        {
            //have appsetting and machine name in settings
            var machineNameFinder = new FakeMachineNameFinder("jonDevAppSettingTest");
            var appsettings = new FakeAppsettingsLoader("myoverridekey", "jonDevAppSettingTest");

            var config = new ConfigurationLoader<DevSettings>(machineNameFinder: machineNameFinder, appSettings: appsettings);

            const string envVariablesName = "myoverridekey";
            Environment.SetEnvironmentVariable(envVariablesName, "myenvvar");

            var devSettings = config.Create(envVariablesName);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(6);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }

        [Test]
        public void TestChain_AppsettingFound()
        {
            //no env variable but appsetting and machine name            
            var machineNameFinder = new FakeMachineNameFinder("jonDevAppSettingTest");
            var appsettings = new FakeAppsettingsLoader("myoverridekey", "jonDevAppSettingTest");

            var config = new ConfigurationLoader<DevSettings>(machineNameFinder: machineNameFinder, appSettings: appsettings);

            const string envVariablesName = "myoverridekey";
            Environment.SetEnvironmentVariable(envVariablesName, "myenvvarx");

            var devSettings = config.Create(envVariablesName);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(7);
            devSettings.SomePath.Should().Be(@"C:\temp\mypath");
        }

        [Test]
        public void TestChain_MachineNameFound()
        {
            //machine name only
            var machineNameFinder = new FakeMachineNameFinder("mymachineName");
            var appsettings = new FakeAppsettingsLoader("myoverridekey", "jonDevAppSettingTestXXX");

            const string envVariablesName = "myoverridekey";
            Environment.SetEnvironmentVariable(envVariablesName, "myenvvarx");

            var config = new ConfigurationLoader<DevSettings>(machineNameFinder: machineNameFinder, appSettings: appsettings);            
            var devSettings = config.Create(envVariablesName);

            devSettings.Should().NotBeNull();
            devSettings.ConnectionStringName.Should().Be("valueabc");
            devSettings.SomeCount.Should().Be(5);
            devSettings.SomePath.Should().Be(@"E:\work");
        }



    }
}
