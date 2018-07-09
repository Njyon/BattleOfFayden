public class Ability : TimedEffect
{
    private string name;
    private string desc;

    public Ability(string name, string desc, float activeTime, float cooldownTime, bool startActive) :
        base(activeTime, cooldownTime, startActive)
    {
        this.name = name;
        this.desc = desc;
    }

    public string GetName()
    {
        return this.name;
    }
    public string GetDescription()
    {
        return this.desc;
    }
}
