using System;
using System.Collections.Generic;
using System.Reflection;
namespace ComboSystem
{
    public class ComboTimeLinePoint
    {
        public MethodInfo ComboFunc;
        public float DoTime;
        public ComboTimeLinePoint(float doTime, MethodInfo comboFunc)
        {
            DoTime = doTime;
            ComboFunc = comboFunc;
        }
    }
    public class ComboTimeLineNode
    {
        public ComboTimeLinePoint timeLinePoint;
        public bool IsDone;
        public float RunTime;
        public ComboTimeLineNode(ComboTimeLinePoint point)
        {
            timeLinePoint = point;
            RunTime = point.DoTime;
            IsDone = false;
        }

    }
    public class ComboTimeLine
    {
        private List<ComboTimeLineNode> timelineNode;
        ComboControl sender;
        public int ComboId;
        public Dictionary<string, object> uDataDic;

        private bool isRunning;
        private Action OnComplete;

        public void Init()
        {
            this.timelineNode = null;
            this.isRunning = false;
            this.ComboId = 0;
            uDataDic = new();
        }

        public bool StartCombo(ComboControl sender, int comboId, Action OnComplete)
        {
            if (this.isRunning)
            {
                return false;
            }

            List<ComboTimeLineNode> timeLine = GM_ComboMgr.Instance.GetNodes(comboId);
            if (timeLine == null)
            {
                return false;
            }

            this.timelineNode = timeLine;
            this.isRunning = true;
            this.OnComplete = OnComplete;

            this.ComboId = comboId;
            return true;
        }

        public void OnUpdate(float dt)
        {
            if (this.isRunning == false)
            {
                return;
            }

            if (this.timelineNode == null)
            {
                this.isRunning = false;
                return;
            }
            bool endFlag = true;
            for (int i = 0; i < this.timelineNode.Count; i++)
            {
                if (this.timelineNode[i].IsDone)
                {
                    continue;
                }
                this.timelineNode[i].RunTime -= dt;
                if (this.timelineNode[i].RunTime <= 0)
                {
                    try
                    {
                        MethodInfo func = this.timelineNode[i].timeLinePoint.ComboFunc;
                        ParameterInfo[] parameterInfos = func.GetParameters();
                        List<object> paraDataList = new();
                        foreach (var info in parameterInfos)
                        {
                            if (uDataDic.ContainsKey(info.Name))
                                paraDataList.Add(uDataDic[info.Name]);
                        }
                        this.timelineNode[i].IsDone = true;
                        this.timelineNode[i].timeLinePoint.ComboFunc.Invoke(null, paraDataList.ToArray());
                    }
                    catch (Exception exception)
                    {
                        this.timelineNode[i].IsDone = true;
                    }
                }

                endFlag = false;
            }

            if (endFlag)
            {
                this.isRunning = false;
                if (this.OnComplete != null)
                {
                    this.OnComplete();
                }
            }
        }
    }
}
