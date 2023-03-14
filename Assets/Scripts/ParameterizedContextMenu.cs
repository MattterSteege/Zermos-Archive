using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ParameterizedContextMenu : Attribute
{
    private string[] parameterNames;
    private string itemName;

    public ParameterizedContextMenu(string itemName, params string[] parameterNames)
    {
        this.parameterNames = parameterNames;
        this.itemName = itemName;
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/Component/ParameterizedContextMenu", true)]
    private static bool Validate(MenuCommand menuCommand)
    {
        var target = menuCommand.context;
        var method = target.GetType().GetMethod(target.name);

        if (method != null)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == 0)
            {
                UnityEngine.Debug.LogError($"ParameterizedContextMenu: Method {method.Name} has no parameters, and cannot be invoked through this context menu.");
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                if (!param.HasDefaultValue)
                {
                    UnityEngine.Debug.LogError($"ParameterizedContextMenu: Method {method.Name} has a non-default parameter ({param.Name}), and cannot be invoked through this context menu.");
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    [MenuItem("CONTEXT/Component/ParameterizedContextMenu", false, 0)]
    private static void InvokeMethodWithDefaultParameters(MenuCommand menuCommand)
    {
        var target = menuCommand.context;
        var method = target.GetType().GetMethod(target.name);

        if (method != null)
        {
            var parameters = method.GetParameters();
            var defaultArgs = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                if (param.HasDefaultValue)
                {
                    defaultArgs[i] = param.DefaultValue;
                }
                else
                {
                    UnityEngine.Debug.LogError($"ParameterizedContextMenu: Method {method.Name} has a non-default parameter ({param.Name}), and cannot be invoked through this context menu.");
                    return;
                }
            }

            method.Invoke(target, defaultArgs);
        }
    }

    public string GetMenuPath(UnityEngine.Object target, System.Reflection.MethodInfo methodInfo)
    {
        return itemName + string.Join("/", parameterNames);
    }
#endif
}
