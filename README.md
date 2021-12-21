# Everquest II Collection Utility
This utility gathers all of the collection data from the EQII census and organizes it by category and level. 
When the tool finds a collection reward as an item in another collection, 
it creates a parent / child relationship between the collections.

The utility is entirely dependent upon the EQII census. When the census is having problems, 
the utility will have problems.

The utility can query the census for the items actually collected by a character in the game 
and flag them in the hierarchy. The result is something like the "Main screen" screenshot shown. 
(The green monster icons are placeholders for icon's that are not available in the census.)

![Startup](Eq2Collections/screenshots/Fabled-Kael-Drakkel-2.png)

## The Tree Pane
Top level nodes in the tree are collection categories grouped by level. 
Under those are the names of the collections in that category. 
A collection with children means that the child rewards an item needed by the parent. 
Those items are marked in the parent with an X in the 'Reward Item' column.

Black text in the tree with a check mark means the character has completed the collection. 
Black text without a check mark means the character currently has no items in that collection. 
Blue text means the character has some collection items but the collection is not complete.

Click on any collection to list its items in the Collection Items pane.

__Note:__ For collections prior to `Visions of Vitrovia`, the census contains
reward items for each collection. 
As of this writing, VoV collection census data does not list any rewards.
So, for VoV collections that are missing rewards, 
the utility uses the name of the item in a collection to search
for a collection with that name to establish the parent / child relationship. 
This seems to work reasonably well.

## Collection Items Pane
This pane displays the list of collection items and rewards and their attributes.

Double clicking any item opens u.eq2wire for that item in your default browser.

## What's Missing
Right click any item in the tree, 
choose the 'What's Missing' menu and the tool generates a list of all of the missing items 
in the collection and its children. An example is shown in the `Want List` screenshot shown.

![Missing](Eq2Collections/screenshots/Wantlist-2.png)

The list can be filtered using the "Options" button at the bottom 
(as shown in the screenshot) for example to remove items rewarded by child collections 
since you don't need to go hunting for those.

The list can be copied to the clipboard via the "Copy List" choice. 
The copy contains the item name and the URL for the item on u.eq2wire.com.

## Find Item
Sometimes a collection reward feeds a collection in a different category or at a different level. 
An example is shown in the "Find an item" screenshot where the items are Reward Items 
but the collection has no children. 

![Find](Eq2Collections/screenshots/Find-item-2.png)

The "Find Item" menu can be used to find the source of that reward. 
(Shortcut: select the item and press ctrl-f.) 
Note that in this particular case, that leads to some "dead" collections in the census data.

The "Find" commands only find exact matches in the census data. 
For instance, the tool will not find "moth" but it will find "plain black moth".


# Changes
__Version 1.3.0__: 
* Added special parsing of `Visions of Vetrovia` collections since rewards are currently missing from the census.
* Added update method to the `Help` menu.


# Installation

The utility is a standalone executable. Download the zip file, unzip it, and run the .exe file
as described below:

1. You might want to first read through all the steps since you will be leaving this page.
2. To go to the download page, click the following link: [Releases Page](https://github.com/jeffjl74/Eq2Collections/releases)
3. On the "Releases" page, for the __Latest release__, click the `Assets 3` title to expand the assets (if needed). 
The page with the expanded view will list three files.
4. Click the `Eq2Collections.zip` link, download and save the file. 
	* Pay attention to where the file was downloaded. 
It will typically be in your "Downloads" folder.
In the Chrome browser, you can select _Show in folder_ after the download to open the folder in File Explorer.
5. Extract the `Eq2Collections.exe` file from the `Eq2Collections.zip` file to a folder of your choice.
  (Double-clicking it in File Explorer provides for extraction.)
6. Double-click the `Eq2Collections.exe` file to run the utility.


# Building the utility from source
The utility is built with Microsoft Visual Studio Community 2019. 
The project file is included in the archive.

Daybreak requires a `service ID` for repeated access to the census. 
A service ID is not supplied in this archive. 
To build an executable program, a service ID must be provided.
Start here if you need a service ID: [Daybreak API](https://census.daybreakgames.com/)

The current code looks for the service ID in a `ServiceId` class (the archive project expects file _ServiceId.cs_)

Create this file with your service ID from the following template:

```c-sharp
namespace Eq2Collections
{
    class ServiceId
    {
        public static string service_id = "s:MyServiceID";
    }
}
```