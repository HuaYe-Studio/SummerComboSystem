using System;
using System.Collections.Generic;
using XNodeEditor;
namespace ComboSystem
{
    [CustomNodeGraphEditor(typeof(ComboGraph))]
    public class ComboGraphEdit : NodeGraphEditor
    {
        static List<Type> types = new();
        public override string GetNodeMenuName(Type type)
        {
            if (!types.Contains(type))
                return null;
            return base.GetNodeMenuName(type);
        }
        public static void RegisterNode(Type type)
        {
            types.Add(type);
        }
    }
}
