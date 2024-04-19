using System;
using BlazorBootstrap;
using CurrieTechnologies.Razor.SweetAlert2;

namespace DatabaseProgramming.Pages;

public partial class Product
{
 private Modal? modal;

    List<ProductModel> products = new List<ProductModel>();
    ProductModel productModel = new ProductModel();
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            FetchData();
        }
    }

    void MyMethodClick()
    {
        Console.WriteLine("MyMethodClick By VSL");
    }
    async Task OpenForm()
    {
        await modal!.ShowAsync();
        Console.WriteLine("ClickOpenFormButton");
    }
    void FetchData()
    {
        try
        {
            products.Clear();
            using Npgsql.NpgsqlConnection conn = new MyConnect().GetConnection();
            using Npgsql.NpgsqlCommand cmd = conn!.CreateCommand();
            cmd.CommandText = "SELECT * FROM tb_product ORDER BY id DESC";

            Npgsql.NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new ProductModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Barcode = reader["barcode"].ToString(),
                        Name = reader["name"].ToString(),
                        Price = Convert.ToInt32(reader["price"])
                    });
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    async Task CloseForm()
    {
        productModel.Id = 0;
        productModel.Barcode = "";
        productModel.Name = "";
        productModel.Price = 0;

        await modal!.HideAsync();
    }
    async Task Save()
    {
        try
        {
            using Npgsql.NpgsqlConnection conn = new MyConnect().GetConnection();
            using Npgsql.NpgsqlCommand cmd = conn!.CreateCommand();
            if (productModel.Id > 0)
            {
                cmd.CommandText = "UPDATE tb_product SET barcode = @barcode, name = @name, price = @price WHERE id = @id";
                cmd.Parameters.AddWithValue("id", productModel.Id);
            }
            else
            {
                cmd.CommandText = "INSERT INTO tb_product (barcode, name, price) VALUES (@barcode, @name, @price)";
            }
            cmd.Parameters.AddWithValue("barcode", productModel.Barcode!);
            cmd.Parameters.AddWithValue("name", productModel.Name!);
            cmd.Parameters.AddWithValue("price", productModel.Price);

            if (cmd.ExecuteNonQuery() != 0)
            {
                FetchData();
                await CloseForm();

                await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "บันทึกข้อมูล",
                    Text = "บันทึกข้อมูลเรียบร้อยแล้ว",
                    Icon = SweetAlertIcon.Success,
                    Timer = 2000
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("void Save : " + ex.Message);
        }
    }
    async Task Edit(ProductModel item)
    {
        await OpenForm();
        productModel = item;
        Console.WriteLine("ClickEditButton : "+ item.Id);
        
    }
    async Task Remove(ProductModel item)
    {
        SweetAlertResult respond = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "ลบข้อมูล",
                Text = "คุณต้องการลบข้อมูลนี้ใช่หรือไม่?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                ShowConfirmButton = true
            }
        );

        Console.WriteLine("ClickRemoveButton : "+ item.Id);

        if (respond.IsConfirmed)
        {
            try
            {
                using Npgsql.NpgsqlConnection conn = new MyConnect().GetConnection();
                using Npgsql.NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM tb_product WHERE id = @id";
                cmd.Parameters.AddWithValue("id", item.Id);

                if (cmd.ExecuteNonQuery() != 0)
                {
                    FetchData(); // Refresh data เพื่อแสดงข้อมูลล่าสุด
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("void Remove : " + ex.Message);
            }
        }
    }

}
