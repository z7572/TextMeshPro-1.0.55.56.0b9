# TextMeshPro-1.0.55.56.0b9

适用于 Unity 5.6.3p4 的 TextMeshPro 1.0.55.56.0b9 版本，专用于 Stick Fight: The Game 游戏内 SDF 字体的构建。

> **2026.4.19 更新：找到了一个[适配 Unity 5.x 的tmp](https://www.aigei.com/item/textmesh_pro.html)， 版本1.0.55.52.0b3**
> 
> 如果有时间的话会去进一步适配，这意味着不用专门安装以及引用 Unity 2017.3 的dll，只需从 Unity 5.6.3p4 里抠出来 dll 就能用

源代码由一个相近的版本 [TextMeshPro 1.0.56.2017.3.0b2](https://archive.org/details/text-mesh-pro-release-1.0.56.2017.3.0b-2-dll-only)（适用于 Unity 2017.3）反编译并还原为 Visual Studio 工程后，修改程序集名称重新编译而强行适配，因此可能会在 Unity 5.6.3p4 中报一些错。

用此 TMP 包构建的字体以及生成的 AssetBundle 可被加载进柴游中，~~实现 Unity 5.6 强兼 Unity 2017.3 。~~

另外，修改了生成字体时的贴图大小限制，最高贴图分辨率支持至 32768x32768。

---

## 🚀 安装与使用

要使用该包为 Stick Fight 生成 tmp 字体，请遵循以下步骤：

### 1. 导入 UnityPackage
最简单的方法是直接使用预编译好的包：
1. 前往本仓库的 [Releases](https://github.com/z7572/TextMeshPro-1.0.55.56.0b9/releases) 页面。
2. 下载 `TextMeshPro-1.0.55.56.0b9.unitypackage` 。
3. 打开 **Unity 5.6.3p4** 创建一个全新的空 3D 项目。
4. 将下载的 `.unitypackage` 直接拖入 Unity 中或手动导入该包，导入全部内容。

*(注：控制台若出现红字报错请忽略，不影响核心的字体生成和打包功能。)*

### 2. 生成字体
1. 准备好你的字体文件（`.ttf` 或 `.otf`），拖入 Unity 项目。
2. 在顶部菜单栏点击 `Window -> TextMeshPro -> Font Asset Creator`。
3. 在弹出的字体生成窗口中：
   * **Source Font File**: 选择你导入的字体。
   * **Atlas Resolution**: 限制已解除！你现在可以根据需要选择 `8192x8192`、`16384x16384` 甚至 `32768x32768`，轻松容纳包含数万个汉字的完整字库。
   * **Character Set**: 导入你需要的中文字符集。
4. 点击 **Generate Font Atlas** 等待图集生成。
5. 点击 **Save** 保存为 `.asset` 字体资源文件。

### 3. 一键打包 AssetBundle
包内已附带了一键打包工具脚本，操作非常便捷：
1. **统一 HashCode**：为了在游戏内能通过 `<font="xxx SDF"></font>` 标签正确调用，请在打包前将生成的 `.asset` 字体文件**重命名**为你打算在代码中使用的准确名称（例如 `xxx SDF`）。
2. 选中该字体文件，在 Inspector 面板最下方的 **AssetBundle** 选项中，点击下拉菜单 `New...`，为其分配一个包名（例如 `testsdf`）。
3. 在 Unity 顶部菜单栏点击 **`AssetBundle -> Build AssetBundle`**。
4. 等待进度条跑完后，系统会自动打开项目目录下的 `StreamingAssets` 文件夹，你刚才打包好的 AssetBundle 文件就在里面。

### 4. 在 Mod 中加载 AssetBundle 中的 SDF 字体

推荐将打包好的 AssetBundle 文件放在根目录并在 Visual Studio 项目属性中设置为 **“嵌入的资源” (Embedded Resource)** ，以便单文件发布 Mod。
随后，你可以通过读取内存流的方式加载 AssetBundle，并提取、注册其中的字体。具体实现代码示例如下：

```csharp
// 1. 读取嵌入资源并加载 AssetBundle 的核心方法
public static AssetBundle GetAssetBundle(Assembly assembly, string name)
{
    var logger = BepInEx.Logging.Logger.CreateLogSource("CNText");
    try
    {
        using Stream stream = assembly.GetManifestResourceStream(assembly.FullName!.Split(',')[0] + "." + name) ?? assembly.GetManifestResourceStream(name)!;
        using MemoryStream stream1 = new();
        
        stream.CopyTo(stream1); // 使用下方的扩展方法将流转存到内存中
        
        var ab = AssetBundle.LoadFromMemory(stream1.ToArray());
        logger.LogInfo($"加载 AssetBundle {name} 成功.");
        return ab;
    }
    catch (Exception e)
    {
        logger.LogError(e.Source);
        logger.LogError($"加载 AssetBundle {name} 失败：\n{e}");
        return null;
    }
}

// 必要的 Stream 扩展方法
public static void CopyTo(this Stream source, Stream destination, int bufferSize = 81920)
{
    byte[] array = new byte[bufferSize];
    int count;
    while ((count = source.Read(array, 0, array.Length)) != 0)
    {
        destination.Write(array, 0, count);
    }
}
```

在插件初始化（如 `Awake`）时调用加载与注册逻辑：

```csharp
// 2. 提取并注册 TextMeshPro 字体
// 获取刚才嵌入并加载好的 AssetBundle (假设包名为 testsdf)
var ab_sdf = GetAssetBundle(Assembly.GetExecutingAssembly(), "testsdf");

// 提取里面的字体资产 (假设生成字体时重命名的资产叫 Test SDF)
TMP_FontAsset sdf = ab_sdf.LoadAsset<TMP_FontAsset>("Test SDF");

// 将其加入 TMP 的底层引用管理器中，此后 <font=Test SDF></font> 标签即可正常工作
MaterialReferenceManager.AddFontAsset(sdf);
```

---

## 🛠️ 从源码编译

如果你希望自行阅读或修改 TextMeshPro 的底层逻辑，可以通过本项目提供的 Visual Studio 工程进行二次编译。

**编译原理说明：**
本工程使用的是 Unity 2017.3 的 TextMeshPro 源码，为了保证代码能顺利通过编译，`refs/` 目录下提供的依赖库（`UnityEngine.dll` 等）为 **Unity 2017.3.0f1** 版本。我们在项目配置中强制将输出的程序集名称篡改为 `TextMeshPro-1.0.55.56.0b9`，从而实现对柴游内置的 TextMeshPro 的强行适配（其可以读取ab包并加载）。

**编译步骤：**
1. 克隆本项目到本地。
2. 确保 `refs/` 目录下包含适用于 Unity 2017.3.0f1 的核心依赖库：
   * `UnityEngine.dll`
   * `UnityEditor.dll`
   * `UnityEngine.UI.dll`
   * `UnityEditor.UI.dll`
3. 使用 Visual Studio 打开 `TextMeshPro-1.0.55.56.0b9.sln`。
4. 执行生成（Build）。
5. 编译输出的 Runtime DLL 和 Editor DLL 将生成在对应的 `bin/` 目录下，可直接提取并替换原 `TextMesh_Pro_-_Release_1.0.56.2017.3.0b2_dll_only.unitypackage`（导入到Unity） 里的对应文件。
