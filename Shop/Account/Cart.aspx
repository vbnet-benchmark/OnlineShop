<%@ Page Title="Shopping Cart" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Cart.aspx.vb" Inherits="OnlineShop.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-5">
    <div class="row g-4">

        <!-- Left: Cart Items -->
        <div class="col-lg-8 col-md-12">
            <h3>Your Shopping Cart</h3>

            <asp:Panel ID="pnlCartEmpty" runat="server" CssClass="alert alert-info mt-3" Visible="False">
                Your cart is empty.
            </asp:Panel>

            <asp:Repeater ID="rptCartItems" runat="server">
                <ItemTemplate>
                    <div class="mb-3 p-3 border rounded shadow-sm">
                        <!-- Product Name & Remove -->
                        <div class="d-flex justify-content-between align-items-center mb-2">
                            <strong class="h5"><%# Eval("ProductName") %></strong>
                            <asp:Button ID="btnRemove" runat="server" 
                                        CommandName="Remove" 
                                        CommandArgument='<%# Eval("CartItemID") %>' 
                                        Text="Remove" CssClass="btn btn-sm btn-danger" />
                        </div>

                        <!-- Product Images -->
                        <div class="d-flex gap-2 mb-2 overflow-auto">
                            <%-- Each image as small thumbnail --%>
                            <asp:Repeater ID="rptProductImages" runat="server" DataSource='<%# Eval("ImageUrls") %>'>
                                <ItemTemplate>
                                    <img src='<%# Container.DataItem %>' class="img-thumbnail" style="width:60px;height:60px;" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Price and Quantity -->
                        <div class="d-flex justify-content-between align-items-center">
                            <span class="fw-bold">$<%# Eval("Price", "{0:F2}") %></span>
                            <span>Qty: <%# Eval("Quantity") %></span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Right: Order Summary -->
        <div class="col-lg-4 col-md-12">
            <div class="p-4 border rounded shadow-sm bg-light">
                <h4>Order Summary</h4>
                <div class="d-flex justify-content-between mb-2">
                    <span>Subtotal</span>
                    <span runat="server" id="lblSubtotal">$0.00</span>
                </div>
                <div class="d-flex justify-content-between mb-2">
                    <span>Shipping</span>
                    <span runat="server" id="lblShipping">$0.00</span>
                </div>
                <div class="d-flex justify-content-between mb-2">
                    <span>Tax</span>
                    <span runat="server" id="lblTax">$0.00</span>
                </div>
                <hr />
                <div class="d-flex justify-content-between mb-3 fw-bold">
                    <span>Total</span>
                    <span runat="server" id="lblTotal">$0.00</span>
                </div>
                <asp:Button ID="btnCheckout" runat="server" Text="Proceed to Checkout" CssClass="btn btn-success w-100" />
            </div>
        </div>

    </div>
</div>

<style>
    .cart-items img {
        object-fit: cover;
        border-radius: 4px;
    }
</style>
</asp:Content>
