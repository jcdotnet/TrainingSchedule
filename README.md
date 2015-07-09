# TRAINING SCHEDULE CLASS LIBRARY

This .NET framework class library provides some components in order to represent graphically training seasons, cycles and sessions in your sports training application. You only have to reference the library and drag the component you need.

I highly recommend that you take a look at the “Test” project in order to learn how the components use your training items and the way they work. 

The user interface is displayed in English unless your operating system’s language is Spanish from Spain. You can check or set your language in “Control Panel” -> “Region and language”. 

The library contains a calendar which allow to insert, edit or delete training seasons, cycles and sessions. We use the TrainingItem class to create them.

Only valid cycles can be inserted: When inserting, if the cycle is not valid, it will be removed and therefore won’t be displayed. When editing, if the new cycle dates are no valid, the cycle will remain unchanged. 

In order to make the navigation faster and easier, days which contain a training session are highlighted in the month calendars.

Some of the calendar properties you should know are:
Items: Gets the list where you can add, remove or delete training seasons, cycles and sessions.
SeasonMinDuration, MacrocycleMinDuration, MesocycleMinDuration, MicrocycleMinDuration: Gets or sets the minimum duration allowed for each training cycle. 

The library also contains a Gantt chart, which shows all the cycles belonging to the running season.

For more information about properties and events, I suggest you again to take a look at the “Test” project which is included in the solution.

Hope this library can be useful for you!
