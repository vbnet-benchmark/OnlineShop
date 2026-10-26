<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="OnlineShop.Login" MasterPageFile="~/site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Login - OnlineShop
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnLogin">

    <div class="login-container" style="max-width: 900px; margin: auto; min-height: 500px; display: flex;">

        <div class="login-form" style="padding: 40px 50px; flex: 1;">
            <h2 class="mb-4">Welcome Back!</h2>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />

            <div class="mb-3">
                <asp:Label ID="lblEmail" runat="server" Text="Email" AssociatedControlID="txtEmail" CssClass="form-label" />
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" />
                <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Enter a valid email" CssClass="text-danger" ValidationExpression="^\S+@\S+\.\S+$" Display="Dynamic" />
            </div>

            <div class="mb-3">
                <asp:Label ID="lblPassword" runat="server" Text="Password" AssociatedControlID="txtPassword" CssClass="form-label" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password" />
                <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="form-check mb-3" style="display:flex; align-items:center;">
                <asp:CheckBox ID="chkRememberMe" runat="server" />
                <asp:Label ID="lblRememberMe" runat="server" Text="Remember me" AssociatedControlID="chkRememberMe" CssClass="form-check-label" style="margin-left:0.5rem; cursor:pointer;" />

                   <!-- Forgot Password link -->
                
    <a href="javascript:void(0);" 
       onclick="
           var modal = document.getElementById('forgotPasswordModal');
           modal.style.display = 'flex';
           modal.style.opacity = '1';
       " 
       style="font-size:14px; color:#007bff;margin-left:20px; text-decoration:none; cursor:pointer;">
       Forgot Password?
    </a>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-success w-100" OnClick="btnLogin_Click" />

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3 d-block text-center" />

            <div class="btn-register-link mt-3">
                <a href="Register.aspx" class="btn btn-outline-primary w-100">Create an Account</a>
            </div>
        </div>

        <div class="info-panel" style="background: linear-gradient(135deg, #ff7e5f 0%, #feb47b 100%); color: white; flex: 1; padding: 50px; display: flex; flex-direction: column; justify-content: center; text-align: center;">
            <h2>Welcome to OnlineShop!</h2>
            <p>Log in now to place bets, track your tokens, and help make a difference. Let’s get started!</p>
        </div>

    </div>
        <div id="forgotPasswordModal" style="position:fixed; top:0; left:0; width:100vw; height:100vh; background:rgba(0,0,0,0.6); display:none; justify-content:center; align-items:center; z-index:1000;">
    <div style="width:400px; background:#fff; border-radius:12px; box-shadow:0 10px 25px rgba(0,0,0,0.2); padding:30px; display:flex; flex-direction:column; gap:20px;">
        
        <!-- Header -->
        <div style="display:flex; justify-content:space-between; align-items:center;">
            <h3 style="margin:0; font-size:20px; font-weight:bold; color:#333;">Forgot Password</h3>
            <button type="button" id="btnCloseForgot" 
        style="background:none; border:none; font-size:24px; cursor:pointer; color:#999;" 
        onclick="
            var modal=document.getElementById('forgotPasswordModal');
            modal.style.display='none';
            modal.style.opacity='0';
        ">
    &times;
</button>
        </div>

        <!-- Instructions -->
        <p style="margin:0; font-size:14px; color:#555;">
            Enter your registered email address and we’ll send you instructions to reset your password.
        </p>

        <!-- Form -->
        <div style="display:flex; flex-direction:column; gap:12px;">
            <asp:TextBox ID="txtForgotEmail" runat="server" placeholder="Email Address" 
                style="width:100%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;" />
            <asp:Label ID="lblForgotError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
        </div>

        <!-- Actions -->
        <div style="display:flex; justify-content:flex-end; gap:12px; margin-top:10px;">
           <asp:Button ID="btnSendReset" runat="server" Text="Send Reset Link"
    style="background-color:#007bff; color:#fff; border:none; padding:10px 16px; font-size:14px; border-radius:8px; cursor:pointer;"
    OnClick="btnSendReset_Click" CausesValidation="False" />

            <button type="button" id="btnCancelForgot" 
        style="background-color:#6c757d; color:#fff; border:none; padding:10px 20px; border-radius:8px; cursor:pointer;"
        onclick="
            var modal=document.getElementById('forgotPasswordModal');
            modal.style.display='none';
            modal.style.opacity='0';
        ">
    Cancel
</button>
        </div>
    </div>
</div>

    </asp:Panel>
</asp:Content>

