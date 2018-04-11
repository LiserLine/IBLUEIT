using Ibit.Core.Util;
using Ibit.Plataform.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ibit.Core.Database
{
    public class StageDb
    {
        public static StageDb Instance = new StageDb();

        private StageDb()
        {
            Instance = this;
            StageList = new List<StageInfo>();
            //LoadStages();
        }

        public bool IsLoaded { get; private set; }
        public List<StageInfo> StageList { get; }
        public StageInfo GetStage(int id) => StageList.Find(x => x.Id == id);

        public void LoadStages()
        {
            var files = Directory.GetFiles(Application.streamingAssetsPath + @"/Stages/");

            foreach (var file in files)
                StageList.Add(Load(file));

            IsLoaded = true;
            Debug.Log($"{StageList.Count} stages loaded.");
        }

        public static StageInfo Load(string filename)
        {
            var path = Application.streamingAssetsPath + @"/Stages/" + filename + ".csv";

            if (!File.Exists(path))
                throw new FileNotFoundException($"Stage file '{path}' not found!");

            var data = FileManager.ReadAllLines(path);

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
    }
}