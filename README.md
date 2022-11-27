# ClassifiedConsoleWindow
Unity Classified Console Window

菜单 Window/Classified/Console 打开 Console 窗口

菜单 Window/Classified/ConsoleSettings 打开Console Settings 窗口

``` C#
// Console Settings 窗口 包含了 大部分能配置的内容
// 以下是代码层面的相关API
void Settins()
{
    // 由此获得并修改，Console Settings中的内容
    var settings = ClassifiedConsole.CDebugSettings.Instance;
    // 修改 Settings中的内容
    // 保存
    ClassifiedConsole.CDebugSettings.Instance.SaveAndRefreshAssets();
}
```

分类标签 [ClassifiedConsole.CDebugSubSystem] 为 枚举类型 的属性，为 枚举类型添加此标签，即可在ClassifiedConsoleWindow 中显示此分类，

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

Console Window 包含了连接真机的功能

``` C#
void StartRemoteListener()
{
    // 真机上调用，一次开启端口监听，端口号可在Settings中配置
    // 在 Console Window 中 填入 真机局域网IP,即可连接真机看Log
    LogFileRemoteListener.Start(false);
}
```



=========================具体使用方式

``` C#
ClassifiedConsole.CDebug.Log()
// 其中 参数 [int skipLine] 为 需要跳过的栈行数，默认值为3（3意为插件本身需要调过的行数），
// 其中 参数 [params int[] subSystem]  为 该条 Log 的对应分类
```


