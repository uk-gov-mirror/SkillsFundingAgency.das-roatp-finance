using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.RoatpFinance.Web.Tests;
public class MockedControllerContext
{
    public static ControllerContext Setup()
    {
        return Setup(string.Empty);
    }

    public static ControllerContext Setup(string buttonToAdd)
    {
        var user = MockedUser.Setup();

        var controllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
        if (!string.IsNullOrEmpty(buttonToAdd))
        {
            var clarificationFileName = "file.pdf";
            var file = new FormFile(new MemoryStream(), 0, 0, clarificationFileName, clarificationFileName){Headers = new HeaderDictionary()};
            file.ContentType = "application/json";
            var formFileCollection = new FormFileCollection { file };
            var dictionary = new Dictionary<string, StringValues>();
            dictionary.Add(buttonToAdd, clarificationFileName);
            controllerContext.HttpContext.Request.Form = new FormCollection(dictionary, formFileCollection);
        }
        else
        {
            controllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());
        }

        return controllerContext;
    }
}
