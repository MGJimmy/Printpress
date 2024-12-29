namespace Printpress.API;


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class SkipResponseWrapperFilterAttribute : Attribute
{
}
