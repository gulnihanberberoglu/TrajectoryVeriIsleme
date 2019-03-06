using System;
using System.Reflection;

namespace TrajectoryVerisiIsleme.WepApi.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}