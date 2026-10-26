<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ShopSearch.aspx.vb" Inherits="OnlineShop.ShopSearch" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-4">
    <h2 class="mb-4">Browse Shops</h2>
    <div class="row" id="shopList">
        <asp:Repeater ID="rptShops" runat="server">
            <ItemTemplate>
                <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
                    <div class="card h-100 text-center shadow-sm">
                        <!-- Shop Logo -->
                        <img src='data:<%# Eval("LogoMimeType") %>;base64,<%# Eval("LogoBase64") %>'
                             class="card-img-top img-fluid"
                             alt='<%# Eval("ShopName") %>'
                             style="max-height:160px; object-fit:contain; padding:10px;" />

                        <div class="card-body">
                            <h5 class="card-title"><%# Eval("ShopName") %></h5>
                            <%--<p class="card-text text-muted">@<%# Eval("Username") %></p>--%>
                            <a href='ShopDetails.aspx?ShopID=<%# Eval("ShopID") %>'
                               class="btn btn-outline-primary w-100 mt-2">Visit Shop</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

</asp:Content>
