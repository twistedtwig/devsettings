# Developer Settings

The principle idea of the application is to allow multiple developers in a team share a configuration file and avoid merge conflicts when their local environments differ.  For example one dev may use SQL express and another use Full SQL.  They would have different connection strings.  Developer settings allows the system to have both of the connection strings (with different names) defined, whilst in the settings section they can identity which connection string name is theirs.

The settings are stored in a custom configuration section in the configuration file.  A global section must be declared.  After that zero to many individual sections can be declared after that.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>


  <configSections>
    <section name="developersettings" type="CustomConfigurations.ConfigurationSectionLoader, CustomConfigurations"/>
  </configSections>

  <developersettings>
    <Configs>
      <ConfigurationGroup name="global">
        <ValueItems>
          <ValueItem key="ConnectionStringName" value="valueabc"/>
          <ValueItem key="SomeCount" value="5"/>
          <ValueItem key="SomePath" value="C:\temp\mypath"/>
        </ValueItems>
      </ConfigurationGroup>
      
      <ConfigurationGroup name="JonSettings">
        <ValueItems>
          <ValueItem key="SomeCount" value="6" />
        </ValueItems>
      </ConfigurationGroup>

      <ConfigurationGroup name="SimonSettings">
        <ValueItems>
          <ValueItem key="SomeCount" value="7" />
        </ValueItems>
      </ConfigurationGroup>
      
    </Configs>
  </developersettings>
  
</configuration>
```


In the example above you can see that there is a global section which has three properties.  These are the default values that will be loaded into the application.  Below that there are two overrides, JonSettings and SimonSettings.  In the two override sections only a couple of properties have been declared.  Developer settings will create the declared object type and populate all the variables that match by name from the global section.  If a valid override section is found it will override only the properties it finds.  Below is what would happen in each of the situations:

Here is the settings class it is trying to populate
```C#
    public class DevSettings
    {
        public string ConnectionStringName { get; set; }
        public int SomeCount { get; set; }
        public string SomePath { get; set; }
    }
```

Below is what values will be returned in the DevSettings class.

#####No valid override found

| Property	| Value	|
------------| --------|
| ConnectionStringName	| valueabc	|
| SomeCount	| 5	|
| SomePath	| C:\temp\mypath	|

#####JonSettings override found

| Property	| Value	|
------------|---------
| ConnectionStringName	| valueabc	|
| SomeCount	| 6	|
| SomePath	| C:\temp\mypath	|

#####SimonSettings override found

| Property	| Value	|
------------|---------
| ConnectionStringName	| valueabc	|
| SomeCount	| 7	|
| SomePath	| C:\temp\mypath	|


When creating the dev settings configuration loader you can specify a number of parameters:

 - global section name
 - override type
 - override key
  
The global section name is is defaulted to "global".  This is the name of the configurationGroup where all the default values are stored.

The override type defines how the system will look for override values, more of this below.

The override key is the key that defines how the appsetting or environmental variable is found.


There are four ways to identity a individual override settings:

- Environmental Variable
- App Setting
- Machine Name
- Chaining 

#####Environmental Variable Override

if an environmental variable key is found it will take the value and use this as the field to search for an override name.

#####Appsetting Override

The appsetting works similarly, it will look for an appsetting with the given key and use its value as the search for an override name.

#####Machine Name Override

If machine name is set as an override it will look for the matching override name for the machine name.

#####Chaining Override

Chaining uses all the above to try and find a valid override.  It will use them in order. Environmental Variable, Appsetting then machine name.  The first valid override found will be used, it will not use multiple overrides.


##An example situation

You have a development team of four, and test environment and live environment.  Two of the developers (Jon and Dave) use SQL Server, the other two use SQL Express.  Jon has his temp directory on the E drive whilst everyone else uses the default.  The Test and Live environments have their own settings.

There are a number of ways a team can identity how their settings are found.  The whole team / environments have to work the same way.  If an environment variable is used each developer would need to set theirs before their application is started.  The benefit of this is there will not be any merge conflicts and someone can switch between setting profiles quickly.  

Machine name can be used, this works well for the local development team, assuming each developer only uses one machine.  If they use more than one they would need to either name them all the same, or have multiple sections with the same information within them.

The app settings can cause merge issues if people set the key and check their config file in.  The app setting is best used for the test and live enviornments as a configuration transform can be used during the build process to set the appsetting to what ever is required.

In the example above Dave can use all the default values, Jon needs to override his temp path and the other two developers need to use a different connection string.

Below is an example configuration file described as above.  Below that is the code that is used to load the settings.  As a side note I would recommend wrapping the code below in a service that would abstract out the use of this, incase you wish to change it, also to allow for the service to be used as a singleton if wanted so that the data is only loaded once.


```xml
<developersettings>
    <Configs>
      <ConfigurationGroup name="global">
        <ValueItems>
          <ValueItem key="ConnectionStringName" value="FullSql"/>
          <ValueItem key="TempPath" value="C:\temp\mypath"/>
        </ValueItems>
      </ConfigurationGroup>
      
      <ConfigurationGroup name="JonSettings">
        <ValueItems>
          <ValueItem key="TempPath" value="E:\workingDir\temp" />
        </ValueItems>
      </ConfigurationGroup>

      <ConfigurationGroup name="sqlExpress">
        <ValueItems>
          <ValueItem key="ConnectionStringName" value="sqlExpress" />
        </ValueItems>
      </ConfigurationGroup>
      
    </Configs>
  </developersettings>
```

```c# 
    public class DevSettings
    {
        public string ConnectionStringName { get; set; }
        public string TempPath { get; set; }
    }
    
    public class SettingsService 
    {
        public DevSettings Get()
        {
            //By default it will use chaining and try its best to find an override
            var config = new ConfigurationLoader<DevSettings>();
            
            //if no key given you are using machine name only as appsettings and environmental Variable need to know what key to use
            return config.Create(); //a string can be passed into the constructor as a key for either appsettings or environmental variable.
            
            /**
            in the example above if an environmental variable was set for "Devkeys" Jon could set the value to be "JonSettings" to get the temp path override.
            The other two developers could set theirs to "sqlExpress".  
            Dave would not need to as no override is required.
            **/
        }
        
    }

```
