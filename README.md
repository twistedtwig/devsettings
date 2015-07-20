# devsettings
Developer settings for .Net

The principle idea of the application is to allow multiple developers in a team share a configuration file and avoid merge conflicts when their local environments differ.  For example one dev may use SQL express and another use Full SQL.  They would have different connection strings.  Developer settings allows the system to have both of the connection strings (with different names) and in the settings section they can identity which connection string name is theirs.

The settings are stored in a custom configuration section in the configuration file.  A global section must be declared.  After that zero to many individual sections can be declared after that.


-- insert XML example here.



In the example above you can see that there is a global section which has three properties.  These are the default values that will be loaded into the application.  Below that there are two overrides, JonSettings and SimonSettings.  In the two override sections only a couple of properties have been declared.  Developer settings will create the declared object type and populate all the variables that match by name from the global section.  If a valid override section is found it will override only the properties it finds.  Below is what would happen in each of the situations:

No valid override found
-----------------------

prop1 val1
prop2 val2

JonSettings override found
--------------------------

prop1 val1
prop2 val3

SimonSettings override found
--------------------------

prop1 val5
prop2 val6


There are four ways to identity a individual override settings:

- Environment Variable
- App Setting
- Machine Name
- Chaining 

