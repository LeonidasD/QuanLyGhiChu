using System;
using System.Collections.Generic;
using System.Linq;
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

        [Route("view")]
        [HttpPost]
        public async Task<IActionResult> View(string code)
        {
            JObject jsonString = new JObject(
                new JProperty("status", "404"),
                new JProperty("message", "Không tìm thấy ghi chú")
            );

            // only accept number and letter, no special characters and spaces
            Regex myRegex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{1,50}$");
            if (code != null && myRegex.IsMatch(code))
            {
                Ghichu gc = _context.Ghichu.SingleOrDefault(p => p.HashCode == code);
                if (gc != null)
                {
                    // if note was hidden
                    if (gc.HienAn == 0)
                    {
                        jsonString = new JObject(
                            new JProperty("status", "403"),
                            new JProperty("message", "Ghi chú đã bị ẩn")
                        );
                    }
                    else
                    {
                        jsonString = new JObject(
                            new JProperty("status", "200"),
                            new JProperty("code", gc.HashCode),
                            new JProperty("title", gc.Title),
                            new JProperty("context", gc.Context),
                            new JProperty("timecreated", gc.TimeCreated.ToString("dd/MM/yyyy hh:mm:ss tt")),
                            new JProperty("timeupdated", gc.TimeUpdated.HasValue ? gc.TimeUpdated.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "N/A")
                        );
                    }
                }
            }

            return Content(jsonString.ToString());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(string title, string context)
        {
            string token = Guid.NewGuid().ToString().Replace("-", "");
            string hashCode = token.Substring(0, 10);
            DateTime dt = DateTime.Now;

            var item = new Ghichu
            {
                HashCode = hashCode,
                Token = token,
                Title = title,
                Context = context,
                TimeCreated = dt,
                TimeUpdated = null,
                HienAn = 1
            };

            await _context.Ghichu.AddAsync(item);
            await _context.SaveChangesAsync();

            JObject jsonString = new JObject(
                new JProperty("status", "200"),
                new JProperty("message", "Tạo ghi chú thành công"),
                new JProperty("code", hashCode),
                new JProperty("token", token)
            );

            return Content(jsonString.ToString());
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(string token, byte hienan = 1, string title = null, string context = null)
        {
            JObject jsonString = new JObject(
                new JProperty("status", "404"),
                new JProperty("message", "Không tìm thấy ghi chú")
            );

            Regex myRegex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{1,50}$");
            if (token != null && myRegex.IsMatch(token))
            {
                Ghichu gc = _context.Ghichu.SingleOrDefault(p => p.Token == token);
                if (gc != null)
                {
                    if (title == null && context == null)
                    {
                        jsonString = new JObject(
                            new JProperty("status", "200"),
                            new JProperty("code", gc.HashCode),
                            new JProperty("token", gc.Token),
                            new JProperty("title", gc.Title),
                            new JProperty("context", gc.Context),
                            new JProperty("timecreated", gc.TimeCreated.ToString("dd/MM/yyyy hh:mm:ss tt")),
                            new JProperty("timeupdated", gc.TimeUpdated.HasValue ? gc.TimeUpdated.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "N/A"),
                            new JProperty("hienan", gc.HienAn)
                        );
                    }
                    else
                    {
                        gc.Title = title;
                        gc.Context = context;
                        gc.TimeUpdated = DateTime.Now;
                        gc.HienAn = hienan;

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

        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string token)
        {
            JObject jsonString = new JObject(
                new JProperty("status", "404"),
                new JProperty("message", "Không tìm thấy ghi chú")
            );

            Regex myRegex = new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]{1,50}$");
            if (token != null && myRegex.IsMatch(token))
            {
                Ghichu gc = _context.Ghichu.SingleOrDefault(p => p.Token == token);
                if (gc != null)
                {
                    _context.Ghichu.Remove(gc);
                    await _context.SaveChangesAsync();

                    jsonString = new JObject(
                        new JProperty("status", "200"),
                        new JProperty("message", "Đã xoá ghi chú")
                    );
                }
            }

            return Content(jsonString.ToString());
        }
    }
}