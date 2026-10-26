<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Register.aspx.vb" Inherits="OnlineShop.Register" MasterPageFile="~/site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create Account - OnlineShop
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlRegister" runat="server" DefaultButton="btnRegister">
    <div class="register-container" style="max-width: 900px; margin: auto; min-height: 500px; display: flex;">

        <div class="register-form" style="padding: 40px 50px; flex: 1;">
            <h2 class="mb-4">Create Your Account</h2>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" />

            <div class="mb-3">
                <asp:Label ID="lblEmail" runat="server" Text="Email" AssociatedControlID="txtEmail" CssClass="form-label" />
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" />
                <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Enter a valid email" CssClass="text-danger" ValidationExpression="^\S+@\S+\.\S+$" Display="Dynamic" />
            </div>

            <div class="mb-3">
                <asp:Label ID="lblUsername" runat="server" Text="Username" AssociatedControlID="txtUsername" CssClass="form-label" />
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Choose a username" />
                <asp:RequiredFieldValidator ID="reqUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required" CssClass="text-danger" Display="Dynamic" />
            </div>


            <div class="mb-3">
                <asp:Label ID="lblPassword" runat="server" Text="Password" AssociatedControlID="txtPassword" CssClass="form-label" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Create a password" />
                <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="mb-3">
                <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password" AssociatedControlID="txtConfirmPassword" CssClass="form-label" />
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Confirm your password" />
                <asp:RequiredFieldValidator ID="reqConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm your password" CssClass="text-danger" Display="Dynamic" />
                <asp:CompareValidator ID="cmpPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" ErrorMessage="Passwords do not match" CssClass="text-danger" Display="Dynamic" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3 d-block text-center" />

            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-success w-100" OnClick="btnRegister_Click" visible="true"/>

           

            <div class="btn-login-link mt-3">
                <a href="Login.aspx" class="btn btn-outline-primary w-100">Already have a registered account? Log In</a>
            </div>
        </div>

        <div class="info-panel" style="background: linear-gradient(135deg, #56CCF2 0%, #2F80ED 100%); color: white; flex: 1; padding: 50px; display: flex; flex-direction: column; justify-content: center; text-align: center;">
            <h2>Welcome to OnlineShop!</h2>
            <p>Join us today to start betting tokens on your favorite events and help make a difference with your winnings. It’s fun, easy, and all about giving back.</p>
        </div>

    </div>
    </asp:Panel>
</asp:Content>