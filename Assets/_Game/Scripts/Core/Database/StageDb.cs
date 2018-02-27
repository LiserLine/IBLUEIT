using Ibit.Core.Util;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Spawn;
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
            StageList = new List<Stage>();
            Load();
        }

        public bool IsLoaded { get; private set; }
        public List<Stage> StageList { get; }

        public Stage GetStage(int id) => StageList.Find(x => x.Id == id);

        public void Load()
        {
            var filePath = Application.streamingAssetsPath + @"/GameSettings/StageList.csv";

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Database file '{filePath}' not found!");

            StageList.Clear();

            var csvData = FileReader.ReadCsv(filePath);
            var grid = CsvParser2.Parse(csvData);

            for (var i = 1; i < grid.Length; i++)
            {
                if (string.IsNullOrEmpty(grid[i][0]))
                    continue;

                var stage = new Stage
                {
                    Id = int.Parse(grid[i][0]),
                    ObjectToSpawn = (ObjectToSpawn)Enum.Parse(typeof(ObjectToSpawn), grid[i][1]),
                    Level = int.Parse(grid[i][2]),
                    SpawnDelay = Parsers.Float(grid[i][3]),
                    GameDifficulty = Parsers.Float(grid[i][4]),
                    ObjectSpeedMultiplier = Parsers.Float(grid[i][5]),
                    HeightIncrement = Parsers.Float(grid[i][6]),
                    HeightLevelUpThreshold = int.Parse(grid[i][7]),
                    HeightLevelDownThreshold = int.Parse(grid[i][8]),
                    SizeIncrement = Parsers.Float(grid[i][9]),
                    SizeLevelUpThreshold = int.Parse(grid[i][10]),
                    SizeLevelDownThreshold = int.Parse(grid[i][11]),
                    RelaxTimeThreshold = int.Parse(grid[i][12]),
                    SpawnDuration = int.Parse(grid[i][13])
                };

                StageList.Add(stage);
            }

            IsLoaded = true;
            Debug.Log($"{StageList.Count} stages loaded.");
        }

#if UNITY_EDITOR

        public static Stage Load(int id)
        {
            var filePath = Application.streamingAssetsPath + @"/GameSettings/StageList.csv";

            if (id < 1)
                throw new Exception("Id must be greater than 0");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Database file '{filePath}' not found!");

            var csvData = FileReader.ReadCsv(filePath);
            var grid = CsvParser2.Parse(csvData);

            if (id > grid.Length - 1)
                throw new Exception($"Id must be <= {grid.Length - 1}");

            if (string.IsNullOrEmpty(grid[id][0]))
                throw new Exception("There is no data to load!");

            return new Stage
            {
                Id = int.Parse(grid[id][0]),
                ObjectToSpawn = (ObjectToSpawn)Enum.Parse(typeof(ObjectToSpawn), grid[id][1]),
                Level = int.Parse(grid[id][2]),
                SpawnDelay = Parsers.Float(grid[id][3]),
                GameDifficulty = Parsers.Float(grid[id][4]),
                ObjectSpeedMultiplier = Parsers.Float(grid[id][5]),
                HeightIncrement = Parsers.Float(grid[id][6]),
                HeightLevelUpThreshold = int.Parse(grid[id][7]),
                HeightLevelDownThreshold = int.Parse(grid[id][8]),
                SizeIncrement = Parsers.Float(grid[id][9]),
                SizeLevelUpThreshold = int.Parse(grid[id][10]),
                SizeLevelDownThreshold = int.Parse(grid[id][11]),
                RelaxTimeThreshold = int.Parse(grid[id][12]),
                SpawnDuration = int.Parse(grid[id][13])
            };
        }

#endif
    }
}