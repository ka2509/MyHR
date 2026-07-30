using ClosedXML.Excel;
using MyHr.Data.Dto;
using MyHr.Data.Enum;
using MyHr.DataImporter;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("=== MyHR Data Importer ===\n");

// Configuration
var excelFilePath = @"C:\Code\MyHR\MyHR\MyHr.Api\Data\EmployeeData.xlsx";
var apiBaseUrl = "http://localhost:5133";

// Check if file exists
if (!File.Exists(excelFilePath))
{
    Console.WriteLine($"❌ File không tồn tại: {excelFilePath}");
    Console.WriteLine("\nVui lòng đặt file EmployeeData.xlsx vào folder Data");
    return;
}

Console.WriteLine($"📂 Đọc file: {excelFilePath}\n");

// Read Excel file (all 5 sheets for 5 organizations)
var employees = ReadExcelFile(excelFilePath);
Console.WriteLine($"✅ Đã đọc được {employees.Count} nhân viên từ {employees.Select(e => e.SheetName).Distinct().Count()} sheet\n");

// Convert to JSON and display
var jsonOptions = new JsonSerializerOptions 
{ 
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

// Check for duplicates within Excel file
var duplicateCCCD = employees
    .GroupBy(e => e.SoCCCD)
    .Where(g => g.Count() > 1)
    .ToList();

var duplicateBHXH = employees
    .GroupBy(e => e.MaSoBHXH)
    .Where(g => g.Count() > 1)
    .ToList();

if (duplicateCCCD.Any() || duplicateBHXH.Any())
{
    Console.WriteLine("⚠️ Cảnh báo: Phát hiện trùng lặp trong file Excel:\n");
    
    if (duplicateCCCD.Any())
    {
        Console.WriteLine($"   🔴 Trùng CCCD ({duplicateCCCD.Count} số CCCD bị trùng):");
        foreach (var group in duplicateCCCD.Take(5))
        {
            Console.WriteLine($"      - CCCD {group.Key}: {string.Join(", ", group.Select(e => e.HoVaTen))}");
        }
        if (duplicateCCCD.Count > 5)
            Console.WriteLine($"      ... và {duplicateCCCD.Count - 5} trùng lặp khác");
    }
    
    if (duplicateBHXH.Any())
    {
        Console.WriteLine($"   🔴 Trùng BHXH ({duplicateBHXH.Count} mã BHXH bị trùng):");
        foreach (var group in duplicateBHXH.Take(5))
        {
            Console.WriteLine($"      - BHXH {group.Key}: {string.Join(", ", group.Select(e => e.HoVaTen))}");
        }
        if (duplicateBHXH.Count > 5)
            Console.WriteLine($"      ... và {duplicateBHXH.Count - 5} trùng lặp khác");
    }
    
    Console.WriteLine("\n   💡 Chỉ nhân viên đầu tiên trong mỗi nhóm trùng sẽ được import.");
    Console.WriteLine("   💡 Vui lòng kiểm tra và sửa file Excel nếu cần.\n");
}

// Remove duplicates: Keep only the first occurrence
var uniqueEmployees = employees
    .GroupBy(e => e.SoCCCD)
    .Select(g => g.First())
    .ToList();

var skippedCount = employees.Count - uniqueEmployees.Count;
if (skippedCount > 0)
{
    Console.WriteLine($"   ⚠️ Đã bỏ qua {skippedCount} nhân viên trùng lặp trong file Excel\n");
}

// Create ImportEmployeeDto list
var importDtos = ConvertToImportDtos(uniqueEmployees);

Console.WriteLine("📋 Thống kê nhân viên theo tổ chức:\n");
var groupedByOrg = uniqueEmployees.GroupBy(e => new { e.SheetName, e.OrganizationId })
    .OrderBy(g => g.Key.SheetName);

foreach (var group in groupedByOrg)
{
    Console.WriteLine($"  📁 {group.Key.SheetName} - {group.Key.OrganizationId}: {group.Count()} người");
}

Console.WriteLine($"\n📊 Tổng cộng: {importDtos.Count} nhân viên\n");

// Ask user to proceed with import
Console.WriteLine(new string('=', 70));
Console.WriteLine($"\n💡 Sẵn sàng import {importDtos.Count} nhân viên vào hệ thống qua API: {apiBaseUrl}");
Console.WriteLine($"   Đảm bảo API đang chạy trước khi tiếp tục!");
Console.WriteLine($"\n   📌 Lưu ý: Nhân viên đã tồn tại (trùng CCCD/BHXH) sẽ tự động bỏ qua.\n");
Console.Write("Bạn có muốn tiếp tục import? (y/n): ");
var answer = Console.ReadLine()?.ToLower();

if (answer == "y" || answer == "yes")
{
    await ImportEmployeesAsync(importDtos, apiBaseUrl);
}
else
{
    Console.WriteLine("❌ Đã hủy import.");
}

Console.WriteLine("\n=== Hoàn thành ===");

// ============================================
// Helper Methods
// ============================================

List<ExcelEmployeeRow> ReadExcelFile(string filePath)
{
    var result = new List<ExcelEmployeeRow>();
    
    using var workbook = new XLWorkbook(filePath);
    
    // Process ALL worksheets (5 sheets for 5 organizations)
    foreach (var worksheet in workbook.Worksheets)
    {
        Console.WriteLine($"  📄 Đọc sheet: {worksheet.Name}");
        
        var sheetName = worksheet.Name;
        string? currentSection = null;
        
        // Find data rows (skip header row)
        var rows = worksheet.RowsUsed().Skip(1);
        
        foreach (var row in rows)
        {
            try
            {
                var sttValue = row.Cell(1).GetString().Trim();
                
                // Check if this is a section header row (no STT number)
                if (string.IsNullOrWhiteSpace(sttValue) || !int.TryParse(sttValue, out int stt))
                {
                    // This might be a section header (e.g., "Phòng Kế hoạch", "Cụm Trà Lĩnh")
                    var fullNameCell = row.Cell(2).GetString().Trim();
                    if (!string.IsNullOrWhiteSpace(fullNameCell) && 
                        (fullNameCell.StartsWith("Phòng", StringComparison.OrdinalIgnoreCase) ||
                         fullNameCell.StartsWith("Cụm", StringComparison.OrdinalIgnoreCase) ||
                         fullNameCell.StartsWith("Tổ", StringComparison.OrdinalIgnoreCase) ||
                         fullNameCell.StartsWith("Ban", StringComparison.OrdinalIgnoreCase)))
                    {
                        currentSection = fullNameCell;
                        Console.WriteLine($"    → Tìm thấy phần: {currentSection}");
                    }
                    continue;
                }
                
                // Parse employee data row
                var employee = new ExcelEmployeeRow
                {
                    STT = stt,
                    SheetName = sheetName,
                    SectionHeader = currentSection,
                    HoVaTen = row.Cell(2).GetString().Trim(),
                    GioiTinhNu = row.Cell(3).GetString().Trim().ToLower() == "x",
                    MaSoBHXH = row.Cell(4).GetString().Trim(),
                    NgayThangNamSinh = ParseDate(row.Cell(5).GetString()),
                    SoCCCD = row.Cell(6).GetString().Trim(),
                    ThoiGianDongBHXH = ParseDateMonthYear(row.Cell(7).GetString()),
                    ChuyenMonNghiepVu = row.Cell(8).GetString().Trim(),
                    TrinhDo = row.Cell(9).GetString().Trim(),
                    BacMoi = ParseGradeLevel(row.Cell(10).GetString()),
                    HeSoMoi = ParseDecimal(row.Cell(11).GetString()),
                    PCTN = ParseNullableDecimal(row.Cell(12).GetString()),
                    PCCV = ParseNullableDecimal(row.Cell(13).GetString()),
                    LuongMoi = ParseSalary(row.Cell(15))
                };
                
                // Debug: Display allowance values
                Console.WriteLine($"      DEBUG: {employee.HoVaTen} - PCTN={employee.PCTN?.ToString() ?? "null"}, PCCV={employee.PCCV?.ToString() ?? "null"}");
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(employee.HoVaTen) || 
                    string.IsNullOrWhiteSpace(employee.SoCCCD) ||
                    string.IsNullOrWhiteSpace(employee.MaSoBHXH))
                {
                    Console.WriteLine($"⚠️ Bỏ qua dòng {row.RowNumber()} - Thiếu thông tin bắt buộc: {employee.HoVaTen}");
                    continue;
                }
                
                // Map to IDs using sheet name and section info
                employee.OrganizationId = MapOrganization(employee);
                employee.PositionId = MapPosition(employee.ChuyenMonNghiepVu);
                employee.ProfessionId = MapProfession(employee.TrinhDo);
                
                // Debug: Show salary data for Ban điều hành employees
                if (employee.OrganizationId == "cty-bdh")
                {
                    Console.WriteLine($"      👔 EXECUTIVE: {employee.HoVaTen}");
                    Console.WriteLine($"         Section: {currentSection}");
                    Console.WriteLine($"         Row number: {row.RowNumber()}");
                    Console.WriteLine($"         Cell 11 (Hệ số mới) raw: '{row.Cell(11).GetString()}'");
                    Console.WriteLine($"         Cell 14 (Lương mới) raw: '{row.Cell(14).GetString()}'");
                    Console.WriteLine($"         Parsed HeSoMoi (cell 11): {employee.HeSoMoi}");
                    Console.WriteLine($"         Parsed LuongMoi (cell 14): {employee.LuongMoi}");
                    Console.WriteLine($"         Position: {employee.ChuyenMonNghiepVu} -> {employee.PositionId}");
                }
                
                result.Add(employee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi đọc dòng {row.RowNumber()} trong sheet '{sheetName}': {ex.Message}");
            }
        }
    }
    
    return result;
}

DateTime ParseDate(string value)
{
    if (DateTime.TryParse(value, out DateTime result))
        return result;
    
    // Try parse format d/M/yyyy
    if (DateTime.TryParseExact(value, new[] { "d/M/yyyy", "dd/MM/yyyy", "M/d/yyyy" }, 
        null, System.Globalization.DateTimeStyles.None, out result))
        return result;
    
    return DateTime.MinValue;
}

DateTime ParseDateMonthYear(string value)
{
    // Format: M/yyyy or MM/yyyy
    var parts = value.Split('/');
    if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
    {
        return new DateTime(year, month, 1);
    }
    return ParseDate(value);
}

int ParseGradeLevel(string value)
{
    // Format: "1/6" or "4/8" - take first number
    var parts = value.Split('/');
    if (parts.Length > 0 && int.TryParse(parts[0], out int grade))
        return grade;
    return 1;
}

decimal ParseDecimal(string value)
{
    value = value.Replace(",", ".").Trim();
    if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, 
        System.Globalization.CultureInfo.InvariantCulture, out decimal result))
        return result;
    return 0;
}

decimal? ParseNullableDecimal(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        return null;
    var result = ParseDecimal(value);
    return result == 0 ? null : result;
}

decimal ParseSalary(ClosedXML.Excel.IXLCell cell)
{
    try
    {
        // Debug: Show cell information
        var cellAddress = cell.Address.ToString();
        var cellType = cell.DataType.ToString();
        
        // Try to get as number first (for formula cells or numeric values)
        if (cell.TryGetValue(out double numericValue))
        {
            Console.WriteLine($"         [DEBUG ParseSalary] Cell {cellAddress}: Type={cellType}, Numeric={numericValue}");
            return (decimal)numericValue;
        }
        
        // Try to get as string and parse
        var stringValue = cell.GetString();
        Console.WriteLine($"         [DEBUG ParseSalary] Cell {cellAddress}: Type={cellType}, String='{stringValue}'");
        if (!string.IsNullOrWhiteSpace(stringValue))
        {
            var parsed = ParseDecimal(stringValue);
            Console.WriteLine($"         [DEBUG ParseSalary] Parsed string '{stringValue}' to {parsed}");
            return parsed;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"         ⚠️ [DEBUG ParseSalary] Failed to parse salary from cell: {ex.Message}");
    }
    
    Console.WriteLine($"         [DEBUG ParseSalary] Returning 0 (no value found)");
    return 0;
}

string? MapOrganization(ExcelEmployeeRow employee)
{
    // Map based on sheet name and section header
    var sheetName = employee.SheetName.ToLower();
    var section = employee.SectionHeader?.Trim() ?? "";
    
    // Determine main organization from sheet name
    string mainOrgId;
    
    if (sheetName.Contains("công ty") || sheetName.Contains("thủy lợi bắc") || sheetName.Contains("cty") || sheetName == "1" || sheetName.StartsWith("sheet1"))
    {
        mainOrgId = "cty-tlb"; // Công ty Thủy lợi Bắc
    }
    else if (sheetName.Contains("hưng hà") || sheetName == "2" || sheetName.StartsWith("sheet2"))
    {
        mainOrgId = "xn-hungha"; // Xí nghiệp Hưng Hà
    }
    else if (sheetName.Contains("đông hưng") || sheetName == "3" || sheetName.StartsWith("sheet3"))
    {
        mainOrgId = "xn-donghung"; // Xí nghiệp Đông Hưng
    }
    else if (sheetName.Contains("quỳnh phụ") || sheetName == "4" || sheetName.StartsWith("sheet4"))
    {
        mainOrgId = "xn-quynhphu"; // Xí nghiệp Quỳnh Phụ
    }
    else if (sheetName.Contains("thái thụy") || sheetName == "5" || sheetName.StartsWith("sheet5"))
    {
        mainOrgId = "xn-thaithuy"; // Xí nghiệp Thái Thụy
    }
    else
    {
        Console.WriteLine($"⚠️ WARNING: Sheet name '{employee.SheetName}' không khớp với pattern nào, mặc định về cty-tlb");
        mainOrgId = "cty-tlb"; // Default to main company
    }
    
    // If no section header, return main organization
    if (string.IsNullOrWhiteSpace(section))
    {
        return mainOrgId;
    }
    
    // Map section to sub-organization
    return MapSubOrganization(mainOrgId, section);
}

string MapSubOrganization(string mainOrgId, string sectionName)
{
    var section = sectionName.ToLower().Trim();
    
    // ============== CÔNG TY THỦY LỢI BẮC (cty-tlb) ==============
    if (mainOrgId == "cty-tlb")
    {
        // Ban điều hành
        if (section.Contains("ban điều hành") || section.Contains("ban dieu hanh"))
            return "cty-bdh";
        
        // Phòng TC-HC
        if (section.Contains("phòng tc") && section.Contains("hc") || 
            section.Contains("phòng tổ chức") && section.Contains("hành chính"))
            return "cty-tchc";
        
        // Phòng Tài Vụ
        if (section.Contains("phòng tài vụ") || section.Contains("phong tai vu"))
            return "cty-taivu";
        
        // Phòng KH - KT
        if (section.Contains("phòng kh") || 
            (section.Contains("phòng kế hoạch") && section.Contains("kỹ thuật")))
            return "cty-khkt";
        
        // Phòng QLN&CT
        if (section.Contains("phòng qln") || section.Contains("qlnct") ||
            section.Contains("quản lý nước") || section.Contains("công trình"))
            return "cty-qlnct";
        
        // Phòng Cơ điện
        if (section.Contains("phòng cơ điện"))
            return "cty-codien";
        
        // Các Cụm
        if (section.Contains("cụm trà linh") || section.Contains("cum tra linh"))
            return "cty-cum-tralinh";
        if (section.Contains("cụm hiệp") || section.Contains("cum hiep"))
            return "cty-cum-hiep";
        if (section.Contains("cụm thuyền quan") || section.Contains("cum thuyen quan"))
            return "cty-cum-thuyenquan";
        if (section.Contains("cụm đồng cống") || section.Contains("cum dong cong"))
            return "cty-cum-dongcong";
        if (section.Contains("cụm nhâm lang") || section.Contains("cum nham lang"))
            return "cty-cum-nhamlang";
    }
    
    // ============== XÍ NGHIỆP HƯNG HÀ (xn-hungha) ==============
    if (mainOrgId == "xn-hungha")
    {
        // Các Tổ
        if (section.Contains("tổ tc") && section.Contains("hc") ||
            (section.Contains("tổ tổ chức") && section.Contains("hành chính")))
            return "xn-hh-to-tchc";
        if (section.Contains("tổ kế toán"))
            return "xn-hh-to-ketoan";
        if (section.Contains("tổ kế hoạch") && section.Contains("kỹ thuật"))
            return "xn-hh-to-kythuat";
        if (section.Contains("tổ cơ khí") && section.Contains("cơ điện"))
            return "xn-hh-to-cokhi";
        
        // Các Cụm
        if (section.Contains("cụm việt yên") || section.Contains("cum viet yen"))
            return "xn-hh-cum-vietyen";
        if (section.Contains("cụm trạm chay") || section.Contains("cum tram chay"))
            return "xn-hh-cum-tramchay";
        if (section.Contains("cụm tịnh xuyên") || section.Contains("cum tinh xuyen"))
            return "xn-hh-cum-tinhxuyen";
        if (section.Contains("cụm đào thành") || section.Contains("cum dao thanh"))
            return "xn-hh-cum-daothanh";
        if (section.Contains("cụm lão khê") || section.Contains("cum lao khe"))
            return "xn-hh-cum-laokhe";
        if (section.Contains("cụm minh tân") || section.Contains("cum minh tan"))
            return "xn-hh-cum-minhtan";
    }
    
    // ============== XÍ NGHIỆP ĐÔNG HƯNG (xn-donghung) ==============
    if (mainOrgId == "xn-donghung")
    {
        // Các Tổ
        if (section.Contains("tổ tc") && section.Contains("hc") ||
            (section.Contains("tổ tổ chức") && section.Contains("hành chính")))
            return "xn-dh-to-tchc";
        if (section.Contains("tổ kế toán"))
            return "xn-dh-to-ketoan";
        if (section.Contains("tổ kế hoạch") && section.Contains("kỹ thuật"))
            return "xn-dh-to-khkt";
        if (section.Contains("tổ cơ khí") && section.Contains("cơ điện"))
            return "xn-dh-to-cokhicodien";
        
        // Các Cụm
        if (section.Contains("cụm hậu thượng") || section.Contains("cum hau thuong"))
            return "xn-dh-cum-hauthuong";
        if (section.Contains("cụm thống nhất 1") || section.Contains("cum thong nhat 1"))
            return "xn-dh-cum-thongnhat1";
        if (section.Contains("cụm quán rô") || section.Contains("cum quan ro"))
            return "xn-dh-cum-quanro";
        if (section.Contains("cụm 217") || section.Contains("cum 217"))
            return "xn-dh-cum-217";
        if (section.Contains("cụm thống nhất 2") || section.Contains("cum thong nhat 2"))
            return "xn-dh-cum-thongnhat2";
        if (section.Contains("cụm quan hoả") || section.Contains("cum quan hoa"))
            return "xn-dh-cum-quanhoa";
        if (section.Contains("cụm sông hoài") || section.Contains("cum song hoai"))
            return "xn-dh-cum-songhoai";
    }
    
    // ============== XÍ NGHIỆP QUỲNH PHỤ (xn-quynhphu) ==============
    if (mainOrgId == "xn-quynhphu")
    {
        // Các Tổ
        if (section.Contains("tổ tc") && section.Contains("hc") ||
            (section.Contains("tổ tổ chức") && section.Contains("hành chính")))
            return "xn-qp-to-tchc";
        if (section.Contains("tổ kế toán"))
            return "xn-qp-to-ketoan";
        if (section.Contains("tổ kế hoạch") && section.Contains("kỹ thuật"))
            return "xn-qp-to-khkt";
        if (section.Contains("tổ cơ khí") && section.Contains("cơ điện"))
            return "xn-qp-to-cokhicodien";
        
        // Các Cụm
        if (section.Contains("cụm tân mỹ") || section.Contains("cum tan my"))
            return "xn-qp-cum-tanmy";
        if (section.Contains("cụm đông yên lộng") || section.Contains("cum dong yen long"))
            return "xn-qp-cum-dongyenlong";
        if (section.Contains("cụm đại nẫm") || section.Contains("cum dai nam"))
            return "xn-qp-cum-dainam";
        if (section.Contains("cụm cao nội") || section.Contains("cum cao noi"))
            return "xn-qp-cum-caonoi";
        if (section.Contains("cụm tây yên lộng") || section.Contains("cum tay yen long"))
            return "xn-qp-cum-tayyenlong";
        if (section.Contains("cụm neo") || section.Contains("cum neo"))
            return "xn-qp-cum-neo";
        if (section.Contains("cụm trang xá") || section.Contains("cum trang xa"))
            return "xn-qp-cum-trangxa";
        if (section.Contains("cụm quỳnh hoa") || section.Contains("cum quynh hoa"))
            return "xn-qp-cum-quynhhoa";
    }
    
    // ============== XÍ NGHIỆP THÁI THỤY (xn-thaithuy) ==============
    if (mainOrgId == "xn-thaithuy")
    {
        // Các Tổ
        if (section.Contains("tổ tc") && section.Contains("hc") ||
            (section.Contains("tổ tổ chức") && section.Contains("hành chính")))
            return "xn-tt-to-tchc";
        if (section.Contains("tổ kế toán"))
            return "xn-tt-to-ketoan";
        if (section.Contains("tổ kế hoạch") && section.Contains("kỹ thuật"))
            return "xn-tt-to-khkt";
        if (section.Contains("tổ cơ khí") && section.Contains("cơ điện"))
            return "xn-tt-to-cokhicodien";
        
        // Các Cụm
        if (section.Contains("cụm thái phúc") || section.Contains("cum thai phuc"))
            return "xn-tt-cum-thaiphuc";
        if (section.Contains("cụm thái hồng") || section.Contains("cum thai hong"))
            return "xn-tt-cum-thaihong";
        if (section.Contains("cụm tn hệ") || section.Contains("cum tn he"))
            return "xn-tt-cum-tnhe";
        if (section.Contains("cụm thái học") || section.Contains("cum thai hoc"))
            return "xn-tt-cum-thaihoc";
        if (section.Contains("cụm tn thái đô") || section.Contains("cum tn thai do"))
            return "xn-tt-cum-tnthaido";
        if (section.Contains("cụm phong lẫm") || section.Contains("cum phong lam"))
            return "xn-tt-cum-phonglam";
        if (section.Contains("cụm tn thụy quỳnh") || section.Contains("cum tn thuy quynh"))
            return "xn-tt-cum-tnthuyyquynh";
        if (section.Contains("cụm tn trường xuân hà") || section.Contains("cum tn truong xuan ha"))
            return "xn-tt-cum-tntruongxuanha";
    }
    
    // Default to main organization if no match
    Console.WriteLine($"⚠️ WARNING: Section '{sectionName}' in {mainOrgId} không khớp với sub-organization nào, trả về {mainOrgId}");
    return mainOrgId;
}

string? MapPosition(string positionName)
{
    var name = positionName.ToLower();
    
    // Exact matches first (most specific)
    if (name.Contains("chủ tịch")) return "pos-ctcty";
    if (name.Contains("giám đốc xí nghiệp") || name.Contains("giám đốc xi nghiệp")) return "pos-gdxn";
    if (name.Contains("phó giám đốc xí nghiệp") || name.Contains("phó giám đốc xi nghiệp")) return "pos-pgdxn";
    if (name.Contains("giám đốc")) return "pos-gd";
    if (name.Contains("phó giám đốc")) return "pos-pgd";
    if (name.Contains("kiểm soát viên")) return "pos-ksv";
    if (name.Contains("kế toán trưởng")) return "pos-ktt";
    
    // Trưởng phòng (specific)
    if (name.Contains("trưởng phòng hành chính") || name.Contains("tp hành chính")) return "pos-tp-hc";
    if (name.Contains("trưởng phòng kế hoạch") || name.Contains("tp kế hoạch")) return "pos-tp-kh";
    if (name.Contains("trưởng phòng quản lý nước") || name.Contains("trưởng phòng ql nước")) return "pos-tp-qln";
    if (name.Contains("trưởng phòng cơ điện") || name.Contains("tp cơ điện")) return "pos-tp-cd";
    
    // Phó phòng (specific)
    if (name.Contains("phó phòng hành chính") || name.Contains("pp hành chính")) return "pos-pp-hc";
    if (name.Contains("phó phòng kế hoạch") || name.Contains("pp kế hoạch")) return "pos-pp-kh";
    if (name.Contains("phó phòng quản lý nước") || name.Contains("phó phòng ql nước")) return "pos-pp-qln";
    if (name.Contains("phó phòng cơ điện") || name.Contains("pp cơ điện")) return "pos-pp-cd";
    if (name.Contains("phó phòng")) return "pos-pp";
    
    // Cụm trưởng/phó
    if (name.Contains("cụm trưởng")) return "pos-cumtruong";
    if (name.Contains("cụm phó")) return "pos-cumpho";
    if (name.Contains("cống trưởng")) return "pos-congtruong";
    if (name.Contains("trạm trưởng")) return "pos-tramtruong";
    
    // Tổ trưởng (specific)
    if (name.Contains("tổ trưởng") && name.Contains("hành chính")) return "pos-tt-hc";
    if (name.Contains("tổ trưởng") && name.Contains("kế toán")) return "pos-tt-kt";
    if (name.Contains("tổ trưởng") && name.Contains("kế hoạch kỹ thuật")) return "pos-tt-khkt";
    if (name.Contains("tổ trưởng") && name.Contains("kế hoạch")) return "pos-tt-kh";
    if (name.Contains("tổ trưởng") && name.Contains("sửa chữa")) return "pos-tt-sc";
    if (name.Contains("tổ trưởng")) return "pos-totruong";
    if (name.Contains("tổ phó")) return "pos-tp-khkt";
    
    // Kế toán (specific)
    if (name.Contains("kế toán lđtl") || name.Contains("kế toán ldtl")) return "pos-kt-ldtl";
    if (name.Contains("kế toán xdcb")) return "pos-kt-xdcb";
    if (name.Contains("kế toán")) return "pos-kt";
    
    // Cán bộ
    if (name.Contains("cán bộ kế hoạch tdct")) return "pos-cb-kh-tdct";
    if (name.Contains("cán bộ kế hoạch kt")) return "pos-cb-kh-kt";
    if (name.Contains("cán bộ kỹ thuật cụm")) return "pos-cb-kt-cum";
    
    // Nhân viên (specific)
    if (name.Contains("nhân viên kế hoạch")) return "pos-nv-kh";
    if (name.Contains("nhân viên tổ khkt")) return "pos-nv-to-khkt";
    if (name.Contains("nhân viên tổ kế hoạch kt")) return "pos-nv-to-khkt2";
    if (name.Contains("nhân viên tổ kế hoạch")) return "pos-nv-to-kh";
    if (name.Contains("nhân viên hành chính")) return "pos-nv-hc";
    
    // Quản lý/Quản trị
    if (name.Contains("quản trị hành chính")) return "pos-qthc";
    if (name.Contains("quản lý ktct")) return "pos-ql-ktct";
    
    // Văn phòng
    if (name.Contains("văn thư")) return "pos-vanthu";
    if (name.Contains("trung cấp văn thư")) return "pos-tcvt";
    if (name.Contains("thủ kho-quỹ") || name.Contains("thủ kho quỹ")) return "pos-thukho-quy";
    if (name.Contains("thủ kho")) return "pos-thukho";
    if (name.Contains("thủ quỹ")) return "pos-thuquy";
    if (name.Contains("kho quỹ")) return "pos-khoquy";
    
    // Công nhân (specific)
    if (name.Contains("công nhân quản lý thủy nông") || name.Contains("cn quản lý thủy nông")) return "pos-cn-qlthuynong";
    if (name.Contains("công nhân cơ điện") || name.Contains("cn cơ điện")) return "pos-cn-codien";
    if (name.Contains("công nhân hàn điện") || name.Contains("cn hàn điện")) return "pos-cn-handien";
    if (name.Contains("công nhân điện") || name.Contains("cn điện")) return "pos-cn-dien";
    if (name.Contains("công nhân vận hành bơm") || name.Contains("cn vận hành bơm")) return "pos-cn-vhbd";
    if (name.Contains("công nhân sửa chữa") || name.Contains("cn sửa chữa")) return "pos-cn-suachua";
    if (name.Contains("công nhân lái xe") || name.Contains("cn lái xe")) return "pos-cn-laixe";
    
    // Khác
    if (name.Contains("cấp dưỡng")) return "pos-capduong";
    if (name.Contains("lái xe con")) return "pos-laixecon";
    if (name.Contains("lái xe")) return "pos-laixe";
    if (name.Contains("bảo vệ")) return "pos-baove";
    if (name.Contains("tạp vụ")) return "pos-tapvu";
    
    // Generic fallbacks
    if (name.Contains("nhân viên")) return "pos-nv";
    if (name.Contains("công nhân")) return "pos-cn-qlthuynong";
    
    return "pos-nv"; // Default
}

string? MapProfession(string professionName)
{
    var name = professionName.ToLower();
    
    // Thạc sĩ / Đại học
    if (name.Contains("thạc sĩ") || name.Contains("thạc sỹ")) return "prof-thacsi";
    if (name.Contains("đại học bách khoa")) return "prof-dhbk";
    
    // Cử nhân / Kỹ sư
    if (name.Contains("cử nhân quản trị tài chính")) return "prof-cnqttc";
    if (name.Contains("cử nhân kế toán")) return "prof-cnkt2";
    if (name.Contains("cử nhân kinh tế")) return "prof-cnkt";
    if (name.Contains("kỹ sư thủy lợi") || name.Contains("kỹ sư thuỷ lợi")) return "prof-kstl";
    if (name.Contains("kỹ sư quản lý đất đai") || name.Contains("kỹ sư đất đai")) return "prof-ksqldd";
    if (name.Contains("kỹ sư giao thông vận tải")) return "prof-ksgtvt";
    if (name.Contains("kỹ sư điện")) return "prof-ksdien";
    if (name.Contains("kỹ sư xây dựng")) return "prof-ksxd";
    
    // Cao đẳng / Trung cấp
    if (name.Contains("cao đẳng kế toán")) return "prof-cdkt";
    if (name.Contains("cao đẳng thủy lợi")) return "prof-cdtl";
    if (name.Contains("cao đẳng điện")) return "prof-cddien";
    if (name.Contains("trung cấp kế toán")) return "prof-tckt";
    if (name.Contains("trung cấp thủy lợi")) return "prof-tctl";
    if (name.Contains("trung cấp văn thư")) return "prof-tcvt";
    if (name.Contains("trung cấp điện")) return "prof-tcdien";
    
    // Công nhân
    if (name.Contains("công nhân quản lý thủy nông") || name.Contains("công nhân quản lý thuỷ nông")) return "prof-cnqltn";
    if (name.Contains("công nhân cơ điện")) return "prof-cncodien";
    if (name.Contains("công nhân hàn điện")) return "prof-cnhandien";
    if (name.Contains("công nhân sửa chữa cơ khí cơ điện")) return "prof-cnscckcd";
    if (name.Contains("công nhân cơ khí, cơ điện") || name.Contains("công nhân cơ khí cơ điện")) return "prof-cnckcd";
    if (name.Contains("công nhân điện")) return "prof-cndien";
    if (name.Contains("công nhân vận hành bơm điện")) return "prof-cnvhbd";
    if (name.Contains("công nhân sửa chữa")) return "prof-cnsc";
    if (name.Contains("công nhân lái xe")) return "prof-cnlx";
    
    // Lái xe
    if (name.Contains("lái xe con")) return "prof-lxcon";
    if (name.Contains("lái xe")) return "prof-cnlx";
    
    // Default: Công nhân quản lý thủy nông
    return "prof-cnqltn";
}

String? MapAllowance(decimal? pctn, decimal? pccv)
{
    // PCTN = Phụ cấp trách nhiệm (Responsibility Allowance) - Type 1
    // PCCV = Phụ cấp công việc (Job Allowance) - Type 2
    // An employee can only have ONE allowance, check both columns
    
    // Check Responsibility Allowance (PCTN) first
    if (pctn.HasValue && pctn.Value > 0)
    {
        return pctn.Value switch
        {
            0.5m => "allow-resp-1",  // Responsibility Level 1 (0.5)
            0.3m => "allow-resp-2",  // Responsibility Level 2 (0.3)
            _ => null  // Unknown coefficient
        };
    }
    
    // Check Job Allowance (PCCV) if no responsibility allowance
    if (pccv.HasValue && pccv.Value > 0)
    {
        return pccv.Value switch
        {
            0.2m => "allow-job-1",   // Job Level 1 (0.2)
            0.1m => "allow-job-2",   // Job Level 2 (0.1)
            _ => null  // Unknown coefficient
        };
    }
    
    return null;  // No allowance
}

List<ImportEmployeeDto> ConvertToImportDtos(List<ExcelEmployeeRow> employees)
{
    return employees.Select(e => {
        // Check if employee is in Ban điều hành (executive with fixed salary)
        var isExecutive = e.OrganizationId == "cty-bdh"; // Ban điều hành organization ID
        
        // For executives, use LuongMoi as fixed salary
        // For regular employees, LuongMoi is calculated and not used during import
        decimal? fixedSalary = null;
        if (isExecutive)
        {
            if (e.LuongMoi > 0)
            {
                fixedSalary = e.LuongMoi;
                Console.WriteLine($"   💰 {e.HoVaTen}: Fixed salary from Excel = {fixedSalary:N0}");
            }
            else if (e.HeSoMoi > 0)
            {
                // Calculate from coefficient if LuongMoi is not available
                fixedSalary = 2340000m * e.HeSoMoi;
                Console.WriteLine($"   💰 {e.HoVaTen}: Calculated fixed salary = 2,340,000 × {e.HeSoMoi} = {fixedSalary:N0}");
            }
            else
            {
                Console.WriteLine($"   ⚠️ ERROR: {e.HoVaTen} - Executive has no salary data in Excel (Column 14 or 11)");
            }
        }
        
        return new ImportEmployeeDto
        {
            FullName = e.HoVaTen,
            Sex = e.GioiTinhNu ? Sex.Female : Sex.Male,
            SocialInsurance = e.MaSoBHXH,
            Dob = e.NgayThangNamSinh,
            IdentityCardNumber = e.SoCCCD,
            SocialInsuranceContributionDate = e.ThoiGianDongBHXH,
            OrganizationId = e.OrganizationId ?? "cty-tlb",
            PositionId = e.PositionId ?? "pos-nv",
            ProfessionId = e.ProfessionId ?? "prof-cnqltn",
            AllowanceId = MapAllowance(e.PCTN, e.PCCV),
            CurrentGradeLevel = isExecutive ? 0 : e.BacMoi, // Grade level is 0 for executives
            SalaryEffectiveFrom = DateTime.Now,
            SalaryReason = "Import từ Excel",
            FixedSalaryAmount = fixedSalary // From column 14 (LuongMoi) or calculated from column 11 (HeSoMoi)
        };
    }).ToList();
}

async Task ImportEmployeesAsync(List<ImportEmployeeDto> employees, string baseUrl)
{
    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(baseUrl);
    httpClient.Timeout = TimeSpan.FromSeconds(30);
    
    var successCount = 0;
    var duplicateCount = 0;
    var validationErrorCount = 0;
    var otherErrorCount = 0;
    var errors = new Dictionary<string, List<(string name, string detail, string? json)>>
    {
        ["Duplicate"] = new List<(string, string, string?)>(),
        ["Validation"] = new List<(string, string, string?)>(),
        ["Other"] = new List<(string, string, string?)>()
    };
    
    Console.WriteLine($"\n🚀 Bắt đầu import {employees.Count} nhân viên...\n");
    
    for (int i = 0; i < employees.Count; i++)
    {
        var employee = employees[i];
        try
        {
            var json = JsonSerializer.Serialize(employee, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("/api/Employees", content);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ [{i + 1}/{employees.Count}] {employee.FullName} - CCCD: {employee.IdentityCardNumber}");
                successCount++;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                
                // Check if it's a duplicate error
                if (errorResponse.Contains("UQ_Employee_IdentityCardNumber") || 
                    errorResponse.Contains("UQ_Employee_SocialInsurance") ||
                    errorResponse.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"⚠️ [{i + 1}/{employees.Count}] {employee.FullName} - ĐÃ TỒN TẠI (CCCD: {employee.IdentityCardNumber})");
                    errors["Duplicate"].Add((employee.FullName, $"CCCD: {employee.IdentityCardNumber}", null));
                    duplicateCount++;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Extract error details
                    var shortError = ExtractErrorMessage(errorResponse);
                    Console.WriteLine($"❌ [{i + 1}/{employees.Count}] {employee.FullName} - LỖI DỮ LIỆU: {shortError}");
                    errors["Validation"].Add((employee.FullName, shortError, json));
                    validationErrorCount++;
                }
                else
                {
                    Console.WriteLine($"❌ [{i + 1}/{employees.Count}] {employee.FullName} - LỖI: {response.StatusCode}");
                    errors["Other"].Add((employee.FullName, response.StatusCode.ToString(), json));
                    otherErrorCount++;
                }
            }
        }
        catch (Exception ex)
        {
            var json = JsonSerializer.Serialize(employee, jsonOptions);
            Console.WriteLine($"❌ [{i + 1}/{employees.Count}] {employee.FullName} - NGOẠI LỆ: {ex.Message}");
            errors["Other"].Add((employee.FullName, ex.Message, json));
            otherErrorCount++;
        }
        
        // Small delay to avoid overwhelming the API
        if (i < employees.Count - 1)
            await Task.Delay(50);
    }
    
    Console.WriteLine($"\n{new string('=', 70)}");
    Console.WriteLine($"📊 Kết quả import:");
    Console.WriteLine($"   ✅ Thành công: {successCount}");
    Console.WriteLine($"   ⚠️ Đã tồn tại (bỏ qua): {duplicateCount}");
    Console.WriteLine($"   ❌ Lỗi dữ liệu: {validationErrorCount}");
    Console.WriteLine($"   ❌ Lỗi khác: {otherErrorCount}");
    Console.WriteLine($"   📈 Tổng: {employees.Count}");
    
    if (errors["Duplicate"].Any())
    {
        Console.WriteLine($"\n⚠️ Nhân viên đã tồn tại trong hệ thống ({duplicateCount}):");
        foreach (var error in errors["Duplicate"].Take(5))
        {
            Console.WriteLine($"   • {error.name} - {error.detail}");
        }
        if (errors["Duplicate"].Count > 5)
            Console.WriteLine($"   ... và {errors["Duplicate"].Count - 5} người khác");
    }
    
    if (errors["Validation"].Any())
    {
        Console.WriteLine($"\n❌ Lỗi dữ liệu/validation ({validationErrorCount}):");
        foreach (var error in errors["Validation"])
        {
            Console.WriteLine($"\n   • {error.name}:");
            Console.WriteLine($"     Lỗi: {error.detail}");
            if (!string.IsNullOrEmpty(error.json))
            {
                Console.WriteLine($"     Dữ liệu gửi lên:");
                Console.WriteLine($"     {error.json.Replace("\n", "\n     ")}");
            }
        }
    }
    
    if (errors["Other"].Any())
    {
        Console.WriteLine($"\n❌ Lỗi khác ({otherErrorCount}):");
        foreach (var error in errors["Other"])
        {
            Console.WriteLine($"\n   • {error.name}:");
            Console.WriteLine($"     Lỗi: {error.detail}");
            if (!string.IsNullOrEmpty(error.json))
            {
                Console.WriteLine($"     Dữ liệu: {error.json.Substring(0, Math.Min(error.json.Length, 200))}...");
            }
        }
    }
}

string ExtractErrorMessage(string errorResponse)
{
    // Try to extract meaningful error from the response
    try
    {
        // Check for specific known errors
        if (errorResponse.Contains("SalaryGrade not found for SalaryScaleId"))
        {
            // Extract scale and level info if possible
            if (errorResponse.Contains("and GradeLevel"))
            {
                return $"Không tìm thấy bậc lương - Chi tiết: {errorResponse.Substring(0, Math.Min(errorResponse.Length, 300))}";
            }
            return "Không tìm thấy bậc lương phù hợp với ngạch lương và bậc hiện tại";
        }
        
        if (errorResponse.Contains("Could not determine SalaryScaleId"))
            return "Không xác định được ngạch lương (cả Position và Profession đều không có ngạch)";
        
        if (errorResponse.Contains("FK_Employee_Organization"))
            return $"Mã tổ chức không hợp lệ - Kiểm tra OrganizationId";
        
        if (errorResponse.Contains("FK_Employee_Position"))
            return $"Mã chức vụ không hợp lệ - Kiểm tra PositionId";
        
        if (errorResponse.Contains("FK_Employee_Profession"))
            return $"Mã trình độ không hợp lệ - Kiểm tra ProfessionId";
        
        // Try to extract from ASP.NET error format
        if (errorResponse.Contains("\"title\""))
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(errorResponse);
                if (jsonDoc.RootElement.TryGetProperty("title", out var title))
                {
                    return title.GetString() ?? errorResponse;
                }
            }
            catch { }
        }
        
        // SQL constraint violations
        if (errorResponse.Contains("FOREIGN KEY constraint"))
        {
            if (errorResponse.Contains("Position"))
                return "Lỗi ràng buộc: Mã chức vụ không tồn tại trong hệ thống";
            if (errorResponse.Contains("Profession"))
                return "Lỗi ràng buộc: Mã trình độ không tồn tại trong hệ thống";
            if (errorResponse.Contains("Organization"))
                return "Lỗi ràng buộc: Mã tổ chức không tồn tại trong hệ thống";
            return "Lỗi ràng buộc dữ liệu - Kiểm tra khóa ngoại";
        }
        
        // Return the full error response for detailed debugging
        return errorResponse;
    }
    catch
    {
        return errorResponse;
    }
}
