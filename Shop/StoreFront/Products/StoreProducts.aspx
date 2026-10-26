<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StoreProducts.aspx.vb" Inherits="OnlineShop.StoreProducts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        window.addEventListener('load', function () {
            var overlay = document.getElementById('pageLoadingOverlay');
            if (overlay) {
                overlay.style.display = 'none';
            }
        });
    </script>

    <style>
        .product-card {
    background: #fff;
    padding: 15px;
    border-radius: 12px;
    box-shadow: 0 2px 6px rgba(0,0,0,0.05);
    text-align: center;
    transition: all 0.2s;
}
.product-card:hover { transform: translateY(-3px); }

.edit-form {
    background: #fff;
    padding: 20px;
    border-radius: 12px;
    box-shadow: 0 4px 10px rgba(0,0,0,0.1);
    margin-bottom: 20px;
}
.form-grid {
    display: flex;
    gap: 20px;
}
.form-left { flex: 2; }
.form-right { flex: 1; }

.preview-container {
    display: flex;
    gap: 8px;
    flex-wrap: wrap;
    margin-top: 10px;
}
.preview-container img {
    width: 70px;
    height: 70px;
    object-fit: cover;
    border-radius: 8px;
    cursor: pointer;
    border: 2px solid transparent;
}
.preview-container img.main {
    border-color: #28a745;
}

    </style>
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {

            const MAX_IMAGES = 4;
            let imageSlots = []; // Stores { mime, data } for each image
            let mainIndex = 1;   // Which image is the main one

            // DOM elements
            const fileInput = document.getElementById('<%= fuProductImages.ClientID %>');
          const hfImages = document.getElementById('<%= hfImages.ClientID %>'); // stores JSON of images
          const hfMain = document.getElementById('<%= hfMainImageIndex.ClientID %>'); // main image index
          const previewContainer = document.getElementById('imagePreviewContainer');
          const btnClearImages = document.getElementById('btnClearImages');
            const loadingOverlay = document.getElementById('imageLoadingOverlay');

          // Update hidden fields with current images + main index
          function updateHiddenField() {
              try {
                  hfImages.value = JSON.stringify(imageSlots);
                  hfMain.value = mainIndex;
              } catch (ex) { console.error('Hidden field update error', ex); }
          }

          // Render the image previews in the container
          function renderPreviews() {
              previewContainer.innerHTML = '';
              imageSlots.forEach((slot, idx) => {
                  const wrapper = document.createElement('div');
                  wrapper.style.position = 'relative';
                  wrapper.style.width = '120px';
                  wrapper.style.height = '120px';
                  wrapper.style.flex = '0 0 auto';

                  // Image element
                  const img = document.createElement('img');
                  img.src = slot.data;
                  img.style.width = '100%';
                  img.style.height = '100%';
                  img.style.objectFit = 'cover';
                  img.style.cursor = 'pointer';
                  img.title = (idx + 1 === mainIndex) ? 'Main image' : 'Click to make main';
                  img.style.border = (idx + 1 === mainIndex) ? '3px solid #007bff' : '1px solid #ccc';
                  img.onclick = function () {
                      mainIndex = idx + 1;   // set clicked image as main
                      updateHiddenField();
                      renderPreviews();
                  };

                  // Delete button
                  const del = document.createElement('button');
                  del.type = 'button';
                  del.innerText = '×';
                  del.style.position = 'absolute';
                  del.style.top = '6px';
                  del.style.right = '6px';
                  del.style.background = 'rgba(0,0,0,0.6)';
                  del.style.color = 'white';
                  del.style.border = 'none';
                  del.style.borderRadius = '50%';
                  del.style.width = '22px';
                  del.style.height = '22px';
                  del.style.cursor = 'pointer';
                  del.onclick = function (e) {
                      e.preventDefault();
                      imageSlots.splice(idx, 1); // remove image
                      if (mainIndex > imageSlots.length) mainIndex = imageSlots.length || 1;
                      updateHiddenField();
                      renderPreviews();
                  };

                  wrapper.appendChild(img);
                  wrapper.appendChild(del);
                  previewContainer.appendChild(wrapper);
              });

              updateHiddenField();
          }

          // Handle files selected in FileUpload
            function handleFileSelect(evt) {
                const files = Array.from(evt.target.files || []);
                if (!files.length) return;

                if (loadingOverlay) loadingOverlay.style.display = 'flex'; // show overlay immediately

                let filesProcessed = 0;

                function processNextFile() {
                    if (filesProcessed >= files.length || imageSlots.length >= MAX_IMAGES) {
                        if (loadingOverlay) loadingOverlay.style.display = 'none'; // hide overlay when done
                        return;
                    }

                    const file = files[filesProcessed];
                    filesProcessed++;

                    const reader = new FileReader();
                    reader.onload = function (e) {
                        imageSlots.push({ mime: file.type, data: e.target.result });
                        if (imageSlots.length === 1) mainIndex = 1;
                        renderPreviews();

                        // process the next file in the next event loop tick
                        requestAnimationFrame(processNextFile);
                    };

                    reader.readAsDataURL(file);
                }

                // start processing the first file
                requestAnimationFrame(processNextFile);

                // reset the input so the same file can be re-added later
                if (evt.target) evt.target.value = '';
            }




          // Clear all images
          function clearImages() {
              imageSlots = [];
              mainIndex = 1;
              renderPreviews();
          }

          // On postback: repopulate previews from hidden fields
          function hydrateFromHidden() {
              try {
                  if (hfImages && hfImages.value) {
                      const arr = JSON.parse(hfImages.value);
                      if (Array.isArray(arr) && arr.length) {
                          imageSlots = arr.map(a => ({ mime: a.mime, data: a.data }));
                          const mi = parseInt(hfMain.value || '1', 10);
                          mainIndex = (mi >= 1 && mi <= imageSlots.length) ? mi : 1;
                          renderPreviews();
                      }
                  }
              } catch (ex) { console.error('Hydrate error', ex); }
          }

          // Expose a function to open modal, optionally clearing fields
            window.openAddProductModal = function (clear) {
                const modal = document.getElementById('addProductModal');
                if (!modal) return;
                if (clear) {
                    modal.querySelectorAll('input[type=text], input[type=number], textarea').forEach(i => i.value = '');
                    if (fileInput) fileInput.value = '';
                    clearImages();
                    hfImages.value = '';
                    hfMain.value = '1';
                    modal.querySelectorAll('[id$="Error"]').forEach(el => el.innerText = '');
                } else {
                    hydrateFromHidden(); // restores image previews
                }
                modal.style.display = 'flex';
            };


          // Attach events
          if (fileInput) fileInput.addEventListener('change', handleFileSelect);
          if (btnClearImages) btnClearImages.addEventListener('click', clearImages);

          // hydrate previews on page load
          hydrateFromHidden();
      });

    </script>


    <script>
        function showLoader() {
            const overlay = document.getElementById('imageLoadingOverlay');
            if (overlay) overlay.style.display = 'flex';
        }
        function hideLoader() {
            const overlay = document.getElementById('imageLoadingOverlay');
            if (overlay) overlay.style.display = 'none';
        }

        function handleFileSelect(evt) {
            const files = Array.from(evt.target.files || []);
            if (!files.length) return;

            // Show loading overlay
            if (loadingOverlay) loadingOverlay.style.display = 'flex';

            let filesProcessed = 0;

            files.forEach((file) => {
                if (imageSlots.length >= MAX_IMAGES) {
                    alert('Maximum ' + MAX_IMAGES + ' images allowed.');
                    filesProcessed++;
                    if (filesProcessed === files.length && loadingOverlay) loadingOverlay.style.display = 'none';
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (e) {
                    if (imageSlots.length < MAX_IMAGES) {
                        imageSlots.push({ mime: file.type, data: e.target.result });
                        if (imageSlots.length === 1) mainIndex = 1; // first image = main
                        renderPreviews();
                    }

                    filesProcessed++;
                    // Hide overlay when all files finished reading
                    if (filesProcessed === files.length && loadingOverlay) loadingOverlay.style.display = 'none';
                };

                reader.readAsDataURL(file);
            });

            // reset input so same file can be re-added later
            if (evt.target) evt.target.value = '';
        }


    </script>




<script type="text/javascript">
    // Send height to parent
    function sendHeightToParent() {
        var height = document.body.scrollHeight;
        window.parent.postMessage(height, "*");
    }
    window.addEventListener('load', sendHeightToParent);
    var observer = new MutationObserver(sendHeightToParent);
    observer.observe(document.body, { childList: true, subtree: true, attributes: true });

    // Input validation
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

</script>




<script>
    // Show add product container
    document.getElementById('btnShowAddProduct').addEventListener('click', function () {
        const modal = document.getElementById('addProductModal');
        modal.style.display = 'flex';
    });

    // Close buttons
    document.getElementById('btnCloseModal').addEventListener('click', function () {
        document.getElementById('addProductModal').style.display = 'none';
    });
    document.getElementById('btnCloseFooter').addEventListener('click', function () {
        document.getElementById('addProductModal').style.display = 'none';
    });

    //document.addEventListener('DOMContentLoaded', function () {
    //    const modal = document.getElementById('addProductModal');
    //    const btnShow = document.getElementById('btnShowAddProduct');
    //    const btnClose = document.getElementById('btnCloseModal');
    //    const btnCloseFooter = document.getElementById('btnCloseFooter');

    //    btnShow.addEventListener('click', function (e) {
    //        e.preventDefault();
    //        modal.classList.add('show');
    //    });

    //    btnClose.addEventListener('click', () => modal.style.display = 'none');
    //    btnCloseFooter.addEventListener('click', () => modal.style.display = 'none');

    //    // Close modal when clicking outside content
    //    modal.addEventListener('click', function (e) {
    //        if (e.target === modal) modal.classList.remove('show');
    //    });
    //});
</script>
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
</head>
<body>
    <form id="form1" runat="server">
      <div style="padding:20px; font-family:Arial, sans-serif; background:#f7f9fc; height:100%; overflow:auto;">

        <!-- Product Message -->
          <asp:ScriptManager ID="ScriptManager1" runat="server">
          </asp:ScriptManager>
        <asp:Label ID="lblProductMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

       <!-- Add Product Button -->
<!-- Add New Product Button -->
<div style="margin-bottom:20px; text-align:right;">
<asp:Button ID="btnShowAddProductModal" runat="server" 
    Text="Add New Product"
    CssClass="btn btn-primary"
    Style="background-color:#007bff; color:#fff; border:none; padding:10px 20px; font-size:14px; border-radius:8px; cursor:pointer;"
    OnClientClick="
        var modal = document.getElementById('addProductModal');
        var btn = this;

        // Hide all edit panels and reset their inputs and borders
        var editPanels = document.querySelectorAll('.edit-product-panel');
        editPanels.forEach(function(panel){
            panel.style.display = 'none';

            // Reset input values and borders
            var inputs = panel.querySelectorAll('input[type=text], input[type=number], textarea');
            inputs.forEach(function(input){
                input.value = '';
                input.style.borderColor = '';
                input.classList.remove('is-invalid'); // remove any bootstrap invalid class
            });

            var ddl = panel.querySelectorAll('select');
            ddl.forEach(function(select){
                select.selectedIndex = 0;
                select.style.borderColor = '';
                select.classList.remove('is-invalid');
            });

            // Clear validation labels
            var validators = panel.querySelectorAll('span[id$=Error], label[id$=Error]');
            validators.forEach(function(lbl){ lbl.innerText = ''; });
        });

        // Toggle add modal visibility
        if (modal.style.display === 'flex') {
            modal.style.display = 'none';
            btn.value = 'Add New Product';
        } else {
            modal.style.display = 'flex';
            btn.value = 'Close New Product';
        }

        // Always clear inputs, selects, files, hidden fields, borders, and labels in add modal
        var inputs = modal.querySelectorAll('input[type=text], input[type=number], textarea');
        inputs.forEach(function(input){
            input.value = '';
            input.style.borderColor = '';
            input.classList.remove('is-invalid');
        });

        var ddl = modal.querySelectorAll('select');
        ddl.forEach(function(select){
            select.selectedIndex = 0;
            select.style.borderColor = '';
            select.classList.remove('is-invalid');
        });

        var fileUploads = modal.querySelectorAll('input[type=file]');
        fileUploads.forEach(function(fu){ fu.value = ''; });

        var hiddenFields = modal.querySelectorAll('input[type=hidden]');
        hiddenFields.forEach(function(hf){ hf.value = hf.defaultValue; });

        var validators = modal.querySelectorAll('span[id$=Error], label[id$=Error]');
        validators.forEach(function(lbl){ lbl.innerText = ''; });

        return false;
    "
/>







</div>

              <!-- Add Product Modal -->
<!-- Add Product Container (was modal) -->
<div id="addProductModal" runat="server" style="display:none; width:100%; background:#fff; border-radius:12px; box-shadow:0 10px 25px rgba(0,0,0,0.2); flex-direction:column; margin:20px auto; overflow:hidden;">


    <!-- Loading overlay for images -->
    <div id="imageLoadingOverlay" 
         style="position:absolute; top:0; left:0; right:0; bottom:0;
                background:rgba(255,255,255,0.8); display:none;
                align-items:center; justify-content:center; z-index:9999;">
        <div class="spinner" 
             style="width:48px; height:48px; border:5px solid #ccc; 
                    border-top:5px solid #007bff; border-radius:50%; 
                    animation: spin 1s linear infinite;"></div>
    </div>

    <!-- Modal Header -->
    <div style="display:flex; justify-content:space-between; align-items:center; padding:20px; border-bottom:1px solid #eee;">
        <h5 style="margin:0; font-size:20px; font-weight:bold; color:#333;">Add Product</h5>
        <%--<button type="button" id="btnCloseModal" style="background:none; border:none; font-size:24px; cursor:pointer; color:#999;">&times;</button>--%>
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
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" Style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px;"></asp:DropDownList>
                <asp:HiddenField ID="hfSelectedCategoryID" runat="server" />
                <asp:Label ID="lblCategoryError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>
            </div>
            <div>
                <asp:TextBox ID="txtProductDescription" runat="server" TextMode="MultiLine" Rows="4" placeholder="Product Description (optional)" style="width:95%; padding:10px; border:1px solid #ccc; border-radius:8px; font-size:14px; resize:none;" />
            </div>
        </div>

        <!-- Right: Images -->
        <div style="flex:1; min-width:220px; display:flex; flex-direction:column; gap:12px;">
            <asp:FileUpload ID="fuProductImages" runat="server" AllowMultiple="true" />
            <div id="imagePreviewContainer" style="display:flex; gap:8px; overflow-x:auto; padding-top:8px;"></div>
            <asp:Label ID="lblImageError" runat="server" ForeColor="Red" Style="font-size:13px;"></asp:Label>

            <asp:HiddenField ID="hfImages" runat="server" />
            <asp:HiddenField ID="hfMainImageIndex" runat="server" Value="1" />
            <asp:HiddenField ID="hfKeepModalOpen" runat="server" Value="false" />
            <asp:HiddenField ID="hfEditProductID" runat="server" />


            <div style="display:flex; gap:8px; margin-top:10px; align-items:center;">
                <button type="button" id="btnClearImages" style="padding:6px 10px; border-radius:6px; border:1px solid #ccc; background:#fff; cursor:pointer;">Clear images</button>
                <small style="color:#666;">Max 4 images. Click image to mark main.</small>
            </div>
        </div>

    </div>

    <!-- Footer -->
    <div style="padding:15px 20px; border-top:1px solid #eee; display:flex; justify-content:flex-end; gap:12px;">
        <asp:Button ID="btnAddProduct" runat="server" Text="Add Product" 
            style="background-color:#28a745; color:#fff; border:none; padding:10px 20px; font-size:14px; border-radius:8px; cursor:pointer;" 
            OnClientClick="return confirm('Are you sure you want to add this product?');" 
        />
        <%--<button type="button" id="btnCloseFooter" style="background-color:#6c757d; color:#fff; border:none; padding:10px 20px; font-size:14px; border-radius:8px; cursor:pointer;">Close</button>--%>
    </div>
</div>







        <!-- Products List -->
        <div style="display:flex; flex-direction:column; gap:20px; margin-top:20px; width:100%;">
    <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
        <ItemTemplate>
            <!-- VIEW MODE -->
            <asp:Panel ID="pnlView" runat="server" 
                Visible='<%# Eval("ProductID").ToString() <> hfEditProductID.Value %>'
                Style="width:100%; display:flex; flex-direction:column; background:#fff; border-radius:12px; padding:20px; box-shadow:0 2px 6px rgba(0,0,0,0.1);">
                <h5><%# Eval("ProductName") %></h5>
                <p>$<%# Eval("Price", "{0:F2}") %></p>
                <p>Stock: <%# Eval("StockQuantity") %></p>
                <asp:Button ID="btnEdit" runat="server" CommandName="EditProduct" 
                            CommandArgument='<%# Eval("ProductID") %>' Text="Edit" CssClass="btn btn-primary" />
            </asp:Panel>

            <!-- EDIT MODE -->
            <asp:Panel ID="pnlEdit" runat="server" CssClass="edit-product-panel"
                Visible='<%# Eval("ProductID").ToString() = hfEditProductID.Value %>'
                Style="width:100%; display:flex; flex-wrap:wrap; gap:20px; background:#f9f9f9; border-radius:12px; padding:20px; box-shadow:0 2px 6px rgba(0,0,0,0.1);">
                
                <!-- Left: Product Info -->
                <div style="flex:2 1 100%; min-width:300px; display:flex; flex-direction:column; gap:15px;">
                    <asp:TextBox ID="txtEditName" runat="server" CssClass="form-control" 
                                 Text='<%# Eval("ProductName") %>' Style="width:100%; padding:10px;"/>
                    <asp:TextBox ID="txtEditPrice" runat="server" CssClass="form-control" 
                                 Text='<%# Eval("Price", "{0:F2}") %>' Style="width:100%; padding:10px;"/>
                    <asp:TextBox ID="txtEditStock" runat="server" CssClass="form-control"
                                 Text='<%# Eval("StockQuantity") %>' Style="width:100%; padding:10px;"/>
                    <asp:TextBox ID="txtEditThreshold" runat="server" CssClass="form-control"
                                 Text='<%# Eval("ThresholdQuantity") %>' Style="width:100%; padding:10px;"/>
                    <asp:DropDownList ID="ddlEditCategory" runat="server" CssClass="form-control" Style="width:100%;" />
                    <asp:TextBox ID="txtEditDescription" runat="server" TextMode="MultiLine" Rows="4" 
                                 Text='<%# Eval("Description") %>' Style="width:100%; padding:10px;"/>
                </div>

                <!-- Right: Images -->
                <div style="flex:1 1 100%; display:flex; flex-direction:column; gap:12px;">
                    <asp:FileUpload ID="fuEditProductImages" runat="server" AllowMultiple="true" />
                    <div id="imageEditPreviewContainer_<%# Eval("ProductID") %>" 
                         style="display:flex; gap:8px; overflow-x:auto; padding-top:8px;"></div>
                    <asp:HiddenField ID="hfEditImages" runat="server" />
                    <asp:HiddenField ID="hfEditMainImageIndex" runat="server" Value="1" />
                </div>

                <!-- Actions -->
                <div style="flex-basis:100%; display:flex; gap:10px; margin-top:15px;">
                    <asp:Button ID="btnSaveEdit" runat="server" CommandName="SaveProduct" 
                                CommandArgument='<%# Eval("ProductID") %>' Text="Save" CssClass="btn btn-success" />
                    <asp:Button ID="btnCancelEdit" runat="server" CommandName="CancelEdit" 
                                Text="Cancel" CssClass="btn btn-secondary" />
                </div>
            </asp:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>


    </div>

        <div id="pageLoadingOverlay" style="
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(255, 255, 255, 0.9);
    z-index: 9999;
    display: flex;
    align-items: center;
    justify-content: center;
">
    <div style="
        width: 60px;
        height: 60px;
        border: 6px solid #ccc;
        border-top: 6px solid #007bff;
        border-radius: 50%;
        animation: spin 1s linear infinite;
    "></div>
</div>

<style>
@keyframes spin {
    0% { transform: rotate(0deg);}
    100% { transform: rotate(360deg);}
}
</style>

    </form>
</body>
</html>
