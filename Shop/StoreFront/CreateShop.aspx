<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CreateShop.aspx.vb" Inherits="OnlineShop.CreateShop" %>
<!DOCTYPE html>
<html>
<head>
    <title>Create Your Shop - OnlineShop</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background: #f4f5f7;
        }
        .container {
            max-width: 650px;
            margin: 50px auto;
            padding: 40px;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 8px 20px rgba(0,0,0,0.08);
        }
        h2 {
            margin-bottom: 25px;
            font-size: 28px;
            color: #333;
            text-align: center;
        }
        .form-group {
            margin-bottom: 20px;
        }
        label {
            display: block;
            font-weight: 600;
            margin-bottom: 6px;
            color: #555;
        }
        .form-control {
            width: 100%;
            padding: 10px 12px;
            font-size: 15px;
            border: 1px solid #ccc;
            border-radius: 6px;
            box-sizing: border-box;
        }
        .btn {
            display: inline-block;
            width: 100%;
            padding: 12px;
            font-size: 16px;
            font-weight: 600;
            border-radius: 6px;
            border: none;
            cursor: pointer;
            transition: 0.2s;
        }
        .btn-success {
            background-color: #28a745;
            color: white;
        }
        .btn-success:hover {
            background-color: #218838;
        }
        .btn-primary {
            background-color: #007bff;
            color: white;
        }
        .btn-primary:hover {
            background-color: #0069d9;
        }
        .text-danger {
            color: #dc3545;
            font-size: 14px;
            margin-top: 4px;
            display: block;
        }
        hr {
            margin: 40px 0;
            border: 0;
            border-top: 1px solid #eee;
        }
        .section-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 15px;
            color: #444;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Create New Shop Section -->
            <h2>Create Your Shop</h2>
            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3" />

            <div class="form-group">
                <label for="txtShopName">Shop Name</label>
                <asp:TextBox ID="txtShopName" runat="server" Placeholder="Enter your shop name" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="reqShopName" runat="server" ControlToValidate="txtShopName"
                    ErrorMessage="Shop name is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label for="txtDescription">Shop Description</label>
                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="form-control"
                             Rows="5" Placeholder="Describe your shop (optional)" MaxLength="1000" />
            </div>

            <asp:Button ID="btnCreateShop" runat="server" Text="Create Shop" CssClass="btn btn-success"
                        OnClick="btnCreateShop_Click" />

            <hr />
           
    </form>
</body>
</html>
