using System;
namespace ComboSystem
{
    public class ComboModel : Attribute
    {
        public int MainType;

        public ComboModel(int sub)
        {
            MainType = sub;

        }
    }
    public class ComboProcessor : Attribute
    {
        public string funcName;
        public int SubType;
        public ComboProcessor(string name, int subType)
        {
            funcName = name;
            SubType = subType;
        }
    }
    public class ComboConfig : Attribute
    {
        public int MainType;
        public ComboConfig(int mainType){
            MainType = mainType;
        }
    }
}
