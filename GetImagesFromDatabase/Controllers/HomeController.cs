using GetImagesFromDatabase.Code;
using GetImagesFromDatabase.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GetImagesFromDatabase.Controllers
{
    public class HomeController : Controller
    {

        
        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult XemAnhSever(getTenThuMuc model)
        // public ActionResult XemAnh()
        {
            if (!ModelState.IsValid)
            {
                return View("Index1", model); // Trả về trang tìm kiếm nếu có lỗi
            }

            string tenThuMuc = model.ID;
            string folderPath = "http://192.168.1.55/fileAnh"; // Đường dẫn gốc

            if (!Directory.Exists(folderPath))
            {
                ModelState.AddModelError("", "Đường dẫn gốc không tồn tại.");
                return View("Index1", model);
            }



           
            string selectedFolder = Directory.GetDirectories(folderPath)
                .FirstOrDefault(folder => Path.GetFileName(folder).Equals(tenThuMuc, StringComparison.OrdinalIgnoreCase));
            if (selectedFolder == null)
            {
                ModelState.AddModelError("", "Số Container '" + tenThuMuc + "' không tồn tại.");
                return View("Index1", model);
            }

            List<string> imgDataURLs = new List<string>();
            string[] imageFiles = Directory.GetFiles(selectedFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                            s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            s.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string imageFile in imageFiles)
            {
                try
                {
                    byte[] byteData = System.IO.File.ReadAllBytes(imageFile);
                    string imreBase64Data = Convert.ToBase64String(byteData);
                    string imgDataURL = string.Format("data:image/{0};base64,{1}", GetImageFormat(imageFile), imreBase64Data);
                    imgDataURLs.Add(imgDataURL);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi đọc file ảnh '" + Path.GetFileName(imageFile) + "': " + ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Index1", model); // Trả về trang tìm kiếm nếu có lỗi
            }

            model.ImageData = imgDataURLs; // Gán dữ liệu ảnh vào model
            return View("XemAnhSever", model); // Trả về View XemAnhSever với model
        }


        private string GetImageFormat(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "jpeg";
                case ".png":
                    return "png";
                case ".gif":
                    return "gif";
                default:
                    return "jpeg"; // Default format
            }

        }

        public async Task<ActionResult> TimKiemFolder(getTenThuMuc model)
        {
            string folderPath2 = "http://icdtancanghp.sangtaoketnoi.vn/cenContainerRepairSNPHAIPHONGGateIn";
          

          

            var folderList = await GetFolderList(folderPath2);
            string folderName = model.ID;
            if (folderList.Contains(folderName))
            {
                string folderUrl = $"{folderPath2}/{folderName}";
                var imageLinks = await GetImageLinks(folderUrl, folderPath2);
                List<string> imgDataURLs = new List<string>();
                WebClient client = new WebClient();
                foreach (string imageFile in imageLinks)
                {
                    try
                    {
                        byte[] byteData = client.DownloadData(imageFile);

                        string imreBase64Data = Convert.ToBase64String(byteData);
                        string imgDataURL = string.Format("data:image/{0};base64,{1}", GetImageFormat(imageFile), imreBase64Data);
                        imgDataURLs.Add(imgDataURL);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Lỗi khi đọc file ảnh '" + Path.GetFileName(imageFile) + "': " + ex.Message);
                    }
                }
                model.ImageData = imgDataURLs; // Lưu vào TempData


                return View("XemAnhSever", model); // Trả về View để hiển thị kết quả
            }
            else
            {
                // Xử lý trường hợp folder không tồn tại
                ModelState.AddModelError("", "Đường dẫn gốc không tồn tại.");
                return View("Index1", model);
            }
            
        }

        private async Task<List<string>> GetFolderList(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                // Logic парсинг HTML để lấy danh sách folder
                // Ví dụ: Tìm các thẻ <a> có chứa link đến folder
                var folderNodes = htmlDocument.DocumentNode.SelectNodes("//a[contains(@href, '/cenContainerRepairSNPHAIPHONGGateIn/')]");

                if (folderNodes != null)
                {
                    return folderNodes.Select(node => node.InnerText).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        private async Task<List<string>> GetImageLinks(string url, string urlImage)
        {
            string folderPath3 = "http://icdtancanghp.sangtaoketnoi.vn";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
               // var response = @"<pre><a href=""/fileAnh/"">[To Parent Directory]</a><br><br> 3/28/2024  3:38 PM       142780 <a href=""/fileAnh/123/bg.jpeg"">bg.jpeg</a><br> 3/28/2024  3:26 PM        57078 <a href=""/fileAnh/123/bg.jpg"">bg.jpg</a><br> 3/28/2024  3:28 PM      8666849 <a href=""/fileAnh/123/bg.png"">bg.png</a><br> 3/28/2024  3:36 PM      2496182 <a href=""/fileAnh/123/img-i4gDM7YPc5V8myCqrgP8t.png"">img-i4gDM7YPc5V8myCqrgP8t.png</a><br></pre><hr></body></html>";

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                // Logic парсинг HTML để lấy link ảnh
                // Ví dụ: Tìm các thẻ <img>
                var linkNodes = htmlDocument.DocumentNode.SelectNodes("//a"); // Select all <a> tags
                List<string> listurl = new List<string>();


                List<string> links = new List<string>();
                if (linkNodes != null)
                {
                    foreach (var aTag in linkNodes)
                    {
                        links.Add(aTag.GetAttributeValue("href", ""));
                      
                    }

                    links.RemoveAt(0);
                    foreach (var item in links)
                    {
                      listurl.Add(folderPath3 + item);
                    }
                    return listurl;   
                }
                else
                {
                    return new List<string>();
                }
            }

        }


    }
}