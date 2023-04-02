 -----------------FIRST BUILD THE LIBRARY-----------------------

Download/clone this library somewhere on your pc.
Open the library and build to generate bin/obj folders and create .dll
Open the solution containing the project you are using Selenium for.





-----------------TO ADD INTO EXISTING PROJECT--------------



1. Right Click
2. Add Project Reference 
3. Browse
4. Navigate to folder containing the class library you downloaded
5.(Local Path to Where You Put Folder)\SeleniumUpdateLibrary\bin\Debug\netstandard2.0)
6.Choose SeleniumUpdateLibrary.dll
7. The project reference should show up now.


--------------------TO USE---------------------------


1. Add using statement at top level - using SeleniumUpdateLibrary;
2. You will now be able to use methods and definitions in the class library within it's class of SeleniumHelper
3. The main method you will want to use is - SeleniumHelper.UpdateDriver();
(this will check your current version, the latest version, and if they don't match, it will delete current and download/install the latest)
4. Include these within your code.

SeleniumHelper.UpdateDriver();
Console.WriteLine("Press Enter to exit...");
Console.ReadLine();

5. The reason for the ReadLine and WriteLine is for testing purposes, once it updates your driver, it will open a browser, writes some text into the search bar, and submits the search.
This will effectively pause the program while you confirm it is working, and then you should be able to continue.




------------------TO MODIFY OR REMOVE TEST-------------------------

    Once you can confirm this program fixes your issue, you will most likely want to remove the test from it as it is quite intrusive.
    
    To do this --

    1. Open class library project solution, In the SeleniumHelper.cs Class - comment or remove line 138 - await TestSel();
    2. Save, build or rebuild.
    Now you can include SeleniumHelper.UpdateDriver() to any project you use Selenium with and it can check driver version and update if necessary
    This will no longer bring up the browser, but will still show the console output of driver versioning.
