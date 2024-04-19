using System;
using BlazorBootstrap;
using CurrieTechnologies.Razor.SweetAlert2;

namespace DatabaseProgramming.Pages;

public partial class ProductApi
{
    private List<ProductModel> list = new List<ProductModel>();
    private HttpClient req = new HttpClient(); // สร้าง Object สำหรับเรียกใช้งาน HttpClient
    private string? Barcode;
    private string? Name;
    private int Price;
    private int Id;

    private Modal? modal;

    protected override async void OnAfterRender(bool firstRender)
    {
        if (firstRender) 
        {
            await FetchData();
        }
    }

    async Task OpenMadal()
    {
        await modal!.ShowAsync();
    } // สร้าง Method สำหรับเปิด Modal

    async Task CloseModal()
    {
        await modal!.HideAsync();
    } // สร้าง Method สำหรับปิด Modal

    async Task HandleSave()
    {
        try {
            object payload = new {
                Barcode = Barcode,
                Name = Name,
                Price = Price,
                Id = Id
            };

            bool IsSuccessStatusCode = false; // สร้างตัวแปรเก็บค่าสถานะการบันทึก
            
            // ถ้า Id เท่ากับ 0 แสดงว่าเป็นการบันทึกข้อมูลใหม่
            if (Id == 0) 
            {
                // ถ้า Id เท่ากับ 0 แสดงว่าเป็นการบันทึกข้อมูลใหม่
                HttpResponseMessage res = await req.PostAsJsonAsync(
                    Config.apiPath + "/api/Product/Create", 
                    payload
                ); // ส่งข้อมูลใหม่ไปบันทึก (สร้างใหม่)
                IsSuccessStatusCode = res.IsSuccessStatusCode; // เก็บค่าสถานะการบันทึก
            } 
            else 
            {
                // ถ้า Id ไม่เท่ากับ 0 แสดงว่าเป็นการแก้ไขข้อมูล
                HttpResponseMessage res = await req.PutAsJsonAsync(
                    Config.apiPath + "/api/Product/Edit/", 
                    payload
                ); // ส่งข้อมูลใหม่ไปบันทึก (แก้ไข)
                IsSuccessStatusCode = res.IsSuccessStatusCode;
            }
            /*
            อธิบายเพิ่มเติม
            ใช้ HttpResponseMessage สำหรับเก็บค่าที่ได้จากการส่งข้อมูลไปบันทึกที่ API
            */


            if (IsSuccessStatusCode) {
                await Swal.FireAsync(
                    new SweetAlertOptions {
                        Title = "Save",
                        Text = "Save Success",
                        Icon = SweetAlertIcon.Success,
                        Timer = 1000
                    }
                );

                await FetchData();
                await CloseModal();

                Id = 0;
            }
        } 
        catch (Exception ex) 
        {
            await Swal.FireAsync(
                new SweetAlertOptions 
                {
                Title = "Error",
                Text = ex.Message,
                Icon = SweetAlertIcon.Error
                }
            );
        }
    } // สร้าง Method สำหรับบันทึกข้อมูล&แก้ไขข้อมูล

    async Task FetchData() 
    {
        try 
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using (var client = new HttpClient(handler))
            {
                list = (
                    await client.GetFromJsonAsync<List<ProductModel>>(Config.apiPath + "/api/Product/List")
                )!;
            }
            StateHasChanged();
        } 
        catch (Exception ex) 
        {
            await Swal.FireAsync(
                new SweetAlertOptions 
                {
                Title = "Error",
                Text = ex.Message,
                Icon = SweetAlertIcon.Error
                }
            );
        }
    }

    async Task Edit(ProductModel productModel)
    {
        await OpenMadal();

        Barcode = productModel.Barcode;
        Name = productModel.Name;
        Price = productModel.Price;
        Id = productModel.Id;

    }
    async Task Remove(int Id) 
    {
        SweetAlertResult button = await Swal.FireAsync(
            new SweetAlertOptions
            {
            Title = "Delete",
            Text = "จะลบข้อมูล Product ID: "+Id+" ออกจากระบบใช่หรือไม่?", 
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
            ShowConfirmButton = true
            }
        );

        if (button.IsConfirmed) 
        {
           HttpResponseMessage res = await req.DeleteAsync(
            Config.apiPath + "/api/Product/Remove/" + Id
            );

            if (res.IsSuccessStatusCode) 
            {
                await Swal.FireAsync(
                    new SweetAlertOptions
                    {
                    Title = "Delete",
                    Text = "Delete Success",
                    Icon = SweetAlertIcon.Success,
                    Timer = 1000
                    }
                );

                await FetchData();
            }
        }
    }
}
