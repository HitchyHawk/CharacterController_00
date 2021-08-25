[System.Serializable]
public class ToggleType
{
    public bool isActive = false;
    public float value;
    public float defaultValue;
    

    public ToggleType(bool newActive, float newDefaultValue){
        isActive = newActive;
        value = newDefaultValue;
        defaultValue = newDefaultValue;
    }

    public void refresh(){
        value = defaultValue;
    }
    
    public static explicit operator float(ToggleType a) => a.isActive? a.value : a.defaultValue;
    public static explicit operator int(ToggleType a) => (int)(a.isActive? a.value : a.defaultValue);
}
