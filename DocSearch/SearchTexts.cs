using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
/* ***************************************************************************
*  SearchTexts.cs
*  Created as part of the Target Interview Process
*  Created by Brijesh Chawla - March'2020 
*  **************************************************************************/
namespace TargetApp.DocSearch
{
    internal class SearchTexts
    {
        #region Private Variables
        private readonly string _sourceDocsPaths;
        private Dictionary<string, string> _allFilesAndDataDict;

        //Utilizing the Dictionary (from System.Collections.Generic namespace), instead of Hashtable (from System.Collections namespace)
        //Since Dictionary is type safe, hence with Hashtable there could be overhead of type conversion since it stores data of type 'Object'
        //With that, Dictionary gives better performance for high volume of transactions
        private Dictionary<string, Dictionary<string, int>> _allFilesAndDataDictOfDict;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTexts"/> class.
        /// </summary>
        /// <param name="documentsLocation">The documents location.</param>
        public SearchTexts(string documentsLocation)
        {
            _sourceDocsPaths = documentsLocation;
            ReadAndLoadAllFilesDataAsTexts();
            CreateDctionaryOfFilesData();
        }
        #endregion

        #region Private Implementation
        /// <summary>
        /// Reads all files data as texts.
        /// </summary>
        private void ReadAndLoadAllFilesDataAsTexts()
        {
            try
            {
                _allFilesAndDataDict = new Dictionary<string, string>();
                string[] allFiles = Directory.GetFiles(_sourceDocsPaths); //Get all the files from the given folder
                for (int i = 0; i < allFiles.Length; i++)
                {
                    var thisFileData = File.ReadAllText(allFiles[i]);
                    string justFileName = Path.GetFileName(allFiles[i]);
                    _allFilesAndDataDict.Add(justFileName, thisFileData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); //Write the exception in logfile, writing in Console for this app
                //Throw the exception back here since this method is called from Constructor, so that calling method would know object had error while instantiating
                throw;
            }
        }

        /// <summary>
        /// Creates the dctionary of files data.
        /// </summary>
        private void CreateDctionaryOfFilesData()
        {
            try
            {
                var charsToTrim = new char[]
                {
                    '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']',
                    '|', ';', ':', '"', '<', '>', ',', '.', '?', '/', '\\', '\'', '"', ' '
                };
                _allFilesAndDataDictOfDict = new Dictionary<string, Dictionary<string, int>>(); //Instantiate the Parent data dictionary containing Dictionary
                string[] allFiles = Directory.GetFiles(_sourceDocsPaths); //Get all the files from the given folder

                //Loop through all the found files
                for (int i = 0; i < allFiles.Length; i++)
                {
                    string thisFileName = Path.GetFileName(allFiles[i]);
                    var thisFileData = File.ReadAllText(allFiles[i]);
                    thisFileData = thisFileData.Replace('\n', ' '); //Replace all the NewLine character by blanks

                    string[] thisFileWords = thisFileData.Split(' ');

                    var thisFileDataDict = new Dictionary<string, int>();
                    
                    for (int j = 0; j < thisFileWords.Length; j++)
                    {
                        string currentWord = thisFileWords[j].ToLower().Trim(charsToTrim); //Trim all the special characters

                        //Update the Dictionary for this Word
                        int thisKeyValue;
                        bool rc = thisFileDataDict.TryGetValue(currentWord, out thisKeyValue);
                        if (rc)
                            thisFileDataDict[currentWord] = thisKeyValue + 1;
                        else
                            thisFileDataDict.Add(currentWord, 1);
                    }
                    //Add the Data Dictionary with it's respective fileName to the parent Dictionary
                    _allFilesAndDataDictOfDict.Add(thisFileName, thisFileDataDict);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); //Write the exception in logfile, writing in Console for this app
                //Throw the exception back here since this method is called from Constructor, so that calling method would know object had error while instantiating
                throw;
            }
        }
        #endregion

        #region Search Using String
        /// <summary>
        /// Searches the given text through string comparison.
        /// </summary>
        /// <param name="textToSearch">The text to search</param>
        /// <param name="errorMsg">The error message returned by the system</param>
        /// <returns>List of sorted FileEntity objects containing FileName and HitCounts</returns>
        public List<FileEntity> SearchThroughStringSearch(string textToSearch, out string errorMsg)
        {
            errorMsg = string.Empty;
            var fEntityList = new List<FileEntity>();

            try
            {
                //Let's find the text utilizing the data already in memory (not reading here from disk/files everytime)
                foreach (var keyValuePair in _allFilesAndDataDict)
                {
                    string thisFileData = keyValuePair.Value;
                    int indexOf = 0;
                    int startIndex = 0;
                    int hitCount = 0;
                    while (indexOf != -1)
                    {
                        indexOf = thisFileData.IndexOf(textToSearch, startIndex, StringComparison.OrdinalIgnoreCase); //Look for the text
                        if (indexOf > -1)
                        {
                            //If we are not at the very start
                            if (startIndex > 0)
                            {
                                //Let's make sure that previous character from the found text is not an AlphaNumeric
                                //So that we don't confuse with partial match, such as returning True for finding word "eat" whereas this word really is "great"
                                if (!Char.IsLetterOrDigit(thisFileData, indexOf - 1))
                                {
                                    //If there's more data after this
                                    if (thisFileData.Length > indexOf + 1)
                                    {
                                        //Let's make sure that next character from the found text is not an AlphaNumeric
                                        //So that we don't confuse with partial match, such as returning True for finding word "manage" whereas this word really is "manager"
                                        if (!Char.IsLetterOrDigit(thisFileData, indexOf + textToSearch.Length))
                                        {
                                            hitCount += 1;
                                        }
                                    }
                                    else //this was the last word found, add to hitCount
                                    {
                                        hitCount += 1;
                                    }
                                }
                            }
                            else //We are the very start of the data (found data in first call)
                            {
                                //If there's more data after this
                                if (thisFileData.Length > indexOf + textToSearch.Length)
                                {
                                    //Let's make sure that next character from the found text is not an AlphaNumeric
                                    //So that we don't confuse with partial match, such as returning True for finding word "manage" whereas this word really is "manager"
                                    if (!Char.IsLetterOrDigit(thisFileData, indexOf + textToSearch.Length))
                                    {
                                        hitCount += 1;
                                    }
                                }
                                else //this was the last word found, add to hitCount
                                {
                                    hitCount += 1;
                                }
                            }
                            startIndex = indexOf + textToSearch.Length;
                        }
                    }
                    var fEntity = new FileEntity(keyValuePair.Key, hitCount);
                    fEntityList.Add(fEntity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); //Write the exception in logfile, writing in Console for this app
                //Depending on the system workflow, return the error code to outside calling function also (in out parameter)
                errorMsg = ex.ToString();
            }
            fEntityList.Sort((fEnt1, fEnt2) => fEnt2.TextSearchHitCount.CompareTo(fEnt1.TextSearchHitCount));
            return fEntityList;
        }
        #endregion

        #region Search Using Regular Expression
        /// <summary>
        /// Searches the given text through regular expression.
        /// </summary>
        /// <param name="textToSearch">The text to search</param>
        /// <param name="errorMsg">The error message returned by the system</param>
        /// <returns>List of sorted FileEntity objects containing FileName and HitCounts</returns>
        public List<FileEntity> SearchThroughRegularExpression(string textToSearch, out string errorMsg)
        {
            errorMsg = string.Empty;
            var fEntityList = new List<FileEntity>();
            try
            {
                //Let's find the text utilizing the data already in memory (not reading here from disk/files everytime)
                foreach (var keyValuePair in _allFilesAndDataDict)
                {                    
                    var findthis = Regex.Escape(textToSearch);
                    var findText = @"\b" + findthis + @"\b";
                    var matchColl = Regex.Matches(keyValuePair.Value, findText, RegexOptions.IgnoreCase);
                    
                    var fEntity = new FileEntity(keyValuePair.Key, matchColl.Count);
                    fEntityList.Add(fEntity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); //Write the exception in logfile, writing in Console for this app
                //Depending on the system workflow, return the error code to outside calling function also (in out parameter)
                errorMsg = ex.ToString();
            }
            fEntityList.Sort((fEnt1, fEnt2) => fEnt2.TextSearchHitCount.CompareTo(fEnt1.TextSearchHitCount));
            return fEntityList;
        }
        #endregion

        #region Search Using In Memory Dictionary
        /// <summary>
        /// Searches the given text through memory data.
        /// </summary>
        /// <param name="textToSearch">The text to search</param>
        /// <param name="errorMsg">The error message returned by the system</param>
        /// <returns>List of sorted FileEntity objects containing FileName and HitCounts</returns>
        public List<FileEntity> SearchThroughMemoryDataDictionary(string textToSearch, out string errorMsg)
        {
            errorMsg = string.Empty;
            var fEntityList = new List<FileEntity>();
            try
            {
                //Let's find the text utilizing the data already in memory (not reading here from disk/files everytime)
                foreach (var keyValuePair in _allFilesAndDataDictOfDict)
                {
                    int hitCount;
                    bool rc = keyValuePair.Value.TryGetValue(textToSearch, out hitCount);
                    var fEntity = new FileEntity(keyValuePair.Key, hitCount);
                    fEntityList.Add(fEntity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); //Write the exception in logfile, writing in Console for this app
                //Depending on the system workflow, return the error code to outside calling function also (in out parameter)
                errorMsg = ex.ToString();
            }
            fEntityList.Sort((fEnt1, fEnt2) => fEnt2.TextSearchHitCount.CompareTo(fEnt1.TextSearchHitCount));
            return fEntityList;
        }
        #endregion
    }
}
