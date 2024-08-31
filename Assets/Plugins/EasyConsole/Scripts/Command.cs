using System;
using System.Reflection;

namespace SA.EasyConsole
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class Command : Attribute
    {
        public readonly string Path;

        public Command()
        {
            
        }

        public Command(string path)
        {
            Path = path;
        }
    }

    public class CommandObject
    {
        public string Path { get; set; }
        public CommandType Type { get; set; }
        public MethodInfo Method { get; set; }
        public FieldInfo Field { get; set; }
        public PropertyInfo Property { get; set; }
        public object Instance { get; set; }

        public CommandObject(MethodInfo method, object instance = null) => CreateObject(method, instance);
        public CommandObject(FieldInfo field, object instance = null) => CreateObject(field, instance);
        public CommandObject(PropertyInfo property, object instance = null) => CreateObject(property, instance);

        public CommandObject(string actionName, object instance)
        {
            var flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            
            var type = instance.GetType();
            var method = type.GetMethod(actionName, flags);

            if (method != null)
            {
                CreateObject(method, instance);
                return;
            }

            var field = type.GetField(actionName, flags);

            if (field != null)
            {
                CreateObject(field, instance);
                return;
            }
            
            CreateObject(type.GetProperty(actionName, flags), instance);
        }

        private void CreateObject(MethodInfo method, object instance = null)
        {
            var attribute = method.GetCustomAttribute<Command>();
            if(attribute != null)
                Path = attribute.Path ?? method.Name;
            
            Method = method;
            Instance = instance;
            Type = CommandType.Method;
        }
        
        private void CreateObject(FieldInfo field, object instance = null)
        {
            var attribute = field.GetCustomAttribute<Command>();
            if(attribute != null)
                Path = attribute.Path ?? field.Name;
            
            Field = field;
            Instance = instance;
            Type = CommandType.Field;
        }
        
        private void CreateObject(PropertyInfo property, object instance = null)
        {
            var attribute = property.GetCustomAttribute<Command>();
            if(attribute != null)
                Path = attribute.Path ?? property.Name;
            
            Property = property;
            Instance = instance;
            Type = CommandType.Property;
        }
        
        public enum CommandType
        {
            Method, 
            Field,
            Property
        }
    }
}