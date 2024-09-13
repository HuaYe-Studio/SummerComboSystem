using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace ComboSystem
{
    public class GM_ComboMgr:MonoBehaviour
    {
        public static GM_ComboMgr Instance = null;
        public Dictionary<int, Dictionary<string, MethodInfo>> FuncModels = new();
        public Dictionary<int, List<ComboTimeLinePoint>> AllComboTimeLine;
        public Dictionary<int, Dictionary<int, object>> Configs;
        ResMgr resMgr = new();
        private void Awake() {
            Init();
            DontDestroyOnLoad(this.gameObject);
        }
        public void Init()
        {
            resMgr.Init();
            Instance = this;
            AllComboTimeLine = new();
            Configs = new();
            ScanAllComboModelOrReadAllConfig();
        }
        private void ScanAllComboModelOrReadAllConfig()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    ComboModel comboModel = types[j].GetCustomAttribute<ComboModel>();
                    if (comboModel != null)
                        ScanOneComboModel(types[j], comboModel);
                    ComboConfig comboConfig = types[j].GetCustomAttribute<ComboConfig>();
                    if (comboConfig != null)
                        ReadOneComboConfig(types[j], comboConfig);
                }
            }

        }
        private void ScanOneComboModel(Type t, ComboModel model)
        {
            MethodInfo[] methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (methods.Length <= 0)
                return;
            for (int i = 0; i < methods.Length; i++)
            {
                ComboProcessor processor = methods[i].GetCustomAttribute<ComboProcessor>();
                if (processor == null)
                    continue;
                int key = model.MainType + processor.SubType;
                Dictionary<string, MethodInfo> comboFuncs;
                if (!FuncModels.ContainsKey(key))
                {
                    comboFuncs = new();
                    FuncModels.Add(key, comboFuncs);
                    comboFuncs.Add(processor.funcName, methods[i]);
                }
                else
                {
                    FuncModels[key].Add(processor.funcName, methods[i]);
                }
            }
        }
        private void ReadOneComboConfig(Type t, ComboConfig comboConfig, string fileName = null)
        {
            if (fileName == null)
                fileName = t.Name;
            string path = "Datas/" + fileName + ".csv";
            string csvText = ResMgr.Instance.LoadAssetSync<TextAsset>(path).text;
            CsvReaderByString reader = new(csvText);
            FieldInfo[] fields = new FieldInfo[reader.ColCount];
            Dictionary<int, object> objDic = new();
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = t.GetField(reader[3, i + 1]);
            }
            for (int i = 4; i < reader.RowCount + 1; i++)
            {
                var configObj = Activator.CreateInstance(t);
                for (int j = 0; j < fields.Length; j++)
                {
                    string csvString = reader[i, j + 1];
                    object setValue = new();
                    switch (fields[j].FieldType.ToString())
                    {
                        case "System.Int32":
                            setValue = int.Parse(csvString);
                            break;
                        case "System.Int64":
                            setValue = long.Parse(csvString);
                            break;
                        case "System.String":
                            setValue = csvString;
                            break;
                        case "System.Single":
                            try
                            {
                                setValue = float.Parse(csvString);
                            }
                            catch (System.Exception)
                            {
                                setValue = 0.0f;
                            }

                            break;
                        default:
                            break;
                    }
                    fields[j].SetValue(configObj, setValue);
                    if (fields[j].Name == "Id")
                        objDic.Add((int)setValue, configObj);
                }
                if (!Configs.ContainsKey(comboConfig.MainType))
                {
                    Configs.Add(comboConfig.MainType, objDic);
                }
                else
                {
                    Configs[comboConfig.MainType] = objDic;
                }
            }
        }
        private object GetConfig(int comboId)
        {
            if (comboId == 0)
                throw new Exception("Id还是别设成零吧");
            int digits = (int)Math.Floor(Math.Log10(comboId));
            int multiplier = (int)Math.Pow(10, digits);
            int mainType = (comboId / multiplier) * multiplier;
            if (Configs.ContainsKey(mainType))
                if (Configs[mainType].ContainsKey(comboId))
                    return Configs[mainType][comboId];
            throw new Exception("没找到Combo信息");
        }
        private MethodInfo GetProcesserFunc(string funcName, Dictionary<string, MethodInfo> funcMap, Dictionary<string, MethodInfo> defaultMap)
        {
            if (funcMap.ContainsKey(funcName))
            {
                return funcMap[funcName];
            }
            else if (defaultMap.ContainsKey(funcName))
            {
                return defaultMap[funcName];
            }
            else
            {
                return null;
            }
        }
        private List<ComboTimeLinePoint> GetPoints(int comboId)
        {
            if (comboId == 0)
                throw new Exception("Id还是别设成零吧");
            if (AllComboTimeLine.ContainsKey(comboId))
                return AllComboTimeLine[comboId];
            int digits = (int)Math.Floor(Math.Log10(comboId));
            int multiplier = (int)Math.Pow(10, digits);
            int mainType = (comboId / multiplier) * multiplier;
            int subType = comboId - mainType;
            int key = mainType - 1;
            Dictionary<string, MethodInfo> funcMap = new();
            Dictionary<string, MethodInfo> defaultMap = new();
            if (FuncModels.ContainsKey(key))
                defaultMap = FuncModels[key];
            if (FuncModels.ContainsKey(comboId))
                funcMap = FuncModels[comboId];
            else
                funcMap = defaultMap;
            if (funcMap == null)
                return null;
            List<ComboTimeLinePoint> points = new();
            var config = GetConfig(comboId);
            Type t = config.GetType();
            FieldInfo field_Time = t.GetField("TimeLine");
            if (field_Time == null)
                return default;
            string timeLine = field_Time.GetValue(config) as string;
            FieldInfo field_Dur = t.GetField("Duration");
            if (field_Dur == null)
                return null;
            float duration = (float)field_Dur.GetValue(config);
            string[] results = timeLine.Split("|");
            points.Add(new ComboTimeLinePoint(0, GetProcesserFunc("Init", funcMap, defaultMap)));
            points.Add(new ComboTimeLinePoint(0, GetProcesserFunc("Begin", funcMap, defaultMap)));
            points.Add(new ComboTimeLinePoint(duration * 0.5f, GetProcesserFunc("Calc", funcMap, defaultMap)));
            points.Add(new ComboTimeLinePoint(duration, GetProcesserFunc("End", funcMap, defaultMap)));
            for (int i = 0; i < results.Length; i += 2)
            {
                switch (results[i])
                {
                    case "Init":
                        {
                            points[0].DoTime = float.Parse(results[i + 1]);
                            break;
                        }
                    case "Begin":
                        {
                            points[1].DoTime = float.Parse(results[i + 1]);
                            break;
                        }
                    case "Calc":
                        {
                            points[2].DoTime = float.Parse(results[i + 1]);
                            break;
                        }
                    case "End":
                        {
                            points[3].DoTime = float.Parse(results[i + 1]);
                            break;
                        }
                    default:
                        {
                            MethodInfo func = GetProcesserFunc(results[i + 0], funcMap, defaultMap);
                            if (func != null)
                            {
                                points.Add(new ComboTimeLinePoint(float.Parse(results[i + 1]), func));
                            }
                            break;
                        }
                }
            }
            AllComboTimeLine.Add(comboId, points);
            return points;
        }
        public List<ComboTimeLineNode> GetNodes(int comboId)
        {
            List<ComboTimeLinePoint> points = GetPoints(comboId);
            List<ComboTimeLineNode> nodes = new();
            for (int i = 0; i < points.Count; i++)
                nodes.Add(new ComboTimeLineNode(points[i]));
            return nodes;
        }

    }
}
