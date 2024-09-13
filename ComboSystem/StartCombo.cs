using XNode;
using UnityEditor;
namespace ComboSystem
{
	[DisallowMultipleNodes]
	[CreateNodeMenu("ComboNode/StartCombo")]
	public class StartCombo : ComboNode
	{
		[Output(typeConstraint = TypeConstraint.Strict)]
		public ComboNode combo;

		// Use this for initialization
		protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return null;
		}
		[InitializeOnLoadMethod]
		static void NodeRegister()
		{
			ComboGraphEdit.RegisterNode(typeof(StartCombo));
		}
	}
}