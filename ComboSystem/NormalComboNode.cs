using UnityEngine;
using XNode;
using UnityEditor;
namespace ComboSystem
{
	[CreateNodeMenu("ComboNode/NormalComboNode")]
	public class NormalComboNode : ComboNode
	{
		[Input(typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
		public ComboNode Fromcombo;
		[Output(typeConstraint = TypeConstraint.Strict)]
		public ComboNode Tocombo;
		protected override void Init()
		{
			base.Init();

		}

		public override object GetValue(NodePort port)
		{
			switch (port.fieldName)
			{
				default: return base.GetValue(port);
			}
		}
		[InitializeOnLoadMethod]
		static void NodeRegister()
		{
			ComboGraphEdit.RegisterNode(typeof(NormalComboNode));
		}
	}
}