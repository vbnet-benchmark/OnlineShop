<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="SearchResults.aspx.vb" Inherits="OnlineShop.SearchResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
        Search Results - OnlineShop

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width:1200px; margin:auto; padding:20px;">
        <h2>Search Results</h2>
        <asp:Label ID="lblQuery" runat="server" CssClass="mb-3 d-block"></asp:Label>

      <!-- Products Section -->
<asp:Panel ID="pnlProducts" runat="server" Visible="false" CssClass="search-section">
    <h3>Products</h3>
    <div class="row">
        <asp:Repeater ID="rptProducts" runat="server">
            <ItemTemplate>
                <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
                    <div class="card shadow-sm search-card h-100">
                        <img src='<%# If(String.IsNullOrEmpty(Eval("ImageBase64")), "~/images/sample-product.png", "data:" & Eval("ImageMimeType") & ";base64," & Eval("ImageBase64")) %>'
                             alt='<%# Eval("ProductName") %>' />
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title"><%# Eval("ProductName") %></h5>
                            <p class="card-text"><%# Eval("Description") %></p>
                            <p class="fw-bold">$<%# Eval("Price") %></p>
                            <a href='ProductDetails.aspx?id=<%# Eval("ProductID") %>' class="btn btn-primary mt-auto">View</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>

<!-- Categories Section -->
<asp:Panel ID="pnlCategories" runat="server" Visible="false" CssClass="search-section">
    <h3>Categories</h3>
    <div class="row">
        <asp:Repeater ID="rptMainCategories" runat="server">
            <ItemTemplate>
                <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
                    <div class="card shadow-sm search-card h-100">
                        <img src='data:<%# Eval("MimeType") %>;base64,<%# Eval("ImageData") %>'
                             alt='<%# Eval("CategoryName") %>' />
                        <div class="card-body text-center">
                            <h5 class="card-title"><%# Eval("CategoryName") %></h5>
                            <div class="d-flex flex-wrap justify-content-center mt-2">
                                <asp:Repeater ID="rptSubCategories" runat="server" DataSource='<%# Eval("SubCategories") %>'>
                                    <ItemTemplate>
                                        <a href='Products.aspx?CategoryID=<%# Eval("CategoryID") %>' 
                                           class="btn btn-outline-primary btn-sm m-1"><%# Eval("CategoryName") %></a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>

<!-- Shops Section -->
<asp:Panel ID="pnlShops" runat="server" Visible="false" CssClass="search-section">
    <h3>Shops</h3>
    <div class="row">
        <asp:Repeater ID="rptShops" runat="server">
            <ItemTemplate>
                <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
                    <div class="card shadow-sm search-card h-100 text-center">
                        <img src='<%# If(String.IsNullOrEmpty(Eval("LogoBase64")), "~/images/sample-shop.png", "data:" & Eval("LogoMimeType") & ";base64," & Eval("LogoBase64")) %>'
                             alt='<%# Eval("ShopName") %>' />
                        <div class="card-body">
                            <h5 class="card-title"><a href='ShopDetails.aspx?ShopID=<%# Eval("ShopID") %>'><%# Eval("ShopName") %></a></h5>
                            <p>Owner: <%# Eval("Username") %></p>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>

        <asp:Label ID="lblNoResults" runat="server" CssClass="text-danger" Visible="False" Text="No results found." />
    </div>
</asp:Content>
