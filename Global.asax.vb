Imports System.Web.Http
Imports System.Web.Mvc
Imports System.Web.Routing

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Register jQuery ScriptResourceMapping
        ScriptManager.ScriptResourceMapping.AddDefinition("jquery", New ScriptResourceDefinition() With {
        .Path = "~/Scripts/jquery-3.6.0.min.js",
        .DebugPath = "~/Scripts/jquery-3.6.0.js",
        .CdnPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.6.0.min.js",
        .CdnDebugPath = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.6.0.js"
    })
    End Sub
End Class