Partial Class VerifyEmail
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim tokenString As String = Request.QueryString("token")

        If String.IsNullOrEmpty(tokenString) Then
            lblResult.Text = "Invalid verification link."
            Return
        End If

        Dim token As Guid
        If Not Guid.TryParse(tokenString, token) Then
            lblResult.Text = "Invalid verification link."
            Return
        End If

        Using db As New OnlineShopContext()
            Dim user = db.Users.FirstOrDefault(Function(u) u.VerificationToken = token)
            If user Is Nothing Then
                lblResult.Text = "Invalid or expired verification link."
                Return
            End If

            user.IsEmailVerified = True
            user.VerificationToken = Nothing
            db.SaveChanges()

            lblResult.Text = "Email verified successfully! You can now log in."
        End Using
    End Sub

End Class
