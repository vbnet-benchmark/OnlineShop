Imports System
Imports System.Security.Cryptography
Imports System.Net.Mail

Partial Class Register
    Inherits System.Web.UI.Page

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        If Page.IsValid Then
            Dim email = txtEmail.Text.Trim()
            Dim username = txtUsername.Text.Trim()
            Dim password = txtPassword.Text.Trim()
            Dim token = Guid.NewGuid()

            Using db As New OnlineShopContext()
                ' Check uniqueness
                Dim user = db.Users.FirstOrDefault(Function(u) u.Email = email)

                If user IsNot Nothing Then
                    If user.IsEmailVerified Then
                        lblMessage.Text = "Email already registered. Please login."
                    Else

                        user.Username = username
                        user.PasswordHash = ComputeHash(password)
                        user.VerificationToken = token

                        db.SaveChanges() ' Persist updates

                        lblMessage.Text = "Account created but not verified. Please check your email for new verification."
                        btnRegister.Visible = False

                        SendVerificationEmail(email, token)
                    End If
                    Return
                End If
                If db.Users.Any(Function(u) u.Username = username) Then
                    lblMessage.Text = "Username already taken."
                    Return
                End If

                ' Create token


                ' Create user
                Dim newUser = New User() With {
                    .UserID = Guid.NewGuid(),
                    .Email = email,
                    .Username = username,
                    .PasswordHash = ComputeHash(password),
                    .IsEmailVerified = False,
                    .VerificationToken = token
                }

                db.Users.Add(newUser)
                db.SaveChanges()

                ' Send verification email
                SendVerificationEmail(email, token)

                lblMessage.Text = "Account created. Please check your email to verify."
            End Using
        End If
    End Sub

    Private Sub SendVerificationEmail(toEmail As String, token As Guid)
        Dim verifyUrl As String = Request.Url.GetLeftPart(UriPartial.Authority) &
                              "/Shop/Account/VerifyEmail.aspx?token=" & token.ToString()

        Dim subject As String = "Verify Your Email - OnlineShop"

        Dim body As String = $"
    <html>
    <head>
      <meta charset='UTF-8'>
      <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; }}
        .container {{
            max-width: 600px; margin: 20px auto; background: #ffffff;
            padding: 30px; border-radius: 8px; border: 1px solid #eaeaea;
        }}
        .header {{
            text-align: center; padding-bottom: 20px; border-bottom: 1px solid #eaeaea;
        }}
        .header h1 {{ color: #2F80ED; margin: 0; font-size: 24px; }}
        .content {{ margin-top: 20px; font-size: 16px; color: #333; }}
        .btn {{
            display: inline-block; margin: 30px 0; padding: 12px 20px;
            background-color: #2F80ED; color: white; text-decoration: none;
            font-weight: bold; border-radius: 5px;
        }}
        .footer {{ font-size: 12px; color: #777; text-align: center; margin-top: 30px; }}
      </style>
    </head>
    <body>
      <div class='container'>
        <div class='header'>
          <h1>Welcome to OnlineShop!</h1>
        </div>
        <div class='content'>
          <p>Hello,</p>
          <p>Thank you for registering with <strong>OnlineShop</strong>. To complete your registration, please verify your email address by clicking the button below:</p>

          <p style='text-align:center;'>
            <a href='{verifyUrl}' class='btn'>Verify Email</a>
          </p>

          <p>If the button doesn’t work, copy and paste the link below into your browser:</p>
          <p><a href='{verifyUrl}'>{verifyUrl}</a></p>

          <p>Thank you,<br>The OnlineShop Team</p>
        </div>
        <div class='footer'>
          <p>&copy; {DateTime.Now.Year} OnlineShop. All rights reserved.</p>
        </div>
      </div>
    </body>
    </html>
    "

        Dim smtp As New System.Net.Mail.SmtpClient("smtp.gmail.com")
        smtp.Port = 587 ' or 25 or 465 depending on your server
        smtp.Credentials = New System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("SmtpUser"), System.Configuration.ConfigurationManager.AppSettings("SmtpPassword"))
        smtp.EnableSsl = True ' if your SMTP server requires SSL

        Dim msg As New System.Net.Mail.MailMessage("stephanr@spydernetworkinc.com", toEmail, subject, body)
        msg.IsBodyHtml = True

        Try
            smtp.Send(msg)
            Console.WriteLine("Email sent successfully!")
        Catch ex As Exception
            Console.WriteLine("Error sending email: " & ex.Message)
        End Try

    End Sub


    Private Function ComputeHash(input As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes = Encoding.UTF8.GetBytes(input)
            Dim hash = sha256.ComputeHash(bytes)
            Return Convert.ToBase64String(hash)
        End Using
    End Function



End Class
