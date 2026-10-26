<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ProductDetails.aspx.vb" Inherits="OnlineShop.ProductDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4" style="max-width:1200px;">
        <div class="row g-4">

            <!-- Left: Main Product -->
            <div class="col-lg-6 col-md-12">
                <img id="imgMainProduct" runat="server" class="img-fluid rounded shadow-sm mb-3" style="cursor:pointer;" data-bs-toggle="modal" data-bs-target="#productModal" />
                <div id="divThumbnails" runat="server" class="d-flex flex-wrap gap-2 mb-3"></div>

             <%--   <div class="p-3 border rounded shadow-sm mb-3">
                    <div class="d-flex align-items-center justify-content-between mb-2">
                        <asp:Label ID="lblProductName" runat="server" CssClass="h3 mb-0"></asp:Label>
                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn btn-success btn-lg" />
                    </div>
                    <h4 class="text-primary mt-2">$<asp:Label ID="lblPrice" runat="server"></asp:Label></h4>
                    <p><asp:Label ID="lblDescription" runat="server"></asp:Label></p>
                </div>--%>
<div class="p-3 border rounded shadow-sm mb-3">
    <div class="d-flex align-items-center justify-content-between mb-2">
        <asp:Label ID="lblProductName" runat="server" CssClass="h3 mb-0"></asp:Label>

        <div class="d-flex align-items-center">
                <asp:TextBox ID="txtQuantity" runat="server" 
                 Text="1" 
                 CssClass="form-control me-2" 
                 style="width:70px;" 
                 TextMode="Number"></asp:TextBox>
            <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" 
                        CssClass="btn btn-success btn-lg" OnClick="btnAddToCart_Click" />
        </div>
                    

    </div>
    <asp:Label ID="lblCartMessage" runat="server" CssClass="text-success fw-bold"></asp:Label>

    <h4 class="text-primary mt-2">$<asp:Label ID="lblPrice" runat="server"></asp:Label></h4>
    <p><asp:Label ID="lblDescription" runat="server"></asp:Label></p>
</div>




            </div>

            <!-- Right: Shop Info + Shop's Products -->
            <div class="col-lg-6 col-md-12">
                <div id="divShop" runat="server" class="rounded shadow-sm mb-3" style='<%# $"border-left: 8px solid {ShopThemeColor}" %>'>
                    
                    <!-- Shop Banner -->
                    <div id="divShopBanner" runat="server" class="rounded shadow-sm mb-3" style="height:120px; background-size:cover; background-position:center;"></div>

                    <!-- Shop Name + Logo -->
                    <div class="p-3 d-flex align-items-center gap-3">
                        <asp:HyperLink ID="hlShopName" runat="server" CssClass="h4 text-primary text-decoration-underline text-primary"></asp:HyperLink>
                        <asp:Image ID="imgShopLogo" runat="server" CssClass="shop-logo rounded-circle" Style="width:60px; height:60px; object-fit:cover;" />
                    </div>

                    <!-- Shop Description -->
                    <div class="p-3 mb-3">
                        <p id="lblShopDescription" runat="server" class="mb-0"></p>
                    </div>

                    <!-- All Shop Products -->
                    <h5>Other Products from this Shop</h5>
                    <div id="otherProductsDiv" runat="server" class="d-flex flex-row overflow-auto gap-3 snap-carousel"></div>
                </div>
            </div>
        </div>

        <!-- You May Also Like Section -->
<div class="mt-5">
    <h5>You May Also Like</h5>

    <div class="d-flex align-items-center">
        <button type="button" class="btn btn-outline-secondary me-2" onclick="scrollSlider('recommendedProductsSlider','left')">&lt;</button>

        <div id="recommendedProductsSlider" runat="server" ClientIDMode="Static" class="d-flex overflow-auto gap-3 snap-carousel" style="scroll-behavior: smooth; padding-bottom: 10px;">

            <!-- Cards added dynamically from VB -->
        </div>

        <button type="button" class="btn btn-outline-secondary ms-2" onclick="scrollSlider('recommendedProductsSlider','right')">&gt;</button>
    </div>
</div>



    </div>

    <!-- Modal for Image Zoom -->
    <div class="modal fade" id="productModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <div class="modal-body p-0">
                    <img id="imgModal" runat="server" class="img-fluid w-100" />
                </div>
            </div>
        </div>
    </div>

    <style>
      .snap-carousel {
    scroll-snap-type: x mandatory;
    -webkit-overflow-scrolling: touch; /* smooth scroll on mobile */
}

.snap-carousel .card {
    scroll-snap-align: start;
    flex: 0 0 auto; /* prevent shrinking */
}

        .shop-logo {
            width: 60px;
            height: 60px;
        }
    </style>
    <script>
        function scrollSlider(sliderId, direction) {
            const slider = document.getElementById(sliderId);
            const card = slider.querySelector(".card");
            if (!card) return; // no cards to scroll

            const cardStyle = getComputedStyle(card);
            const gap = parseInt(cardStyle.marginRight); // get margin-right
            const scrollAmount = card.offsetWidth + gap;

            if (direction === 'left') {
                slider.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
            } else {
                slider.scrollBy({ left: scrollAmount, behavior: 'smooth' });
            }
        }
    </script>


</asp:Content>
