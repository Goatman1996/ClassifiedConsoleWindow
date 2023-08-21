# ClassifiedConsoleWindow
Unity Classified Console Window

Unity 可分类的日志系统

### 使用方式

``` C#
ClassifiedConsole.CDebug.Log()
// 其中 参数 [int skipLine] 为 需要额外跳过的栈行数
// 其中 参数 [int subSystem]  为 该条 Log 的对应分类
```
### 打开配套的Editor 窗口

菜单 Window/Classified/Console 打开 Console 窗口

### 打开相关的设置窗口

菜单 Window/Classified/Settings 打开Console Settings 窗口

``` C#
// Console Settings 窗口 包含了 大部分能配置的内容
// 以下是代码层面的相关API
void Settins()
{
    // 由此获得并修改，Console Settings中的内容
    var settings = ClassifiedConsole.CDebugSettings.Instance;
    {
        // 修改 Settings中的内容
    }
    // 保存
    ClassifiedConsole.CDebugSettings.Save();
}
```
### 日志分类相关
分类标签 [ClassifiedConsole.CDebugSubSystem] 为 枚举类型 的属性，为 枚举类型添加此标签，即可在 配套窗口中 中显示此分类，

分类标签 起名 [ClassifiedConsole.CDebugSubSystemLabel] 为 特定分类类型的别称，用于在窗口中显示

``` C#
// 这个标签是 用于标记 特定的枚举类型 为 Log分类类型
[ClassifiedConsole.CDebugSubSystem]
private enum TestType
{
    [ClassifiedConsole.CDebugSubSystemLabel("这是第一个标签")]
    MyLog = 0,
    MyLog1 = 1,
}
```
### 局域网 调试真机（基于Http）
Console Window 包含了连接真机的功能

``` C#
void StartRemoteListener()
{
    // 真机上调用，一次开启端口监听，端口号可在Settings中配置
    // 在 Console Window 中 填入 真机局域网IP,即可连接真机看Log
    string url = ClassifiedConsole.CDebugRemoteListener.Start.Start(false);
}
```

### 命令行工具

在Console窗口的最下方有一个输入框，输入完整的（静态）方法名，可调起静态方法。

参数使用空格分开（支持大部分默认的值类型）

完整名称例子:[namespace].[class].[method] [param1] [param2] [...]

局域网 真机调试 亦可