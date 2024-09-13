using System.Collections;
namespace ComboSystem
{
public class WaitForComboInput : IEnumerator
{
    const sbyte COUNTDOWN_INVALID = -1;
    const sbyte STATE_SUCCESS = 1;
    const sbyte STATE_FAILURE = -1;
    const sbyte STATE_WAIT = 0;
    int mCurrentIndex;
    ComboCmd[] mComboArray;
    public WaitForComboInput(ComboCmd[] comboCmdArray)
    {
        mComboArray = comboCmdArray;
    }
    object IEnumerator.Current { get { return null; } }
    void IEnumerator.Reset()
    {
        mCurrentIndex = 0;
    }
    bool IEnumerator.MoveNext()
    {
        var result = false;
        var state = STATE_FAILURE;
        mCurrentIndex = 0;
        mComboArray[mCurrentIndex] = mComboArray[mCurrentIndex].Tick(out state);
        switch (state)
        {
            case STATE_FAILURE:
                mCurrentIndex = 0;
                for (int i = 0, iMax = mComboArray.Length; i < iMax; i++)
                mComboArray[i] = mComboArray[i].ReSet();
                result = true; 
                break;
            case STATE_SUCCESS:
                mCurrentIndex++;
                if(mCurrentIndex == mComboArray.Length)
                    result = false;
                else
                    result = true;
                break;
            case STATE_WAIT:
                result = true;
                break;
        }
        return result;
    }
}
}
