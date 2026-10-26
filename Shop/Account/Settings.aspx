<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Settings.aspx.vb" Inherits="OnlineShop.Settings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Account Settings - OnlineShop
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="display:flex; width:100%; height:100vh; min-height:600px; margin:0; box-shadow:0 0 10px rgba(0,0,0,0.05); border-radius:8px; overflow:hidden;">
    
    <!-- Left Navigation -->
    <div style="width:220px; padding:20px; background:#f8f9fa; border-right:1px solid #ddd;">
        <asp:Button ID="btnProfile" runat="server" Text="Profile" CssClass="w-100 mb-2 btn btn-outline-secondary" OnClick="btnProfile_Click" />
        <asp:Button ID="btnOrders" runat="server" Text="Orders" CssClass="w-100 mb-2 btn btn-outline-secondary" OnClick="btnOrders_Click" />
        <asp:Button ID="btnPassword" runat="server" Text="Change Password" CssClass="w-100 mb-2 btn btn-outline-secondary" OnClick="btnPassword_Click" />
        <asp:Button ID="btnShop" runat="server" Text="Shop" CssClass="w-100 mb-2 btn btn-outline-primary" OnClick="btnShop_Click" />
        <asp:Button ID="btnLogout" runat="server" Text="Logout" CssClass="w-100 mt-4 btn btn-danger" OnClick="btnLogout_Click" OnClientClick="return confirm('Are you sure you want to log out?');" />
    </div>

    <!-- Right Content Area -->
    <div style="flex:1; padding:20px; background:white;">
        <asp:MultiView ID="mvSettings" runat="server" ActiveViewIndex="0">
            
            <!-- Profile View -->
            <asp:View ID="vProfile" runat="server">
                <p>Update your personal information here.</p>
            </asp:View>

            <!-- Orders View -->
            <asp:View ID="vOrders" runat="server">
                <p>View your past orders.</p>
            </asp:View>

            <!-- Change Password View -->
            <asp:View ID="vPassword" runat="server">
                <p>Update your password securely.</p>
            </asp:View>

              <!-- Shop View -->
<asp:View ID="vShop" runat="server">
    <div style="display:flex; flex-direction:column; height:100%;">
        <div style="flex:1; display:flex;">
            <iframe ID="iframeShop" runat="server" style="flex:1; width:100%; border:1px solid #ccc; border-radius:5px;"></iframe>
        </div>
    </div>
</asp:View>



        </asp:MultiView>
    </div>
</div>

</asp:Content>
