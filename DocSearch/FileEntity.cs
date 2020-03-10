/* ***************************************************************************
*  FileEntity.cs
*  Created as part of the Target Interview Process
*  Created by Brijesh Chawla - March'2020 
*  **************************************************************************/
namespace TargetApp.DocSearch
{
    class FileEntity
    {
        //All the Search Return methods in other classes utilizes List of this FileEntity object 
        //to return back because the output need to be in Sorted (Relevance Hit/Count) format.
        //If sorting wasn't a requirement, a Dictionary would have been simpler implementation and this FileEntity object wouldn't be needed
        //Utilizing List and then sorting it before returning back gives much better performance Vs. sorting the Dictionary through Value

        public int TextSearchHitCount;
        public string FileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEntity"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="textSearchHitCount">The text search hit count.</param>
        public FileEntity(string fileName, int textSearchHitCount)
        {
            FileName = fileName;
            TextSearchHitCount = textSearchHitCount;
        }
    }
}
