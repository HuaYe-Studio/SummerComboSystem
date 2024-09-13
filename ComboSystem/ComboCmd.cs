using System;
namespace ComboSystem
{
[Serializable]
public struct ComboCmd 
{
    const sbyte COUNTDOWN_INVALID = -1;
    const sbyte STATE_SUCCESS = 1;
    const sbyte STATE_FAILURE = -1;
    const sbyte STATE_WAIT = 0;
    float mLimitTime;
    float mHoldTime;
    public float LimitTime { get;set; }
    public float HoldTime { get; set; }
    public Func<bool> Conditional {  get; set; }
    public Func<float> DeltaTime { get; set; }
    public ComboCmd ReSet()
    {
        mLimitTime = LimitTime;
        mHoldTime = HoldTime;
        return this;
    }
    public ComboCmd Tick(out sbyte state)
    {
        state = STATE_WAIT;
        if (Conditional())
        {
            if (mHoldTime > 0)
                mHoldTime -= DeltaTime();
            else
                state = STATE_SUCCESS;
        }
        if(state != STATE_SUCCESS && mLimitTime > COUNTDOWN_INVALID)
        {
            mLimitTime -= DeltaTime();
            if(mLimitTime <= 0)
            {
                state = STATE_FAILURE;
                mLimitTime = 0;
            }
        }
        return this;
    }
}
}
