<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Products.aspx.vb" Inherits="OnlineShop.Products" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-5">
        <h2 class="mb-4">All Products</h2>
        <div class="row">
           <asp:Repeater ID="rptAllProducts" runat="server">
    <ItemTemplate>
        <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
            <div class="card h-100 text-center shadow-sm">
                <!-- Product Image -->
                <img src='<%# "data:" & Eval("ImageMimeType") & ";base64," & Eval("ImageBase64") %>' 
     class="card-img-top img-fluid" 
     alt='<%# Eval("ProductName") %>' 
     style="max-height:200px; object-fit:cover;" />


                <div class="card-body d-flex flex-column">
                    <h5 class="card-title"><%# Eval("ProductName") %></h5>
                    <p class="card-text fw-bold mb-3">$<%# Eval("Price", "{0:F2}") %></p>
                    <a href='ProductDetails.aspx?ID=<%# Eval("ProductID") %>' 
                       class="btn btn-outline-primary mt-auto">View Details</a>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

        </div>
    </div>
</asp:Content>
