using System.Reflection;

namespace Dhole.AuditLogs.Application.DependencyInjection;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
