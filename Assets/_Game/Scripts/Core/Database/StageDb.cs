using Ibit.Core.Util;
using Ibit.Plataform.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ibit.Core.Database
{
    public class StageDb
    {
        public static StageDb Instance = new StageDb();

        private static readonly string _stagesPath = Application.streamingAssetsPath + @"/Stages/";

        private StageDb()
        {
            Instance = this;
            StageList = new List<StageInfo>();
        }

        /// <summary>
        /// A list with all the stages avaiable.
        /// </summary>
        public List<StageInfo> StageList { get; }

        /// <summary>
        /// Gets stage data from stage list.
        /// </summary>
        /// <param name="id">Stage Id</param>
        /// <returns></returns>
        public StageInfo GetStage(int id) => StageList.Find(x => x.Id == id);

        /// <summary>
        /// Load stages from csv files.
        /// </summary>
        public void LoadStages()
        {
            StageList.Clear();

            var files = Directory.GetFiles(_stagesPath);

            foreach (var file in files)
            {
                var info = new FileInfo(file);

                if (info.Name.Contains("Demo") || !info.Name.EndsWith(".csv"))
                    continue;

                StageList.Add(LoadStageFromFile(info.Name));
            }

            Debug.Log($"{StageList.Count} stages loaded.");
        }

        /// <summary>
        /// Loads stage from a csv file.
        /// </summary>
        /// <param name="filename">File to be loaded.</param>
        /// <returns></returns>
        public static StageInfo LoadStageFromFile(string filename)
        {
            var path = _stagesPath + filename;

            if (!File.Exists(path))
                throw new FileNotFoundException($"Stage file '{path}' not found!");

            var data = FileManager.ReadAllLines(path);

            CleanData(ref data);

            var stageHeader = $"{data[0]}\n{data[1]}";
            var grid = CsvParser2.Parse(stageHeader);

            var stageInfo = new StageInfo
            {
                //header
                Id = int.Parse(grid[1][0]),
                Phase = int.Parse(grid[1][1]),
                Level = int.Parse(grid[1][2]),
                ObjectSpeedFactor = Parsers.Float(grid[1][3]),
                HeightIncrement = Parsers.Float(grid[1][4]),
                HeightUpThreshold = int.Parse(grid[1][5]),
                HeightDownThreshold = int.Parse(grid[1][6]),
                SizeIncrement = Parsers.Float(grid[1][7]),
                SizeUpThreshold = int.Parse(grid[1][8]),
                SizeDownThreshold = int.Parse(grid[1][9]),
                Loops = int.Parse(grid[1][10]),
            };

            var template = "ObjectType;DifficultyFactor;PositionYFactor;PositionXSpacing";

            // Read each line from the file to get objects to spawn
            for (int i = 2; i < data.Length; i++)
            {
                if (data[i].StartsWith("%") || string.IsNullOrEmpty(data[i]))
                    continue;

                var objDataGrid = CsvParser2.Parse($"{template}\n{data[i]}");

                var obj = new StageObject
                {
                    Id = i,
                    Type = (StageObjectType)Enum.Parse(typeof(StageObjectType), (objDataGrid[1][0])),
                    DifficultyFactor = Parsers.Float(objDataGrid[1][1]),
                    PositionYFactor = Parsers.Float(objDataGrid[1][2]),
                    PositionXSpacing = Parsers.Float(objDataGrid[1][3]),
                };

                stageInfo.StageObjects.Add(obj);
            }

            return stageInfo;
        }

        /// <summary>
        /// Regex Patterns to be cleaned on CleanData() call.
        /// </summary>
        private static readonly string[] _patternsToBeCleaned =
        {
            ";;"
        };

        /// <summary>
        /// Cleans all excel unnecessary character / pattern in the file.
        /// </summary>
        /// <param name="data">Array data from the stage file.</param>
        private static void CleanData(ref string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                foreach (var pattern in _patternsToBeCleaned)
                {
                    data[i] = Regex.Replace(data[i], pattern, "");
                }
            }
        }
    }
}