using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
/* ***************************************************************************
*  Program.cs
*  Created as part of the Target Interview Process
*  Created by Brijesh Chawla - March'2020 
*  **************************************************************************/
namespace TargetApp.DocSearch
{
    class Program
    {
        private static string _sourceFilesPath = @"C:\Scratch\TargetInterviewApp\TestFiles"; //Hard code this path for this app

        /// <summary>Defines the entry point of the application.</summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            DoWork();
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        private static void DoWork()
        {
            try
            {
                Console.Clear();
                Console.Title = "Target Interview Process App - by Brijesh Chawla";
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("             Welcome to Target App Search Document Utility");
                Console.WriteLine("             Version: " + Assembly.GetExecutingAssembly().GetName().Version);
                Console.WriteLine("             Time Now: " + DateTime.Now);
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                SearchTextUsingVariousMethods();
                RunPerformanceTest();
                
                Console.WriteLine();
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Searches the text using various methods.
        /// </summary>
        private static void SearchTextUsingVariousMethods()
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the search term:");
                string searchTerm = Console.ReadLine();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    Console.WriteLine("Search Term cannot be empty...Enter any key to exit");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Enter the Search Method selection (search is case insensitive for all methods), as follows:");
                Console.WriteLine("Enter 1 for String Match based searching");
                Console.WriteLine("Enter 2 for Regular Expression based searching");
                Console.WriteLine("Enter 3 for Index/Memory based searching");
                Console.WriteLine("Enter anything else to exit the system");
                string selectedOption = Console.ReadLine();

                string errorMsg;
                List<FileEntity> fileEntityList;
                var stopWatch = new Stopwatch();
                var searchTxt = new SearchTexts(_sourceFilesPath); //Hard coding the path for this purpose

                stopWatch.Start();
                switch (selectedOption)
                {
                    case "1":
                        fileEntityList = searchTxt.SearchThroughStringSearch(searchTerm, out errorMsg);
                        break;
                    case "2":
                        fileEntityList = searchTxt.SearchThroughRegularExpression(searchTerm, out errorMsg);
                        break;
                    case "3":
                        fileEntityList = searchTxt.SearchThroughMemoryDataDictionary(searchTerm, out errorMsg);
                        break;
                    default:
                        return;
                }
                stopWatch.Stop();

                if (string.IsNullOrEmpty(errorMsg)) //If no error returned by the method
                {
                    Console.WriteLine();
                    Console.WriteLine("Search Result: ");
                    for (int i = 0; i < fileEntityList.Count; i++)
                    {
                        Console.WriteLine("        " + fileEntityList[i].FileName + " - " + fileEntityList[i].TextSearchHitCount + " matches");
                    }
                    Console.WriteLine("Elapsed Time: " + stopWatch.ElapsedMilliseconds + " ms");
                }
                else
                {
                    Console.WriteLine("Error: " + errorMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Runs the performance test.
        /// </summary>
        private static void RunPerformanceTest()
        {
            try
            {
                string allTheData = string.Empty;
                string[] allFiles = Directory.GetFiles(_sourceFilesPath); //Get all the files from the given folder
                for (int i = 0; i < allFiles.Length; i++)
                {
                    allTheData += File.ReadAllText(allFiles[i]) + " ";
                }
                string[] allTheWords = allTheData.Split(' ');

                Console.WriteLine();
                Console.WriteLine("~~~~~~~~ Performance Testing Mode ~~~~~~~~");
                Console.WriteLine();
                Console.WriteLine("How many times should the searching be performed (enter numeric value)?");
                int loopCount = 0;

                while (true)
                {
                    try
                    {
                        loopCount = Convert.ToInt32(Console.ReadLine());
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Please enter only numeric value...Try again");
                    }
                }
                
                //Use this below to display something to the user as processing takes time...
                int theTenthPortion = 1;
                if (loopCount > 10)
                    theTenthPortion = loopCount / 10;

                Console.WriteLine("Press enter to start testing...");
                Console.ReadLine();

                string errorMsg;
                var stopWatch = new Stopwatch();
                var rndm = new Random();
                var searchTxt = new SearchTexts(_sourceFilesPath);
                
                Console.WriteLine("Starting: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                Console.WriteLine();

                //~~~~~~~~~~~~~~~~~~ Performance using String Search
                stopWatch.Start();
                for (int i = 0; i < loopCount; i++)
                {
                    var wordToSearch = allTheWords[rndm.Next(0, allTheWords.Length - 2)];
                    searchTxt.SearchThroughStringSearch(wordToSearch, out errorMsg);
                    if (i % theTenthPortion == 0)
                        Console.Write(".");
                }
                stopWatch.Stop();
                Console.WriteLine(" Using String Search, total time taken to execute " + loopCount + " searches: " + stopWatch.ElapsedMilliseconds + " ms");

                //~~~~~~~~~~~~~~~~~~ Performance using Regular Expression
                stopWatch.Reset(); //Reset the watch clock to reset the elapsed time
                stopWatch.Start(); //Start it back now
                for (int i = 0; i < loopCount; i++)
                {
                    var wordToSearch = allTheWords[rndm.Next(0, allTheWords.Length - 2)];
                    searchTxt.SearchThroughRegularExpression(wordToSearch, out errorMsg);
                    if (i % theTenthPortion == 0)
                        Console.Write(".");
                }
                stopWatch.Stop();
                Console.WriteLine(" Using Regular Expression, total time taken to execute " + loopCount + " searches: " + stopWatch.ElapsedMilliseconds + " ms");

                //~~~~~~~~~~~~~~~~~~ Performance using PreLoaded Dictionary
                stopWatch.Reset(); //Reset the watch clock to reset the elapsed time
                stopWatch.Start(); //Start it back now
                for (int i = 0; i < loopCount; i++)
                {
                    var wordToSearch = allTheWords[rndm.Next(0, allTheWords.Length - 2)];
                    searchTxt.SearchThroughMemoryDataDictionary(wordToSearch, out errorMsg);
                    if (i % theTenthPortion == 0)
                        Console.Write(".");
                }
                stopWatch.Stop();
                Console.WriteLine(" Using In Memory Dictionary, total time taken to execute " + loopCount + " searches: " + stopWatch.ElapsedMilliseconds + " ms");
                
                Console.WriteLine();
                Console.WriteLine("End: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }
}
