<%@ Page Title="Checkout" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Checkout.aspx.vb" Inherits="OnlineShop.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-5">
    <div class="row g-4">

        <!-- Left: Order Summary -->
        <div class="col-lg-6 col-md-12">
            <h3>Checkout</h3>

            <div class="p-4 border rounded shadow-sm bg-light mb-3">
                <div class="d-flex justify-content-between mb-2">
                    <span>Subtotal</span>
                    <span runat="server" id="lblSubtotal">$0.00</span>
                </div>
                <hr />
                <div class="d-flex justify-content-between mb-3 fw-bold">
                    <span>Total</span>
                    <span runat="server" id="lblTotal">$0.00</span>
                </div>
            </div>

            <asp:HiddenField ID="hfClientTotal" runat="server" />
            <asp:HiddenField ID="hfFreeShippingEligible" runat="server" />
            <asp:HiddenField ID="hfMembershipTier" runat="server" />
            <asp:HiddenField ID="hfBulkTierCode" runat="server" />
            <asp:HiddenField ID="hfPromoPercent" runat="server" />

            <span runat="server" id="lblCheckoutMessage" class="d-block mt-3 fw-bold"></span>
        </div>

        <!-- Right: Checkout Actions -->
        <div class="col-lg-6 col-md-12">

            <div class="p-3 border rounded mb-3">
                <h5>Place Order</h5>
                <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" CssClass="btn btn-success w-100 mb-2" />
                <asp:Button ID="btnPlaceOrderSafe" runat="server" Text="Place Order (Safe)" CssClass="btn btn-outline-success w-100" />
            </div>

            <div class="p-3 border rounded mb-3">
                <h5>Apply Promo Code</h5>
                <asp:TextBox ID="txtPromoCode" runat="server" CssClass="form-control mb-2" placeholder="Promo code" />
                <asp:CheckBox ID="chkUsePromo" runat="server" Text="Apply promo" CssClass="mb-2" />
                <asp:Button ID="btnApplyPromo" runat="server" Text="Apply Promo" CssClass="btn btn-primary w-100 mb-2" />
                <asp:Button ID="btnApplyPromoSafe" runat="server" Text="Apply Promo (Safe)" CssClass="btn btn-outline-primary w-100" />
            </div>

            <div class="p-3 border rounded mb-3">
                <h5>Shipping</h5>
                <asp:Button ID="btnPlaceOrderWithShipping" runat="server" Text="Calculate With Shipping" CssClass="btn btn-primary w-100 mb-2" />
                <asp:Button ID="btnPlaceOrderWithShippingSafe" runat="server" Text="Calculate With Shipping (Safe)" CssClass="btn btn-outline-primary w-100" />
            </div>

            <div class="p-3 border rounded mb-3">
                <h5>Membership Pricing</h5>
                <asp:Button ID="btnPlaceOrderMember" runat="server" Text="Calculate Member Total" CssClass="btn btn-primary w-100 mb-2" />
                <asp:Button ID="btnPlaceOrderMemberSafe" runat="server" Text="Calculate Member Total (Safe)" CssClass="btn btn-outline-primary w-100" />
            </div>

            <div class="p-3 border rounded mb-3">
                <h5>Bulk Discount</h5>
                <asp:Button ID="btnApplyBulkDiscount" runat="server" Text="Apply Bulk Discount" CssClass="btn btn-primary w-100 mb-2" />
                <asp:Button ID="btnApplyBulkDiscountSafe" runat="server" Text="Apply Bulk Discount (Safe)" CssClass="btn btn-outline-primary w-100" />
            </div>

        </div>

    </div>
</div>

</asp:Content>
