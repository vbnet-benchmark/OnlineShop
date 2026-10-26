<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.vb" Inherits="OnlineShop._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="container mt-4 px-4">

    <!-- Hot Products Slider -->
    <asp:Panel ID="pnlHotProducts" runat="server" Visible="false" CssClass="search-section mb-5">
        <h3 class="text-center">Hot Products</h3>
        <div class="d-flex align-items-center">
            <button type="button" class="btn btn-outline-secondary me-2" onclick="scrollSlider('hotProductsSlider','left')">&lt;</button>
            <div id="hotProductsSlider" class="d-flex overflow-auto flex-nowrap" style="scroll-behavior: smooth; gap:1rem;">
                <asp:Repeater ID="rptHotProductsGroup" runat="server">
                    <ItemTemplate>
                        <div class="card shadow-sm text-center flex-shrink-0" style="width:200px;">
                            <img src='<%# If(String.IsNullOrEmpty(Eval("ImageBase64").ToString()), ResolveUrl("~/images/placeholder.png"), "data:" & Eval("ImageMimeType") & ";base64," & Eval("ImageBase64")) %>'
                                 alt='<%# Eval("ProductName") %>' class="card-img-top" style="height:150px; object-fit:cover;" />
                            <div class="card-body d-flex flex-column">
                                <h6 class="card-title"><%# Eval("ProductName") %></h6>
                                <p class="fw-bold">$<%# Eval("Price") %></p>
                                <a href='ProductDetails.aspx?id=<%# Eval("ProductID") %>' class="btn btn-primary mt-auto btn-sm">View</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <button type="button" class="btn btn-outline-secondary ms-2" onclick="scrollSlider('hotProductsSlider','right')">&gt;</button>
        </div>
    </asp:Panel>

    <!-- Hot Categories Slider -->
    <asp:Panel ID="pnlHotCategories" runat="server" Visible="false" CssClass="search-section mb-5">
        <h3 class="text-center">Hot Categories</h3>
        <div class="d-flex align-items-center">
            <button type="button" class="btn btn-outline-secondary me-2" onclick="scrollSlider('hotCategoriesSlider','left')">&lt;</button>
            <div id="hotCategoriesSlider" class="d-flex overflow-auto flex-nowrap" style="scroll-behavior: smooth; gap:1rem;">
                <asp:Repeater ID="rptHotCategories" runat="server">
                    <ItemTemplate>
                        <div class="card shadow-sm text-center flex-shrink-0" style="width:200px;">
                            <img src='<%# If(String.IsNullOrEmpty(Eval("ImageData").ToString()), ResolveUrl("~/images/placeholder.png"), "data:" & Eval("MimeType") & ";base64," & Eval("ImageData")) %>' 
                                 alt='<%# Eval("CategoryName") %>' class="card-img-top" style="height:150px; object-fit:cover;" />
                            <div class="card-body d-flex flex-column text-center">
                                <h6 class="card-title">
                                    <a href='Products.aspx?CategoryID=<%# Eval("CategoryID") %>'>
                                        <%# Eval("CategoryName") %>
                                    </a>
                                </h6>
                                <div class="d-flex flex-wrap justify-content-center mt-2">
                                    <asp:Repeater ID="rptHotSubCategories" runat="server" DataSource='<%# Eval("SubCategories") %>'>
                                        <ItemTemplate>
                                            <a href='Products.aspx?CategoryID=<%# Eval("CategoryID") %>' class="btn btn-outline-primary btn-sm m-1">
                                                <%# Eval("CategoryName") %>
                                            </a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <button type="button" class="btn btn-outline-secondary ms-2" onclick="scrollSlider('hotCategoriesSlider','right')">&gt;</button>
        </div>
    </asp:Panel>

    <!-- Hot Shops Slider -->
    <asp:Panel ID="pnlHotShops" runat="server" Visible="false" CssClass="search-section mb-5">
        <h3 class="text-center">Popular Shops</h3>
        <div class="d-flex align-items-center">
            <button type="button" class="btn btn-outline-secondary me-2" onclick="scrollSlider('hotShopsSlider','left')">&lt;</button>
            <div id="hotShopsSlider" class="d-flex overflow-auto flex-nowrap" style="scroll-behavior: smooth; gap:1rem;">
                <asp:Repeater ID="rptHotShopsGroup" runat="server">
                    <ItemTemplate>
                        <div class="card shadow-sm text-center flex-shrink-0" style="width:200px;">
                            <img src='<%# If(String.IsNullOrEmpty(Eval("LogoBase64").ToString()), ResolveUrl("~/images/sample-shop.png"), "data:" & Eval("LogoMimeType") & ";base64," & Eval("LogoBase64")) %>' 
                                 alt='<%# Eval("ShopName") %>' class="card-img-top" style="height:150px; object-fit:cover;" />
                            <div class="card-body d-flex flex-column">
                                <h6 class="card-title"><%# Eval("ShopName") %></h6>
                                <a href='<%# "ShopDetails.aspx?ShopID=" & Eval("ShopID").ToString() %>' class="btn btn-primary mt-auto btn-sm">Visit Shop</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <button type="button" class="btn btn-outline-secondary ms-2" onclick="scrollSlider('hotShopsSlider','right')">&gt;</button>
        </div>
    </asp:Panel>

</div>

<script>
    function scrollSlider(sliderId, direction) {
        const slider = document.getElementById(sliderId);
        const scrollAmount = 220; // card width + gap
        if (direction === 'left') slider.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
        else slider.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    }
</script>
</asp:Content>
