using UnityEngine;
using System;
using System.Collections;

public class TimedEffect
{
    private enum TimerState
    {
        Active,
        Cooldown,
        Stopped
    }

    public Action onEffectStart;
    public Action<float> onEffectTick;
    public Action onEffectEnd;
    public Action onCooldownDone;

    private TimerState state;
    private float remainingTime;
    private float activeTime;
    private float coolDownTime;

    public TimedEffect(float activeTime = 10.0f, float coolDownTime = 50.0f, bool startActive = false)
    {
        this.remainingTime = activeTime + coolDownTime;
        this.activeTime = activeTime;
        this.coolDownTime = coolDownTime;
        this.state = (startActive == true ? TimerState.Active : TimerState.Stopped);
    }

    public bool IsActive()
    {
        return (this.state == TimerState.Active);
    }
    public bool IsCooling()
    {
        return (this.state == TimerState.Cooldown);
    }

    public float GetRunningTimePercent()
    {
        if (this.IsActive())
        {
            return ((this.remainingTime - this.coolDownTime) / this.activeTime * 100);
        }
        return 0;
    }
    public float GetCooldownPercent()
    {
        if (this.IsCooling())
        {
            return (this.remainingTime / this.coolDownTime * 100);
        }
        return 0;
    }

    public void Activate()
    {
        if (!this.IsActive() && !this.IsCooling())
            this.state = TimerState.Active;
    }
    public void Update()
    {
        if (this.IsActive() || this.IsCooling())
        {
            // activate the effect
            if (this.remainingTime == (this.activeTime + this.coolDownTime))
                if (this.onEffectStart != null)
                    this.onEffectStart(); // set action

            this.remainingTime -= Time.deltaTime; // decrement time of timer

            if (!this.IsCooling())
            {
                if (this.onEffectTick != null)
                    this.onEffectTick((this.activeTime + this.coolDownTime) - this.remainingTime);
            }

            if (!(this.state == TimerState.Cooldown) &&
                (this.remainingTime <= this.coolDownTime))
            {
                // end effect, activate cooldown
                this.state = TimerState.Cooldown;

                if (this.onEffectEnd != null)
                    this.onEffectEnd(); // reset action

                return;
            }
            if (this.remainingTime <= 0.0f)
            {
                // reset timer
                this.state = TimerState.Stopped;
                this.remainingTime = this.activeTime + this.coolDownTime;

                if (this.onCooldownDone != null)
                    this.onCooldownDone(); // callback for end of cooldown
            }
        }
    }
}
