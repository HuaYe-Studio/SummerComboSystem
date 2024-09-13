using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;
namespace ComboSystem
{
	public enum MouseInputType
	{
		Left = 0,
		Right = 1,
	}
	public abstract class ComboNode : Node
	{
		// public Combo _Combo { get; set; }
		public int ComboId;
		public List<MouseInputType> MouseInput = new List<MouseInputType>();
		public List<KeyCode> KeyBoardInput = new List<KeyCode>();
		public ComboCmd[] comboCmd { get; set; }
		[SerializeField] protected float HoldTime = -1;
		[SerializeField] protected float LimitTime = .2f;
		// Use this for initialization
		protected override void Init()
		{
			base.Init();
		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return null; // Replace this
		}

		public virtual void Initiation()
		{
			comboCmd = new ComboCmd[]
	{
			new ComboCmd()
			{
				Conditional = () => Conditional(),
				DeltaTime = () => Time.deltaTime,
				HoldTime = this.HoldTime,
				LimitTime = this.LimitTime
			}
	};
		}
		protected bool Conditional()
		{
			bool KeyBoard = false;
			bool Mouse = false;
			if (MouseInput.Count > 0)
			{
				if (MouseInput.All(a => Input.GetMouseButton((int)a)))
					Mouse = true;
				else
					Mouse = false;
			}
			else
				Mouse = true;
			if (KeyBoardInput.Count > 0)
			{
				if (KeyBoardInput.All(a => Input.GetKey(a)))
					KeyBoard = true;
				else
					KeyBoard = false;
			}
			else
				KeyBoard = true;
			return Mouse && KeyBoard;
		}
	}
}