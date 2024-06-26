﻿using DigitalSignage.PowerPoint;
using DigitalSignage.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace DigitalSignage.Media
{
    internal class MediaManager
    {
        private PowerPointExtractor powerPointExtractor;
        private MediaCollection MediaCollection;
        private DirectoryManager directoryManager;
        private DualWriter dualWriter;

        public MediaManager()
        {
            powerPointExtractor = new PowerPointExtractor();
            MediaCollection = new MediaCollection();
            directoryManager = new DirectoryManager();
            dualWriter = new DualWriter();
        }

        private void GetMediaFromFolder(string folderPath, List<string> mediaList)
        {
            if (Directory.Exists(folderPath))
            {
                // Get all files from the folder and its subfolders
                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    mediaList.Add(file);
                }
            }
        }

        public void LoadFiles()
        {
            try
            {
                //Retrieve folder paths for each media type
                string imagesFolderPath = directoryManager.GetMediaFolderPath("Images");
                string levelImagesFolderPath = directoryManager.GetMediaFolderPath("Level Images");
                string powerpointFolderPath = directoryManager.GetMediaFolderPath("PowerPoint");
                string powerpointImagesFolderPath = directoryManager.GetMediaFolderPath("PowerPoint\\PowerPointImages");
                string videosFolderPath = directoryManager.GetMediaFolderPath("Videos");

                //Retrieve and process media based on the paths provided by pathManager
                GetMediaFromFolder(imagesFolderPath, MediaCollection.Images);
                GetMediaFromFolder(levelImagesFolderPath, MediaCollection.ImageLevels);
                GetMediaFromFolder(powerpointImagesFolderPath, MediaCollection.PowerpointImages);
                powerPointExtractor.ExtractPowerPointSlides(powerpointFolderPath, powerpointImagesFolderPath);
                GetMediaFromFolder(videosFolderPath, MediaCollection.Videos);

                //Create and store PowerPoint files in 2D array
                PowerPointManager powerPointManager = new PowerPointManager();
                GlobalVariables.PowerPointFiles = powerPointManager.StorePowerPointFiles(powerpointImagesFolderPath);

                //Log loaded PPT files for verification
                string horizontalRule = new string('-', 50);
                Console.WriteLine($"{horizontalRule}\nLoaded PPT in Array:\n{horizontalRule}");
                foreach (var folder in GlobalVariables.PowerPointFiles)
                {
                    Console.WriteLine("Folder: " + folder[0]);
                    for (int i = 1; i < folder.Length; i++)
                    {
                        Console.WriteLine("  File: " + folder[i]);
                    }
                }

                //Display loaded media paths in console
                Console.WriteLine($"{horizontalRule}\nLoaded Media:\n{horizontalRule}");
                dualWriter.LogSection("Images", MediaCollection.Images);
                dualWriter.LogSection("Image Levels", MediaCollection.ImageLevels);
                dualWriter.LogSection("Videos", MediaCollection.Videos);

                Console.WriteLine("Files Loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Loading Media: {ex.Message}");
                return;
            }
        }

    }
}
