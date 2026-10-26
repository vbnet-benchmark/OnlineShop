<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Categories.aspx.vb" Inherits="OnlineShop.Categories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="container mt-4">
    <h2 class="mb-4">Shop by Category</h2>
    <div class="row" id="categoryList">
        <asp:Repeater ID="rptCategories" runat="server">
            <ItemTemplate>
                <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
                    <div class="card h-100 text-center shadow-sm">
                        <!-- Category Image -->
                        <img src='<%# If(String.IsNullOrEmpty(Eval("ImageData").ToString()), ResolveUrl("~/images/placeholder.png"), "data:" & Eval("MimeType") & ";base64," & Eval("ImageData")) %>'
                             class="card-img-top img-fluid" 
                             alt='<%# Eval("CategoryName") %>' 
                             style="max-height:180px; object-fit:cover;" />

                        <div class="card-body">
                            <h5 class="card-title"><%# Eval("CategoryName") %></h5>
                            <a href='Products.aspx?CategoryID=<%# Eval("CategoryID") %>' class="btn btn-outline-primary mt-2">View Products</a>

                            <!-- Subcategories -->
                            <div class="mt-2">
                                <asp:Repeater ID="rptSubCategories" runat="server" DataSource='<%# Eval("SubCategories") %>'>
                                    <ItemTemplate>
                                        <a href='Products.aspx?CategoryID=<%# Eval("CategoryID") %>' class="btn btn-sm btn-outline-secondary m-1">
                                            <%# Eval("CategoryName") %>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
</asp:Content>
