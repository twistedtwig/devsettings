# devsettings
Developer settings for .Net

The principle idea of the application is to allow multiple developers in a team share a configuration file and avoid merge conflicts when their local environments differ.  For example one dev may use SQL express and another use Full SQL.  They would have different connection strings.  Developer settings allows the system to have both of the connection strings (with different names) defined, whilst in the settings section they can identity which connection string name is theirs.

The settings are stored in a custom configuration section in the configuration file.  A global section must be declared.  After that zero to many individual sections can be declared after that.

```xml
-- insert XML example here.
```


In the example above you can see that there is a global section which has three properties.  These are the default values that will be loaded into the application.  Below that there are two overrides, JonSettings and SimonSettings.  In the two override sections only a couple of properties have been declared.  Developer settings will create the declared object type and populate all the variables that match by name from the global section.  If a valid override section is found it will override only the properties it finds.  Below is what would happen in each of the situations:

Here is the settings class it is trying to populate
```C#
settings class here
```

No valid override found
-----------------------
```c#
prop1 val1
prop2 val2
```
JonSettings override found
--------------------------
```c#
prop1 val1
prop2 val3
```
SimonSettings override found
--------------------------
```c#
prop1 val5
prop2 val6
```

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

Environmental Variable Override
-------------------------------

if an environmental variable key is found it will take the value and use this as the field to search for an override name.

Appsetting Override
-------------------------------

The appsetting works similarly, it will look for an appsetting with the given key and use its value as the search for an override name.

Machine Name Override
-------------------------------

If machine name is set as an override it will look for the matching override name for the machine name.

Chaining Override
-------------------------------

Chaining uses all the above to try and find a valid override.  It will use them in order. Environmental Variable, Appsetting then machine name.  The first valid override found will be used, it will not use multiple overrides.


An example situation
--------------------

You have a development team of four, and test environment and live environment.  Two of the developers (Jon and Dave) use SQL Server, the other two use SQL Express.  Jon has his temp directory on the E drive whilst evryone else uses teh default.  The Test and Live environments have their own settings.

There are a number of ways a team can identity how their settings are found.  The whole team / environments have to work the same way.  If an environment variable is used each developer would need to set theirs before their application is started.  The benefit of this is there will not be any merge conflicts and someone can switch between setting profiles quickly.  

Machine name can be used, this works well for the local development team, assuming each developer only uses one machine.  If they use more than one they would need to either name them all the same, or have multiple sections with the same information within them.

The app settings can cause merge issues if people set the key and check their config file in.  The app setting is best used for the test and live enviornments as a configuration transform can be used during the build process to set the appsetting to what ever is required.

Below is an example configuration file described as above.  Below that is the code that is used to load the settings.  As a side note I would recommend wrapping the code below in a service that would abstract out the use of this, incase you wish to change it, also to allow for the service to be used as a singleton if wanted so that the data is only loaded once.


```xml
insert xml config code here
```

```c# 
insert C# code to load the configuration here
```
