=== BinSight Application Setup ===

There are several things that you need to setup before you will be able to compile/run the BinSight App.

First let's configure your databse. BinSight app uses two databases. The first database is used for structural
data (such as methods or class) names and calls. The second database is used for binary data, e.g., the content of a single
smali file or an image from the APK file. In fact, APK's themselves are also stored in that DB.

--- Database Configuration and setup ---

1. Download MS SQL database server and install it. MS SQL Express will suffice for most of the research projects, but
   you might need to upgrade if you are planning to process large set of APK files.
2. When installing MS SQL make sure to use mixed authentication mode, and configure a password for sa (that is a super admin).
3. Create two databases: BinSight and BinSightBoc. WARNING: For large sample size, try to put them on separate drives (RAID arrays) 
   for performance improvements.
4. Run recreatedb.bat file from sql/install directory. The batch file accepts 3 positional arguments, DB server name, sa password, and DB Name.
   You have to run the batch file twice, once for BinSight and once for BinSightBoc as follows:
     - recreatedb.bat . p@ssw0rd BinSight
     - recreatedb.bat . p@ssw0rd BinSightBoc
   NOTE: If you want to completely erase and re-create your db from scratch, it is safe to call recreatedb.bat file on an exeisting database.
   It will delete all tables and data and recreate the structure from scratch.

You can now move to the setup and configuration of the Visual Studio Dev environment.
--- Development environment ---

There are several libraries and components that one need to install, in order to be able to run BinSight.
1. Install JRE or JDK. This is required to run apktool.
2. Download apktool.jar and copy it to scripts directory. (https://ibotpeaches.github.io/Apktool/)
   Make sure to rename file to apktool.jar (remove all version info in the file name).
3. Make sure you can run apktool.bat on a given file (you might need to take a look at how-to use apktool). Apktool is used as the main disassembler
   for android application packages. It creates a set of smali files that the BinSight uses for the analysis.
   WARNING: Due to wearing-off logic in SSDs, I would highly recommended using RAM drives for the analsysis part, since disassembling part creates a
   significant amount of IO for the storage.
4. Go to src/BinSight directory.
5. Create a copy of \_App.config file and call it App.Config
6. Modify App.config file and provide details for connection to BinSight and BinSightBoc databases.
7. Provide path for the apktool directory.
8. Specify where you want your apk files to be disassembled to for during import.
9. You might be required to install MS GLEE library. I've included the DLL directly into this repo, but if you need to install it, you can find it here:
   https://www.microsoft.com/en-us/research/project/microsoft-automatic-graph-layout/
10. Open BinSight.sln file with Visual Studio, build and run. By now you should be able to run the app as we were running it during our analysis (presented in
    Asia CCS 2018 Paper: https://dl.acm.org/citation.cfm?id=3196494.3196538

Other notes:
- I am refactoring code, to make database fully optional and to clean up stuff, so stay tuned. I will, however, keep v1\_asiaccs2018 code directory as-is, so
  that results from 2018 can be reproduced.
- If you have a question on how to use it (and you will) send me an email at ildarm at ECE period UBC dot CA. I will try to answer as soon as I can.
- If you have an issue, just create an issue (if it is a bug, please post steps to reproduce it)
- If you happen to use this project for your research, we would like to ask you to reference Asia CCS 2018 paper.
- Have fun hacking :) 
