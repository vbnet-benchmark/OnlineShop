<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ResetPassword.aspx.vb" Inherits="OnlineShop.ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Reset Password

    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <style>
       body {
    font-family: Arial, sans-serif;
    background: #f4f6f9;
    margin: 0;
}

    .reset-container {
    background: #fff;
    padding: 40px;
    border-radius: 12px;
    box-shadow: 0 8px 25px rgba(0,0,0,0.15);
    width: 100%;
    max-width: 700px;  /* widen from 420px */
    margin: auto; /* center with top spacing */
}


        .reset-header {
            text-align: center;
            margin-bottom: 25px;
        }
        .reset-header h2 {
            margin: 0;
            font-size: 24px;
            font-weight: bold;
            color: #333;
        }
        .form-group {
            margin-bottom: 18px;
            display: flex;
            flex-direction: column;
        }
        .form-group label {
            font-size: 14px;
            margin-bottom: 6px;
            font-weight: 500;
            color: #444;
        }
        .form-group input {
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 6px;
            font-size: 15px;
        }
        .form-group input:focus {
            outline: none;
            border-color: #007bff;
            box-shadow: 0 0 4px rgba(0,123,255,0.3);
        }
        .password-hint {
            font-size: 12px;
            color: #777;
            margin-top: 4px;
        }
        .btn-submit {
            width: 100%;
            background: #007bff;
            color: white;
            border: none;
            padding: 12px;
            font-size: 16px;
            font-weight: bold;
            border-radius: 8px;
            cursor: pointer;
            transition: background 0.2s ease;
        }
        .btn-submit:hover {
            background: #0056b3;
        }
        .message {
            margin-top: 18px;
            text-align: center;
            font-size: 14px;
        }
        .message.success { color: green; }
        .message.error { color: red; }
    </style>
    <div class="reset-container">
        <div class="reset-header">
            <h2>Reset Your Password</h2>
            <p style="font-size:14px; color:#666; margin-top:8px;">Enter and confirm your new password below.</p>
        </div>

        <asp:Panel ID="pnlResetForm" runat="server">
            <!-- New Password -->
            <div class="form-group">
                <label for="txtNewPassword">New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control" />
                <span class="password-hint">Must be at least 8 characters, include upper/lowercase and a number.</span>
                <asp:RequiredFieldValidator ID="reqNewPassword" runat="server" ControlToValidate="txtNewPassword" ErrorMessage="New password is required." CssClass="message error" Display="Dynamic" />
            </div>

            <!-- Confirm Password -->
            <div class="form-group">
                <label for="txtConfirmPassword">Confirm Password</label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Please confirm your password." CssClass="message error" Display="Dynamic" />
                <asp:CompareValidator ID="cmpPasswords" runat="server" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword" ErrorMessage="Passwords do not match." CssClass="message error" Display="Dynamic" />
            </div>

            <!-- Submit -->
            <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="btn-submit" />
        </asp:Panel>

        <!-- Feedback -->
        <asp:Label ID="lblMessage" runat="server" CssClass="message"></asp:Label>
    </div>
</asp:Content>
