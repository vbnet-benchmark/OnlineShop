<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="StoreProductsControl.ascx.vb" Inherits="OnlineShop.StoreProductsControl" %>

<!-- Styles -->
<style type="text/css">
/* Modal overlay */
.custom-modal {
    position: fixed;
    top: 0; left: 0;
    width: 100vw; height: 100vh;
    background: rgba(0,0,0,0.5);
    display: none;
    justify-content: center; align-items: center;
    z-index: 9999;
    opacity: 0;
    transition: opacity 0.3s ease;
}
.custom-modal.show { display: flex; opacity: 1; }

/* Modal box */
.custom-modal-dialog { width: 90%; max-width: 900px; }
.custom-modal-content {
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 3px 12px rgba(0,0,0,0.08);
    display: flex;
    flex-direction: column;
    overflow: hidden;
}

/* Header */
.custom-modal-header {
    padding: 1rem 1.25rem;
    border-bottom: 1px solid #dee2e6;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

/* Body */
.custom-modal-body { padding: 1rem 1.25rem; overflow-y: auto; }

/* Footer */
.custom-modal-footer {
    padding: 0.75rem 1.25rem;
    border-top: 1px solid #dee2e6;
    display: flex;
    justify-content: flex-end;
    gap: 10px;
}

/* Buttons */
.custom-btn-success { background-color: #28a745; color:#fff; border:none; padding:0.5rem 1rem; border-radius:6px; cursor:pointer; }
.custom-btn-secondary { background-color:#6c757d; color:#fff; border:none; padding:0.5rem 1rem; border-radius:6px; cursor:pointer; }
.custom-btn-close { background:none; border:none; font-size:1.2rem; cursor:pointer; }

.form-group { margin-bottom: 15px; }
</style>

<!-- Control markup -->
<div id="storeProductsWrapper" style="padding:20px; font-family:Arial, sans-serif; background:#f7f9fc;">

    <!-- Product Message -->
    <asp:Label ID="lblProductMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

    <!-- Add Product Button -->
    <asp:Button ID="btnShowAddProduct" runat="server" Text="+ Add New Product" CssClass="btn btn-primary mb-3" OnClick="btnShowAddProduct_Click" />

    <!-- Add Product Panel -->
    <asp:Panel ID="pnlAddProduct" runat="server" Visible="False" 
               Style="background:#fff; padding:20px; border-radius:8px; box-shadow:0 3px 10px rgba(0,0,0,0.08); margin-bottom:20px;">
        <div class="form-group">
            <asp:TextBox ID="txtProductName" runat="server" placeholder="Product Name" CssClass="form-control" />
            <asp:Label ID="lblNameError" runat="server" ForeColor="Red" />
        </div>

        <div class="form-group">
            <asp:TextBox ID="txtPrice" runat="server" placeholder="Price ($)" CssClass="form-control" />
            <asp:Label ID="lblPriceError" runat="server" ForeColor="Red" />
        </div>

        <div class="form-group">
            <asp:TextBox ID="txtStock" runat="server" placeholder="Stock Quantity" CssClass="form-control" />
            <asp:Label ID="lblStockError" runat="server" ForeColor="Red" />
        </div>

        <div class="form-group">
            <asp:TextBox ID="txtThreshold" runat="server" placeholder="Threshold Quantity" CssClass="form-control" />
            <asp:Label ID="lblThresholdError" runat="server" ForeColor="Red" />
        </div>

        <div class="form-group">
            <asp:TextBox ID="txtCategory" runat="server" placeholder="Category" CssClass="form-control" />
            <asp:Label ID="lblCategoryError" runat="server" ForeColor="Red" />
        </div>

        <div class="form-group">
            <asp:TextBox ID="txtProductDescription" runat="server" TextMode="MultiLine" Rows="4" placeholder="Description (optional)" CssClass="form-control" />
        </div>

        <div class="form-group">
            <asp:FileUpload ID="fuProductImages" runat="server" AllowMultiple="true" />
            <asp:Label ID="lblImageError" runat="server" ForeColor="Red" />
        </div>

        <asp:Button ID="btnAddProduct" runat="server" Text="Add Product" CssClass="btn btn-success" OnClick="btnAddProduct_Click" />
        <asp:Button ID="btnCancelAddProduct" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancelAddProduct_Click" />
    </asp:Panel>

    <!-- Products List -->
    <asp:Repeater ID="rptProducts" runat="server">
        <ItemTemplate>
            <div style="background:#fff; padding:10px; border-radius:8px; margin-bottom:10px; box-shadow:0 2px 6px rgba(0,0,0,0.05);">
                <h4><%# Eval("ProductName") %></h4>
                <span>Price: $<%# Eval("Price") %></span> | 
                <span>Stock: <%# Eval("StockQuantity") %></span> |
                <span>Threshold: <%# Eval("Threshold") %></span><br />
                <span>Category: <%# Eval("Category") %></span><br />
                <asp:LinkButton ID="lnkEditProduct" runat="server" CommandArgument='<%# Eval("ProductID") %>'>Edit</asp:LinkButton>
                <asp:LinkButton ID="lnkDeleteProduct" runat="server" CommandArgument='<%# Eval("ProductID") %>'>Delete</asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
