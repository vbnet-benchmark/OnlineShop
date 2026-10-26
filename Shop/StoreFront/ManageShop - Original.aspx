<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageShop - Original.aspx.vb" Inherits="OnlineShop.ManageShop" %>
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

    <style>
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
</style>

    <script type="text/javascript">


        function isDecimalKey(evt) {
            var charCode = evt.which ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) return false;
            var input = evt.target;
            if (charCode == 46 && input.value.includes('.')) return false;
            return true;
        }

        function isIntegerKey(evt) {
            var charCode = evt.which ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) return false;
            return true;
        }

        function previewProductImages(event) {
            const files = event.target.files;
            for (let i = 0; i < 4; i++) {
                const preview = document.getElementById('imgPreview' + (i + 1));
                if (files[i]) {
                    const reader = new FileReader();
                    reader.onload = function (e) { preview.src = e.target.result; };
                    reader.readAsDataURL(files[i]);
                } else {
                    preview.src = '/images/placeholder.png';
                }
            }
            setMainImage(1);
        }

        function setMainImage(index) {
            for (let i = 1; i <= 4; i++) {
                const preview = document.getElementById('imgPreview' + i);
                preview.style.border = i === index ? '2px dashed #007bff' : '1px dashed #ccc';
            }
            document.getElementById('<%= hfMainImageIndex.ClientID %>').value = index;
        }
    </script>

<script>
    document.addEventListener('DOMContentLoaded', function () {
        const modal = document.getElementById('addProductModal');
        const btnShow = document.getElementById('btnShowAddProduct');
        const btnClose = document.getElementById('btnCloseModal');
        const btnCloseFooter = document.getElementById('btnCloseFooter');

        btnShow.addEventListener('click', function (e) {
            e.preventDefault();
            modal.classList.add('show');
        });

        btnClose.addEventListener('click', () => modal.style.display = 'none');
        btnCloseFooter.addEventListener('click', () => modal.style.display = 'none');

        // Close modal when clicking outside content
        modal.addEventListener('click', function (e) {
            if (e.target === modal) modal.classList.remove('show');
        });
    });
</script>

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
    <div style="padding:20px; font-family:Arial, sans-serif; background:#f7f9fc; height:100%; overflow:auto;">

        <!-- Product Message -->
        <asp:Label ID="lblProductMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

       <!-- Add Product Button -->
<div style="margin-bottom:20px; display:flex; justify-content:flex-end;">
    <button id="btnShowAddProduct" type="button" 
        style="
            background-color:#007bff;
            color:white;
            border:none;
            padding:10px 18px;
            font-size:15px;
            font-weight:bold;
            border-radius:8px;
            cursor:pointer;
            transition: background 0.2s;
        "
        onmouseover="this.style.background='#0056b3'"
        onmouseout="this.style.background='#007bff'"
         onclick="
    var modal = document.getElementById('addProductModal');
    modal.style.display = 'flex';
    modal.style.opacity = '1';
    
    // Clear all text/number inputs
    modal.querySelectorAll('input[type=text], input[type=number], input[type=color]').forEach(function(input){
        input.value = '';
        input.style.border = '1px solid #ccc';
    });

    // Clear textareas
    modal.querySelectorAll('textarea').forEach(function(textarea){
        textarea.value = '';
        textarea.style.border = '1px solid #ccc';
    });

    // Clear file inputs
    modal.querySelectorAll('input[type=file]').forEach(function(file){
        file.value = '';
        file.style.border = '1px solid #ccc';
    });

    // Reset hidden fields
    modal.querySelectorAll('input[type=hidden]').forEach(function(hidden){
        hidden.value = '1';
    });

    // Reset image previews
    modal.querySelectorAll('img').forEach(function(img){
        img.src = '/images/placeholder.png';
    });

    // Clear validation labels
    modal.querySelectorAll('label, span').forEach(function(el){
        if(el.id && el.id.includes('Error')) { 
            el.innerText = '';
        }
    });
">
        + Add New Product
    </button>
</div>


    <!-- Add Product Modal -->
<div id="addProductModal" style="position:fixed; top:0; left:0; width:100vw; height:100vh; background:rgba(0,0,0,0.6); display:none; justify-content:center; align-items:center; z-index:1000; transition:opacity 0.3s;">
    <div style="width:90%; max-width:900px; background:#fff; border-radius:12px; box-shadow:0 10px 25px rgba(0,0,0,0.2); display:flex; flex-direction:column; max-height:90vh; overflow:hidden;">

        <!-- Modal Header -->
        <div style="display:flex; justify-content:space-between; align-items:center; padding:20px; border-bottom:1px solid #eee;">
            <h5 style="margin:0; font-size:20px; font-weight:bold; color:#333;">Add Product</h5>
            <button type="button" id="btnCloseModal" style="background:none; border:none; font-size:24px; cursor:pointer; color:#999;">&times;</button>
        </div>

        <!-- Modal Body -->
        <div style="padding:20px; overflow-y:auto; display:flex; flex-wrap:wrap; gap:20px;">

            <!-- Left: Product Info -->
            <div style="flex:2; min-width:300px; display:flex; flex-direction:column; gap:15px;">
                <div>
                    <asp:TextBox ID="txtProductName" runat="server" placeholder="Product Name" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;" />
                    <asp:Label ID="lblNameError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
                </div>
                <div style="display:flex; gap:12px;">
                    <div style="flex:1;">
                        <asp:TextBox ID="txtPrice" runat="server" placeholder="Price ($)" onkeypress="return isDecimalKey(event);" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;" />
                        <asp:Label ID="lblPriceError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
                    </div>
                    <div style="flex:1;">
                        <asp:TextBox ID="txtStock" runat="server" placeholder="Stock Qty" onkeypress="return isIntegerKey(event);" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;" />
                        <asp:Label ID="lblStockError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
                    </div>
                    <div style="flex:1;">
                        <asp:TextBox ID="txtThreshold" runat="server" placeholder="Threshold" onkeypress="return isIntegerKey(event);" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;" />
                        <asp:Label ID="lblThresholdError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
                    </div>
                </div>
                <div>
                    <asp:TextBox ID="txtCategory" runat="server" placeholder="Category (type or select)" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;" />
                    <asp:HiddenField ID="hfSelectedCategoryID" runat="server" />
                    <asp:Label ID="lblCategoryError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
                </div>
                <div>
                    <asp:TextBox ID="txtProductDescription" runat="server" TextMode="MultiLine" Rows="4" placeholder="Product Description (optional)" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px; resize:none;" />
                </div>
            

 <!-- Image Viewer -->
<div style="flex:1; min-width:220px; display:flex; flex-direction:column; gap:12px;">
    <asp:FileUpload ID="fuProductImages" runat="server" AllowMultiple="true" onchange="previewProductImages(event)" style="width:95%; padding:8px; border:1px solid #ccc; border-radius:8px;" />
    <asp:Label ID="lblImageError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>

    <!-- Images horizontal row -->
    <div style="display:flex; gap:8px; margin-top:10px; overflow-x:auto;">
        <asp:Image ID="imgPreview1" runat="server" Width="120px" Height="120px" Style="border:2px dashed #007bff; object-fit:cover; border-radius:8px; cursor:pointer;" onclick="setMainImage(1)" />
        <asp:Image ID="imgPreview2" runat="server" Width="120px" Height="120px" Style="border:1px dashed #ccc; object-fit:cover; border-radius:8px; cursor:pointer;" onclick="setMainImage(2)" />
        <asp:Image ID="imgPreview3" runat="server" Width="120px" Height="120px" Style="border:1px dashed #ccc; object-fit:cover; border-radius:8px; cursor:pointer;" onclick="setMainImage(3)" />
        <asp:Image ID="imgPreview4" runat="server" Width="120px" Height="120px" Style="border:1px dashed #ccc; object-fit:cover; border-radius:8px; cursor:pointer;" onclick="setMainImage(4)" />
    </div>

    <asp:HiddenField ID="hfMainImageIndex" runat="server" Value="1" />
</div>
</div>

        </div>

        <!-- Modal Footer -->
        <div style="padding:15px 20px; border-top:1px solid #eee; display:flex; justify-content:flex-end; gap:12px;">
            <asp:Button ID="btnAddProduct" runat="server" Text="Add Product" 
                style="background-color:#28a745; color:#fff; border:none; padding:10px 20px; font-size:14px; border-radius:8px; cursor:pointer;" 
                    OnClientClick="return confirm('Are you sure you want to add this product?');" 
                />
            <button type="button" id="btnCloseFooter" style="background-color:#6c757d; color:#fff; border:none; padding:10px 20px; font-size:14px; border-radius:8px; cursor:pointer;">Close</button>
        </div>

    </div>
</div>





        <!-- Products List -->
        <div style="display:flex; flex-wrap:wrap; gap:20px; margin-top:20px;">
            <asp:Repeater ID="rptProducts" runat="server">
                <ItemTemplate>
                    <div style="flex:1 1 280px; background:#fff; border-radius:12px; box-shadow:0 3px 10px rgba(0,0,0,0.08); padding:15px;">

                        <!-- Product Info -->
                        <h4 style="margin:0 0 5px 0; color:#222;"><%# Eval("ProductName") %></h4>
                        <span style="color:#555;">Price: $<%# Eval("Price") %></span>|
                        <span style="color:#555;">Stock: <%# Eval("StockQuantity") %></span> |
                        <span style="color:#555;">Threshold: <%# Eval("ThresholdQuantity") %></span><br />
                        <span style="color:#555;">Category: <%# Eval("CategoryName") %></span>

                        <!-- Images -->
                        <div style="display:flex; gap:8px; margin-top:10px; overflow-x:auto;">
                            <asp:Repeater ID="rptProductImages" runat="server" DataSource='<%# Eval("Images") %>'>
                                <ItemTemplate>
                                    <img src='data:image/png;base64,<%# Eval("ImageData") %>'
                                         style='width:70px; height:70px; border-radius:6px; object-fit:cover; border:<%# If(Eval("IsMain"), "2px solid #007bff", "1px solid #ccc") %>;' />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Actions -->
                        <div style="display:flex; gap:10px; margin-top:10px;">
                            <asp:LinkButton ID="lnkEditProduct" runat="server" CommandArgument='<%# Eval("ProductID") %>' CssClass="btn btn-outline-primary flex-1">Edit</asp:LinkButton>
                            <asp:LinkButton ID="lnkDeleteProduct" runat="server" CommandArgument='<%# Eval("ProductID") %>' CssClass="btn btn-outline-danger flex-1">Delete</asp:LinkButton>
                        </div>

                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>

    <!-- Inline Scripts -->
    

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
