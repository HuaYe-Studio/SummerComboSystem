using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
namespace ComboSystem
{
    public class ComboControl : MonoBehaviour
    {
        [SerializeField] private NodeGraph ComboGraph;
        private ComboNode CurrentNode;
        private List<Coroutine> comboCacheList = new List<Coroutine>();
        public CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationToken token;
        ComboTimeLine comboTimeLine;
        Dictionary<string, object> uDataDic = new();
        private void Start()
        {
            Init(new ComboTimeLine());
        }
        private void Update()
        {
            comboTimeLine.OnUpdate(Time.deltaTime);
        }

        public void Init(ComboTimeLine comboTimeLine)
        {
            ActionOnCancel();
            ResetCombo();
            this.comboTimeLine = comboTimeLine;
            comboTimeLine.uDataDic = uDataDic;
        }
        public void RegAddCombo(Coroutine combo)
        {
            comboCacheList.Add(combo);
        }
        public void ClearCache()
        {
            foreach (Coroutine enumerator in comboCacheList)
            {
                StopCoroutine(enumerator);
            }
            comboCacheList.Clear();
        }
        void ResetCombo()
        {
            ClearCache();
            foreach (var node in ComboGraph.nodes.Where(a => a.GetType() == typeof(StartCombo)))
            {
                StartCombo startCombo = node as StartCombo;
                RegisterCombo(startCombo);
                break;
            }
        }
        public void RegisterCombo(ComboNode node)
        {
            var misson = StartCoroutine(WaitComboInput(node));
            RegAddCombo(misson);
        }
        public async Task WaitComboComplete()
        {
            foreach (var port in CurrentNode.Outputs)
                foreach (var connection in port.GetConnections())
                {
                    RegisterCombo(connection.node as ComboNode);
                }
            try
            {
                await Task.Delay((int)(0.5 * 1000), cancellationToken: token);
                ResetCombo();
            }
            catch (TaskCanceledException)
            {

            }
        }
        IEnumerator WaitComboInput(ComboNode node)
        {
            node.Initiation();
            yield return new WaitForComboInput(node.comboCmd);
            tokenSource.Cancel();
            CurrentNode = node;
            ChangeuData("comboId",node.ComboId);
            ClearCache();
            comboTimeLine.StartCombo(this, node.ComboId, () => WaitComboComplete());
        }
        void ActionOnCancel()
        {
            tokenSource.Dispose();
            tokenSource = new();
            token = tokenSource.Token;
            token.Register(ActionOnCancel);
        }
        public void ChangeuData(string fieldName, object fieldValue)
        {
            if (uDataDic.ContainsKey(fieldName))
                uDataDic[fieldName] = fieldValue;
            else
                uDataDic.Add(fieldName, fieldValue);
        }
    }
}
