Public Class ResetPassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim token As String = Request.QueryString("token")

            If String.IsNullOrEmpty(token) Then
                lblMessage.Text = "Invalid password reset link."
                lblMessage.CssClass = "message error"
                pnlResetForm.Visible = False
                Return
            End If

            Try
                Using conn As New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("OnlineShop").ConnectionString)
                    conn.Open()

                    Dim cmd As New SqlClient.SqlCommand("
                    SELECT ExpirationDate, IsUsed
                    FROM PasswordResetTokens
                    WHERE TokenId = @TokenId", conn)
                    cmd.Parameters.AddWithValue("@TokenId", token)

                    Dim reader As SqlClient.SqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Dim expiry As DateTime = Convert.ToDateTime(reader("ExpirationDate"))
                        Dim isUsed As Boolean = Convert.ToBoolean(reader("IsUsed"))
                        reader.Close()

                        If isUsed Then
                            lblMessage.Text = "This reset link has already been used."
                            lblMessage.CssClass = "message error"
                            pnlResetForm.Visible = False
                        ElseIf DateTime.Now > expiry Then
                            lblMessage.Text = "This reset link has expired."
                            lblMessage.CssClass = "message error"
                            pnlResetForm.Visible = False
                        Else
                            ' Token is valid → show the reset form
                            pnlResetForm.Visible = True
                            lblMessage.Text = ""
                        End If
                    Else
                        ' Token not found in DB
                        reader.Close()
                        lblMessage.Text = "Invalid password reset link."
                        lblMessage.CssClass = "message error"
                        pnlResetForm.Visible = False
                    End If
                End Using
            Catch ex As Exception
                lblMessage.Text = "Error validating reset link: " & ex.Message
                lblMessage.CssClass = "message error"
                pnlResetForm.Visible = False
            End Try
        End If
    End Sub


    Protected Sub btnResetPassword_Click(sender As Object, e As EventArgs) Handles btnResetPassword.Click
        ' 1. Check if passwords match
        If txtNewPassword.Text <> txtConfirmPassword.Text Then
            lblMessage.Text = "Passwords do not match."
            lblMessage.CssClass = "message error"
            Return
        End If

        Dim token As String = Request.QueryString("token")
        If String.IsNullOrEmpty(token) Then
            lblMessage.Text = "Invalid password reset token."
            lblMessage.CssClass = "message error"
            pnlResetForm.Visible = False
            Return
        End If

        Try
            Using conn As New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("OnlineShop").ConnectionString)
                conn.Open()

                ' 2. Retrieve token and associated email
                Dim getTokenCmd As New SqlClient.SqlCommand("
                SELECT UserEmail, ExpirationDate, IsUsed
                FROM PasswordResetTokens
                WHERE TokenId = @TokenId", conn)
                getTokenCmd.Parameters.AddWithValue("@TokenId", token)

                Dim reader As SqlClient.SqlDataReader = getTokenCmd.ExecuteReader()
                If reader.Read() Then
                    Dim userEmail As String = reader("UserEmail").ToString()
                    Dim expiry As DateTime = Convert.ToDateTime(reader("ExpirationDate"))
                    Dim isUsed As Boolean = Convert.ToBoolean(reader("IsUsed"))
                    reader.Close()

                    ' 3. Validate token
                    If isUsed Then
                        lblMessage.Text = "This reset link has already been used."
                        lblMessage.CssClass = "message error"
                        Return
                    ElseIf DateTime.Now > expiry Then
                        lblMessage.Text = "This reset link has expired."
                        lblMessage.CssClass = "message error"
                        Return
                    End If

                    ' 4. Update user's password
                    ' Use secure hashing
                    Dim passwordHash As String = HashPassword(txtNewPassword.Text)

                    Dim updateUserCmd As New SqlClient.SqlCommand("
                    UPDATE Users
                    SET PasswordHash = @PasswordHash
                    WHERE Email = @Email", conn)
                    updateUserCmd.Parameters.AddWithValue("@PasswordHash", passwordHash)
                    updateUserCmd.Parameters.AddWithValue("@Email", userEmail)

                    Dim rowsAffected As Integer = updateUserCmd.ExecuteNonQuery()
                    If rowsAffected = 0 Then
                        lblMessage.Text = "Failed to update password. User not found."
                        lblMessage.CssClass = "message error"
                        Return
                    End If

                    ' 5. Mark token as used
                    Dim markUsedCmd As New SqlClient.SqlCommand("
                    UPDATE PasswordResetTokens
                    SET IsUsed = 1
                    WHERE TokenId = @TokenId", conn)
                    markUsedCmd.Parameters.AddWithValue("@TokenId", token)
                    markUsedCmd.ExecuteNonQuery()

                    ' 6. Success
                    lblMessage.Text = "Your password has been reset successfully."
                    lblMessage.CssClass = "message success"
                    pnlResetForm.Visible = False
                Else
                    reader.Close()
                    lblMessage.Text = "Invalid password reset token."
                    lblMessage.CssClass = "message error"
                    pnlResetForm.Visible = False
                End If
            End Using
        Catch ex As Exception
            lblMessage.Text = "Error resetting password: " & ex.Message
            lblMessage.CssClass = "message error"
        End Try
    End Sub

    Private Function HashPassword(password As String) As String
        ' Example SHA256 hashing
        Using sha As New System.Security.Cryptography.SHA256Managed()
            Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha.ComputeHash(bytes)
            Return Convert.ToBase64String(hash)
        End Using
    End Function





End Class