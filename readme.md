## Readme file for CheckConsolidate.sln

Perform the Nuget consolidate check on build server (command line)

Add it to your build by using  the Command Line task



### Use as command line tool in TFS/VSTS build

1. Insert two command line prompt tasks
2. Task 1: Get Consolidate executable (Display name)
3. Tool:  nuget.exe
4. Arguments: install KDISim.CheckConsolidate   -source https://api.nuget.org/v3/index.json -ExcludeVersion  -o "$(Build.SourcesDirectory)\toolspackages"

5. Task 2:  Check nuget consolidate  (Display name)
6. Tool: $(Build.SourcesDirectory)\toolspackages\KDISim.CheckConsolidate\Tools\CheckConsolidate.exe
7. Arguments:   -s   (if you want the short notation) 
8. Advanced/Working folder: $(Build.SourcesDirectory)
9. Advanced/Fail on Standard Error:  true if you want red/yellow builds, false, if you just want a log
10. Control Options:  Continue on error:  true if you want yellow builds, false if you want red and build stops


### Options

-s   Enable short notation,  default is long notation with more information

-r   ReportOnly,  doesn't trigger yellow build in TFS/VSTS builds.  Exit code always = 0.
