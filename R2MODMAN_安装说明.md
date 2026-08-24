# r2modman 安装步骤

1. 在 r2modman 里选择 How to Fish 和你正在使用的 Profile。
2. Settings -> Browse profile folder。
3. 记下这个 Profile 文件夹完整路径。
4. Steam -> How to Fish -> 属性 -> 已安装文件 -> 浏览，记下游戏根目录。
5. 解压本包，在文件夹空白处 Shift+右键 -> 在终端中打开。
6. 执行：

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\build-r2modman.ps1 `
  -GameDir "D:\SteamLibrary\steamapps\common\How to Fish" `
  -ProfileDir "C:\Users\你的用户名\AppData\Roaming\r2modmanPlus-local\HowToFish\profiles\Default"
```

把两个路径替换成你自己的实际路径。

成功后 DLL 会自动安装到：

```text
<ProfileDir>\BepInEx\plugins\HowToFishMagicBullet\HowToFishMagicBullet.dll
```

7. 回到 r2modman，点 Start modded。

进入游戏后：
- F8 开关
- [ 减少 FOV
- ] 增加 FOV

如果 PowerShell 报错，把完整报错复制给我即可。


## v1.0.1 修复

- 增加 `UnityEngine.dll` 引用，修复 `CS0012 MonoBehaviour`。
- 自带 `global.json`，强制使用已安装的 .NET 8 SDK，避免 .NET 10 CET 编译器错误。
- 编译失败时脚本会直接停止，不再继续提示“找不到输出 DLL”。
