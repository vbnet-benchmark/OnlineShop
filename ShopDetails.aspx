<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ShopDetails.aspx.vb" Inherits="OnlineShop.ShopDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-4">

  <!-- Shop Banner -->
<div id="divShopBanner" runat="server" 
     class="rounded-3 shadow-sm mb-4 d-flex align-items-center px-4" 
     style="height: 200px; background-size: cover; background-position: center;">
    <div class="d-flex align-items-center bg-dark bg-opacity-50 p-3 rounded">
        <asp:Image ID="imgShopLogo" runat="server" 
                   CssClass="rounded-circle border border-white shadow me-3" 
                   Style="width: 80px; height: 80px; object-fit: cover;" />
        <div>
            <h2 class="text-white mb-1"><asp:Label ID="lblShopName" runat="server" /></h2>
            <small class="text-white-50">Owned by <asp:Label ID="lblOwner" runat="server" /></small>
        </div>
    </div>
</div>


        <!-- Shop Info -->
        <div class="mb-4">
            <p class="text-muted mb-1">Created on: <asp:Label ID="lblCreatedAt" runat="server" /></p>
            <p><asp:Label ID="lblDescription" runat="server" /></p>
            <asp:Button ID="btnFollow" runat="server" CssClass="btn btn-sm btn-primary" Text="Follow Shop" />
        </div>

        <!-- Products Grid -->
        <h4 class="mb-3">Products</h4>
        <div class="row row-cols-1 row-cols-md-3 row-cols-lg-4 g-4">
            <asp:Repeater ID="rptShopProducts" runat="server">
                <ItemTemplate>
                    <div class="col">
                        <div class="card h-100 shadow-sm border-0" style="cursor:pointer;" onclick="window.location='ProductDetails.aspx?ID=<%# Eval("ProductID") %>';">
                            <img class="card-img-top" 
                                 src='<%# If(String.IsNullOrEmpty(Eval("ImageBase64")), ResolveUrl("~/Images/sample-product.png"), Eval("ImageBase64")) %>' 
                                 alt="Product Image" style="height:200px; object-fit:cover;" />
                            <div class="card-body">
                                <h6 class="card-title text-truncate"><%# Eval("ProductName") %></h6>
                                <p class="card-text text-primary fw-bold mb-0">$<%# Eval("Price", "{0:F2}") %></p>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>
</asp:Content>
