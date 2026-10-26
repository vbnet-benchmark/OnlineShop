Imports Microsoft.Owin
Imports Microsoft.Owin.Security.Cookies
Imports Owin
Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        app.UseCookieAuthentication(New CookieAuthenticationOptions With {
            .AuthenticationType = "ApplicationCookie",
            .LoginPath = New PathString("~/shop/Account/Login")
        })
    End Sub
End Class
