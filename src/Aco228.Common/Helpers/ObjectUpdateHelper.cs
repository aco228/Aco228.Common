namespace Aco228.Common.Helpers;

public static class ObjectUpdateHelper
{
    public static void UpdateObjectValue<T>(object objectToUpdate, string parameterName, T? newValue)
    {
        if (newValue == null)
            return;
        
        var type = typeof(T);
        if (!(type.IsPrimitive ||
              type.IsEnum ||
              type == typeof(string) ||
              type == typeof(decimal)))
            return;
        
        var prop = objectToUpdate.GetType().GetProperties().FirstOrDefault(x => x.Name == parameterName);
        if(prop == null)
            return;
        
        T? value = (T?)prop.GetValue(objectToUpdate);
        if(!Equals(value, newValue))
            prop.SetValue(objectToUpdate, newValue);
    }
}