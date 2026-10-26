Public Class Site
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'PopulateCategories()
            HighlightCurrentPage()
            UpdateUserUI() ' New method to show username if logged in
            UpdateCartCount()
        End If
    End Sub

    ' --- New Method to Show Cart Quantity ---
    Public Sub UpdateCartCount()
        Dim count As Integer = 0
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim email = HttpContext.Current.User.Identity.Name
            Using db As New OnlineShopContext()
                Dim user = db.Users.FirstOrDefault(Function(u) u.Email = email)
                If user IsNot Nothing Then
                    count = db.UserCartItems.Where(Function(c) c.UserID = user.UserID).Sum(Function(c) c.Quantity)
                End If
            End Using
        End If

        lblCartCount.Text = count.ToString()
        upCart.Update() ' Refresh the UpdatePanel
    End Sub


    Private Sub UpdateUserUI()
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim email = HttpContext.Current.User.Identity.Name

            Using db As New OnlineShopContext()
                Dim user = db.Users.FirstOrDefault(Function(u) u.Email = email)
                If user IsNot Nothing Then
                    ' Transform lnkLogin into a user menu
                    lnkLogin.NavigateUrl = "~/shop/account/Settings.aspx"
                    lnkLogin.Text = user.Username
                    lnkLogin.CssClass = "nav-link btn btn-outline-primary ms-2 dropdown-toggle"


                End If
            End Using
        Else
            lnkLogin.NavigateUrl = "~/shop/account/Login.aspx"
            lnkLogin.Text = "Login"
            lnkLogin.CssClass = "nav-link btn btn-outline-primary ms-2"
        End If

    End Sub


    ' Populate categories dropdown dynamically from DB
    'Private Sub PopulateCategories()
    '    Using db As New OnlineShopContext()
    '        Dim categories = db.Categories.OrderBy(Function(c) c.CategoryName).ToList()
    '        ddlCategories.Items.Clear()
    '        ddlCategories.Items.Add(New ListItem("All Categories", ""))
    '        For Each cat In categories
    '            ddlCategories.Items.Add(New ListItem(cat.CategoryName, cat.CategoryID.ToString()))
    '        Next
    '    End Using
    'End Sub

    ' Highlight the active page link in navbar
    Private Sub HighlightCurrentPage()
        Dim current As String = VirtualPathUtility.ToAppRelative(Request.Path).ToLowerInvariant()

        ' Keep base classes (login has button styles)
        Dim baseNav As String = "nav-link"
        Dim baseLogin As String = "nav-link btn btn-outline-primary ms-2"

        ' Reset all
        lnkHome.CssClass = baseNav
        lnkCategories.CssClass = baseNav
        lnkShops.CssClass = baseNav
        lnkCart.CssClass = baseNav
        lnkLogin.CssClass = baseLogin

        ' Map routes -> links
        Dim map = New Dictionary(Of String, Web.UI.WebControls.HyperLink) From {
        {"~/default.aspx", lnkHome},
        {"~/categories.aspx", lnkCategories},
        {"~/shopsearch.aspx", lnkShops},
        {"~/shop/account/cart.aspx", lnkCart},
        {"~/shop/account/login.aspx", lnkLogin}
    }

        For Each kv In map
            If current = kv.Key Then
                ' Append active (no dot)
                kv.Value.CssClass &= " current-link"
            End If
        Next
    End Sub


    ' Search button click
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchQuery As String = txtSearch.Text.Trim()
        '     Dim selectedCategory As String = ddlCategories.SelectedValue
        Dim url As String = "~/SearchResults.aspx?q=" & Server.UrlEncode(searchQuery)
        'If Not String.IsNullOrEmpty(selectedCategory) Then
        '    url &= "&cat=" & Server.UrlEncode(selectedCategory)
        'End If
        Response.Redirect(url)
    End Sub

End Class
