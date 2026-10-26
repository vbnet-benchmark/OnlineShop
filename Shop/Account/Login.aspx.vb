Imports System.Security.Cryptography

Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim email = txtEmail.Text.Trim()
        Dim password = txtPassword.Text
        Dim rememberMe = chkRememberMe.Checked


        Dim passwordHash = ComputeHash(password)

        Using db As New OnlineShopContext()
            Dim user = db.Users.FirstOrDefault(Function(u) u.Email = email AndAlso u.PasswordHash = passwordHash)
            If user Is Nothing Then
                lblMessage.Text = "Invalid email or password."
                Return
            End If

            ' Declare userGuid here
            Dim userGuid As Guid = user.UserID

            ' ==============================
            ' Migrate anonymous data
            ' ==============================

            If Session("AnonUserID") IsNot Nothing Then
                Dim anonId As Guid = CType(Session("AnonUserID"), Guid)

                ' ---- UserProductViews ----
                Dim anonViews = db.UserProductViews.Where(Function(v) v.UserID = anonId).ToList()
                For Each v In anonViews
                    v.UserID = userGuid
                Next

                ' ---- CartItems ----
                Dim anonCart = db.UserCartItems.Where(Function(c) c.UserID = anonId).ToList()
                For Each item In anonCart
                    item.UserID = userGuid
                Next

                db.SaveChanges()
                Session.Remove("AnonUserID")
            End If
            ' Redirect to default page
            'FormsAuthentication.RedirectFromLoginPage(user.Email, chkRememberMe.Checked)
            ' Create a FormsAuthenticationTicket
            Dim ticket As New FormsAuthenticationTicket(
    1,                      ' version
    user.Email,             ' username
    DateTime.Now,           ' issue date
    DateTime.Now.AddHours(2), ' expiration date (2 hours)
    chkRememberMe.Checked,  ' persistent
    ""                      ' user data (optional)
)

            ' Encrypt the ticket
            Dim encryptedTicket As String = FormsAuthentication.Encrypt(ticket)

            ' Create a cookie and add it to the response
            Dim cookie As New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            cookie.HttpOnly = True
            If ticket.IsPersistent Then
                cookie.Expires = ticket.Expiration
            End If
            Response.Cookies.Add(cookie)

            ' Redirect to the originally requested page
            Dim returnUrl As String = Request.QueryString("ReturnUrl")
            If String.IsNullOrEmpty(returnUrl) Then
                returnUrl = "~/Default.aspx"
            End If
            Response.Redirect(returnUrl)


        End Using
    End Sub

    Private Function ComputeHash(input As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes = System.Text.Encoding.UTF8.GetBytes(input)
            Dim hash = sha256.ComputeHash(bytes)
            Return Convert.ToBase64String(hash)
        End Using
    End Function

    Protected Sub btnSendReset_Click(sender As Object, e As EventArgs) Handles btnSendReset.Click
        Dim userEmail As String = txtForgotEmail.Text.Trim()

        If String.IsNullOrEmpty(userEmail) Then
            lblForgotError.Text = "Please enter your email address."
            Return
        End If

        Try
            ' Create reset token (basic example, you should generate a secure GUID + store in DB)
            ' Create reset token
            Dim resetToken As String = Guid.NewGuid().ToString()
            Dim expiry As DateTime = DateTime.Now.AddHours(1)

            ' Save to DB
            Using conn As New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("OnlineShop").ConnectionString)
                conn.Open()
                Dim cmd As New SqlClient.SqlCommand("
        INSERT INTO PasswordResetTokens (TokenId, UserEmail, ExpirationDate, IsUsed)
        VALUES (@TokenId, @UserEmail, @Expiry, 0)", conn)
                cmd.Parameters.AddWithValue("@TokenId", resetToken)
                cmd.Parameters.AddWithValue("@UserEmail", userEmail)
                cmd.Parameters.AddWithValue("@Expiry", expiry)
                cmd.ExecuteNonQuery()
            End Using


            ' Build reset link
            Dim resetLink As String = Request.Url.GetLeftPart(UriPartial.Authority) & "/Shop/Account/ResetPassword.aspx?token=" & resetToken

            ' Save token in DB with expiration (pseudo-code)
            ' SavePasswordResetToken(userEmail, resetToken)

            ' Build email
            Dim mail As New Net.Mail.MailMessage()
            mail.To.Add(userEmail)
            mail.From = New Net.Mail.MailAddress("no-reply@yourshop.com")
            mail.Subject = "Password Reset Request"
            mail.Body = "
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;'>
    <tr>
      <td style='background-color: #007bff; color: white; padding: 20px; text-align: center; font-size: 24px;'>
        Your Shop Name
      </td>
    </tr>
    <tr>
      <td style='padding: 30px; color: #333; font-size: 16px; line-height: 1.5;'>
        <p>Hello,</p>
        <p>We received a request to reset your password. Click the button below to reset it. This link will expire in 1 hour.</p>
        <p style='text-align: center; margin: 30px 0;'>
          <a href='" & resetLink & "' style='background-color: #007bff; color: white; text-decoration: none; padding: 12px 25px; border-radius: 5px; display: inline-block;'>Reset Password</a>
        </p>
        <p>If you did not request a password reset, please ignore this email.</p>
        <p>Thank you,<br>Your Shop Name Team</p>
      </td>
    </tr>
    <tr>
      <td style='background-color: #f4f4f4; color: #888; text-align: center; font-size: 12px; padding: 15px;'>
        &copy; " & DateTime.Now.Year & " Your Shop Name. All rights reserved.
      </td>
    </tr>
  </table>
</body>
</html>"

            mail.IsBodyHtml = True

            ' SMTP client
            Dim smtp As New System.Net.Mail.SmtpClient("smtp.gmail.com")
            smtp.Port = 587 ' or 25 or 465 depending on your server
            smtp.Credentials = New System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("SmtpUser"), System.Configuration.ConfigurationManager.AppSettings("SmtpPassword"))
            smtp.EnableSsl = True ' if your SMTP server requires SSL

            ' Send it
            smtp.Send(mail)

            lblForgotError.ForeColor = Drawing.Color.Green
            lblForgotError.Text = "Reset link sent to your email!"
        Catch ex As Exception
            lblForgotError.ForeColor = Drawing.Color.Red
            lblForgotError.Text = "Error sending email: " & ex.Message
        End Try
    End Sub

End Class
