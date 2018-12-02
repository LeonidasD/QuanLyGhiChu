using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QuanLyGhiChu.Models;

namespace QuanLyGhiChu.Controllers
{
    [Produces("application/json")]
    [Route("api/ghichu")]
    public class GhiChuApiController : ControllerBase
    {
        private readonly QuanLyGhiChuContext _context;

        public GhiChuApiController(QuanLyGhiChuContext context)
        {
            _context = context;
        }

        [HttpPost("view")]
        public IActionResult View([FromBody] Ghichu gc)
        {
            JObject jsonString = new JObject(
                new JProperty("status", "404"),
                new JProperty("message", "Không tìm thấy ghi chú")
            );

            // only accept number and letter, no special characters and spaces
            Regex myRegex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{1,50}$");
            if (gc.HashCode != null && myRegex.IsMatch(gc.HashCode))
            {
                Ghichu item = _context.Ghichu.SingleOrDefault(p => p.HashCode == gc.HashCode);
                if (item != null)
                {
                    // if note was hidden
                    if (item.HienAn == 0)
                    {
                        jsonString = new JObject(
                            new JProperty("status", "403"),
                            new JProperty("message", "Ghi chú đã bị ẩn")
                        );
                    }
                    else if (item.Password != null && (gc.Password == null || Md5Hash(gc.Password) != item.Password))
                    {
                        jsonString = new JObject(
                            new JProperty("status", "403"),
                            new JProperty("message", "Mật khẩu ghi chú không đúng")
                        );
                    }
                    else
                    {
                        jsonString = new JObject(
                            new JProperty("status", "200"),
                            new JProperty("code", item.HashCode),
                            new JProperty("title", item.Title),
                            new JProperty("context", item.Context),
                            new JProperty("timecreated", item.TimeCreated.ToString("dd/MM/yyyy hh:mm:ss tt")),
                            new JProperty("timeupdated", item.TimeUpdated.HasValue ? item.TimeUpdated.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "N/A")
                        );
                    }
                }
            }

            return Content(jsonString.ToString());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Ghichu gc)
        {
            string token = Guid.NewGuid().ToString().Replace("-", "");
            string hashCode = token.Substring(0, 10);
            DateTime dt = DateTime.Now;

            var item = new Ghichu
            {
                HashCode = hashCode,
                Token = token,
                Title = gc.Title,
                Context = gc.Context,
                Password = Md5Hash(gc.Password),
                TimeCreated = dt,
                TimeUpdated = null,
                HienAn = 1
            };

            _context.Ghichu.Add(item);
            await _context.SaveChangesAsync();

            JObject jsonString = new JObject(
                new JProperty("status", "200"),
                new JProperty("message", "Tạo ghi chú thành công"),
                new JProperty("code", hashCode),
                new JProperty("token", token)
            );

            return Content(jsonString.ToString());
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] Ghichu gc)
        {
            JObject jsonString = new JObject(
                new JProperty("status", "404"),
                new JProperty("message", "Không tìm thấy ghi chú")
            );

            Regex myRegex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{1,50}$");
            if (gc.Token != null && myRegex.IsMatch(gc.Token))
            {
                Ghichu item = _context.Ghichu.SingleOrDefault(p => p.Token == gc.Token);
                if (item != null)
                {
                    if (item.Title == null && item.Context == null)
                    {
                        jsonString = new JObject(
                            new JProperty("status", "200"),
                            new JProperty("code", item.HashCode),
                            new JProperty("token", item.Token),
                            new JProperty("title", item.Title),
                            new JProperty("context", item.Context),
                            new JProperty("password", item.Password == null ? "" : "protected"),
                            new JProperty("timecreated", item.TimeCreated.ToString("dd/MM/yyyy hh:mm:ss tt")),
                            new JProperty("timeupdated", item.TimeUpdated.HasValue ? item.TimeUpdated.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "N/A"),
                            new JProperty("hienan", item.HienAn)
                        );
                    }
                    else
                    {
                        item.Title = gc.Title;
                        item.Context = gc.Context;
                        item.Password = gc.Password == "" ? null : Md5Hash(gc.Password);
                        item.TimeUpdated = DateTime.Now;
                        item.HienAn = gc.HienAn;

                        _context.Ghichu.Update(gc);
                        await _context.SaveChangesAsync();

                        jsonString = new JObject(
                            new JProperty("status", "200"),
                            new JProperty("message", "Đã sửa ghi chú")
                        );
                    }
                }
            }

            return Content(jsonString.ToString());
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] Ghichu gc)
        {
            JObject jsonString = new JObject(
                new JProperty("status", "404"),
                new JProperty("message", "Không tìm thấy ghi chú")
            );

            Regex myRegex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{1,50}$");
            if (gc.Token != null && myRegex.IsMatch(gc.Token))
            {
                Ghichu item = _context.Ghichu.SingleOrDefault(p => p.Token == gc.Token);
                if (item != null)
                {
                    _context.Ghichu.Remove(item);
                    await _context.SaveChangesAsync();

                    jsonString = new JObject(
                        new JProperty("status", "200"),
                        new JProperty("message", "Đã xoá ghi chú")
                    );
                }
            }

            return Content(jsonString.ToString());
        }

        public static string Md5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}