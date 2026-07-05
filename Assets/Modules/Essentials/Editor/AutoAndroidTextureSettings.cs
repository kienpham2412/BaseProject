using UnityEditor;
using UnityEngine;

public class AutoAndroidTextureSettings : AssetPostprocessor
{
    // Hàm này được tự động gọi ngay trước khi một file ảnh (Texture) được import vào Unity
    void OnPreprocessTexture()
    {
        // Lấy thông tin Importer của file đang được import
        TextureImporter importer = (TextureImporter)assetImporter;

        // Bỏ qua nếu đây không phải là ảnh mới (tránh việc ghi đè liên tục mỗi khi reimport nếu bạn muốn đổi setting thủ công sau này)
        // Nếu bạn MÚỐN script này luôn luôn ép setting kể cả khi reimport, hãy comment hoặc xóa dòng if này đi.
        if (!assetImporter.importSettingsMissing)
        {
            return;
        }

        // Lấy cấu hình của nền tảng Android
        TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");

        // Bật Override for Android
        androidSettings.overridden = true;

        // Đặt Max Size là 1024
        androidSettings.maxTextureSize = 1024;

        // Đặt Format là ASTC 6x6. 
        // Lưu ý: Unity sẽ tự động nhận diện dùng dạng RGB hay RGBA tùy thuộc vào việc ảnh gốc của bạn có kênh Alpha (trong suốt) hay không.
        androidSettings.format = TextureImporterFormat.ASTC_6x6;

        // Đặt Compressor Quality là Best (tương đương giá trị 100)
        androidSettings.compressionQuality = (int)TextureCompressionQuality.Best;

        // Áp dụng cấu hình vừa tạo vào TextureImporter
        importer.SetPlatformTextureSettings(androidSettings);
    }
}