Imports System
Imports System.Linq

Public Class StoreProductsControl
    Inherits System.Web.UI.UserControl

    Public Property ShopID As Guid

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadProducts()
            RegisterClientScripts()
        End If
    End Sub

    Protected Sub btnShowAddProduct_Click(sender As Object, e As EventArgs)
        pnlAddProduct.Visible = True
    End Sub

    Protected Sub btnCancelAddProduct_Click(sender As Object, e As EventArgs)
        pnlAddProduct.Visible = False
        ClearForm()
    End Sub

    Protected Sub btnAddProduct_Click(sender As Object, e As EventArgs)
        Dim hasError As Boolean = False
        Dim priceDecimal As Decimal

        If String.IsNullOrWhiteSpace(txtProductName.Text) Then
            lblNameError.Text = "Product name is required"
            hasError = True
        End If

        If Not Decimal.TryParse(txtPrice.Text, priceDecimal) Then
            lblPriceError.Text = "Valid price required"
            hasError = True
        End If

        If hasError Then Return

        Using db As New OnlineShopContext()
            Dim product As New Product With {
                .ProductID = Guid.NewGuid(),
                .ShopID = ShopID,
                .ProductName = txtProductName.Text.Trim(),
                .Price = priceDecimal
            }
            db.Products.Add(product)
            db.SaveChanges()
        End Using

        ClearForm()
        pnlAddProduct.Visible = False
        LoadProducts()
    End Sub

    Private Sub LoadProducts()
        Using db As New OnlineShopContext()
            Dim products = db.Products.Where(Function(p) p.ShopID = ShopID).ToList()
            rptProducts.DataSource = products
            rptProducts.DataBind()
        End Using
    End Sub

    Private Sub ClearForm()
        txtProductName.Text = ""
        txtPrice.Text = ""
        txtStock.Text = ""
        txtThreshold.Text = ""
        txtCategory.Text = ""
        txtProductDescription.Text = ""
        lblNameError.Text = ""
        lblPriceError.Text = ""
    End Sub

    Private Sub RegisterClientScripts()
        Dim script As String = $"
            document.addEventListener('DOMContentLoaded', function() {{
                const btnShow = document.getElementById('{btnShowAddProduct.ClientID}');
                const pnl = document.getElementById('{pnlAddProduct.ClientID}');

                btnShow.addEventListener('click', function(e) {{
                    e.preventDefault();
                    pnl.style.display = 'block';
                }});
            }});
        "
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "StoreProductsScript", script, True)
    End Sub

End Class
