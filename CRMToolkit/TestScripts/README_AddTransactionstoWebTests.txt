The _AddTransactionstoWebTests utility will process webtest files and add numbered transactions. The updated webtests will be placed in a "Converted" subdirectory. The files can then be added into Visual Studio and Converted to code to finish the scripting process.

1. Record Test in Fiddler. Add Comments for each action in Fiddler.
2. Export the Fiddler Trace as a Visual Studio WebTest and save the file in the TestScripts folder. The WebTest Name should be begin with a 2 digit number (01-99).
3. Run the _AddTransactionstoWebTests utility once WebTests have been saved in the folder. The test number will be extracted from the test name and appended to the transaction names. The Updated webtests will be in the converted folder.
4. Open the Performance Toolkit Solution and add the web tests to the solution. 