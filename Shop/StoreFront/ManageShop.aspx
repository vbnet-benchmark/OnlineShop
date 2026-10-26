<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageShop.aspx.vb" Inherits="OnlineShop.ManageShop" %>

<%--<%@ Register Src="~/shop/storefront/products/StoreProducts.ascx" TagPrefix="uc1" TagName="StoreProducts" %>--%>

<!DOCTYPE html>
<html>
<head>
    <title>Manage Your Shop - OnlineShop</title>
    <style>
        body { margin:0; padding:0; font-family: Arial, sans-serif; background:#f8f9fa; }
        .container { display:flex; height:100vh; width:100vw; box-sizing:border-box;}
        .sidebar {
            width:220px;
            background:#ffffff;
            border-right:1px solid #ddd;
            padding:20px;
            box-shadow:0 0 5px rgba(0,0,0,0.1);
        }
        .sidebar h3 { margin-top:0; font-size:18px; margin-bottom:15px; }
        .sidebar ul { list-style:none; padding:0; margin:0; }
        .sidebar ul li { margin-bottom:10px; }
        .sidebar-link { display:block; padding:8px 12px; text-decoration:none; color:#333; border-radius:4px; }
        .sidebar-link:hover { background:#f0f0f0; cursor:pointer; }

        .main-content { flex:1; padding:30px; overflow-y:auto; }
        .card { background:#fff; padding:20px; border-radius:8px; box-shadow:0 2px 5px rgba(0,0,0,0.1); margin-bottom:20px; }
        .card h4 { margin-top:0; }
        .btn { padding:8px 15px; font-size:14px; border:none; border-radius:4px; cursor:pointer; }
        .btn-success { background:#28a745; color:white; }
        .btn-primary { background:#007bff; color:white; }
        .btn-outline { background:#fff; border:1px solid #007bff; color:#007bff; }
        table { width:100%; border-collapse:collapse; margin-top:10px; }
        th, td { padding:10px; border:1px solid #ccc; text-align:left; }
        th { background:#f1f1f1; }
        .sidebar-link {
    display: block;
    padding: 12px 16px;
    color: #555;
    text-decoration: none;
    border-radius: 6px;
    transition: background 0.2s, color 0.2s;
}

.sidebar-link:hover {
    background: #f0f2f5;
    color: #000;
}

.sidebar-link.active {
    background: #007bff;
    color: #fff !important;
    font-weight: bold;
}

    </style>

    




</head>
<body>
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container">


        <!-- Sidebar -->
        <div class="sidebar">
            <h3>Shop Dashboard</h3>
            <ul>
                <li><asp:LinkButton ID="lnkDashboard" runat="server" CssClass="sidebar-link" OnClick="lnkDashboard_Click">Dashboard</asp:LinkButton></li>
                <li><asp:LinkButton ID="lnkProducts" runat="server" CssClass="sidebar-link" OnClick="lnkProducts_Click">Products</asp:LinkButton></li>
                <li><asp:LinkButton ID="lnkOrders" runat="server" CssClass="sidebar-link" OnClick="lnkOrders_Click">Orders</asp:LinkButton></li>
                <li><asp:LinkButton ID="lnkSettings" runat="server" CssClass="sidebar-link" OnClick="lnkSettings_Click">Shop Settings</asp:LinkButton></li>
            </ul>
        </div>

        <!-- Main Content -->
        <div class="main-content">
            <asp:MultiView ID="mvShop" runat="server" ActiveViewIndex="0">

                <!-- Dashboard View -->
                <asp:View ID="viewDashboard" runat="server">
                    <div class="card">
                        <div style="display:flex; gap:20px; flex-wrap:wrap;">
                            <div class="card" style="flex:1; min-width:200px;">
                                <h4>Shop Name</h4>
                                <p><asp:Label ID="lblShopName" runat="server" Text=""></asp:Label></p>
                            </div>
                            <div class="card" style="flex:1; min-width:200px;">
                                <h4>Created On</h4>
                                <p><asp:Label ID="lblCreatedAt" runat="server" Text=""></asp:Label></p>
                            </div>
                            <div class="card" style="flex:1; min-width:200px;">
                                <h4>Description</h4>
                                <p><asp:Label ID="lblShopDescription" runat="server" Text=""></asp:Label></p>
                            </div>
                        </div>
                    </div>
                </asp:View>

                <!-- Products View -->
               <asp:View ID="viewProducts" runat="server">
<iframe 
    id="iframeProducts" 
    runat="server"
    style="width:100%; height:100%; border:none;" 
    frameborder="0">
</iframe>    <%--               <uc1:storeproducts runat="server" id="StoreProducts" />--%>
    
    

</asp:View>







 





                <!-- Orders View -->
                <asp:View ID="viewOrders" runat="server">
                    <h2>Orders</h2>
                    <asp:Repeater ID="rptOrders" runat="server">
                        <HeaderTemplate>
                            <table>
                                <tr>
                                    <th>Order ID</th>
                                    <th>Customer</th>
                                    <th>Total</th>
                                    <th>Status</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("OrderID") %></td>
                                <td><%# Eval("CustomerName") %></td>
                                <td>$<%# Eval("Total") %></td>
                                <td><%# Eval("Status") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:View>

                <!-- Settings View -->
<asp:View ID="viewSettings" runat="server">
    <!-- Full screen container -->
    <div style="display:flex; flex-direction:column; height:100%; width:100%; background:#f7f9fc; font-family:Arial, sans-serif;">

        <!-- Scrollable content -->
        <div style="flex:1; padding:0px; overflow-y:auto;">

       <!-- Shop Status + Save Button Container -->
<div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:25px;">
    <!-- Shop Status -->
    <div style="display:flex; align-items:center; gap:10px;">
        <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
        <asp:Label ID="lblIsActive" runat="server" AssociatedControlID="chkIsActive" 
                   Text="Publish shop (make it live)" style="font-weight:bold; font-size:15px; color:#555;"></asp:Label>
    </div>

    <!-- Save Button -->
    <div>
        <asp:Button ID="btnSaveSettings" runat="server" Text="Save Settings"
                    CssClass="btn btn-primary"
                    style="padding:12px 20px; font-size:16px; border-radius:8px; background:#007bff; border:none; color:white;" 
                    OnClick="btnSaveSettings_Click" />
    </div>
</div>


            <!-- General Information -->
            <div style="background:#fff; padding:20px; border-radius:12px; box-shadow:0 2px 6px rgba(0,0,0,0.05); margin-bottom:30px;">
    <h4 style="margin-top:0; margin-bottom:15px; color:#333;">General Information</h4>

   <div style="display:flex; gap:20px; flex-wrap:wrap;">
    <!-- Shop Name + Theme Color stacked -->
    <div style="flex:1; min-width:250px; display:flex; flex-direction:column; gap:15px;">
        <!-- Shop Name -->
        <div>
            <label for="txtEditShopName" style="font-weight:bold; color:#555;">Shop Name</label>
            <asp:TextBox ID="txtEditShopName" runat="server" CssClass="form-control"
                         placeholder="Enter your shop name"
                         style="width:95%; padding:10px; border-radius:6px; border:1px solid #ccc;" />
        </div>

        <!-- Theme Color (under Shop Name, bigger size) -->
        <div>
            <label for="txtThemeColor" style="font-weight:bold; color:#555;">Theme Color</label>
            <asp:TextBox ID="txtThemeColor" runat="server" CssClass="form-control"
                         TextMode="Color"
                         style="width:100%; height:45px; padding:6px; border-radius:6px; border:1px solid #ccc; cursor:pointer;" />
        </div>
    </div>

    <!-- Shop Description -->
    <div style="flex:2; min-width:300px;">
        <label for="txtEditDescription" style="font-weight:bold; color:#555;">Shop Description</label>
        <asp:TextBox ID="txtEditDescription" runat="server" TextMode="MultiLine" CssClass="form-control"
                     Rows="6" placeholder="Describe your shop (optional)"
                     style="width:99%; padding:10px; border-radius:6px; border:1px solid #ccc; resize:none;" />
    </div>
</div>

</div>


            <!-- Visual Customization -->
            <div style="background:#fff; padding:20px; border-radius:12px; box-shadow:0 2px 6px rgba(0,0,0,0.05); margin-bottom:30px;">
                <h4 style="margin-top:0;  color:#333;">Visual Customization</h4>
                <div style="display:flex; gap:25px; flex-wrap:wrap;">
                    <!-- Logo -->
                    <div style="flex:1; min-width:200px;">
                        <label for="fuLogo" style="font-weight:bold; color:#555;">Shop Logo</label>
                        <asp:FileUpload ID="fuLogo" runat="server" CssClass="form-control" style="width:100%; margin-bottom:10px;" />
                        <asp:Image ID="imgLogoPreview" runat="server" Width="250px" Height="250px"
                                   Style="border-radius:8px; border:1px solid #ccc; object-fit:contain;" />
                    </div>

                    <!-- Banner -->
                    <div style="flex:2; min-width:250px;">
                        <label for="fuBanner" style="font-weight:bold; color:#555;">Shop Banner</label>
                        <asp:FileUpload ID="fuBanner" runat="server" CssClass="form-control" style="width:100%; margin-bottom:10px;" />
                        <asp:Image ID="imgBannerPreview" runat="server" Width="100%" Height="250px"
                                   Style="border-radius:8px; border:1px solid #ccc; object-fit:contain;" />
                    </div>
                </div>
            </div>

        
        </div>
    </div>
</asp:View>







            </asp:MultiView>
        </div>
    </div>

</form>
</body>
</html>
